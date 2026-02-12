using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Engine
{
    public class BuffInfoManager : TSingleton<BuffInfoManager>
    {
        private List<int> passiveList = new List<int>(); //角色被动技能
        private Dictionary<int,int> roleTalentDict = new Dictionary<int,int>();  //角色天赋
        private List<PetItemInfo> petInfoList = new List<PetItemInfo>();  //上阵的宠物
        
        private List<ConfigSkillAchieveUnit> skillBuffs = new List<ConfigSkillAchieveUnit>();
        List<ConfigSkillAchieveUnit> objBuffs = new List<ConfigSkillAchieveUnit>();
        
        private List<ConfigSkillAchieveUnit> attackBuffs = new List<ConfigSkillAchieveUnit>();

        private bool isChouka = false;
        private PetItemInfo petItemInfo;
        
        public void OnInit()
        {
            
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }

        #region 获取buff
        // 获取对象身上的buff
        public List<ConfigSkillAchieveUnit> GetObjectBuffs(MapMoveObject obj, ConfigSkillUnit cfgSkill)
        {
            objBuffs.Clear();
            if (obj is MapMonsterObject)
            {
                //todo 测试  死亡后复活 86011  减攻速 84020, 减攻击 84021  每隔30秒回复所有血量 84024
                // obj.Attr.MonsterBuffs = new List<int>() { 84024, 84024 }; 
                
                for (int i = 0; i < obj.Attr.MonsterBuffs.Count; i++)
                {
                    if (ConfigUtils.GetSkillAchieveById(obj.Attr.MonsterBuffs[i]) != null && ConfigUtils.GetSkillAchieveById(obj.Attr.MonsterBuffs[i]).DevSide == 0)
                        objBuffs.Add(ConfigUtils.GetSkillAchieveById(obj.Attr.MonsterBuffs[i]));
                }
                return objBuffs;
            }
            else
            {
                isChouka = false;
                if (obj is MapSkillProxy)
                {
                    if (cfgSkill.SkillType == (int)EN_SKILL_TYPE.ChoukaSkill)
                    {
                        isChouka = true;
                    }
                }
                
                //角色天赋携带buff
                if ((obj is MapHeroObject && obj.ObjectType == MapObjectType.Hero) || obj is MapHeroSkillProxy || isChouka)
                {
                    roleTalentDict.Clear();
                    roleTalentDict = TalentInfoManager.Instance.GetTalentDict().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    foreach (var talent in roleTalentDict)
                    {
                        var unit = ConfigUtils.GetAptitudeUnitByAptitudeIdAndLv(talent.Key, talent.Value);
                        if (unit != null && ConfigUtils.GetSkillAchieveById(unit.Id) != null && ConfigUtils.GetSkillAchieveById(unit.Id).DevSide == 0)
                            objBuffs.Add(ConfigUtils.GetSkillAchieveById(unit.Id));
                    }

                    //技能书直接作用角色身上
                    if (obj is MapHeroObject && obj.ObjectType == MapObjectType.Hero)
                    {
                        foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
                        {
                            foreach (var bookItem in petItem.bookSlotsList)  //宠物技能书 部分直接给角色加的buff
                            {
                                if (bookItem.BookId != 0 && bookItem.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                {
                                    var unit = ConfigUtils.GetPetSkillBookUnitById(bookItem.BookId);
                                    if (unit != null)
                                    {
                                        var achieve = ConfigUtils.GetSkillAchieveById(unit.Id);
                                        if (achieve != null && achieve.Type == 5 &&  achieve.DevSide == 0)
                                        {
                                            if ( achieve.Trigger == 103 && (achieve.HandleObject == 1 || (achieve.HandleObject == 2 && achieve.ActID == 2018)) )
                                            {
                                                if (achieve.CommonAttr != "" && achieve.CommonAttr.Split(",").Length > 1 && int.Parse(achieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.CounterAtk)
                                                {
                                                    foreach (var attr in bookItem.battleAttrList)
                                                    {
                                                        if (attr.AttrId == (int)EN_BUFF_ADD_TYPE.CounterAtk)
                                                        {
                                                            achieve.CommonAttr = attr.AttrId + "," + attr.AttrVal;
                                                        }
                                                    }
                                                }
                                                objBuffs.Add(achieve);
                                            }else if ( (achieve.Trigger == 104 || achieve.Trigger == 108 || achieve.Trigger == 202) && achieve.HandleObject == 1)
                                            {
                                                objBuffs.Add(ConfigUtils.GetSkillAchieveById(unit.Id));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        // todo 测试
                        // var book = ConfigUtils.GetPetSkillBookUnitById(52032);
                        // if (book != null && ConfigUtils.GetSkillAchieveById(book.Id) != null)
                        //     objBuffs.Add(ConfigUtils.GetSkillAchieveById(book.Id));
                    }
                    
                    // 测试 减少CD 1020 重置CD 1021  无敌 2020   闪电 1014 爆炸 1016 每秒恢复血量 3016  普攻伤害提升 1007  技能伤害提升 1009  伤害减少 2005  伤害反弹 2007 
                    // 降低目标防御 2012  恢复生命值 3012 降低目标的攻击速度 3018  击杀回复血量 3014  释放技能回血 3003
                
                    // var unit1 = ConfigUtils.GetAptitudeUnitByAptitudeIdAndLv(2020, 10);   //2005
                    // if (unit1 != null && ConfigUtils.GetSkillAchieveById(unit1.Id) != null)
                    //     objBuffs.Add(ConfigUtils.GetSkillAchieveById(unit1.Id));
                }else{
                    //宠物天赋、技能书 携带buff
                    petItemInfo = null;
                    petInfoList.Clear();
                    petInfoList = PetInfoManager.Instance.GetBattlePetList().ToList();
                    foreach (var petItem in petInfoList)
                    {
                        if (obj is MapPetObject && (obj as MapPetObject).PetAttr.petItemInfo.skillId == petItem.skillId)
                        {
                            petItemInfo = petItem;
                            break;
                        }
                        else if (obj is MapSkillProxy && cfgSkill.Id == petItem.skillId)
                        {
                            petItemInfo = petItem;
                            break;
                        }
                    }

                    if (petItemInfo != null)
                    {
                        // 天赋直接加属性  不在buff处理
                        // foreach (var talentItem in petItemInfo.talentsList) //宠物天赋
                        // {
                        //     var unit = ConfigUtils.GetPetAptitudeUnitById(talentItem);
                        //     if (unit != null && ConfigUtils.GetSkillAchieveById(unit.Id) != null && ConfigUtils.GetSkillAchieveById(unit.Id).DevSide == 0)
                        //         objBuffs.Add(ConfigUtils.GetSkillAchieveById(unit.Id));
                        // }
                    
                        foreach (var bookItem in petItemInfo.bookSlotsList)  //宠物技能书
                        {
                            if (bookItem.BookId != 0 && bookItem.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                            {
                                var unit = ConfigUtils.GetPetSkillBookUnitById(bookItem.BookId);
                                if (unit != null && ConfigUtils.GetSkillAchieveById(unit.Id) != null && ConfigUtils.GetSkillAchieveById(unit.Id).DevSide == 0
                                                 && ConfigUtils.GetSkillAchieveById(unit.Id).Trigger != 0)
                                    objBuffs.Add(ConfigUtils.GetSkillAchieveById(unit.Id));
                            }
                        }
                    }
                    
                    //todo 测试   复活 53025 击杀伤害提升 51005   流血 51012   反弹 52034  回血 53027  反弹 52015  伤害值的生命护盾 53006
                    // var book = ConfigUtils.GetPetSkillBookUnitById(51038);
                    // if (book != null && ConfigUtils.GetSkillAchieveById(book.Id) != null)
                    //     objBuffs.Add(ConfigUtils.GetSkillAchieveById(book.Id));
                    // var book1 = ConfigUtils.GetPetSkillBookUnitById(52008);
                    // if (book1 != null && ConfigUtils.GetSkillAchieveById(book1.Id) != null)
                    //     objBuffs.Add(ConfigUtils.GetSkillAchieveById(book1.Id));
                }
                
            }
            return objBuffs;
        }
        
        // 技能携带的buff
        public List<ConfigSkillAchieveUnit> GetSkillAchieveBySkillId(int skillId)
        {
            skillBuffs.Clear();
            passiveList.Clear();
            
            ConfigSkillUnit cfgSkill = ConfigUtils.GetSkillById(skillId);
            
            //被动技能携带buff
            ConfigHeroUnit heroUnit = ConfigUtils.GetHeroBySkillId(skillId);
            if (heroUnit != null) //是否英雄主动技能
                passiveList = HeroInfoManager.Instance.GetHeroUnlockPassiveSkill(cfgSkill.Id);
            if (passiveList.Count > 0)
            {
                foreach (var buffId in passiveList)
                {
                    ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(buffId);
                    skillBuffs.Add(buffSkillAchieve);
                }
            }
            
            //技能携带buff （包含 英雄主动技能、组合技能（抽卡技能）、宠物技能）
            string[] buffs = cfgSkill.BuffId.Split("|");
            if (int.Parse(buffs[0]) != 0)
            {
                //添加技能buff
                for (int i = 0; i < buffs.Length; i++)
                {
                    ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(int.Parse(buffs[i]));
                    bool isExist = false;
                    for (int j = 0; j < skillBuffs.Count; j++)
                    {
                        if (buffSkillAchieve.ActID == skillBuffs[j].ActID)  //技能buffid 触发值比被动技能低，策划规定用角色技能的触发值
                        {
                            isExist = true;
                            break;
                        }
                    }

                    if (!isExist)
                    {
                        skillBuffs.Add(buffSkillAchieve);
                    }
                }
            }
            
            return skillBuffs;
        }
        #endregion

        #region 击杀目标时
        public void ShowHitTargetDeadBuff(MapMoveObject attack,ConfigSkillUnit cfgSkill, UnitDamageVo damage)
        {
            attackBuffs.Clear();
            // var attack = MapObjectManager.Instance.GetMapMoveObjectById(damage.unitID);
            // if (attack == null && MapObjectManager.Instance.GetLocalHero() != null)  //是技能 那么转成角色
            // {
            //     attackBuffs = BuffInfoManager.Instance.GetSkillAchieveBySkillId(cfgSkill.Id).ToList();
            //     attack = MapObjectManager.Instance.GetLocalHero();
            // }
            
            attackBuffs = BuffInfoManager.Instance.GetSkillAchieveBySkillId(cfgSkill.Id).ToList();
            if (attack is MapSkillProxy)
            {
                if (cfgSkill.SkillType == (int)EN_SKILL_TYPE.ChoukaSkill)
                {
                    attack = MapObjectManager.Instance.GetLocalHero();
                    attackBuffs.AddRange(BuffInfoManager.Instance.GetObjectBuffs(attack, cfgSkill).ToList());
                }
                else
                {
                    //宠物技能  攻击对象 设置为宠物
                    foreach (var t in MapObjectManager.Instance.lstPetObj)
                    {
                        if ( attack.Attr.skillXPID == (t as MapPetObject).PetAttr.petItemInfo.skillId )
                        {
                            attack = (t as MapPetObject);
                            attackBuffs.AddRange(BuffInfoManager.Instance.GetObjectBuffs(attack, cfgSkill).ToList());
                            break;
                        }
                    }
                }
            }
            else if (attack is MapHeroSkillProxy)
            {
                attack = MapObjectManager.Instance.GetLocalHero();
                attackBuffs.AddRange(BuffInfoManager.Instance.GetObjectBuffs(attack, cfgSkill).ToList());
            }
            else
            {
                attackBuffs.AddRange(BuffInfoManager.Instance.GetObjectBuffs(attack, cfgSkill).ToList());
            }

            foreach (var buffSkillAchieve in attackBuffs)
            {
                bool isShow = false;  //死亡时是触发buff
                if (buffSkillAchieve.Trigger == 105 || buffSkillAchieve.Trigger == 108 || buffSkillAchieve.Trigger == 203 || buffSkillAchieve.Trigger == 208)
                    isShow = true;
                else if (buffSkillAchieve != null && buffSkillAchieve.BuffTime == 2)
                    isShow = true;
                else
                {
                    ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID);
                    if (buffAction != null && buffAction.BuffTime == 2)
                        isShow = true;
                }
                        
                if (isShow)
                {
                    CastSkillVo vo = new CastSkillVo();
                    vo.unitID = damage.unitID;
                    vo.skillID = cfgSkill.Id;
                    vo.lstTarget = new List<UnitDamageVo>();
                    vo.lstTarget.Add(damage);
                    AttackedKillTarget(buffSkillAchieve, vo, attack);
                }
            }
            
            //目标死亡
            var target = MapObjectManager.Instance.GetMapMoveObjectById(damage.targetID);
            foreach (var buffSkillAchieve in BuffInfoManager.Instance.GetObjectBuffs(target, cfgSkill).ToList())
            {
                if (buffSkillAchieve.Trigger == 108 && buffSkillAchieve.ActID == 1007)
                {
                    TargetKillBuff(buffSkillAchieve, target, attack);
                }
            }
        }

        // 死亡对象的buff
        private void TargetKillBuff(ConfigSkillAchieveUnit buffSkillAchieve, MapMoveObject target, MapMoveObject attack)
        {
            if (Random.Range(0.0f, 1.0f) > (buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
            {
                return;
            }
            
            ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID).Clone();
            buffAction.CoverLastTime = buffSkillAchieve.CoverLastTime != 0 ? buffSkillAchieve.CoverLastTime : buffAction.CoverLastTime; //持续时间
            buffAction.EffectTime = buffSkillAchieve.CoverEffectTime != 0 ? buffSkillAchieve.CoverEffectTime : buffAction.EffectTime; //生效时间
            buffAction.COpFrequency = buffSkillAchieve.COpFrequency != 0 ? buffSkillAchieve.COpFrequency : buffAction.COpFrequency;  //生效次数
            buffAction.Common = buffSkillAchieve.CoverCommonParam != "0" ? buffSkillAchieve.CoverCommonParam : buffAction.Common;
            buffAction.BuffTime = buffSkillAchieve.BuffTime != 0 ? buffSkillAchieve.BuffTime : buffAction.BuffTime;
            buffAction.Ruantity = buffSkillAchieve.Ruantity != 0 ? buffSkillAchieve.Ruantity : buffAction.Ruantity;

            if (buffSkillAchieve.CoverEffectTime > 1) //怪的复活buff，时间间隔触发
            {
                if (target.lifeShieldTime <= RealTime.time)
                {
                    target.lifeShieldTime = RealTime.time + buffAction.EffectTime;
                }
                else
                {
                    return;
                }
            }
            
            if (target.HitSpineDic.TryGetValue(buffAction.Id, out GLoader3D load3D))
            {
                load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                load3D.animationName = buffAction.BuffEffect + "_buff1";
                load3D.frame = 0;
                load3D.loop = false;
                load3D.playing = true;
                load3D.visible = true;
                load3D.parent.SetChildIndex(load3D, 100);//显示最上层
                load3D.SetXY(-20, -88);
                
                GLoader3D load3dBrith = target.HitSpineDic[(int)EN_BUFF_TYPE.ReBirth1];
                load3dBrith.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                load3dBrith.animationName = buffAction.BuffEffect + "_buff2";
                load3dBrith.frame = 0;
                load3dBrith.loop = false;
                load3dBrith.playing = true;
                load3dBrith.visible = true;
                load3dBrith.SetXY(-20, -88);
                load3D.parent.RemoveChild(load3dBrith);
                load3D.parent.AddChildAt(load3dBrith, 0);
                
                PlayReBrithEffect(target, buffAction);
            }
        }
        
        //死亡时的攻击对象的buff
        private void AttackedKillTarget(ConfigSkillAchieveUnit buffSkillAchieve, CastSkillVo vo, MapMoveObject attack)
        {
            if (buffSkillAchieve ==null || buffSkillAchieve.AttrOrAction == -1) return;
            // if (attack is MapHeroSkillProxy || attack is MapSkillProxy)
            // {
            //     attack = MapObjectManager.Instance.GetLocalHero();
            // }
            if (buffSkillAchieve.Trigger == 108 && attack.Attr.HP > 0) //108死亡时才会触发
                return;
            
            if (buffSkillAchieve.Trigger == 105 || buffSkillAchieve.Trigger == 108 ||
                buffSkillAchieve.Trigger == 203 || buffSkillAchieve.Trigger == 208)
            {
                
                
                
                //todo 测试
                // buffSkillAchieve.TriggerValue = 10000;
                
                
                
                if (Random.Range(0.0f, 1.0f) > (buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
                {
                    return;
                }
            }

            if (buffSkillAchieve.AttrOrAction == 2) //2--状态动作和特殊处理  
            {
                ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID).Clone();
                buffAction.CoverLastTime = buffSkillAchieve.CoverLastTime != 0 ? buffSkillAchieve.CoverLastTime : buffAction.CoverLastTime; //持续时间
                buffAction.EffectTime = buffSkillAchieve.CoverEffectTime != 0 ? buffSkillAchieve.CoverEffectTime : buffAction.EffectTime; //生效时间
                buffAction.COpFrequency = buffSkillAchieve.COpFrequency != 0 ? buffSkillAchieve.COpFrequency : buffAction.COpFrequency;  //生效次数
                buffAction.Common = buffSkillAchieve.CoverCommonParam != "0" ? buffSkillAchieve.CoverCommonParam : buffAction.Common;
                buffAction.BuffTime = buffSkillAchieve.BuffTime != 0 ? buffSkillAchieve.BuffTime : buffAction.BuffTime;
                buffAction.Ruantity = buffSkillAchieve.Ruantity != 0 ? buffSkillAchieve.Ruantity : buffAction.Ruantity;
                if (buffAction.BuffEffect > 0) //特效 效果的
                {
                    PlayBuffSpecialEffect(buffAction, vo, attack, buffSkillAchieve);
                }
                else
                {   //buff实现 没有特效
                    PlayBuffAchieve(buffAction, vo, attack, buffSkillAchieve);
                }
            }
            else
            { 
                if (buffSkillAchieve.Trigger == 203) //203=角色击杀概率触发
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)
                    {
                        if (attack != null)
                        {
                            double value = attack.Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                            attack.PlayRecovery(attack, value);
                        }
                    }
                }else if (buffSkillAchieve.Trigger == 208) //208=释放技能击杀目标，概率触发
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)
                    {
                        if (attack != null)
                        {
                            double value = attack.Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                            attack.PlayRecovery(attack, value);
                        }
                    }
                }
            }
            
        }
        #endregion
        
        #region 被攻击时  //todo 攻击时
        public void ShowHitTargetBuff(MapMoveObject attack, MapMoveObject target,ConfigSkillUnit cfgSkill, UnitDamageVo damage, int skillAtkCount)
        {
            CastSkillVo vo = new CastSkillVo();
            vo.unitID = damage.unitID;
            vo.skillID = cfgSkill.Id;
            vo.lstTarget = new List<UnitDamageVo>();
            vo.lstTarget.Add(damage);
            
            var targetBuffs = BuffInfoManager.Instance.GetObjectBuffs(target, cfgSkill).ToList();
            foreach (var buffSkillAchieve in targetBuffs)
            {
                //被攻击时触发的才执行
                if (buffSkillAchieve.Trigger == 103 || buffSkillAchieve.Trigger == 104 || 
                    (buffSkillAchieve.Trigger == 207 && buffSkillAchieve.ActID == 2018) || buffSkillAchieve.Trigger == 209)
                {

                    if (buffSkillAchieve.Trigger == 209) //每受到10次伤害，下次受到的伤害反弹15%
                    {
                        if (attack is MapMonsterObject && MapObjectManager.Instance.GetLocalHero() != null &&
                            MapObjectManager.Instance.GetLocalHero().hurtCount != 0 &&
                            MapObjectManager.Instance.GetLocalHero().hurtCount % buffSkillAchieve.TriggerValue != 0)
                        {
                            continue;
                        }
                    }
                    else if (buffSkillAchieve.Trigger == 104 && buffSkillAchieve.AttrOrAction == 1) //宠物给角色的特殊处理
                    {
                        if (buffSkillAchieve.Type == 5 && buffSkillAchieve.HandleObject == 1)
                        {
                            if (attack.lifeShieldTime > RealTime.time || (target.Attr.HP/target.Attr.HPMax > buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX) )
                            {
                                continue;
                            }
                        }
                    }
                    else if (buffSkillAchieve.Trigger != 104)
                    {
                        //todo 测试
                        // buffSkillAchieve.TriggerValue = 10000;
                        
                        
                        
                        if (Random.Range(0.0f, 1.0f) > (buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
                            continue;
                    }
                    
                    if (buffSkillAchieve.AttrOrAction == 2) //2--状态动作和特殊处理  
                    {
                        ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID).Clone();
                        buffAction.CoverLastTime = buffSkillAchieve.CoverLastTime != 0 ? buffSkillAchieve.CoverLastTime : buffAction.CoverLastTime; //持续时间
                        buffAction.EffectTime = buffSkillAchieve.CoverEffectTime != 0 ? buffSkillAchieve.CoverEffectTime : buffAction.EffectTime; //生效时间
                        buffAction.COpFrequency = buffSkillAchieve.COpFrequency != 0 ? buffSkillAchieve.COpFrequency : buffAction.COpFrequency;  //生效次数
                        buffAction.Common = buffSkillAchieve.CoverCommonParam != "0" ? buffSkillAchieve.CoverCommonParam : buffAction.Common;
                        buffAction.BuffTime = buffSkillAchieve.BuffTime != 0 ? buffSkillAchieve.BuffTime : buffAction.BuffTime;
                        buffAction.Ruantity = buffSkillAchieve.Ruantity != 0 ? buffSkillAchieve.Ruantity : buffAction.Ruantity;

                        /* //反弹  单独处理 只反弹一次*/
                        if (buffAction.Id == (int)EN_BUFF_ACHIEVE_TYPE.Bounce)  //反弹 只反弹一次
                        {
                            var value = int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                            var damage1 = new UnitDamageVo();
                            damage1.unitID = damage.targetID;
                            damage1.targetID = damage.unitID;
                            damage1.skillID = cfgSkill.Id;
                            damage1.damage = damage.damage * value;
                            damage1.damageType = EN_DAMAGE_TYPE.Bounce;
                            if (attack != null && attack.Attr.HP > damage1.damage)
                            {
                                attack.SyncDamage(damage1);
                            }
                            continue;
                        }
                        
                        if (buffSkillAchieve.Trigger == 104)  //104=角色血量低于d%触发
                        {
                            if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.Invincible) //生命值小于百分10 无敌
                            {
                                if ((target.Attr.HP / target.Attr.HPMax > buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (target.lifeShieldTime > RealTime.time ) 
                                    {
                                        continue;
                                    }
                                    target.lifeShieldTime = RealTime.time + buffAction.EffectTime;
                                }
                            }
                            else if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.LifeShield) //生命值小于百分30 护盾
                            {
                                if (target.lifeShieldTime > RealTime.time || target.Attr.HP / target.Attr.HPMax > buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX)
                                {
                                    continue;
                                }
                                if (buffAction.CoverLastTime > 100)
                                {
                                    buffAction.CoverLastTime = buffAction.EffectTime;
                                }
                                target.lifeShieldTime = RealTime.time + buffAction.EffectTime;
                            }
                            else if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.AddBlood) //生命值小于百分50 回血
                            {
                                if ((target.Attr.HP / target.Attr.HPMax > buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (target.lifeShieldTime > RealTime.time ) 
                                    {
                                        continue;
                                    }
                                    target.lifeShieldTime = RealTime.time + buffAction.EffectTime;
                                }
                            }
                        }
                        
                        if (buffAction.BuffEffect > 0) //特效 效果的
                        {
                            PlayBuffSpecialEffect(buffAction, vo, target, buffSkillAchieve); //被攻击时，是目标携带的buff，所以对象传 目标
                        }
                        else
                        {   //buff实现 没有特效
                            PlayBuffAchieve(buffAction, vo, target, buffSkillAchieve);
                        }
                    }
                    else
                    {
                        if (buffSkillAchieve.Trigger == 103) // 受击触发
                        {
                            if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.CounterAtk ) //受击触发1次反击
                            {
                                if (buffSkillAchieve.Type == 3) //角色受击  角色天赋触发反击
                                {
                                    var counterDamage = new UnitDamageVo();
                                    counterDamage.unitID = target.id;
                                    counterDamage.targetID = vo.unitID;
                                    counterDamage.skillID = 0;
                                    counterDamage.damageType = EN_DAMAGE_TYPE.COUNTER;
                                    counterDamage.damage = MapObjectManager.Instance.GetCounterDamage(target, attack);
                                    if (attack != null && attack.Attr.HP > counterDamage.damage)
                                    {
                                        attack.SyncDamage(counterDamage);
                                        // 连击/反击提示
                                        attack.SyncAttackTip(counterDamage);
                                    }
                                }
                                else if (buffSkillAchieve.Type == 5)  //角色受击  宠物技能书触发反击  是带这个技能书的宠物反击
                                {
                                    foreach (var petObject in MapObjectManager.Instance.lstPetObj)
                                    {
                                        foreach (var bookItem in petObject.PetAttr.petItemInfo.bookSlotsList) //宠物技能书
                                        {
                                            bool isCounterAtk = false;
                                            if (bookItem.BookId != 0 && bookItem.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                            {
                                                if (bookItem.BookId == buffSkillAchieve.Id)
                                                {
                                                    foreach (var item in bookItem.battleAttrList)
                                                    {
                                                        if (item.AttrId == (int)EN_BUFF_ADD_TYPE.CounterAtk && item.AttrVal == int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) )
                                                        {
                                                            if (Random.Range(0.0f, 1.0f) < (item.AttrVal * ConstDefine.CONFIG_PLACE_EX) )
                                                            {
                                                                isCounterAtk = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (isCounterAtk)
                                            {
                                                var counterDamage = new UnitDamageVo();
                                                counterDamage.unitID = target.id;
                                                counterDamage.targetID = vo.unitID;
                                                counterDamage.skillID = 0;
                                                counterDamage.damageType = EN_DAMAGE_TYPE.COUNTER;
                                                counterDamage.damage = MapObjectManager.Instance.GetCounterDamage(petObject, attack);
                                                if (attack != null && attack.Attr.HP > counterDamage.damage)
                                                {
                                                    attack.SyncDamage(counterDamage);
                                                    // 连击/反击提示
                                                    attack.SyncAttackTip(counterDamage);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.Mitigation)
                            {
                                damage.damage -= damage.damage * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                            }
                            else if (buffSkillAchieve.Type == 8 && int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)
                            {
                                double value = target.Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                target.PlayRecovery(target, value);
                            }
                        }
                        else if (buffSkillAchieve.Trigger == 104) // 角色血量
                        {
                            if (buffSkillAchieve.Type == 5 && buffSkillAchieve.HandleObject == 1)
                            {
                                if (attack.lifeShieldTime < RealTime.time && (target.Attr.HP/target.Attr.HPMax < buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX) )
                                {
                                    attack.lifeShieldTime = RealTime.time + buffSkillAchieve.LastTime;
                                    double value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), value);
                                }
                            }
                        }
                        
                    }
                }
            }

            if (target != null)
            {
                if (target.IsBuffInvincible)  //无敌buff状态 不扣血
                {
                    damage.damage = 0;
                }
                if (target.isRoleHurtReduced != 0)  // 2016	受到的伤害减少
                {   //天赋 每释放5个技能，下次受到的伤害减少10%
                    damage.damage -= damage.damage * target.isRoleHurtReduced;
                    damage.damage = damage.damage < 0 ? 0 : damage.damage;
                    target.isRoleHurtReduced = 0;
                }
                if (target.BounceValue != 0)  //持续反弹状态
                {
                    var damage1 = new UnitDamageVo();
                    damage1.unitID = damage.targetID;
                    damage1.targetID = damage.unitID;
                    damage1.skillID = cfgSkill.Id;
                    damage1.damage = damage.damage * target.BounceValue;
                    damage1.damageType = EN_DAMAGE_TYPE.Bounce;
                    if (attack != null && attack.Attr.HP > damage1.damage)
                    {
                        attack.SyncDamage(damage1);
                    }
                }
                if (target is MapHeroObject && target.petSkillRecover != 0)
                {
                    var damage2 = new UnitDamageVo();
                    damage2.unitID = target.id;
                    damage2.targetID = target.id;
                    damage2.skillID = 0;
                    damage2.damage = target.petSkillRecover;
                    damage2.damageType = EN_DAMAGE_TYPE.RECOVERY;
                    target.SyncDamage(damage2);
                }
            }
            
            if (attack != null && attack.Attr.DemageUpRate != 0) //伤害提升
            {
                damage.damage += damage.damage * attack.Attr.DemageUpRate * ConstDefine.CONFIG_PLACE_EX;
            }
        }
        #endregion
        
        #region 攻击时  //todo 攻击时
        public void ShowAttackBuff(ConfigSkillUnit cfgSkill, CastSkillVo vo, MapMoveObject attack,int skillAtkCount)
        {
            
            //todo 测试。。。
            // if (attack is MapSkillProxy || attack is MapPetObject)
            // {
                // cfgSkill.Id = 10201008;
                // cfgSkill.Id = 10301013;
                //  cfgSkill.Id = 10401017;
            // }
            
            
                
            //攻击方 buff
            attackBuffs.Clear();
            attackBuffs = BuffInfoManager.Instance.GetSkillAchieveBySkillId(cfgSkill.Id).ToList();
            attackBuffs.AddRange(BuffInfoManager.Instance.GetObjectBuffs(attack, cfgSkill).ToList());
            
            foreach (var buffSkillAchieve in attackBuffs)
            {
                if (buffSkillAchieve.Trigger == 105 || buffSkillAchieve.Trigger == 108 || //击杀 死亡触发 死亡时处理
                    buffSkillAchieve.Trigger == 203 || buffSkillAchieve.Trigger == 208 || 
                    (buffSkillAchieve.Trigger == 103 && buffSkillAchieve.Type != 10) || //被攻击时触发 被攻击时处理  不是宠物技能
                    buffSkillAchieve.Trigger == 104 || buffSkillAchieve.Trigger == 209 || 
                    (buffSkillAchieve.Trigger == 207 && buffSkillAchieve.ActID == 2018) // c宠物技能书
                   )
                {
                    continue; 
                }
                if (buffSkillAchieve != null && buffSkillAchieve.BuffTime == 1 )
                {
                    HandleBuffInfo(buffSkillAchieve.Clone(), vo, attack, skillAtkCount, cfgSkill);
                }
                else
                {
                    ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID);
                    if (buffAction != null && buffAction.BuffTime == 1)
                    {
                        HandleBuffInfo(buffSkillAchieve.Clone(), vo, attack, skillAtkCount, cfgSkill);
                    }
                }
            }
        }
        
        private string[] commonStr = null;
        /// <summary>
        /// buff信息处理
        /// </summary>
        /// <param name="cfgSkill"></param>
        /// <param name="vo"></param>
        private int bombTargetId = 0;  //爆炸
        private bool isLightning = false; // 闪电
        private void HandleBuffInfo(ConfigSkillAchieveUnit buffSkillAchieve, CastSkillVo vo, MapMoveObject attack,int skillAtkCount,ConfigSkillUnit cfgSkill)
        {
            if (buffSkillAchieve ==null || buffSkillAchieve.AttrOrAction == -1) return;
            if (buffSkillAchieve.Trigger == 202) //角色 释放技能触发
            {
                if (!(attack is MapSkillProxy) && !(attack is MapHeroSkillProxy))
                    return;
            }
            else if (buffSkillAchieve.Trigger == 101) //宠物 普攻触发
            {
                if (!(attack is MapPetObject))
                    return;
            }
            else if (buffSkillAchieve.Trigger == 102) //宠物 技能触发
            {
                if (!IsPetSkill(attack, cfgSkill) )
                {
                    return;
                }
                else
                {
                    if ((attack as MapSkillProxy).isRepeatCast && buffSkillAchieve.ActID == (int)EN_BUFF_ACHIEVE_TYPE.DoubleCastSkill)
                    {
                        (attack as MapSkillProxy).isRepeatCast = false;
                        return;
                    }
                }
            }
            else if (buffSkillAchieve.Trigger == 211 && !attack.nextNormalAtk) //技能释放触发
            {
                if (!(attack is MapHeroSkillProxy) && !(attack is MapSkillProxy))
                    return;
            }
            
            if (attack is MapMonsterObject && TriggerRoleHoly(attack, buffSkillAchieve.ActID) != 0)
            {   //怪物  触发角色穿戴圣物
                buffSkillAchieve.TriggerValue -= TriggerRoleHoly(attack, buffSkillAchieve.ActID);
            }
            
            if (buffSkillAchieve.Trigger == 204)
            {
                if (attack.atkCount == 0 || attack.atkCount % buffSkillAchieve.TriggerValue != 0) //普攻每5次攻击，下一次普攻伤害提升230%
                    return;
            }
            else if (buffSkillAchieve.Trigger == 205)
            {
                if (skillAtkCount == 0 || skillAtkCount % buffSkillAchieve.TriggerValue != 0 || (!(attack is MapSkillProxy) && !(attack is MapHeroSkillProxy)) ) //每释放10次技能，获得1个10%生命值的护盾，持续3秒
                    return;
            }
            else if (buffSkillAchieve.Trigger == 106)  //定时触发
            {
                if (attack is MapPetObject)
                {
                    if (attack.lifeShieldTime < RealTime.time)
                    {
                        attack.lifeShieldTime = RealTime.time + buffSkillAchieve.TriggerValue;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            else if (buffSkillAchieve.Trigger == 107)  //宠物释放技能
            {
                if (attack is MapSkillProxy)
                {
                    if (!IsPetSkill(attack, cfgSkill) )
                    {
                        return;
                    }
                    else
                    {
                        if (MapObjectManager.Instance.GetLocalHero() != null)
                        {
                            if (MapObjectManager.Instance.GetLocalHero().Attr.HP/MapObjectManager.Instance.GetLocalHero().Attr.HPMax > buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX)
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else if (buffSkillAchieve.Trigger != 207 && buffSkillAchieve.Trigger != 0) // 207 天赋 生命值每降低就会触发
                // (buffSkillAchieve.Trigger == 101 || buffSkillAchieve.Trigger == 102 ||
                //      buffSkillAchieve.Trigger == 103 || buffSkillAchieve.Trigger == 105 ||
                //      buffSkillAchieve.Trigger == 201 || buffSkillAchieve.Trigger == 202 ||
                //      buffSkillAchieve.Trigger == 203 || buffSkillAchieve.Trigger == 208 ||
                //      buffSkillAchieve.Trigger == 206 )
            {
                
                
                
                //todo 测试
                // buffSkillAchieve.TriggerValue = 10000;
                
                
                

                if (buffSkillAchieve.Trigger == 211 && attack is MapHeroObject && attack.nextNormalAtk)
                {
                    attack.nextNormalAtk = false;
                    buffSkillAchieve.TriggerValue = 10000;
                }
                if (Random.Range(0.0f, 1.0f) > (buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE_EX))
                {
                    return;
                }

            }

            if (buffSkillAchieve.Trigger == 211 && (attack is MapSkillProxy || attack is MapHeroSkillProxy)) //211=技能释放后，第一次普攻
            {
                MapObjectManager.Instance.GetLocalHero().nextNormalAtk = true;
                return;
            }
            
            if (buffSkillAchieve.AttrOrAction == 2) //2--状态动作和特殊处理  
            {
                ConfigBuffActionTemplateUnit buffAction = ConfigUtils.GetBuffActionByBuffId(buffSkillAchieve.ActID).Clone();
                buffAction.CoverLastTime = buffSkillAchieve.CoverLastTime != 0 ? buffSkillAchieve.CoverLastTime : buffAction.CoverLastTime; //持续时间
                buffAction.EffectTime = buffSkillAchieve.CoverEffectTime != 0 ? buffSkillAchieve.CoverEffectTime : buffAction.EffectTime; //生效时间
                buffAction.COpFrequency = buffSkillAchieve.COpFrequency != 0 ? buffSkillAchieve.COpFrequency : buffAction.COpFrequency;  //生效次数
                buffAction.Common = buffSkillAchieve.CoverCommonParam != "0" ? buffSkillAchieve.CoverCommonParam : buffAction.Common;
                buffAction.BuffTime = buffSkillAchieve.BuffTime != 0 ? buffSkillAchieve.BuffTime : buffAction.BuffTime;
                buffAction.Ruantity = buffSkillAchieve.Ruantity != 0 ? buffSkillAchieve.Ruantity : buffAction.Ruantity;
                if (buffSkillAchieve.Type == 10) // 宠物初始技能
                {
                    buffAction.BuffEffect = buffSkillAchieve.BuffEffect != 0 ? buffSkillAchieve.BuffEffect : buffAction.BuffEffect;
                }else if (buffSkillAchieve.Type == 5) // 宠物技能书
                {
                    if (commonStr == null)
                    {
                        ConfigCommonUnit _commonUnit300011 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(300011);
                        commonStr = _commonUnit300011.Param1.Split(",");
                    }

                    foreach (var strId in commonStr)
                    {
                        if (int.Parse(strId) ==  buffSkillAchieve.ActID && buffAction.Common.Split(",").Length > 1)
                        {
                            foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
                            {
                                if (attack.Attr.PetGuid != 0 && petItem.PetGuid == attack.Attr.PetGuid)
                                {
                                    foreach (var bookSlot in petItem.bookSlotsList)
                                    {
                                        if (bookSlot.BookId == buffSkillAchieve.Id && bookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                        {
                                            foreach (var item in bookSlot.battleAttrList)
                                            {
                                                if (item.AttrId == int.Parse(buffAction.Common.Split(",")[0]))
                                                {
                                                    buffAction.Common = item.AttrId + "," + item.AttrVal;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    
                }
                
                if (buffSkillAchieve.Trigger == 0 && buffSkillAchieve.ActID == 1004) //怪的 buff  特殊处理 回春
                {
                    if (attack is MapMonsterObject)
                    {
                        if ((int)attack.lifeShieldTime < (int)RealTime.time)
                            attack.lifeShieldTime = RealTime.time + buffAction.EffectTime;
                        else
                            return;
                    }
                    else
                        return;
                }
                
                if (buffAction.BuffEffect > 0) //特效 效果的
                {
                    PlayBuffSpecialEffect(buffAction, vo, attack, buffSkillAchieve);
                }
                else
                {   //buff实现 没有特效
                    PlayBuffAchieve(buffAction, vo, attack, buffSkillAchieve);
                }
            }
            else
            {
                //0=无条件加属性   1--属性变动     调整属性值  是服务器下发 不用管
                if (buffSkillAchieve.Trigger == 101) // 101=宠物普攻概率触发，
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.ComboAtk ||  //连击
                        int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.CriticalStrike || //提升暴击
                        int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.CriticalInjury ||  //提升爆伤
                        int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.AtkSpeed ||  // 攻速
                        int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.ATK ||  // 攻击
                        int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.PetAtkADD  // 宠物伤害加成
                        ) 
                    {
                        foreach (var damage in vo.lstTarget)
                        {
                            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                            if (objTarget != null && objTarget.Attr.HP > damage.damage)
                            {
                                //连击 马上触发单独
                                if (buffSkillAchieve.LastTime == 0 && int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.ComboAtk)
                                {
                                    var damage1 = new UnitDamageVo();
                                    damage1.unitID = damage.unitID;
                                    damage1.targetID = damage.targetID;
                                    damage1.skillID = damage.skillID;
                                    damage1.damage = damage.damage;
                                    damage1.damageType = EN_DAMAGE_TYPE.COMBO;
                                    vo.lstTarget.Add(damage1);
                                    break;
                                }
                                else
                                {
                                    foreach (var bookSlot in (attack as MapPetObject).PetAttr.petItemInfo.bookSlotsList)
                                    {
                                        if (bookSlot.BookId == buffSkillAchieve.Id && bookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                        {
                                            foreach (var item in bookSlot.battleAttrList)
                                            {
                                                SetObjectAttribute(attack,objTarget,true, buffSkillAchieve.LastTime, item.AttrId, (int)item.AttrVal);
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                else if (buffSkillAchieve.Trigger == 102) //宠物释放技能触发
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.IgnoreDef)
                    {
                        attack.Attr.IgnoreDef += int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                        foreach (var damage in vo.lstTarget)
                        {
                            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                            if (objTarget != null )
                            {   //防御改变  重新算伤害
                                damage.damage = MapObjectManager.Instance.GetBaseDamage(damage.skillID, attack, objTarget, CastSkillType.None);
                            }
                        }
                    }
                }
                else if (buffSkillAchieve.Trigger == 107)
                {
                    if (buffSkillAchieve.Type == 5 && buffSkillAchieve.HandleObject == 1 && MapObjectManager.Instance.GetLocalHero() != null)
                    {
                        double value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                        MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), value);
                    }
                }
                else if (buffSkillAchieve.Trigger == 201) // 201=角色普攻概率触发，
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.SkillCd ) //技能CD
                    {
                        PlayResetSkillCD(attack, int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                    }
                    else if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.ComboAtk ) //连击
                    {
                        foreach (var damage in vo.lstTarget)
                        {
                            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                            if (objTarget != null && objTarget.Attr.HP > damage.damage)
                            {
                                var damage1 = new UnitDamageVo();
                                damage1.unitID = damage.unitID;
                                damage1.targetID = damage.targetID;
                                damage1.skillID = damage.skillID;
                                damage1.damage = damage.damage;
                                damage1.damageType = EN_DAMAGE_TYPE.COMBO;
                                vo.lstTarget.Add(damage1);
                                break;
                                // objTarget.SyncDamage(damage1);
                                // // 连击/反击提示
                                // attack.SyncAttackTip(damage1);
                            }
                        }
                    }
                    else if (buffSkillAchieve.Type == 8 && int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)  //怪的普攻回血
                    {
                        double value = attack.Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                        attack.PlayRecovery(attack, value);
                    }
                }
                else if (buffSkillAchieve.Trigger == 202 && (attack is MapSkillProxy || attack is MapHeroSkillProxy)) //释放技能时，5%概率回复自身2%生命值
                {
                    if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP )
                    {
                        if (MapObjectManager.Instance.GetLocalHero() != null)
                        {
                            double value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                            MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), value);
                        }
                    }
                    else if (buffSkillAchieve.Type == 5 && int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.IgnoreDef)  // 有技能书 修改 角色 无视防御属性
                    {
                        if (MapObjectManager.Instance.GetLocalHero() != null)
                        {
                            double num = 0;
                            foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
                            {
                                foreach (var bookSlot in (attack as MapPetObject).PetAttr.petItemInfo.bookSlotsList)
                                {
                                    if (bookSlot.BookId == buffSkillAchieve.Id && bookSlot.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                    {
                                        foreach (var item in bookSlot.battleAttrList)
                                        {
                                            if (item.AttrId == (int)EN_BUFF_ADD_TYPE.IgnoreDef)
                                            {
                                                num += item.AttrVal;
                                            }
                                        }
                                    }
                                }
                            }

                            if (num > 0)
                            {
                                DataManager.Instance.GetRoleData().FightAttrVo.IgnoreDef += num;
                                foreach (var damage in vo.lstTarget)
                                {
                                    MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                                    if (objTarget != null && objTarget.Attr.HP > damage.damage)
                                    {
                                        if (cfgSkill.SkillType == (int)EN_SKILL_TYPE.XP)
                                        {
                                            damage.damage = MapObjectManager.Instance.GetBaseDamage(cfgSkill.Id,objTarget, attack, CastSkillType.Hero);
                                        }else 
                                            damage.damage = MapObjectManager.Instance.GetBaseDamage(cfgSkill.Id,objTarget, attack, CastSkillType.SkillProxy);
                                    }
                                }
                                DataManager.Instance.GetRoleData().FightAttrVo.IgnoreDef -= num;
                            }
                        }
                        
                    }
                }
                else if (buffSkillAchieve.Trigger == 205)  //205=角色技能每释放d次触发
                { 
                    if (skillAtkCount != 0 && skillAtkCount % buffSkillAchieve.TriggerValue == 0)
                    {
                        if (attack is MapHeroSkillProxy || attack is MapSkillProxy)
                        {
                            //每释放5次技能，回复自身2%生命值
                            if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP )
                            {
                                if (MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    double value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax *
                                                   int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), value);
                                }
                            }
                            else if (int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.Def)
                            {
                                foreach (var damage in vo.lstTarget)
                                {
                                    MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                                    if (objTarget != null)
                                    {
                                        SetObjectAttribute(attack,objTarget,false, buffSkillAchieve.LastTime, int.Parse(buffSkillAchieve.CommonAttr.Split(",")[0]), int.Parse(buffSkillAchieve.CommonAttr.Split(",")[1]));
                                    }
                                }
                            }
                        }
                    }
                }
                else if (buffSkillAchieve.Trigger == 206)  //206=角色释放技能后
                {
                    if (buffSkillAchieve.DevSide == 0)
                    {
                        foreach (var damage in vo.lstTarget)
                        {
                            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                            if (objTarget != null)
                            {
                                string[] attrs = buffSkillAchieve.CommonAttr.Split("|");
                                for (int i = 0; i < attrs.Length; i++)
                                {
                                    string[] attr = attrs[i].Split(",");
                                    if (attr.Length > 1)
                                        SetObjectAttribute(attack,objTarget,false, buffSkillAchieve.LastTime, int.Parse(attr[0]), int.Parse(attr[1]));
                                }
                            }
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// buff实现  没有特效效果的
        /// </summary>
        private void PlayBuffAchieve(ConfigBuffActionTemplateUnit buffAction, CastSkillVo vo, MapMoveObject attack, ConfigSkillAchieveUnit buffSkillAchieve)
        {
            foreach (var damage in vo.lstTarget)
            {
                MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                if (objTarget != null)
                {
                    switch (buffAction.Id)
                    {
                        case (int)EN_BUFF_ACHIEVE_TYPE.Bounce: // 1006 持续反弹
                            PlayBounceEffect(objTarget, attack as MapMoveObject, damage, buffAction);
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.ResetSkillCD: // 1008 重置CD
                            PlayResetSkillCD(attack);
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.DizaayRate: // 1012 减少怪物眩晕概率10%
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.RecoveryRate: // 1013 怪物回复效果比例减少1%
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.DefHurt: // 2003	追加防御值伤害
                            string[] attrs = buffAction.Common.Split("|");
                            string[] attr = attrs[0].Split(",");
                            if (attr.Length > 1)
                            {
                                if ((attack is MapHeroSkillProxy || attack is MapSkillProxy) && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    attack = MapObjectManager.Instance.GetLocalHero();
                                }
                                if (objTarget.Attr.HP > (attack.Attr.Def * int.Parse(attr[1]) * ConstDefine.CONFIG_PLACE_EX))
                                {
                                    var extraDamage = new UnitDamageVo();
                                    extraDamage.unitID = damage.unitID;
                                    extraDamage.targetID = damage.targetID;
                                    extraDamage.skillID = 0;
                                    extraDamage.damage = attack.Attr.Def * int.Parse(attr[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    objTarget.SyncDamage(extraDamage);
                                }
                            }
                            // if (buffSkillAchieve.Trigger == 206) // 技能自带 
                            // {}else if (buffSkillAchieve.Trigger == 202)  // 释放技能攻击
                            // {}else if (buffSkillAchieve.Trigger == 201)  // 普攻 
                            // {}
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.HurtAdd: // 2004	追加防御值伤害
                            if (objTarget.Attr.HP / objTarget.Attr.HPMax < 0.1)
                            {
                                string[] attrsHurt = buffAction.Common.Split("|");
                                string[] attrHurt = attrsHurt[0].Split(",");
                                if (attrHurt.Length > 1)
                                {
                                    damage.damage += damage.damage * int.Parse(attrHurt[1]) * ConstDefine.CONFIG_PLACE_EX;
                                }
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.HpHurt: // 2005 追加生命值伤害
                        case (int)EN_BUFF_ACHIEVE_TYPE.HpPercentHurt: // 2008 造成目标当前生命值d%的伤害
                            if (int.Parse(buffAction.Common.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)
                            {
                                if (objTarget.Attr.HP > (MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX))
                                {
                                    var extraDamage = new UnitDamageVo();
                                    extraDamage.unitID = damage.unitID;
                                    extraDamage.targetID = damage.targetID;
                                    extraDamage.skillID = 0;
                                    extraDamage.damage = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    objTarget.SyncDamage(extraDamage);
                                }
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.AttrHurt: // 2006 追加主属性
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.HurtRecovery: // 2007 受击回复攻击者伤害值比例的生命
                            if (int.Parse(buffAction.Common.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP )
                            {
                                double value = damage.damage * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                attack.PlayRecovery(attack, value);
                            }
                            break;
                        // case (int)EN_BUFF_ACHIEVE_TYPE.HpPercentHurt: // 2008 造成目标当前生命值d%的伤害
                            ////和2005一样
                            // break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.PetLevelHurt: // 2009 为角色提供等同自身宠物等级比例的属性加成
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.PetHurtRatio: // 2010 追加主属性
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.PetHurtRecovery: // 2011 受击回复攻击者伤害值比例的生命
                            if (buffSkillAchieve.Type == 5 && MapObjectManager.Instance.GetLocalHero() != null)
                            {
                                if (int.Parse(buffAction.Common.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP )
                                {
                                    double value = damage.damage * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    MapObjectManager.Instance.GetLocalHero().PlayRecovery(MapObjectManager.Instance.GetLocalHero(), value);
                                }
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.DoubleCastSkill: // 2012 释放技能时，法术概率再释放1次
                            (attack as MapSkillProxy).isRepeatCast = true;
                            (attack as MapSkillProxy).ResetSkillCDWithId(damage.skillID);
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.UpgradeHurt: // 2013 普攻对同目标伤害持续提升，最多提升至X%
                            string[] info = (attack as MapPetObject).targetIdWithNum.Split(",");
                            if (info.Length > 1)
                            {
                                if (int.Parse(info[0]) == damage.targetID)
                                {   // buffAction.Common[0]  提升值    buffAction.Common[1]  最大提升值
                                    damage.damage += damage.damage * int.Parse(info[1]) * (int.Parse(buffAction.Common.Split(",")[0]) * ConstDefine.CONFIG_PLACE_EX);
                                    
                                    // if (!UnityEngine.Mathf.Approximately((int.Parse(info[1]) * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX)),1.0f))
                                    if ( ( int.Parse(info[1]) * int.Parse(buffAction.Common.Split(",")[0]) ) < int.Parse(buffAction.Common.Split(",")[1]) )
                                    {
                                        (attack as MapPetObject).targetIdWithNum = damage.targetID + "," + (int.Parse(info[1]) + 1);
                                    }
                                }
                                else
                                {
                                    (attack as MapPetObject).targetIdWithNum = damage.targetID +",1";
                                }
                            }
                            else
                            {
                                (attack as MapPetObject).targetIdWithNum = damage.targetID +",1";
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.RoleAttr:  // 2014 减少怪物对玩家的属性降低
                            
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.DamageUp:  // 2015 伤害提升
                            if (buffSkillAchieve.Trigger == 105) //宠物技能书 每击杀 1 个敌人，伤害提升
                            {
                                DemageUp(attack, buffAction);
                            }
                            else if (buffSkillAchieve.Trigger == 204) //普攻每5次攻击，下一次普攻伤害提升240%
                            {
                                damage.damage = damage.damage * int.Parse(buffAction.Common) * ConstDefine.CONFIG_PLACE_EX;
                            }
                            else if (buffSkillAchieve.Trigger == 205) //天赋 每释放5个技能，下次技能伤害提升%
                            {
                                if (attack is MapHeroSkillProxy || attack is MapSkillProxy)
                                {
                                    if (buffAction.Common.Split(",").Length <= 1)
                                    {
                                        damage.damage += damage.damage * int.Parse(buffAction.Common) * ConstDefine.CONFIG_PLACE_EX;
                                    }
                                }
                            }
                            else if (buffSkillAchieve.Trigger == 207) //生命值降低
                            {
                                if ( (buffSkillAchieve.Type == 3 || buffSkillAchieve.Type == 5 ) && attack is MapHeroObject) //天赋 生命值每降低10%伤害提升
                                {
                                    if (attack is MapPetObject)
                                    {
                                        attack = MapObjectManager.Instance.GetLocalHero();
                                    }
                                    if (attack.Attr.HPMax > 0)
                                    {
                                        var num2 = (attack.Attr.HP / attack.Attr.HPMax).ToString("0.0");
                                        var num = (1- float.Parse( num2 ) ) * 10 ;
                                        damage.damage += damage.damage * num * int.Parse(buffAction.Common) * ConstDefine.CONFIG_PLACE_EX;
                                    }
                                }else if (buffSkillAchieve.Type == 8 && attack.Attr.HP > 0)  //怪物 血量越低 伤害越高 每损失百分5血量  伤害提升百分50
                                {
                                    int changePercent = (int)((attack.Attr.HPMax - attack.Attr.HP) / attack.Attr.HPMax * 100);
                                    int num3 = (int)(changePercent / (buffSkillAchieve.TriggerValue * ConstDefine.CONFIG_PLACE) );
                                    damage.damage += damage.damage * num3 * int.Parse(buffAction.Common) * ConstDefine.CONFIG_PLACE_EX;
                                }
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.DamageReduce:  // 2016 受到的伤害减少
                            if (buffSkillAchieve.Trigger == 205) 
                            {
                                if ((attack is MapHeroSkillProxy || attack is MapSkillProxy) && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    attack = MapObjectManager.Instance.GetLocalHero();
                                }
                                attack.isRoleHurtReduced = int.Parse(buffAction.Common) * ConstDefine.CONFIG_PLACE_EX;
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.TargetLifeDamage:  // 2017 造成目标自身生命值d%的伤害
                            string[] attrs2017 = buffAction.Common.Split("|");
                            string[] attr2017 = attrs2017[0].Split(",");
                            if (attr2017.Length > 1)
                            {
                                if (objTarget.Attr.HP > (MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(attr2017[1]) * ConstDefine.CONFIG_PLACE_EX))
                                {
                                    var extraDamage = new UnitDamageVo();
                                    extraDamage.unitID = damage.unitID;
                                    extraDamage.targetID = damage.targetID;
                                    extraDamage.skillID = 0;
                                    extraDamage.damage = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(attr2017[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    objTarget.SyncDamage(extraDamage);
                                }
                            }
                            break;
                        case (int)EN_BUFF_ACHIEVE_TYPE.CastPetSkill:  // 2018 再释放一次宠物技能
                            if (objTarget is MapHeroObject && objTarget.ObjectType == MapObjectType.Hero)
                            {
                                foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
                                {
                                    var isExit = false;
                                    foreach (var bookItem in petItem.bookSlotsList)  //宠物技能书
                                    {
                                        if (bookItem.BookId != 0 && bookItem.SlotStatus == eSlotStatus.eSlotStatus_Normal)
                                        {
                                            if (bookItem.BookId == buffSkillAchieve.Id)
                                            {
                                                isExit = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (isExit)
                                    {
                                        if (buffSkillAchieve.Trigger == 103) //角色被攻击 触发宠物释放技能
                                        {
                                            foreach (var item in RoleManager.Instance.GetMapSkillProxy())
                                            {
                                                if (item.Value.Attr.skillXPID == petItem.skillId)
                                                {
                                                    item.Value.ResetSkillCDWithId(petItem.skillId);
                                                }
                                            }
                                        }else if (buffSkillAchieve.Trigger == 207) //角色没降低百分10 触发宠物释放技能
                                        {
                                            double changePercent = (attack.Attr.HPMax - attack.Attr.HP) / attack.Attr.HPMax * 100;
                                            if (changePercent !=0 && UnityEngine.Mathf.Approximately((float)changePercent % 10 , 0f) )
                                            {
                                                foreach (var item in RoleManager.Instance.GetMapSkillProxy())
                                                {
                                                    if (item.Value.Attr.skillXPID == petItem.skillId)
                                                    {
                                                        item.Value.ResetSkillCDWithId(petItem.skillId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private double petAtk = 0;
        private int effectIndex = 0;
        private GLoader3D loadBottom = null;
        /// <summary>
        /// 播放buff表现 有特效效果的
        /// </summary>
        private void PlayBuffSpecialEffect(ConfigBuffActionTemplateUnit buffAction, CastSkillVo vo, MapMoveObject attack, ConfigSkillAchieveUnit buffSkillAchieve)
        {
            if (buffAction.Object == 1) //给自己
            {
                if (attack is MapPetObject)
                {
                    petAtk = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.Atk;
                }
                if (attack is MapHeroSkillProxy || attack is MapSkillProxy || attack is MapPetObject)
                {
                    attack = MapObjectManager.Instance.GetLocalHero();
                }

                if (buffSkillAchieve.Type != 10)
                {
                    if (attack != null && attack.HitSpineDic.TryGetValue(buffAction.Id, out GLoader3D load3D))
                    {
                        load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                        load3D.animationName = "Common_" + buffAction.BuffEffect + "_buff";
                        load3D.frame = 0;
                        load3D.loop = false;
                        load3D.playing = true;
                        load3D.visible = true;
                        load3D.parent.SetChildIndex(load3D, 100);//显示最上层

                        switch (buffAction.Id)
                        {
                            case (int)EN_BUFF_TYPE.AddBlood: // 1004 回春 回血
                                load3D.playing = false;
                                load3D.visible = false;
                                PlayAddHpEffect(attack as MapMoveObject, buffAction, buffSkillAchieve);
                                break;
                            case (int)EN_BUFF_TYPE.Invincible: // 1005 无敌
                                load3D.loop = true;
                                load3D.SetXY(10, -88);
                                PlayInvincibleEffect(attack as MapMoveObject, buffAction);
                                break;
                            case (int)EN_BUFF_TYPE.LifeShield: // 1009 生命转护盾
                            case ((int)EN_BUFF_TYPE.LifeShield1):  // 1010 攻击转护盾
                            case ((int)EN_BUFF_TYPE.LifeShield2): // 1011防御转护盾
                                load3D.playing = false;
                                load3D.visible = false;
                                if (vo.lstTarget.Count > 0)
                                {
                                    load3D.SetXY(10, -128);
                                    load3D.loop = true;
                                    load3D.playing = true;
                                    load3D.visible = true;
                                    PlayBuffLifeShieldEffect(attack as MapMoveObject, vo.lstTarget[0], buffAction, buffSkillAchieve, petAtk);
                                }
                                break;
                        }
                    }
                }
                else
                {   //宠物初始技能 给角色的buff
                    if (attack != null && attack.HitSpineDic.TryGetValue(buffAction.BuffEffect, out GLoader3D load3DPetSkill))
                    {
                        //上层
                        load3DPetSkill.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                        load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff";
                        load3DPetSkill.frame = 0;
                        load3DPetSkill.loop = true;
                        load3DPetSkill.playing = true;
                        load3DPetSkill.visible = true;
                        load3DPetSkill.SetXY(-20, -88);
                        load3DPetSkill.parent.SetChildIndex(load3DPetSkill, 100);//显示最上层
                        
                        //下层
                        loadBottom = null;
                        if (attack.HitSpineDic.ContainsKey(int.Parse(buffAction.BuffEffect+"1")))
                        {
                            loadBottom = attack.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")];
                            loadBottom.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                            loadBottom.animationName = buffAction.BuffEffect + "_buff2";
                            loadBottom.frame = 0;
                            loadBottom.loop = true;
                            loadBottom.playing = true;
                            loadBottom.visible = true;
                            loadBottom.SetXY(-20, -100);
                            load3DPetSkill.parent.RemoveChild(loadBottom);
                            load3DPetSkill.parent.AddChildAt(loadBottom, 0);
                        }

                        switch (buffAction.BuffEffect)
                        {
                            case ((int)EN_BUFF_TYPE.PetAtk):  // 宠物释放技能后为角色提供等同自身宠物伤害{0}%的主属性，持续3秒
                                load3DPetSkill.animationName = buffAction.BuffEffect + "_buff1";
                                loadBottom.animationName = buffAction.BuffEffect + "_buff2";
                                break;
                            case ((int)EN_BUFF_TYPE.PetAtkAdd):  // 宠物释放技能后为角色提供等同自身宠物等级{0}%的伤害加成，持续3秒
                                load3DPetSkill.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>()?.AnimationState.SetAnimation(0, buffAction.BuffEffect + "_buff1", true); // 基础层动画
                                load3DPetSkill.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>()?.AnimationState.SetAnimation(1, buffAction.BuffEffect + "_buff2", true);
                                loadBottom.animationName = buffAction.BuffEffect + "_buff3";
                                break;
                            case ((int)EN_BUFF_TYPE.ElementAdd):  // 10201009 宠物释放技能后为角色提供等同自身宠物等级{0}%的伤害加成，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff_shang";
                                load3DPetSkill.SetXY(-20, -108);
                                
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff_xia";
                                break;
                            case ((int)EN_BUFF_TYPE.BaoJiAdd):  // 宠物释放技能后为角色提供等同自身宠物等级{0}%的伤害加成，持续3秒
                                load3DPetSkill.animationName = "fengshang";
                                load3DPetSkill.SetXY(-20, -88);
                                loadBottom.animationName = "fengxia";
                                break;
                            case ((int)EN_BUFF_TYPE.BaoShangAdd):  // 10201011 宠物释放技能后为角色提供等同自身宠物等级{0}%的伤害加成，持续3秒
                                load3DPetSkill.animationName = "dian";
                                load3DPetSkill.SetXY(-20, -88);
                                loadBottom.animationName = "quan";
                                loadBottom.SetXY(-20, -105);
                                break;
                            case ((int)EN_BUFF_TYPE.PetRecovery):  // 10301012 宠物释放技能后回复角色等同自身宠物伤害{0}%的血量
                                load3DPetSkill.loop = false;
                                load3DPetSkill.SetXY(-20, -120);
                                
                                if (GetBattlePetByPetSkillId(vo.skillID) != null && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    double value = GetBattlePetByPetSkillId(vo.skillID).FightAttrVo.Atk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    // double value = 18 * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    var damage = new UnitDamageVo();
                                    damage.unitID = attack.id;
                                    damage.targetID = attack.id;
                                    damage.skillID = 0;
                                    damage.damage = value;
                                    damage.damageType = EN_DAMAGE_TYPE.RECOVERY;
                                    attack.SyncDamage(damage);
                                }
                                break;
                            case ((int)EN_BUFF_TYPE.PetRecoveryTime):  // 10301013 宠物释放技能后回复角色等同自身宠物伤害{0}%的血量，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff1";
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff2";
                                
                                if (GetBattlePetByPetSkillId(vo.skillID) != null && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    double hpValue = GetBattlePetByPetSkillId(vo.skillID).FightAttrVo.Atk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                    PlayAddHpEffect(attack as MapMoveObject, buffAction, buffSkillAchieve, hpValue);
                                }
                                break;
                            case ((int)EN_BUFF_TYPE.PetLifeShield):  // 10301014 宠物释放技能后为角色提供等同自身宠物伤害{0}%的生命护盾，持续3秒
                                load3DPetSkill.SetXY(-25, -150);
                                SkeletonAnimation skeleton = load3DPetSkill.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                                if (skeleton != null)
                                {
                                    skeleton.AnimationState.SetAnimation(0, "Common_" + buffAction.BuffEffect + "_buff1", false).Complete +=
                                        (Spine.TrackEntry track) =>
                                        {
                                            skeleton.AnimationState.SetAnimation(0, "Common_" + buffAction.BuffEffect + "_buff2", true);
                                        };
                                }
                                
                                if (GetBattlePetByPetSkillId(vo.skillID) != null && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    PlayBuffLifeShieldEffect(attack as MapMoveObject, vo.lstTarget[0], buffAction, buffSkillAchieve, GetBattlePetByPetSkillId(vo.skillID).FightAttrVo.Atk);
                                }
                                break;
                            case ((int)EN_BUFF_TYPE.PetGongJiRecovery):  // 10301015 宠物释放技能后提供角色等同自身宠物伤害{0}%的攻击回复，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff1";
                                load3DPetSkill.loop = true;
                                load3DPetSkill.SetXY(-20, -100);
                                
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff2";
                                loadBottom.SetXY(-20, -100);
                                break;
                            case ((int)EN_BUFF_TYPE.PetShouJiRecovery):  // 10301016 角色受到攻击时，回复等同自身宠物伤害{0}%的血量，持续3秒
                                load3DPetSkill.loop = true;
                                
                                if (GetBattlePetByPetSkillId(vo.skillID) != null && MapObjectManager.Instance.GetLocalHero() != null)
                                {
                                    attack.petSkillRecover = GetBattlePetByPetSkillId(vo.skillID).FightAttrVo.Atk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                }
                                break;
                            case ((int)EN_BUFF_TYPE.PetDef): //10401017 宠物释放技能后为角色提供等同自身宠物伤害{0}%的防御属性，持续3秒
                                load3DPetSkill.SetXY(60, -88);
                                break;
                            case ((int)EN_BUFF_TYPE.PetParry):  // 10401018 宠物释放技能后为角色提供等同自身宠物伤害{0}%的格挡值属性，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff1";
                                load3DPetSkill.SetXY(-20, -120);
                                
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff2";
                                break;
                            case ((int)EN_BUFF_TYPE.PetDefAdd):  // 10401019 宠物释放技能后为角色提供等同自身宠物等级{0}%的防御加成，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff1";
                                load3DPetSkill.SetXY(-50, -180);
                                
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff2";
                                loadBottom.SetXY(-50, -180);
                                break;
                            case ((int)EN_BUFF_TYPE.PetJouk):  // 10401020 宠物释放技能后为角色提供等同自身宠物等级{0}%的闪避率，持续3秒
                                load3DPetSkill.animationName = "Common_" + buffAction.BuffEffect + "_buff1";
                                load3DPetSkill.SetXY(0, -180);
                                
                                loadBottom.animationName = "Common_" + buffAction.BuffEffect + "_buff2";
                                loadBottom.SetXY(0, -180);
                                break;
                            case ((int)EN_BUFF_TYPE.PetParryRate):  // 10401021 宠物释放技能后为角色提供等同自身宠物等级{0}%的格挡率，持续3秒
                                load3DPetSkill.SetXY(40, -200);
                                break;
                        }
                        
                        if (buffAction.BuffEffect != (int)EN_BUFF_TYPE.PetRecovery || buffAction.BuffEffect != (int)EN_BUFF_TYPE.PetRecoveryTime || buffAction.BuffEffect != (int)EN_BUFF_TYPE.PetLifeShield)
                        {
                            PetSkillBuffEnd(attack, buffAction);
                        }
                    }
                }
            }
            else if (buffAction.Object == 2) //给目标
            {
                foreach (var damage in vo.lstTarget)
                {
                    MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(damage.targetID) as MapMoveObject;
                    if (objTarget != null)
                    {
                        if (objTarget.HitSpineDic.TryGetValue(buffAction.Id, out GLoader3D load3D))
                        {
                            load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                            load3D.animationName = "Common_" + buffAction.BuffEffect + "_buff";
                            load3D.frame = 0;
                            load3D.loop = false;
                            load3D.playing = true;
                            load3D.visible = true;
                            load3D.parent.SetChildIndex(load3D, 100);//显示最上层
                            
                            switch (buffAction.Id)
                            {
                                case (int)EN_BUFF_TYPE.Dizzy: //眩晕
                                    //再次眩晕刷新重置眩晕时间
                                    load3D.loop = true;
                                    PlayBuffDizzyEffect(load3D, objTarget, buffAction);
                                    break;
                                case (int)EN_BUFF_TYPE.Burn: // 灼烧
                                    load3D.SetXY(10, -88);
                                    load3D.loop = true;
                                    PlayBuffHurtContinueEffect(objTarget, attack as MapMoveObject, damage, buffAction);
                                    break;
                                case (int)EN_BUFF_TYPE.Bleed: // 流血
                                    load3D.SetXY(20, -118);
                                    load3D.loop = true;
                                    load3D.animationName = "xue";
                                    
                                    GLoader3D loadBurn = objTarget.HitSpineDic[(int)EN_BUFF_TYPE.Bleed1];
                                    loadBurn.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                                    loadBurn.animationName = "quan";
                                    loadBurn.frame = 0;
                                    loadBurn.loop = true;
                                    loadBurn.playing = true;
                                    loadBurn.visible = true;
                                    loadBurn.SetXY(40, -58);
                                    loadBurn.SetScale(0.7f, 0.7f);
                                    load3D.parent.RemoveChild(loadBurn);
                                    load3D.parent.AddChildAt(loadBurn, 0);
                                    
                                    PlayBuffHurtContinueEffect(objTarget, attack as MapMoveObject, damage, buffAction);
                                    break;
                                case (int)EN_BUFF_TYPE.AtkReduce: // 减伤目标攻击
                                    load3D.SetXY(-20, -100);
                                    load3D.loop = true;
                                    PlayBuffSpeedReduce(load3D, attack, objTarget, buffAction);
                                    break;
                                case (int)EN_BUFF_TYPE.SpeedReduce: // 减伤目标攻速
                                    load3D.SetXY(-20, -100);
                                    load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                                    SkeletonAnimation skeleton = load3D.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                                    skeleton?.AnimationState.SetAnimation(0, "guang", true);  // 基础层动画
                                    skeleton?.AnimationState.SetAnimation(1, "jian", true);
                                    
                                    GLoader3D loadSpeedReduce = objTarget.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1];
                                    loadSpeedReduce.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                                    loadSpeedReduce.animationName = "diquan";
                                    loadSpeedReduce.frame = 0;
                                    loadSpeedReduce.loop = true;
                                    loadSpeedReduce.playing = true;
                                    loadSpeedReduce.visible = true;
                                    loadSpeedReduce.SetXY(-20, -95);
                                    load3D.parent.RemoveChild(loadSpeedReduce);
                                    load3D.parent.AddChildAt(loadSpeedReduce, 0);
                                    
                                    PlayBuffSpeedReduce(load3D, attack, objTarget, buffAction);
                                    break;
                                case (int)EN_BUFF_TYPE.Lightning: // 闪电
                                    load3D.playing = false;
                                    load3D.visible = false;
                                    isLightning = true;
                                    break;
                                case (int)EN_BUFF_TYPE.Bomb: // 爆炸
                                    load3D.playing = false;
                                    load3D.visible = false;
                                    bombTargetId = damage.targetID;
                                    break;
                                
                            }
                        }
                    }
                }
                if (bombTargetId != 0) //爆炸
                {
                    PlayBuffBombEffect(attack as MapMoveObject, bombTargetId, vo, buffAction);
                    bombTargetId = 0; 
                }
                if (isLightning)  //闪电
                {
                    isLightning = false;
                    PlayBuffLightningEffect(attack as MapMoveObject, vo, buffAction);
                }
            }

            
        }
        #endregion
        
        #region buff表现
        /// <summary>
        /// 宠物技能buff结束回调
        /// </summary>
        private void PetSkillBuffEnd(MapMoveObject target, ConfigBuffActionTemplateUnit buffAction)
        {
            GTween.To(1, 0, buffAction.CoverLastTime)
                .SetTarget(target)
                .SetEase(EaseType.Linear)
                .OnStart(() => {})
                .OnUpdate((fillnum) => {})
                .OnComplete(() =>
                {
                    target.HitSpineDic[buffAction.BuffEffect].playing = false;
                    target.HitSpineDic[buffAction.BuffEffect].visible = false;

                    if (target.HitSpineDic.ContainsKey(int.Parse(buffAction.BuffEffect+"1")))
                    {
                        target.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].playing = false;
                        target.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].visible = false;
                    }
                    
                    switch (buffAction.BuffEffect)
                    {
                        case (int)EN_BUFF_TYPE.PetShouJiRecovery:
                            target.petSkillRecover = 0;
                            break;
                    }
                });
        }
        
        /// <summary>
        /// 设置对象属性值  //  属性修改  不修改角色的属性(角色属性变化服务器下发)
        /// </summary>
        private void SetObjectAttribute(MapMoveObject attack, MapMoveObject target, bool isAdd, float time, int attrId, int value)
        {
            if (isAdd)
            {
                if (attack is MapPetObject)
                {
                    if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalStrike) //暴击
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalStrike += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalStrike * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalInjury) //加爆伤
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalInjury += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalInjury * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ComboAtk) //加连击
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.ComboAtk += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.ComboAtk * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK) //攻击
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.Atk += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.Atk * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed) //攻速
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.AtkSpeed += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.AtkSpeed * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.PetAtkADD) //宠物伤害加成
                        (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.PetAtkADD += (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.PetAtkADD * value * ConstDefine.CONFIG_PLACE_EX;
                    
                }
                else if (attack is MapHeroObject)
                {
                    if (attrId == (int)EN_BUFF_ADD_TYPE.ATK) //攻击
                        DataManager.Instance.GetRoleData().FightAttrVo.Atk += DataManager.Instance.GetRoleData().FightAttrVo.Atk * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalStrike) //暴击
                        DataManager.Instance.GetRoleData().FightAttrVo.CriticalStrike += DataManager.Instance.GetRoleData().FightAttrVo.CriticalStrike * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalInjury) //加爆伤
                        DataManager.Instance.GetRoleData().FightAttrVo.CriticalInjury += DataManager.Instance.GetRoleData().FightAttrVo.CriticalInjury * value * ConstDefine.CONFIG_PLACE_EX;

                    
                }
                else
                {
                    if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalInjury) //加爆伤
                    {
                        attack.Attr.CriticalInjury += attack.Attr.CriticalInjury * value * ConstDefine.CONFIG_PLACE_EX;
                    }
                }
                
                GTween.To(1, 0, time)
                    .SetTarget(attack)
                    .SetEase(EaseType.Linear)
                    .OnStart(() => { })
                    .OnUpdate((fillnum) => {})
                    .OnComplete(() =>
                    {
                        if (attack != null && attack is MapPetObject)
                        {
                             if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalStrike) //暴击
                                (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalStrike = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalStrike / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                             else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalInjury) //爆伤
                                (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalInjury = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.CriticalInjury / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                             else if (attrId == (int)EN_BUFF_ADD_TYPE.ComboAtk) //连击
                                (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.ComboAtk = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.ComboAtk / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                             else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK) //攻击
                                 (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.Atk = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.Atk / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                             else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed) //攻速
                                 (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.AtkSpeed = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.AtkSpeed / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                             else if (attrId == (int)EN_BUFF_ADD_TYPE.PetAtkADD) //宠物伤害加成
                                 (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.PetAtkADD = (attack as MapPetObject).PetAttr.petItemInfo.FightAttrVo.PetAtkADD / (1 + value * ConstDefine.CONFIG_PLACE_EX);

                        }
                        else if (attack is MapHeroObject)
                        {
                            if (attrId == (int)EN_BUFF_ADD_TYPE.ATK) //攻击
                                DataManager.Instance.GetRoleData().FightAttrVo.Atk = DataManager.Instance.GetRoleData().FightAttrVo.Atk / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                            else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalStrike) // 暴击
                                DataManager.Instance.GetRoleData().FightAttrVo.CriticalStrike = DataManager.Instance.GetRoleData().FightAttrVo.CriticalStrike / (1 + value * ConstDefine.CONFIG_PLACE_EX);
                            else if (attrId == (int)EN_BUFF_ADD_TYPE.CriticalInjury) //爆伤
                                DataManager.Instance.GetRoleData().FightAttrVo.CriticalInjury = DataManager.Instance.GetRoleData().FightAttrVo.CriticalInjury / (1 + value * ConstDefine.CONFIG_PLACE_EX);

                            
                        }
                        else
                        {
                            if (attack != null && attack.Attr.HP > 0)
                            {
                                
                            }
                        }
                    });
            }
            else
            {
                if (target is MapHeroObject)
                {
                    FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
                    if (attrId == (int)EN_BUFF_ADD_TYPE.Def) //减防御
                        fightAttrVo.Def -= fightAttrVo.Def * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ParryRate) //格挡率
                        fightAttrVo.ParryRate -= fightAttrVo.ParryRate * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed) //攻击速度
                    {
                        fightAttrVo.AtkSpeed -= fightAttrVo.AtkSpeed * value * ConstDefine.CONFIG_PLACE_EX;
                        fightAttrVo.AtkSpeed = fightAttrVo.AtkSpeed <= 0 ? 0.01f : fightAttrVo.AtkSpeed;
                    }
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK)  //攻击力
                        fightAttrVo.Atk -= fightAttrVo.Atk * value * ConstDefine.CONFIG_PLACE_EX;
            
                    GTween.To(1, 0, time)
                        .SetTarget(target)
                        .SetEase(EaseType.Linear)
                        .OnStart(() => { })
                        .OnUpdate((fillnum) => {})
                        .OnComplete(() =>
                        {
                            if (target != null && target.Attr.HP > 0)
                            {
                                if (attrId == (int)EN_BUFF_ADD_TYPE.Def)
                                    fightAttrVo.Def = fightAttrVo.Def / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.ParryRate) //格挡率
                                    fightAttrVo.ParryRate = fightAttrVo.ParryRate / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed)  //攻速
                                    fightAttrVo.AtkSpeed = fightAttrVo.AtkSpeed / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK)
                                    fightAttrVo.Atk = fightAttrVo.Atk / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                            }
                        });
                }
                else
                {
                    if (attrId == (int)EN_BUFF_ADD_TYPE.Def) //减防御
                        target.Attr.Def -= target.Attr.Def * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ParryRate) //格挡率
                        target.Attr.ParryRate -= target.Attr.ParryRate * value * ConstDefine.CONFIG_PLACE_EX;
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed) //攻击速度
                    {
                        target.Attr.AtkSpeed -= target.Attr.AtkSpeed * value * ConstDefine.CONFIG_PLACE_EX;
                        target.Attr.AtkSpeed = target.Attr.AtkSpeed <= 0 ? 0.01f : target.Attr.AtkSpeed;
                    }
                    else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK)  //攻击力
                        target.Attr.Atk -= target.Attr.Atk * value * ConstDefine.CONFIG_PLACE_EX;

                    GTween.To(1, 0, time)
                        .SetTarget(target)
                        .SetEase(EaseType.Linear)
                        .OnStart(() => { })
                        .OnUpdate((fillnum) => {})
                        .OnComplete(() =>
                        {
                            if (target != null && target.Attr.HP > 0)
                            {
                                if (attrId == (int)EN_BUFF_ADD_TYPE.Def)
                                    target.Attr.Def = target.Attr.Def / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.ParryRate) //格挡率
                                    target.Attr.ParryRate = target.Attr.ParryRate / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.AtkSpeed)  //攻速
                                    target.Attr.AtkSpeed = target.Attr.AtkSpeed / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                                else if (attrId == (int)EN_BUFF_ADD_TYPE.ATK)
                                    target.Attr.Atk = target.Attr.Atk / (1 - value * ConstDefine.CONFIG_PLACE_EX);
                            }
                        });
                }
            }
        }

        /// <summary>
        /// 伤害提升
        /// </summary>
        private void DemageUp(MapMoveObject target, ConfigBuffActionTemplateUnit buffAction)
        {
            target.Attr.DemageUpRate += int.Parse(buffAction.Common);
            GTween.To(1, 0, buffAction.CoverLastTime)
                .SetTarget(target)
                .SetEase(EaseType.Linear)
                .OnStart(() => {})
                .OnUpdate((fillnum) => {})
                .OnComplete(() =>
                {
                    target.Attr.DemageUpRate -= int.Parse(buffAction.Common);
                    if (target.Attr.DemageUpRate < 0)
                    {
                        target.Attr.DemageUpRate = 0;
                    }
                });
        }
        
        private GTweener dizzyTweener;
        /// <summary>
        /// 眩晕
        /// </summary>
        private void PlayBuffDizzyEffect(GLoader3D load3D, MapMoveObject target, ConfigBuffActionTemplateUnit buffAction = null, bool isRest = true)
        {
            target.isPause = true;
            target.SetObjectAnimCompPause(target);
            
            load3D.SetXY(30, -170);
            
            dizzyTweener = null;
            GTween.Kill(target);
            
            dizzyTweener = GTween.To(1, 0, buffAction.CoverLastTime);
            dizzyTweener.SetTarget(target)
                .SetEase(EaseType.Linear)
                .OnStart(() =>
                {
                })
                .OnUpdate((fillnum) =>
                {
                    if (target.Attr.HP <= 0)
                    {
                        load3D.visible = false;
                        load3D.playing = false;
                        target.isPause = false;
                        target.SetObjectAnimComp(target);
                        GTween.Kill(target);
                        dizzyTweener = null;
                    }
                })
                .OnComplete(() =>
                {
                    load3D.visible = false;
                    load3D.playing = false;
                    target.isPause = false;
                    target.SetObjectAnimComp(target);
                    dizzyTweener = null;
                });
        }
        
        /// <summary>
        /// 灼烧、掉血 每秒攻击力百分5的伤害
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="buffUnit"></param>
        /// <param name="demage"></param>
        private void PlayBuffHurtContinueEffect(MapMoveObject target,MapMoveObject attack,UnitDamageVo demage, ConfigBuffActionTemplateUnit buffAction)
        {
            if (buffAction.Common.Split(",").Length < 1)
            {
                return;
            }
            var newDamage = new UnitDamageVo();
            newDamage.unitID = demage.unitID;
            newDamage.targetID = demage.targetID;
            newDamage.skillID = demage.skillID;

            if ( int.Parse(buffAction.Common.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.HP)
            {
                newDamage.damage = target.Attr.HP * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
            }else if (int.Parse(buffAction.Common.Split(",")[0]) == (int)EN_BUFF_ADD_TYPE.ATK)
            {
                newDamage.damage = attack.Attr.Atk * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                if (target is MapMonsterObject)
                {
                    newDamage.damage = DataManager.Instance.GetRoleData().FightAttrVo.Atk * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX); //
                }else if (target is MapHeroObject)
                {
                    newDamage.damage = attack.Attr.Atk * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                }
            }
            
            if (target.Attr.HP > newDamage.damage) //保证扣除时怪物不要死亡，后续PlaySkill才是攻击扣除的伤害
            {
                target.buffCount = 0;
                target.timeRate = buffAction.EffectTime;
                target.hitCallbackDic[buffAction.Id] = () =>
                {
                    if (target.buffCount >= buffAction.CoverLastTime || target.Attr.HP <= 0 || target.Attr.HP <= newDamage.damage)
                    {
                        target.hitCallbackDic[buffAction.Id] = null;
                        target.hitCallbackDic.Remove(buffAction.Id);
                        target.buffCount = 0;
                        target.HitSpineDic[buffAction.Id].visible = false;
                        if (buffAction.Id == (int)EN_BUFF_TYPE.Bleed)
                        {
                            target.HitSpineDic[(int)EN_BUFF_TYPE.Bleed1].visible = false;
                        }
                        return;
                    }
                    target.HitSpineDic[buffAction.Id].visible = true;
                    target.SyncDamage(newDamage); //直接弹伤害
                    target.buffCount++;
                };
                target.hitCallbackDic[buffAction.Id]?.Invoke();
            }
        }
        
        /// <summary>
        /// 减伤目标攻速  减伤目标攻击  减伤目标防御
        /// </summary>
        /// <param name="load3D"></param>
        /// <param name="target"></param>
        /// <param name="buffAction"></param>
        private void PlayBuffSpeedReduce(GLoader3D load3D, MapMoveObject attack, MapMoveObject target, ConfigBuffActionTemplateUnit buffAction)
        {
            SetObjectAttribute(attack, target, false, buffAction.CoverLastTime, int.Parse(buffAction.Common.Split(",")[0]), int.Parse(buffAction.Common.Split(",")[1]));
            
            GTween.To(1, 0, buffAction.CoverLastTime)
                .SetTarget(target)
                .SetEase(EaseType.Linear)
                .OnStart(() =>
                {
                })
                .OnUpdate((fillnum) => { })
                .OnComplete(() =>
                {
                    if (buffAction.Id == (int)EN_BUFF_TYPE.AtkReduce)
                        if (GetEnemyCount((int)EN_BUFF_TYPE.AtkReduce))
                            return;
                    else if (buffAction.Id == (int)EN_BUFF_TYPE.SpeedReduce)
                        if (GetEnemyCount((int)EN_BUFF_TYPE.SpeedReduce))
                            return;
                    
                    load3D.visible = false;
                    load3D.playing = false;
                    if (buffAction.Id == (int)EN_BUFF_TYPE.SpeedReduce)
                    {
                        target.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1].visible = false;
                        target.HitSpineDic[(int)EN_BUFF_TYPE.SpeedReduce1].playing = false;
                    }
                    GTween.Kill(target);
                });
        }

        public bool GetEnemyCount(int buffType)
        {
            if (MapObjectManager.Instance.GetLocalHero() != null)
            {
                List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(MapObjectManager.Instance.GetLocalHero(), MapObjectManager.Instance.GetLocalHero().Position, 0);
                foreach (var item in lstEnemy)
                {
                    if (buffType == (int)EN_BUFF_TYPE.AtkReduce && item.Attr.debuffDemageValue != 0)
                    {
                        return true;
                    }else if (buffType == (int)EN_BUFF_TYPE.SpeedReduce && item.Attr.debuffAtkSpeedValue != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// 技能buff 生命护盾
        /// </summary>
        /// <returns></returns>
        private void PlayBuffLifeShieldEffect(MapMoveObject attack,UnitDamageVo demage, ConfigBuffActionTemplateUnit buffAction, ConfigSkillAchieveUnit buffSkillAchieve, double petAtk)
        {
            if (attack is MapPetObject || !(attack is MapHeroObject))
            {
                return;
            }

            var value = 0d;
            if (buffAction.Id == (int)EN_BUFF_TYPE.LifeShield) //取生命百分比
            {
                if (buffAction.Object == 1)
                    value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                else
                    value = attack.Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
            }else if (buffAction.Id == ((int)EN_BUFF_TYPE.LifeShield1)) //取攻击力百分比
            {
                if (buffAction.Object == 1)
                {
                    if (buffSkillAchieve.Type == 5 && buffSkillAchieve.Trigger == 106)
                    {
                        value = petAtk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                    }
                    else
                    {
                        value = MapObjectManager.Instance.GetLocalHero().Attr.Atk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                    }
                }
                else
                    value = attack.Attr.Atk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
            }else if (buffAction.Id == ((int)EN_BUFF_TYPE.LifeShield2)) //取防御百分比
            {
                if (buffAction.Object == 1)
                    value = MapObjectManager.Instance.GetLocalHero().Attr.Def * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                else
                    value = attack.Attr.Def * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
            }else if (buffSkillAchieve.Type == 10) //宠物技能 给的护盾
            {
                value = petAtk * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
            }
            
            attack.lifeShieldCount++;
            attack.lifeShieldList.Insert(0, value);
            
            MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(attack.id) as MapMoveObject;
            if (objTarget != null)
            {
                GTween.To(1, 0, buffAction.CoverLastTime)
                    .SetTarget(attack)
                    .SetEase(EaseType.Linear)
                    .OnStart(() =>
                    {
                    })
                    .OnUpdate((fillnum) =>
                    {
                        if (attack.lifeShieldList.Count > 0)
                        {
                            bool isNull = true;
                            for (int i = 0; i < attack.lifeShieldList.Count; i++)
                            {
                                if (attack.lifeShieldList[i] != 0)
                                {
                                    isNull = false;
                                    break;
                                }
                            }
                            if (isNull)
                            {
                                if (buffSkillAchieve.Type == 10)
                                {
                                    objTarget.HitSpineDic[buffAction.BuffEffect].visible = false;
                                    objTarget.HitSpineDic[buffAction.BuffEffect].playing = false;
                                    if (objTarget.HitSpineDic.ContainsKey(int.Parse(buffAction.BuffEffect+"1")))
                                    {
                                        objTarget.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].playing = false;
                                        objTarget.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].visible = false;
                                    }
                                }
                                else
                                {
                                    objTarget.HitSpineDic[buffAction.Id].visible = false;
                                    objTarget.HitSpineDic[buffAction.Id].playing = false;
                                }
                                attack.lifeShieldCount = 0;
                                attack.lifeShieldList.Clear();
                                GTween.Kill(attack);
                            }
                        }
                    })
                    .OnComplete(() =>
                    {
                        if (attack.lifeShieldCount > 0)
                        {
                            attack.lifeShieldCount--;
                        }

                        if (attack.lifeShieldCount <= 0)
                        {
                            if (buffSkillAchieve.Type == 10)
                            {
                                objTarget.HitSpineDic[buffAction.BuffEffect].visible = false;
                                objTarget.HitSpineDic[buffAction.BuffEffect].playing = false;
                                if (objTarget.HitSpineDic.ContainsKey(int.Parse(buffAction.BuffEffect+"1")))
                                {
                                    objTarget.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].playing = false;
                                    objTarget.HitSpineDic[int.Parse(buffAction.BuffEffect+"1")].visible = false;
                                }
                            }
                            else
                            {
                                objTarget.HitSpineDic[buffAction.Id].visible = false;
                                objTarget.HitSpineDic[buffAction.Id].playing = false;
                            }
                            attack.lifeShieldList.Clear();
                        }
                        else
                            attack.lifeShieldList.RemoveAt(attack.lifeShieldList.Count - 1);
                    });
            }
        }
        
        /// <summary>
        /// 技能buff 重置技能CD
        /// </summary>
        /// <returns></returns>
        private void PlayResetSkillCD(MapMoveObject attack, float cdTime = 0)
        {
            if (attack.ObjectType == MapObjectType.Hero || attack.ObjectType == MapObjectType.Pet)
            {
                if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
                    RoleManager.Instance.GetMapHeroSkillProxy().ResetSkillCD(cdTime);
                foreach (var item in RoleManager.Instance.GetMapSkillProxy())
                {
                    if (item.Value != null)
                        item.Value.ResetSkillCD(cdTime);
                }
                EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_SKILL_CD, cdTime);
            }
        }
        
        /// <summary>
        /// 技能buff 无敌状态
        /// </summary>
        /// <returns></returns>
        private void PlayInvincibleEffect(MapMoveObject target, ConfigBuffActionTemplateUnit buffAction)
        {
            target.IsBuffInvincible = true;
            target.buffCount = 0;
            target.hitCallbackDic[buffAction.Id] = () =>
            {
                if (target.Attr.HP <= 0 || target.buffCount >= buffAction.CoverLastTime)
                {
                    target.hitCallbackDic[buffAction.Id] = null;
                    target.hitCallbackDic.Remove(buffAction.Id);
                    target.buffCount = 0;
                    target.HitSpineDic[buffAction.Id].visible = false;
                    target.IsBuffInvincible = false;
                    return;
                }
                target.HitSpineDic[buffAction.Id].visible = true;
                target.timeRate = 1;
                target.buffCount++;
            };
            target.hitCallbackDic[buffAction.Id]?.Invoke();
        }

        /// <summary>
        /// 技能buff 反弹伤害状态
        /// </summary>
        /// <returns></returns>
        private void PlayBounceEffect(MapMoveObject target, MapMoveObject attack,UnitDamageVo damage, ConfigBuffActionTemplateUnit buffAction)
        {
            if (attack is MapPetObject || attack is MapSkillProxy || attack is MapHeroSkillProxy)
            {
                attack = MapObjectManager.Instance.GetLocalHero();
            }
            attack.BounceValue = int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;

            // if (buffAction.CoverLastTime > 1)  //持续反弹
            // {
                // if (target.Attr.HP > damage.damage) //保证怪物正常受击不要死亡，后续PlaySkill才是攻击扣除的伤害
                // {
                attack.buffCount = 0;
                attack.hitCallbackDic[buffAction.Id] = () =>
                {
                    // if (target.Attr.HP <= 0 || attack.buffCount >= 6)//buffAction.CoverLastTime)
                    if (attack.buffCount >= buffAction.CoverLastTime)
                    {
                        attack.hitCallbackDic[buffAction.Id] = null;
                        attack.hitCallbackDic.Remove(buffAction.Id);
                        attack.buffCount = 0;
                        attack.BounceValue = 0;
                        return;
                    }
                    attack.timeRate = 1;
                    attack.buffCount++;
                };
                //attack.hitCallbackDic[buffAction.Id]?.Invoke();
                // }
            // }
            // else
            // {  
            //     //只反弹一次  直接触发 被攻击的时候触发的，所以是攻击的对象受伤
            //     var damage1 = new UnitDamageVo();
            //     damage1.unitID = damage.targetID;
            //     damage1.targetID = damage.unitID;
            //     damage1.skillID = 0;
            //     damage1.damage = damage.damage * attack.BounceValue;
            //     damage1.damageType = EN_DAMAGE_TYPE.Bounce;
            //     if (attack != null && attack.Attr.HP > damage1.damage)
            //     {
            //         attack.SyncDamage(damage1);
            //     }
            //     attack.BounceValue = 0;
            // }
            
        }
        
        /// <summary>
        /// 技能buff 复活
        /// </summary>
        /// <returns></returns>
        private void PlayReBrithEffect(MapMoveObject target,ConfigBuffActionTemplateUnit buffAction)
        {
            if (target.Attr.HP <= 0)
            {
                target.Attr.HP = 0;
                // this.Attr.HP += this.Attr.HPMax * 0.6;
            }
            
            var damage = new UnitDamageVo();
            damage.unitID = target.id;
            damage.targetID = target.id;
            damage.skillID = 0;
            damage.damage = target.Attr.HPMax * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
            damage.damageType = EN_DAMAGE_TYPE.RECOVERY;
            if (target is MapMonsterObject && TriggerRoleHoly(target, (int)EN_BUFF_TYPE.ReBirth) != 0)
            {
                damage.damage -= damage.damage * TriggerRoleHoly(target, (int)EN_BUFF_TYPE.ReBirth) * ConstDefine.CONFIG_PLACE_EX;
            }
            target.SyncDamage(damage);
        }
        
        /// <summary>
        /// 技能buff 回血特效
        /// </summary>
        /// <returns></returns>
        private void PlayAddHpEffect(MapMoveObject attack, ConfigBuffActionTemplateUnit buffAction, ConfigSkillAchieveUnit buffSkillAchieve, double hpValue = 0)
        {
            if (attack == null)
                return;
            if (attack is MapMonsterObject && buffAction.CoverLastTime >= 999 )//&& attack.hitCallbackDic.Count > 0 && attack.hitCallbackDic.ContainsKey(buffSkillAchieve.Id))
            {
                buffAction.EffectTime = 1;  //上面用总时间限制  这边调用一次就可以
                buffAction.CoverLastTime = 1;
                // return;  //怪物每秒回复血量，一个就可以
            }
            
            attack.buffCount = 0;
            attack.timeRate = buffAction.EffectTime;
            attack.hitCallbackDic[buffSkillAchieve.Id] = () =>
            {
                if (attack.buffCount >= buffAction.CoverLastTime)
                {
                    attack.hitCallbackDic[buffSkillAchieve.Id] = null;
                    attack.hitCallbackDic.Remove(buffSkillAchieve.Id);
                    attack.buffCount = 0;
                    return;
                }
                attack.buffCount++;
                    
                if (attack is MapHeroObject)
                {
                    if (buffSkillAchieve.Type == 10)
                    {
                        var damage = new UnitDamageVo();
                        damage.unitID = attack.id;
                        damage.targetID = attack.id;
                        damage.skillID = 0;
                        damage.damage = hpValue;
                        damage.damageType = EN_DAMAGE_TYPE.RECOVERY;
                        attack.SyncDamage(damage);
                    }
                    else
                    {
                        double value = MapObjectManager.Instance.GetLocalHero().Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                        attack.PlayRecovery(attack, value);
                    }
                }
                else
                {
                    attack.PlayRecovery(attack, attack.Attr.HPMax * int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                }
            };
            attack.hitCallbackDic[buffSkillAchieve.Id]?.Invoke();
        }
        
        /// <summary>
        /// 爆炸受击特效
        /// </summary>
        List<UnitDamageVo> targetList = new List<UnitDamageVo>();
        private void PlayBuffBombEffect(MapMoveObject attack, int targetId, CastSkillVo skillVo, ConfigBuffActionTemplateUnit buffAction)
        {
            if (attack == null) return;
            
            targetList.Clear();
            List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(attack, attack.Position, 0);
            lstEnemy.Sort(MapObjectManager.SortZUIJIN);

            for (int i = 0; i < lstEnemy.Count; i++)
            {
                if (targetId != lstEnemy[i].id && targetList.Count < buffAction.Ruantity)
                {
                    var damage = new UnitDamageVo();
                    damage.unitID = skillVo.unitID;
                    damage.targetID = lstEnemy[i].id;
                    damage.skillID = skillVo.skillID;
                    damage.damage = attack.Attr.Atk * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                    
                    var target = MapObjectManager.Instance.GetMapMoveObjectById(damage.targetID);
                    // if (target.Attr.HP > damage.damage) //先扣除buff伤害（保证扣除时怪物不要死亡，后续PlaySkill才是攻击扣除的伤害）
                    // {
                        // target.SyncDamage(damage);
                        targetList.Add(damage);
                    // }
                }
            }
            
            //播放 受击特效
            foreach (var damage in targetList)
            {
                var target = damage;
                MapMoveObject objTarget = MapObjectManager.Instance.GetMapObjectById(target.targetID) as MapMoveObject;
                if (objTarget != null)
                {
                    GLoader3D load3D = objTarget.HitSpineDic[buffAction.Id];
                    load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_buff";
                    load3D.animationName = "Common_" + buffAction.BuffEffect + "_buff";
                    load3D.frame = 0;
                    load3D.loop = true;
                    load3D.visible = true;
                    load3D.playing = true;
                    load3D.SetXY(-50, -220);
                    load3D.parent.SetChildIndex(load3D, 100);//显示最上层

                    SkeletonAnimation skeleton = load3D.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                    var spineTime = skeleton.Skeleton.Data.FindAnimation(skeleton.AnimationName).Duration;
                    GTween.To(1, 0, spineTime)
                        .SetTarget(objTarget)
                        .SetEase(EaseType.Linear)
                        .OnStart(() => { })
                        .OnUpdate((fillnum) => { })
                        .OnComplete(() =>
                        {
                            if (objTarget != null)
                            {
                                load3D.visible = false;
                                objTarget.SyncDamage(damage);
                            }
                        });
                }
            }
        }
        
        /// <summary>
        /// 连锁闪电受击特效
        /// </summary>
        private void PlayBuffLightningEffect(MapMoveObject attack, CastSkillVo skillVo, ConfigBuffActionTemplateUnit buffAction)
        {
            targetList.Clear();
            List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(attack, attack.Position, 0);
            lstEnemy.Sort(MapObjectManager.SortZUIJIN);

            for (int i = 0; i < lstEnemy.Count; i++)
            {
                if (targetList.Count < buffAction.Ruantity)
                {
                    var damage = new UnitDamageVo();
                    damage.unitID = skillVo.unitID;
                    damage.targetID = lstEnemy[i].id;
                    damage.skillID = skillVo.skillID;
                    damage.damage = attack.Attr.Atk * (int.Parse(buffAction.Common.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX);
                    
                    var target = MapObjectManager.Instance.GetMapMoveObjectById(damage.targetID);
                    if (target.Attr.HP > damage.damage) //先扣除buff伤害（保证扣除时怪物不要死亡，后续PlaySkill才是攻击扣除的伤害）
                    {
                        target.SyncDamage(damage);
                        targetList.Add(damage);
                    }
                }
            }
            
            foreach (var damage in targetList)
            {
                var damageVo = damage;
                MapMoveObject target = MapObjectManager.Instance.GetMapObjectById(damageVo.targetID) as MapMoveObject;
                if (target != null)
                {
                    GLoader3D load3D = target.HitSpineDic[buffAction.Id];
                    load3D.url = "ui://Common/Common_" + buffAction.BuffEffect + "_fly";
                    load3D.frame = 0;
                    load3D.loop = true;
                    load3D.visible = true;
                    load3D.SetXY(-15, -75);
                    load3D.parent.SetChildIndex(load3D, 100);//显示最上层
                    Utils.PlaySpineAnim(load3D, "Common_" +buffAction.BuffEffect + "_fly", false, () => { load3D.visible = false;} );
                }
            }
        }
        
        #endregion
        
        /// <summary>
        /// 是否宠物技能
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="cfgSkill"></param>
        /// <returns></returns>
        public bool IsPetSkill(MapMoveObject attack, ConfigSkillUnit cfgSkill)
        {
            if (cfgSkill != null)
            {
                foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
                {
                    if (attack is MapSkillProxy && cfgSkill.Id == petItem.skillId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// 根据宠物技能获取宠物
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="cfgSkill"></param>
        /// <returns></returns>
        public PetItemInfo GetBattlePetByPetSkillId(int petSkillId)
        {
            foreach (var petItem in PetInfoManager.Instance.GetBattlePetList().ToList())
            {
                if (petSkillId == petItem.skillId)
                {
                    return petItem;
                }
            }
            return null;
        }

        public void UpdateHolyData()
        {
            if (MapObjectManager.Instance.GetLocalHero() == null) return;
                
            List<MapObject> lstEnemy = MapObjectManager.Instance.GetAllEnemy(MapObjectManager.Instance.GetLocalHero(), MapObjectManager.Instance.GetLocalHero().Position, 0);
            if (_buffInfoHolyItemInfoList.Count > 0) //原来圣物，先还原怪物属性
            {
                foreach (var target in lstEnemy)
                {
                    if (target.Attr.HP > 0)
                    {
                        foreach (var itemInfo in _buffInfoHolyItemInfoList) //移除之前的圣物 数据
                        {
                            ConfigHolyUnit unit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(itemInfo.HolyId, itemInfo.Level);
                            ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(unit.Id);
                            if (buffSkillAchieve.Type == 7 && buffSkillAchieve.Trigger == 401)
                            {
                                if (buffSkillAchieve.HandleObject == 3)
                                {
                                    string[] commonAttrs1 = buffSkillAchieve.CommonAttr.Split('|');
                                    if (commonAttrs1.Length > 0)
                                    {
                                        foreach (var attrs in commonAttrs1)
                                        {
                                            string[] commonAttrs = attrs.Split(',');
                                            if (commonAttrs.Length > 1)
                                            {
                                                if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalStrike)
                                                {
                                                    target.Attr.CriticalStrike = target.Attr.CriticalStrike / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalInjury)
                                                {
                                                    target.Attr.CriticalInjury = target.Attr.CriticalInjury / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.AtkSpeed)
                                                {
                                                    target.Attr.AtkSpeed = target.Attr.AtkSpeed / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ATK)
                                                {
                                                    target.Attr.Atk = target.Attr.Atk / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Recovery)
                                                {
                                                    target.Attr.Recovery = target.Attr.Recovery / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Def)
                                                {
                                                    target.Attr.Def = target.Attr.Def / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryRate)
                                                {
                                                    target.Attr.ParryRate = target.Attr.ParryRate / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryValue)
                                                {
                                                    target.Attr.ParryValue = target.Attr.ParryValue / (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else
                                                {
                                                    FightUtils.SetMonsterAttrBySkillAchieve(target.Attr, int.Parse(commonAttrs[0]), int.Parse(commonAttrs[1]));
                                                }

                                            }
                                        }
                                    }
                                }
                                else if (buffSkillAchieve.HandleObject == 1)
                                {
                                    if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.SpeedReduce && target.Attr.debuffAtkSpeedValue > 0)
                                    {
                                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed += target.Attr.debuffAtkSpeedValue;
                                        var value = DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed * int.Parse(buffSkillAchieve.CoverCommonParam.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                        target.Attr.debuffAtkSpeedValue += value;
                                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed -= target.Attr.debuffAtkSpeedValue;
                                        if (DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed <= 0)
                                        {
                                            DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed = 0.01f;
                                        }
                                    }
                                    else if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.AtkReduce && target.Attr.debuffDemageValue > 0)
                                    {
                                        DataManager.Instance.GetRoleData().FightAttrVo.Atk += target.Attr.debuffDemageValue;
                                        var value = DataManager.Instance.GetRoleData().FightAttrVo.Atk * int.Parse(buffSkillAchieve.CoverCommonParam.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                        target.Attr.debuffDemageValue += value;
                                        DataManager.Instance.GetRoleData().FightAttrVo.Atk -= target.Attr.debuffDemageValue;
                                    }
                                }
                            }
                        }
                    }
                } 
            }
            
            SetBuffInfoHolyItemInfo();
            if (_buffInfoHolyItemInfoList.Count > 0) //现在有圣物  减少怪物buff属性
            {
                foreach (var target in lstEnemy)  //添加新的圣物 数据
                {
                    if (target.Attr.HP > 0)
                    {
                        foreach (var itemInfo in _buffInfoHolyItemInfoList)
                        {
                            ConfigHolyUnit unit = ConfigUtils.GetHolyUnitByHolyIdAndLevel(itemInfo.HolyId, itemInfo.Level);
                            ConfigSkillAchieveUnit buffSkillAchieve = ConfigUtils.GetSkillAchieveById(unit.Id);
                            if (buffSkillAchieve.Type == 7 && buffSkillAchieve.Trigger == 401)
                            {
                                if (buffSkillAchieve.HandleObject == 3)
                                {
                                    string[] commonAttrs1 = buffSkillAchieve.CommonAttr.Split('|');
                                    if (commonAttrs1.Length > 0)
                                    {
                                        foreach (var attrs in commonAttrs1)
                                        {
                                            string[] commonAttrs = attrs.Split(',');
                                            if (commonAttrs.Length > 1)
                                            {
                                                if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalStrike)
                                                {
                                                    target.Attr.CriticalStrike = target.Attr.CriticalStrike * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.CriticalInjury)
                                                {
                                                    target.Attr.CriticalInjury = target.Attr.CriticalInjury * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.AtkSpeed)
                                                {
                                                    target.Attr.AtkSpeed = target.Attr.AtkSpeed * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ATK)
                                                {
                                                    target.Attr.Atk = target.Attr.Atk * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Recovery)
                                                {
                                                    target.Attr.Recovery = target.Attr.Recovery * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.Def)
                                                {
                                                    target.Attr.Def = target.Attr.Def * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryRate)
                                                {
                                                    target.Attr.ParryRate = target.Attr.ParryRate * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else if (int.Parse(commonAttrs[0]) == (int)EN_BUFF_ADD_TYPE.ParryValue)
                                                {
                                                    target.Attr.ParryValue = target.Attr.ParryValue * (1 - int.Parse(commonAttrs[1]) * ConstDefine.CONFIG_PLACE_EX);
                                                }
                                                else
                                                {
                                                    FightUtils.SetMonsterAttrBySkillAchieve(target.Attr, int.Parse(commonAttrs[0]), -int.Parse(commonAttrs[1]));
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (buffSkillAchieve.HandleObject == 1)
                                {
                                    if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.SpeedReduce && target.Attr.debuffAtkSpeedValue > 0)
                                    {
                                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed += target.Attr.debuffAtkSpeedValue;
                                        var value = DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed * int.Parse(buffSkillAchieve.CoverCommonParam.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                        target.Attr.debuffAtkSpeedValue -= value; 
                                        DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed -= target.Attr.debuffAtkSpeedValue;
                                        if (DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed <= 0)
                                        {
                                            DataManager.Instance.GetRoleData().FightAttrVo.AtkSpeed = 0.01f;
                                        }
                                    }
                                    else if (buffSkillAchieve.ActID == (int)EN_BUFF_TYPE.AtkReduce && target.Attr.debuffDemageValue > 0)
                                    {
                                        DataManager.Instance.GetRoleData().FightAttrVo.Atk += target.Attr.debuffDemageValue;
                                        var value = DataManager.Instance.GetRoleData().FightAttrVo.Atk * int.Parse(buffSkillAchieve.CoverCommonParam.Split(",")[1]) * ConstDefine.CONFIG_PLACE_EX;
                                        target.Attr.debuffDemageValue -= value; 
                                        DataManager.Instance.GetRoleData().FightAttrVo.Atk -= target.Attr.debuffDemageValue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public List<HolyInfo> _buffInfoHolyItemInfoList = new List<HolyInfo>();//上阵的圣物列表
        public class HolyInfo
        {
            public int HolyId;
            public int Level;
            public int TableKeyId;
        }
        public void SetBuffInfoHolyItemInfo()
        {
            _buffInfoHolyItemInfoList.Clear();
            foreach (var itemInfo in HolyManager.Instance.GetHolyItemBattleList().ToList())
            {
                HolyInfo holyInfo = new HolyInfo();
                holyInfo.HolyId = itemInfo.HolyId;
                holyInfo.Level = itemInfo.Level;
                holyInfo.TableKeyId = itemInfo.TableKeyId;
                _buffInfoHolyItemInfoList.Add(holyInfo);
            }
        }
        //触发圣物
        public int TriggerRoleHoly(MapMoveObject attack, int actId)//, ConfigSkillAchieveUnit buffSkillAchieve = null, ConfigBuffActionTemplateUnit buffAction = null)
        {
            if (_buffInfoHolyItemInfoList.Count <= 0)
            {
                SetBuffInfoHolyItemInfo();
            }
            
            //角色佩戴的圣物
            foreach (var itemInfo in _buffInfoHolyItemInfoList)
            {
                // itemInfo.TableKeyId = 7230;
                ConfigSkillAchieveUnit skillAchieve = ConfigUtils.GetSkillAchieveById(itemInfo.TableKeyId);
                
                if (skillAchieve.Type == 7 && skillAchieve.Trigger == 401)
                {
                    if (actId != 0)
                    {
                        if (skillAchieve.ActID == (int)EN_BUFF_ACHIEVE_TYPE.DizaayRate && actId == (int)EN_BUFF_TYPE.Dizzy && skillAchieve.CoverCommonParam != "0") //减少眩晕概率
                        {
                            return int.Parse(skillAchieve.CoverCommonParam);
                        }
                        else if (skillAchieve.ActID == (int)EN_BUFF_ACHIEVE_TYPE.RecoveryRate && skillAchieve.CoverCommonParam.Split(",").Length > 1) //怪物回复效果比例减少
                        {
                            return int.Parse(skillAchieve.CoverCommonParam.Split(",")[1]);
                        }
                        else if (skillAchieve.ActID == (int)EN_BUFF_TYPE.ReBirth && skillAchieve.CoverCommonParam.Split(",").Length > 1) //减少怪物复活时的生命比例1%
                        {
                            return int.Parse(skillAchieve.CoverCommonParam.Split(",")[1]);
                        }
                        else if (skillAchieve.ActID == (int)EN_BUFF_TYPE.AtkReduce && skillAchieve.CoverCommonParam.Split(",").Length > 1) //减少玩家伤害的降低比例2%
                        {
                            return int.Parse(skillAchieve.CoverCommonParam.Split(",")[1]);
                        }
                        else if (skillAchieve.ActID == (int)EN_BUFF_TYPE.SpeedReduce && skillAchieve.CoverCommonParam.Split(",").Length > 1) //减少玩家攻速的降低比例3%
                        {
                            return int.Parse(skillAchieve.CoverCommonParam.Split(",")[1]);
                        }
                    }
                }
            }
            return 0;
        }

    }
}