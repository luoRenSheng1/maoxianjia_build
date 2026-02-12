using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Config;
using UnityEngine;
using EngineBase;
using Spine.Unity;

namespace Engine
{
    public class MapHeroObjectAttr : MapMoveObjectAttr
    {
        public HeroVo HeroVo;
        public ConfigHeroUnit HeroUnit;

        public void Init(HeroVo vo)
        {
            if (vo == null)
            {
                return;
            }

            this.HeroVo = vo;
            this.unitID = vo.unitID;
            this.ObjectTypeID = vo.generalsType;
            this.Camp = vo.CampType;

            HeroUnit = ConfigUtils.GetHeroById(this.ObjectTypeID);

            if (HeroUnit != null)
            {
                InitByAttrID(HeroUnit.AtkSpeed, HeroUnit.Speed);
                
                this.skillNormalID = HeroUnit.AtkSkill;
                // this.skillXPID = HeroUnit.ActiveSkill;
            }
        }
        
        public void UpdateHero(HeroVo heroVo)
        {
            this.ObjectTypeID = heroVo.generalsType;
            HeroUnit = ConfigUtils.GetHeroById(this.ObjectTypeID);

            if (HeroUnit != null)
            {
                InitByAttrID(HeroUnit.AtkSpeed, HeroUnit.Speed);
                
                this.skillNormalID = HeroUnit.AtkSkill;
                // this.skillXPID = HeroUnit.ActiveSkill;
            }

            this.HPMax = this.HP;
        }
    }

    /// <summary>
    /// Hero
    /// </summary>
    public class MapHeroObject : MapMoveObject
    {
        protected Dictionary<int, float> m_mapSkillCastTime = new Dictionary<int, float>(); // 下一次技能允许释放的时间

        public bool IsInvincible { get; set; }

        private GameObject _skillWudi;

        protected MapFxObject _hitEffect;
        
        public override MapObjectType ObjectType
        {
            get { return MapObjectType.Hero; }
        }

        protected override MapObjectAttr CreateNewAttr()
        {
            return new MapHeroObjectAttr();
        }

        public MapHeroObjectAttr HeroAttr
        {
            get { return this.Attr as MapHeroObjectAttr; }
            set => Attr = value;
        }

        public MapHeroObject()
        {
            IsNeedSyncHP = true;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED;
        }

        public override void TriggerSetGameObject()
        {
            if (HeroAttr != null)
            {
                int heroSize = 100;
                if (HeroAttr.HeroUnit.ModelScale != 0)
                    heroSize = HeroAttr.HeroUnit.ModelScale;  // TODO 先写死120   英雄初始尺寸  改读取配置
                float size = heroSize * ConstDefine.CONFIG_PLACE * ConstDefine.MODEL_SCALE;
                this.Scale = new Vector3(size, size, 1);
                UpdateHeroTotalAttr();
            }
            
            base.TriggerSetGameObject();
            if (UIContainerHP != null)
            {
                // 英雄血条位置
                UIContainerHP.xy = new Vector2(80, -130);
            }
            
            ModelManager.Instance.LoadNormalPrefab("Effect/Skill_wudi", 
                (go) =>
                {
                    var wudi = GameObject.Instantiate(go);
                    wudi.transform.SetParent(this.gameObj.transform.parent);
                    wudi.transform.gameObject.layer = this.gameObj.transform.gameObject.layer;
                    wudi.transform.localPosition = Vector3.one;
                    SkeletonAnimation spineAnimation = wudi.transform.GetComponent<SkeletonAnimation>();
                    spineAnimation.state.SetAnimation(0, "idle", true);
                    wudi.GetComponent<MeshRenderer>().sortingOrder =
                        this.gameObj.transform.GetComponent<MeshRenderer>().sortingOrder + 1;
                    _skillWudi = wudi;
                    _skillWudi.SetActive(false);
                });
        }

        public override void Destroyed()
        {
            base.Destroyed();
            IsInvincible = false;//xp技能全局cd
            // InvincibleEndTime = 0;
            if(_skillWudi != null)
                GameObject.Destroy(_skillWudi);
            ClearMovePath();
            if (_hitEffect != null && !_hitEffect.Recycled)
            {
                MapObjectManager.Instance.DestroyActor(_hitEffect);
                _hitEffect = null;
            }
            IsAutoAttack = false;
        }

