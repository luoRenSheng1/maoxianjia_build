
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
    /// <summary>
    /// Hero
    /// </summary>
    public class MapHeroSkillProxy : MapMoveObject
    {
        protected Dictionary<int, float> m_mapSkillCastTime = new Dictionary<int, float>(); // 下一次技能允许释放的时间

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

        public MapHeroSkillProxy()
        {
            IsNeedSyncHP = true;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED;
        }

        public override void TriggerSetGameObject()
        {
        }

        public override void Destroyed()
        {
            base.Destroyed();
            IsAutoXPAttack = false;
            m_mapSkillCastTime.Clear();
            EndCurSkill();
        }

        float skillTime = 0.0f;
        public void ResetSkillCD(float cdTime)
        {
            if (cdTime == 0)
            {
                m_mapSkillCastTime.Clear();

                if (this.Attr.skillXPID != 0) //cd重置  保存数据上传服务器
                {
                    if (!RoleManager.Instance.heroSkillCDDic.ContainsKey(this.Attr.skillXPID))
                    {
                        RoleManager.SkillCD skillcd = new RoleManager.SkillCD();
                        skillcd.skillId = this.Attr.skillXPID;
                        skillcd.petGuid = 0;
                        RoleManager.Instance.heroSkillCDDic.Add(this.Attr.skillXPID, skillcd);
                    }
                }
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
            // skillType.Cd = 180000;
            
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
                return true;
            }

            return RealTime.time >= fTime;
        }

        public virtual int GetIdleSkill()
        {
            if ((IsAutoXPAttack || MapObjectManager.Instance.isHeroPlayXPSkill) && IsCanCastSkillByCDTime(this.Attr.skillXPID))
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
            
            //todo  测试  
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

            if (MapObjectManager.Instance.isMeleeHero())
            {
                MapObjectManager.Instance.isHeroPlayXPSkill = true;
            }
            
            CastSkill(this.Attr.skillXPID);
            
            return true;
        }

        private bool AutoAttack()
        {
            if (MapObjectManager.Instance.GetLocalHero() == null ||
                !MapObjectManager.Instance.GetLocalHero().IsAutoAttack)
                return false;
            if (!IsAutoXPAttack && !MapObjectManager.Instance.isHeroPlayXPSkill)
            {
                return false;
            }
        
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

            if (MapObjectManager.Instance.isMeleeHero() && MapObjectManager.Instance.isHeroPlayNormalSkill)
            {
                return ;
            }
            
            if (!MapObjectManager.Instance.battleSign)
            {
                MapObjectManager.Instance.isHeroPlayXPSkill = false;
                return;
            }
            
            var skillType = ConfigUtils.GetSkillById(skillID);

            if (skillType == null)
            {
                MapObjectManager.Instance.isHeroPlayXPSkill = false;
                return;
            }
            
            CastSkillVo vo = new CastSkillVo();
            
            vo.unitID = this.id;
            vo.skillID = skillID;
            vo.lstTarget = new List<UnitDamageVo>();
            
            // 主目标 及附属目标
            List<MapObject> lstTarget;
            Vector3 posAttack = Vector3.zero;
            
            MapObjectManager.Instance.GetSkillTarget(this, TargetAttackRole, skillID, out lstTarget, out posAttack);
            if (lstTarget.Count <= 0)
            {
                MapObjectManager.Instance.isHeroPlayXPSkill = false;
                return;
            }
            
            // 攻击伤害
            foreach (var target in lstTarget)
            {
                CalcDamageByCastSkill(vo, target, CastSkillType.Hero);
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
            
            if (MapObjectManager.Instance.isMeleeHero())
            {
                MapObjectManager.Instance.isHeroPlayXPSkill = true;
                float time = MapObjectManager.Instance.GetLocalHero().GetAnimatorTimeByName(skillType.SkillAction) / (MapObjectManager.Instance.Speed * (this.Attr.AtkSpeed != 0 ? this.Attr.AtkSpeed : 1));
                GameManager.Instance.TimerManager.SetTimer(time - 0.5f, () =>  // 0.5f 回调延迟误差？
                {
                    MapObjectManager.Instance.isHeroPlayXPSkill = false;
                });
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_HERO_CASTSKILL, skillID, this.Attr.Camp);
            
            //统计技能信息上报服务器
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