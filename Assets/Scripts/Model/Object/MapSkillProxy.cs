using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using UnityEngine;
using EngineBase;
using Spine.Unity;

namespace Engine
{
    public class MapSkillProxyObjectAttr : MapMoveObjectAttr
    {
        public int PosIndex;
        public void Init(int AttackSkillId, int posIndex, EN_CAMP_TYPE campType = EN_CAMP_TYPE.FRIEND)
        {
            this.Camp = campType;
            this.PosIndex = posIndex;
            this.skillXPID = AttackSkillId;
        }
    }

    /// <summary>
    /// Hero
    /// </summary>
    public class MapSkillProxy : MapMoveObject
    {
        protected Dictionary<int, float> m_mapSkillCastTime = new Dictionary<int, float>(); // 下一次技能允许释放的时间

        public bool isRepeatCast = false;
        
        public override MapObjectType ObjectType
        {
            get { return MapObjectType.Pet; }
        }

        protected override MapObjectAttr CreateNewAttr()
        {
            return new MapSkillProxyObjectAttr();
        }

        public MapSkillProxyObjectAttr ProxyAttr
        {
            get { return this.Attr as MapSkillProxyObjectAttr; }
            set => Attr = value;
        }

        public MapSkillProxy()
        {
            IsNeedSyncHP = true;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED;
        }
        
        public override void Destroyed()
        {
            base.Destroyed();
            IsAutoXPAttack = false;
            m_mapSkillCastTime.Clear();
            EndCurSkill();
        }

        //重置技能CD 时 设置上报服务器数据
        public void SetReportSkillData()
        {
            if (this.Attr.skillXPID != 0) //cd重置  保存数据上传服务器
            {
                ConfigSkillUnit skillType = ConfigUtils.GetSkillById(this.Attr.skillXPID);
                if (skillType != null)
                {
                    if (BuffInfoManager.Instance.IsPetSkill(this, skillType))
                    {   //宠物初始技能
                        if (!RoleManager.Instance.petCastSkillCDDic.ContainsKey(this.Attr.skillXPID))
                        {
                            RoleManager.SkillCD skillcd = new RoleManager.SkillCD();
                            skillcd.skillId = this.Attr.skillXPID;
                            skillcd.petGuid = this.Attr.PetGuid;
                            RoleManager.Instance.petCastSkillCDDic.Add(this.Attr.skillXPID, skillcd);
                        }
                    }
                    else
                    {   //角色抽卡技能
                        if (!RoleManager.Instance.heroSkillCDDic.ContainsKey(this.Attr.skillXPID))
                        {
                            RoleManager.SkillCD skillcd = new RoleManager.SkillCD();
                            skillcd.skillId = this.Attr.skillXPID;
                            skillcd.petGuid = 0;
                            RoleManager.Instance.heroSkillCDDic.Add(this.Attr.skillXPID, skillcd);
                        }
                    }
                }
            }
        }
        
        public void ResetSkillCDWithId(int nSkillID)
        {
            GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
            {
                //if (m_mapSkillCastTime[nSkillID] != null)
                if(m_mapSkillCastTime.ContainsKey(nSkillID))
                {
                    m_mapSkillCastTime.Remove(nSkillID);
                }
                SetReportSkillData();
                CastXPSkill();
            });
        }
        