        public override void SyncDamage(UnitDamageVo damage, MapMoveObject baseAttack = null)
        {
            ConfigSkillUnit cfgSkill = null;
            if (damage.skillID > 0)
            {
                cfgSkill = ConfigUtils.GetSkillById(damage.skillID);
            }
            
            if (damage.damageType == EN_DAMAGE_TYPE.RECOVERY)
            {
                this.Attr.HP += damage.damage;
                this.Attr.HP = Math.Min(this.Attr.HPMax, this.Attr.HP);
            }
            else
            {
                //被攻击时
                MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                if (cfgSkill != null)
                {   //被攻击时
                    BuffInfoManager.Instance.ShowHitTargetBuff(baseAttack, objTarget,cfgSkill, damage,  skillAtkCount);
                }
                
                this.hurtCount++; //统计受伤次数
                
                RoleManager.Instance.HeroBeAtkCounter++;  //英雄被攻击次数（只有角色会被攻击） 上报服务器
                
                if (this.lifeShieldCount > 0)  //有护盾先扣除护盾血量
                {
                    double valueToSubtract = damage.damage;
                    for (int i = this.lifeShieldList.Count - 1; i >= 0; i--)
                    {
                        valueToSubtract -= this.lifeShieldList[i];
                        if (valueToSubtract > 0)
                            this.lifeShieldList[i] = 0;
                        else
                        {
                            this.lifeShieldList[i] = Math.Abs(valueToSubtract);
                            valueToSubtract = 0;
                            break;
                        }
                    }
                    if (valueToSubtract != 0)
                    {
                        this.Attr.HP -= valueToSubtract;
                        RoleManager.Instance.HeroHurtCounter++; //英雄受到伤害(有减HP) 的次数 上报服务器
                    }
                    damage.damageType = EN_DAMAGE_TYPE.LifeShield;
                }
                else
                {
                    this.Attr.HP -= damage.damage;
                    RoleManager.Instance.HeroHurtCounter++;  //英雄受到伤害(有减HP) 的次数 上报服务器
                }
            }
            // Debug.LogWarningFormat("this.Attr.HP={0} damage.damageType={1}   damage={2}", this.Attr.HP, damage.damageType, damage.damage);
            if (IsNeedSyncHP)
            {
                if (UIContainerHP != null)
                {
                    UIContainerHP.UpdateUI(this.Attr, true);
                    UIContainerHP.DamageFly(this, damage);
                    if (this is MapMonsterObject && DungeonMapManager.Instance.IsInCopy)
                    {
                        ((UI_BossBlood) MapObjectManager.Instance.GetMapObjectRootTrans().GetChild("copyBlood")).UpdateMonsterHP(this.Attr, false);
                    }
                }
            }
            
            // 受击
            if (damage.damage > 0 && damage.skillID != 0)
            {
                PlayHit();
            }
            
            if (cfgSkill != null && cfgSkill.HitEffect != 0)
            {
                if(_hitEffect != null)
                    MapObjectManager.Instance.DestroyActor(_hitEffect);
                _hitEffect = MapObjectManager.Instance.SpawnFxActor(cfgSkill.HitEffect, this.Position, this.Rotation, this);
            }
            
            if (this.Attr.HP<= 0 && !IsDead)
            {
                if (cfgSkill != null)
                {   // 死亡 是否触发buff
                    BuffInfoManager.Instance.ShowHitTargetDeadBuff(baseAttack, cfgSkill, damage);
                }
            }
            
            if (this.Attr.HP<= 0 && !IsDead)
            {
                PlayDead(this, cfgSkill);
                MapObjectManager.Instance.DestroyMapObjectById(this.id);
            }
            else if (this.Attr.HP > 0 && IsDead)
            {
                PlayAlive();
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_HP_CHANGE);
        }

        // public override void EndCurSkill()
        // {
        //     if (this.HeroAttr != null && curSkill.skillID == this.HeroAttr.HeroUnit.SkillXp)
        //     {
        //         IsInvincible = false;
        //         if(_skillWudi != null)
        //             _skillWudi.SetActive(false);
        //         this.AnimatorPlay("idle", true, 1);
        //         // Debug.Log("skillXP END:"+UnityEngine.Time.realtimeSinceStartup);
        //     }
        //     base.EndCurSkill();
        // }

