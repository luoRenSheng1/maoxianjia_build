using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;
using EngineBase;
using Spine.Unity;

namespace Engine
{
    public class MapPetObjectAttr : MapMoveObjectAttr
    {
        public PetVo PetVo;
        public ConfigPetBasisUnit PetBasisUnit;
        public PetItemInfo petItemInfo;
        public void Init(PetVo vo, PetItemInfo petInfo)
        {
            if (vo == null)
            {
                return;
            }

            this.PetVo = vo;
            this.unitID = vo.unitID;
            this.ObjectTypeID = vo.generalsType;
            this.Camp = vo.CampType;
            this.petItemInfo = petInfo;
            
            PetBasisUnit = ConfigUtils.GetPetById(this.ObjectTypeID);
            if (PetBasisUnit != null)
            {
                MapHeroObject localHero = MapObjectManager.Instance.GetLocalHero();
                InitByAttrID(PetBasisUnit.AtkSpeed, localHero?.HeroAttr.Speed ?? 320);
                this.skillNormalID = PetBasisUnit.AttackSkillId;
                // this.skillXPID = PetBasisUnit.SkillId;
            }
        }

        public void UpdatePet(PetVo petVo)
        {
            this.ObjectTypeID = petVo.generalsType;
            PetBasisUnit = ConfigUtils.GetPetById(this.ObjectTypeID);
            if (PetBasisUnit != null)
            {
                MapHeroObject localHero = MapObjectManager.Instance.GetLocalHero();
                InitByAttrID(PetBasisUnit.AtkSpeed, localHero?.HeroAttr.Speed ?? 320);
                this.skillNormalID = PetBasisUnit.AttackSkillId;
                // this.skillXPID = PetBasisUnit.SkillId;
            }
        }

        public void AddPassiveSkillAttr(List<int> PassiveSkillList)
        {
            for (int i = 0; i < PassiveSkillList.Count; i++)
            {
                AdditionalBuff(PassiveSkillList[i]);
            }
        }
    }

    /// <summary>
    /// Pet
    /// </summary>
    public class MapPetObject : MapHeroObject
    {
        protected Dictionary<int, float> m_mapSkillCastTime = new Dictionary<int, float>(); // 下一次技能允许释放的时间

        public string targetIdWithNum = "";
        private GameObject _skillCommon;
        
        public override MapObjectType ObjectType
        {
            get { return MapObjectType.Pet; }
        }

        protected override MapObjectAttr CreateNewAttr()
        {
            return new MapPetObjectAttr();
        }

        public MapPetObjectAttr PetAttr
        {
            get { return this.Attr as MapPetObjectAttr; }
        }

        public MapPetObject()
        {
            IsNeedSyncHP = false;
            MoveSpeed = ConstDefine.DEFAULT_MOVE_SPEED;
        }
        
        public override void TriggerSetGameObject()
        {
            int petSize = 100;
            if (PetAttr.PetBasisUnit.ModelScale != 0)
                petSize = PetAttr.PetBasisUnit.ModelScale;
            float size = petSize * ConstDefine.CONFIG_PLACE * ConstDefine.MODEL_SCALE;
            this.Scale = new Vector3(size, size, 1);

            base.TriggerSetGameObject();
            
            ModelManager.Instance.LoadNormalPrefab("Effect/Common_pet", 
                (go) =>
                {
                    var common = GameObject.Instantiate(go);
                    common.transform.SetParent(this.gameObj.transform.parent);
                    common.transform.gameObject.layer = this.gameObj.transform.gameObject.layer;
                    common.transform.localPosition = new Vector3(this.gameObj.transform.localPosition.x, this.gameObj.transform.localPosition.y + this.gameObj.transform.localScale.y * 0.5f, this.gameObj.transform.position.z);
                    common.transform.localScale = new Vector3(30, 30, 30);
                    common.GetComponent<MeshRenderer>().sortingOrder = this.gameObj.transform.GetComponent<MeshRenderer>().sortingOrder + 100;
                    _skillCommon = common;
                    _skillCommon.SetActive(false);
                });
        }

        public override void Destroyed()
        {
            base.Destroyed();
            if(_skillCommon != null)
                GameObject.Destroy(_skillCommon);
        }

        public void PlaySkillCommonEffect()
        {
            if (_skillCommon != null && _skillCommon.transform.GetComponent<SkeletonAnimation>() != null)
            {
                _skillCommon.SetActive(true);
                _skillCommon.transform.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "chongwu_eff", false).Complete +=
                    (Spine.TrackEntry track) =>
                    {
                        _skillCommon.SetActive(false);
                    };
            }
        }
        
        public override int GetIdleSkill()
        {
            if (IsAutoXPAttack && IsCanCastSkillByCDTime(this.Attr.skillXPID))
            {
                return this.Attr.skillXPID;
            }
            
            if (IsCanCastSkillByCDTime(this.Attr.skillNormalID))
            {
                return this.Attr.skillNormalID;
            }

            return 0;
        }