        float skillTime = 0.0f;
        public void ResetSkillCD(float cdTime)
        {
            if (cdTime == 0)
            {
                m_mapSkillCastTime.Clear();
                SetReportSkillData();
            }
            else
            {
                foreach (var key in m_mapSkillCastTime.Keys.ToList())
                {
                    var skillType = ConfigUtils.GetSkillById(key);
                    skillTime = 0;
                    if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
                    {
                        skillTime = 1 / this.Attr.AtkSpeed/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);
                    }
                    else
                    {
                        skillTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/MapObjectManager.Instance.Speed;
                        skillTime = Math.Max(0, skillTime * (1 - Attr.SkillCd));
                    }
                    m_mapSkillCastTime[key] -= skillTime * cdTime;
                    if (RealTime.time > m_mapSkillCastTime[key])
                    {
                        EndCurSkill();
                    }
                }
            }
        }
        
        protected float RecordCastSkillTime(int nSkillID, float attackTime)
        {
            var skillType = ConfigUtils.GetSkillById(nSkillID);
            
            
            //todo 测试
            //skillType.Cd = 50000;
            
            
            if (skillType == null)
            {
                return 0.0f;
            }

            float fCDTime = 0.0f;

            if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
            {
                fCDTime = 1 / this.Attr.AtkSpeed/(MapObjectManager.Instance.Speed*curSkill.AtkSpeed);
            }
            else
            {
                fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/MapObjectManager.Instance.Speed;
                fCDTime += attackTime;
                fCDTime = Math.Max(0, fCDTime * (1 - Attr.SkillCd));
            }
            
            float fTime = RealTime.time + fCDTime;

            if (m_mapSkillCastTime.ContainsKey(nSkillID))
            {
                m_mapSkillCastTime[nSkillID] = fTime;
            }
            else
            {
                m_mapSkillCastTime.Add(nSkillID, fTime);
            }

            return fCDTime;
        }

        public float GetCastSkillTime(int nSkillID)
        {
            float value;
            return m_mapSkillCastTime.TryGetValue(nSkillID, out value) ? value : 0.0f;
        }

        public bool IsCanCastSkillByCDTime(int nSkillID)
        {
            if (nSkillID == 0) return false;
            float fTime = GetCastSkillTime(nSkillID);
            if (Mathf.Approximately(fTime, 0.0f))
            {
                fTime =  RealTime.time + RoleManager.Instance.GetSkillDelayTime(this.ProxyAttr.PosIndex);
                m_mapSkillCastTime.Add(nSkillID, fTime);
            }
            if (Mathf.Approximately(fTime, 0.0f))
            {
                return true;
            }

            return RealTime.time >= fTime;
        }

        public virtual int GetIdleSkill()
        {
            if (IsAutoXPAttack && IsCanCastSkillByCDTime(this.Attr.skillXPID))
            {
                return this.Attr.skillXPID;
            }

            return 0;
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);
            
            if (MapObjectManager.Instance.GetLocalHero().isPause)
                return;
            
            //todo 测试
            AutoAttack();
        }
        
        public bool IsAutoXPAttack { get; set; } = false;
        public MapObject TargetAttackRole { get; private set; } = null;
        public int TargetAttackRoleID { get; private set; } = 0;

        private void SetAttackTarget(MapObject part)
        {
            if (part != null)
            {
                TargetAttackRole = part;
                TargetAttackRoleID = TargetAttackRole.id;
            }
            else
            {
                TargetAttackRole = null;
                TargetAttackRoleID = 0;
            }
        }

        public bool CastXPSkill()
        {
            if (IsDead)
            {
                return false;
            }

            if (IsInMove())
            {
                return false;
            }
            
            if (TargetAttackRole == null)
            {
                // 最近怪物
                var targetRole = MapObjectManager.Instance.GetNearestEnemy(this, 0.0f);
    
                if (targetRole == null)
                {
                    return false;
                }
    
                TargetAttackRole = targetRole;
                
            }
            
            if (!IsCanCastSkillByCDTime(this.Attr.skillXPID))
            {
                return false;
            }

            CastSkill(this.Attr.skillXPID);
            
            return true;
        }

        private bool AutoAttack()
        {
            if (MapObjectManager.Instance.GetLocalHero() == null ||
                !MapObjectManager.Instance.GetLocalHero().IsAutoAttack)
                return false;
            
            if (this.Attr.skillXPID != 0 && BuffInfoManager.Instance.IsPetSkill(this, ConfigUtils.GetSkillById(this.Attr.skillXPID) ) ) //宠物初始技能 不受自动开关控制 总是释放
            {
                IsAutoXPAttack = true;
            }
            
            if (!IsAutoXPAttack)
            {
                return false;
            }

            if (this.ProxyAttr.skillXPID == 0)
                return false;
        
            if (IsInPlaySkill())
            {
                return false;
            }
            if (TargetAttackRole != null)
            {
                if (!TargetAttackRole.CanFight || TargetAttackRole.id != TargetAttackRoleID)
                {
                    SetAttackTarget(null);
                }
            }
        
            if (TargetAttackRole == null)
            {
                // 最近怪物
                var targetRole = MapObjectManager.Instance.GetNearestEnemy(this, 0.0f);
        
                if (targetRole == null)
                {
                    return false;
                }
        
                TargetAttackRole = targetRole;
            }
        
            var skillID = GetIdleSkill();
        
            if (skillID == 0)
            {
                return false;
            }
        
            CastSkill(skillID);
        
            return false;
        }

        private void CastSkill(int skillID)
        {
            if (TargetAttackRole == null)
            {
                return;
            }

            if (!MapObjectManager.Instance.battleSign)
            {
                return;
            }
            
            var skillType = ConfigUtils.GetSkillById(skillID);

            if (skillType == null)
            {
                return;
            }
            
            if (this.Attr.PetGuid != 0)  // 宠物技能释放 增加通用前摇特效
            {
                MapPetObject objTarget = MapObjectManager.Instance.GetMapPetObjectByGuid(this.Attr.PetGuid) as MapPetObject;
                if (objTarget != null)
                {
                    objTarget.PlaySkillCommonEffect();
                }
            }
            
            CastSkillVo vo = new CastSkillVo();
            
            vo.unitID = this.id;
            vo.skillID = skillID;
            vo.lstTarget = new List<UnitDamageVo>();
            
            // 主目标 及附属目标
            List<MapObject> lstTarget;
            Vector3 posAttack = Vector3.zero;
            
            MapObjectManager.Instance.GetSkillTarget(this, TargetAttackRole, skillID, out lstTarget, out posAttack);
            
            // 攻击伤害
            foreach (var target in lstTarget)
            {
                CalcDamageByCastSkill(vo, target, CastSkillType.SkillProxy);
            }
            // 反击伤害
            CalcCounterDamageByCastSkill(vo, lstTarget);

            //计算buff
            AddBuff(vo);
            
            var skillTime = this.PlaySkill(vo, posAttack, this.Attr.AtkSpeed);
            if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
            {
                // 普通攻击 cd时间不含攻击时间 补上攻击时间
                this.RecordCastSkillTime(skillID, skillTime);
            }
            else
            {
                // 技能攻击 cd时间含攻击时间
                this.RecordCastSkillTime(skillID, 0.0f);
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_HERO_CASTSKILL, skillID, this.Attr.Camp);
            
            //统计技能信息上报服务器
            if (BuffInfoManager.Instance.IsPetSkill(this, skillType))
            {   //宠物初始技能
                if (RoleManager.Instance.petAtkInfoDIc.ContainsKey(this.MoveAttr.PetGuid))
                {
                    if (RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].skillId != 0)
                    {
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].petHurt = this.Attr.Atk;
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].skillCount++;
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].atkValueList.Add(this.Attr.Atk);
                    }
                    else
                    {
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].petHurt = this.Attr.Atk;
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].skillCount = 1;
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].skillId = skillID;
                        RoleManager.Instance.petAtkInfoDIc[this.MoveAttr.PetGuid].atkValueList.Add(this.Attr.Atk);
                    }
                }
                else
                {
                    RoleManager.PetAttackData petAtkData = new RoleManager.PetAttackData();
                    petAtkData.petHurt = this.Attr.Atk;
                    petAtkData.skillCount = 1;
                    petAtkData.skillId = skillID;
                    petAtkData.atkValueList.Add(this.Attr.Atk);
                    RoleManager.Instance.petAtkInfoDIc.Add(this.MoveAttr.PetGuid, petAtkData);
                }
            }
            else
            {   //角色抽卡技能
                if (RoleManager.Instance.heroCastSkillInfo.ContainsKey(skillID))
                {
                    RoleManager.Instance.heroCastSkillInfo[skillID].skillCount++;
                    RoleManager.Instance.heroCastSkillInfo[skillID].atkValueList.Add(this.Attr.Atk);
                }
                else
                {
                    RoleManager.HeroAttackData heroAtkData = new RoleManager.HeroAttackData();
                    heroAtkData.skillCount = 1;
                    heroAtkData.atkValueList.Add(this.Attr.Atk);
                    RoleManager.Instance.heroCastSkillInfo.Add(skillID, heroAtkData);
                }
            }
        }

        
    }
}