        protected float RecordCastSkillTime(int nSkillID, float attackTime)
        {
            var skillType = ConfigUtils.GetSkillById(nSkillID);

            if (skillType == null)
            {
                return 0.0f;
            }

            float fCDTime = 0.0f;

            if (skillType.SkillType == (int)EN_SKILL_TYPE.NORMAL)
            {
                fCDTime = 1 / (this.Attr.AtkSpeed <= 0 ? 1 : this.Attr.AtkSpeed) / (MapObjectManager.Instance.Speed * (curSkill.AtkSpeed <= 0 ? 1 : curSkill.AtkSpeed));
            }
            // else
            // {
            //     fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_TIME/MapObjectManager.Instance.Speed;
            //     fCDTime += attackTime;
            //     fCDTime = Math.Max(0, fCDTime * (1 - Attr.SkillCd));
            // }
            
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
            // if (IsAutoXPAttack && IsCanCastSkillByCDTime(this.Attr.skillXPID))
            // {
            //     return this.Attr.skillXPID;
            // }
            
            if (IsCanCastSkillByCDTime(this.Attr.skillNormalID))
            {
                return this.Attr.skillNormalID;
            }

            return 0;
        }

        public override void Tick(float deltaSeconds)
        {
            base.Tick(deltaSeconds);

            if (isPause)
                return;
            //todo  测试
            AutoAttack();
        }

        public bool IsAutoAttack { get; set; } = false;
        public bool IsAutoXPAttack { get; set; } = true;
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
                return false;
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
            if (IsInvincible)
                return false;
            if (!IsAutoAttack)
            {
                return false;
            }
            
            if (IsDead)
            {
                return false;
            }

            if (IsInMove())
            {
                return false;
            }
            
            if (this is MapPetObject)  // 宠物  如果角色暂停  那么宠物不进行攻击
            {
                if (MapObjectManager.Instance.GetLocalHero() != null && MapObjectManager.Instance.GetLocalHero().isPause)
                {
                    return false;
                }
            }
            else
            {
                if (MapObjectManager.Instance.isMeleeHero() && MapObjectManager.Instance.isHeroPlayXPSkill)
                {
                    return false;
                }
            }
            
            if (IsInPlaySkill())  //角色技能和普攻不互相排次
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

            var skillType = ConfigUtils.GetSkillById(skillID);

            if (skillType == null)
            {
                return;
            }

            if (MapObjectManager.Instance.isMeleeHero() && this.ObjectType == MapObjectType.Hero)
            {
                float distance = Mathf.Abs(this.Position.x - TargetAttackRole.Position.x); //Utils.DistanceIgnoreZ(this.Position, TargetAttackRole.Position);
                if (distance > skillType.Radius)
                {
                    // 施法距离不够，继续行进
                    var lstPos = new List<Vector3>();
                    lstPos.Add(this.Position);
                    var forward = TargetAttackRole.Position.x > this.Position.x ? 1 : -1;
                    lstPos.Add(new Vector3(this.Position.x + this.MoveSpeed * 0.1f * forward, this.Position.y, 0));
                    this.UpdateMoveByPath(lstPos, null, null);
                    return;
                }
            }

            if (!MapObjectManager.Instance.battleSign)
            {
                return;
            }
            
            if (this.ObjectType == MapObjectType.Hero)
            {
                RoleManager.Instance.HeroAtkCounter++;  //英雄普攻次数 上报服务器
            }else if (this.ObjectType == MapObjectType.Pet)
            {
                if (RoleManager.Instance.petAtkInfoDIc.ContainsKey((this as MapPetObject).PetAttr.petItemInfo.PetGuid))
                {
                    RoleManager.Instance.petAtkInfoDIc[(this as MapPetObject).PetAttr.petItemInfo.PetGuid].atkCount++;
                    RoleManager.Instance.petAtkInfoDIc[(this as MapPetObject).PetAttr.petItemInfo.PetGuid].petHurt = this.Attr.Atk;
                }
                else
                {
                    RoleManager.PetAttackData petAtkData = new RoleManager.PetAttackData();
                    petAtkData.atkCount = 1;
                    petAtkData.petHurt = this.Attr.Atk;
                    RoleManager.Instance.petAtkInfoDIc.Add((this as MapPetObject).PetAttr.petItemInfo.PetGuid, petAtkData);
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
                CalcDamageByCastSkill(vo, target, CastSkillType.Hero);
            }
            // 反击伤害
            CalcCounterDamageByCastSkill(vo, lstTarget);
            
            // 攻击回复血量
            RecoverHpByAttack(this);

            if (this is MapHeroObject)
            {
                this.atkCount++;
            }
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

            if (MapObjectManager.Instance.isMeleeHero() && this.ObjectType == MapObjectType.Hero)
            {
                MapObjectManager.Instance.isHeroPlayNormalSkill = true;
                GameManager.Instance.TimerManager.SetTimer(skillTime, () =>
                {
                    MapObjectManager.Instance.isHeroPlayNormalSkill = false;
                });
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.FIGHT_DATA_HERO_CASTSKILL, skillID, this.Attr.Camp);
        }