        public void UpdatePetTotalAttribute(FightAttrVo fightAttrVo = null)
        {
            if(this.PetAttr.ObjectTypeID == 0) return;
            if(fightAttrVo == null)
                fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(this.PetAttr.ObjectTypeID);
            if (petBasisUnit != null)
            {
                // this.PetAttr.Atk = petBasisUnit.Inherit * fightAttrVo.Atk * ConstDefine.CONFIG_PLACE_EX;
                // this.PetAttr.CriticalInjury = petBasisUnit.Inherit * fightAttrVo.CriticalInjury * ConstDefine.CONFIG_PLACE_EX;
                // this.PetAttr.CriticalStrike = petBasisUnit.Inherit * fightAttrVo.CriticalStrike * ConstDefine.CONFIG_PLACE_EX;
                
                //伤害放在 PlayerAttrUtils 获得宠物时算
                // ConfigPetSkillLevelUnit skillData = ConfigUtils.GetPetSkillLevelUnitBySkillIdWithLevel(this.PetAttr.petItemInfo.skillId, this.PetAttr.petItemInfo.skillLevel);
                // this.PetAttr.Atk =
                //     DataManager.Instance.mRoleData.FightAttrVo.Atk * (this.PetAttr.petItemInfo.petCfg.Inherit * ConstDefine.CONFIG_PLACE_EX) *
                //     (skillData.SkillHurt * ConstDefine.CONFIG_PLACE_EX) * this.PetAttr.petItemInfo.growRate +
                //     DataManager.Instance.mRoleData.FightAttrVo.PetAtk * (1 + DataManager.Instance.mRoleData.FightAttrVo.PetAtkADD);
                
                
                //在生成宠物时，直接设置宠物的 FightAttrVo ，这边应该应该没用了 暴击爆伤等有些属性还会用到
                /**/
                this.PetAttr.Atk = this.PetAttr.petItemInfo.FightAttrVo.Atk;
                this.PetAttr.AtkSpeed = this.PetAttr.petItemInfo.FightAttrVo.AtkSpeed; //攻速 默认读配置

                this.PetAttr.Recovery = this.PetAttr.petItemInfo.FightAttrVo.Recovery;
                this.PetAttr.CriticalStrike = this.PetAttr.petItemInfo.FightAttrVo.CriticalStrike;
                this.PetAttr.CriticalInjury = this.PetAttr.petItemInfo.FightAttrVo.CriticalInjury;
                this.PetAttr.BossDamageAdd = this.PetAttr.petItemInfo.FightAttrVo.BossDamageAdd;
                this.PetAttr.MonsterDamageAdd = this.PetAttr.petItemInfo.FightAttrVo.MonsterDamageAdd;
                this.PetAttr.Mitigation = this.PetAttr.petItemInfo.FightAttrVo.Mitigation;
                this.PetAttr.Bloodsucking = this.PetAttr.petItemInfo.FightAttrVo.Bloodsucking;
                this.PetAttr.SkillDamage = this.PetAttr.petItemInfo.FightAttrVo.SkillDamage;
                this.PetAttr.MagicTimes = this.PetAttr.petItemInfo.FightAttrVo.MagicTimes;
                this.PetAttr.GoldAdd = this.PetAttr.petItemInfo.FightAttrVo.GoldAdd;
                this.PetAttr.SkillCd = this.PetAttr.petItemInfo.FightAttrVo.SkillCd;
                this.PetAttr.ComboAtk = this.PetAttr.petItemInfo.FightAttrVo.ComboAtk;
                this.PetAttr.CounterAtk = this.PetAttr.petItemInfo.FightAttrVo.CounterAtk;
            
                this.PetAttr.ParryRate = (float)this.PetAttr.petItemInfo.FightAttrVo.ParryRate; //格挡率
                this.PetAttr.ParryValue = this.PetAttr.petItemInfo.FightAttrVo.ParryValue;  //格挡值
                this.PetAttr.JoukRate = this.PetAttr.petItemInfo.FightAttrVo.JoukRate;  //闪避率
            
                this.PetAttr.Def = this.PetAttr.petItemInfo.FightAttrVo.Def;
                this.PetAttr.IgnoreDef = this.PetAttr.petItemInfo.FightAttrVo.IgnoreDef;
                this.PetAttr.AtkHitRate = this.PetAttr.petItemInfo.FightAttrVo.AtkHitRate;
                this.PetAttr.HPMultiple = this.PetAttr.petItemInfo.FightAttrVo.HPMultiple;
                this.PetAttr.AtkMultiple = this.PetAttr.petItemInfo.FightAttrVo.AtkMultiple;
                
            }

        }
    }
}