        private void RecoverHpByAttack(MapMoveObject attack)
        {
            if (attack == null || attack.ObjectType != MapObjectType.Hero)
            {
                return;
            }

            if (DataManager.Instance.GetRoleData().FightAttrVo.AtkHPRecovery > 0)
            {
                var hero = MapObjectManager.Instance.GetLocalHero();
                if (hero != null)
                {
                    hero.PlayRecovery(this, DataManager.Instance.GetRoleData().FightAttrVo.AtkHPRecovery);
                }
            }
        }
        
        public void UpdateHeroTotalAttr(FightAttrVo fightAttrVo = null)
        {
            if (fightAttrVo == null)
                fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            double curHp = this.Attr.HP;
            double curMax = this.Attr.HPMax;
            this.HeroAttr = GetHeroTotalAttr(fightAttrVo);
            if (curMax < HeroAttr.HPMax)
            {
               curHp += HeroAttr.HPMax - curMax;
            }
            else
            {
               curHp -= (curMax - HeroAttr.HPMax);
               curHp = curHp < 1 ? 1 : curHp;
            }

            this.HeroAttr.UpdateHp(curHp, HeroAttr.HPMax);
            //刷新进度条
            this.UpdateHpBar();
            
            // Debug.Log( "========最终值  HeroId="+this.HeroAttr.ObjectTypeID + " " + HeroAttr.ToString());
        }
        
        //TODO 给角色属性赋值
        private MapHeroObjectAttr GetHeroTotalAttr(FightAttrVo fightAttrVo)
        {
            MapHeroObjectAttr tempAttr = new MapHeroObjectAttr();
            tempAttr.Init(this.HeroAttr.HeroVo);
            tempAttr.Atk = fightAttrVo.Atk;
            tempAttr.HP = fightAttrVo.HP;
            tempAttr.Recovery = fightAttrVo.Recovery;
            tempAttr.CriticalStrike = fightAttrVo.CriticalStrike;
            tempAttr.CriticalInjury = fightAttrVo.CriticalInjury;
            tempAttr.BossDamageAdd = fightAttrVo.BossDamageAdd;
            tempAttr.MonsterDamageAdd = fightAttrVo.MonsterDamageAdd;
            tempAttr.Mitigation = fightAttrVo.Mitigation;
            tempAttr.Bloodsucking = fightAttrVo.Bloodsucking;
            tempAttr.SkillDamage = fightAttrVo.SkillDamage;
            tempAttr.MagicTimes = fightAttrVo.MagicTimes;
            tempAttr.GoldAdd = fightAttrVo.GoldAdd;
            tempAttr.SkillCd = fightAttrVo.SkillCd;
            tempAttr.AtkSpeed = fightAttrVo.AtkSpeed;
            tempAttr.ComboAtk = fightAttrVo.ComboAtk;
            tempAttr.CounterAtk = fightAttrVo.CounterAtk;
            
            tempAttr.ParryRate = (float)fightAttrVo.ParryRate; //格挡率
            tempAttr.ParryValue = fightAttrVo.ParryValue;  //格挡值
            tempAttr.JoukRate = fightAttrVo.JoukRate;  //闪避率
            
            tempAttr.HPMax = tempAttr.HP;
            
            tempAttr.Def = fightAttrVo.Def;
            tempAttr.IgnoreDef = fightAttrVo.IgnoreDef;
            tempAttr.AtkHitRate = fightAttrVo.AtkHitRate;
            tempAttr.HPMultiple = fightAttrVo.HPMultiple;
            tempAttr.AtkMultiple = fightAttrVo.AtkMultiple;
            tempAttr.HarmType = fightAttrVo.HarmType;
            return tempAttr;
        }
    }
}