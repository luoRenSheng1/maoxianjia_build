using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerBattleAttrInfo4Login_PC_Recv : IReceiver
    {
        public PlayerBattleAttrInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerBattleAttrInfo4Login_PC;
        }

        public void Process()
        {   //登录后，主动下发战斗总属性
            FightAttrVo fightAttrVo = new FightAttrVo();
            RoleManager.Instance.UpdateFightInfo(FightUtils.GetFightAttrVo(msg.BattleAttrList,fightAttrVo), true);
            /*
            FightAttrVo fightAttrVo = new FightAttrVo();
            foreach (var item in msg.BattleAttrList)
            {
                double attrValue = double.Parse(item.AttrValue.ToString("f4"));
                switch (item.AttrId)
                {
                    // case eBattleAttr.eBattleAttr_Extra_Atk_Rate:  //攻击加成比率  细分用其它的
                    //     fightAttrVo.ATKADD = (double) attrValue * ConstDefine.CONFIG_PLACE_EX;
                    //     break;
                    case eBattleAttr.eBattleAttr_Begin:  // 开始
                        fightAttrVo.Begin = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_FinalAttack:  // 攻击
                        fightAttrVo.Atk = Math.Ceiling(attrValue);
                        break;
                    //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalAttack = 101 用作Attack伤害  
                    case eBattleAttr.eBattleAttr_PhysicAttack:  // 物理伤害
                        fightAttrVo.PhysicAtk = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_MagicAttack:  // 魔法伤害
                        fightAttrVo.MagicAtk = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_SorceryAttack:  // 道术伤害
                        fightAttrVo.SorceryAtk = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_HP:       // 生命
                        fightAttrVo.HP = Math.Ceiling(attrValue);;
                        break;
                    case eBattleAttr.eBattleAttr_FinalDefence:  // 防御
                        fightAttrVo.Def = Math.Ceiling(attrValue);
                        break;
                    //以下三种伤害客户端无视，需取服务器端下发的eBattleAttr_FinalDefence = 102 用作防御值 
                    case eBattleAttr.eBattleAttr_PhysicDefence:  // 物理防御
                        fightAttrVo.PhysicDef = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_MagicDefence:  // 魔法防御
                        fightAttrVo.MagicDef = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_SorceryDefence:  // 道术防御
                        fightAttrVo.SorceryDef = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_HP_Recovery:  //生命恢复
                        fightAttrVo.Recovery = attrValue;
                        break;
                    case eBattleAttr.eBattleAttr_PetAtk:  //附加的宠物伤害（角色各功能加给宠物身上生效的伤害）
                        fightAttrVo.PetAtkADD = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_Extra_PhysicAttack:  // 物理伤害加成
                        fightAttrVo.PhysicAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_MagicAttack:  // 魔法伤害加成
                        fightAttrVo.MagicAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_SorceryAttack:  // 道术伤害加成
                        fightAttrVo.SorceryAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_HP_Rate:     //生命加成比率
                        fightAttrVo.HPADD = (double)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_PhysicDefence:  // 物理防御加成
                        fightAttrVo.PhysicDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_MagicDefence:  // 魔法防御加成
                        fightAttrVo.MagicDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_SorceryDefence:  // 道术防御加成
                        fightAttrVo.SorceryDefADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_EarthAtk:  // 地系伤害加成
                        fightAttrVo.EarthAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_WaterAtk:  // 水系伤害加成
                        fightAttrVo.WaterAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_FireAtk:  // 火系伤害加成
                        fightAttrVo.FireAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_AirAtk:  // 气系伤害加成
                        fightAttrVo.AirAtkADD = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Multiple_HP_Rate:  // 生命倍率
                        fightAttrVo.HPMultiple = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Multiple_Atk_Rate:  // 伤害倍率
                        fightAttrVo.AtkMultiple = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Jouk_Rate:  // 闪避率
                        fightAttrVo.JoukRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_AtkHit_Rate:  // 普通攻击命中率
                        fightAttrVo.AtkRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Parry_Rate:  // 格挡率
                        fightAttrVo.ParryRate = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Parry_Value:  // 格挡值
                        fightAttrVo.ParryValue = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_Ignore_Defence_Rate:  // 无视防御
                        fightAttrVo.IgnoreDef = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalStrike_Rate:  // 暴击比率
                        fightAttrVo.CriticalStrike =  attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalInjury_Rate:    // 爆伤比率
                        fightAttrVo.CriticalInjury = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_InjuryBoss_Rate:   // BOSS伤害加成比率
                        fightAttrVo.BossDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_InjuryMonster_Rate:   // 小怪伤害加成比率
                        fightAttrVo.MonsterDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_ReduceHurt_Rate:    // 减伤比率
                        fightAttrVo.Mitigation =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_AbsorptionEnemyHP_Rate:   // 吸血比率
                        fightAttrVo.Bloodsucking =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Hurt_HPRecovery:   // 普攻回复的生命具体值
                        fightAttrVo.AtkHPRecovery =  (float) attrValue;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtk_Rate:    // 技能伤害比率
                        fightAttrVo.SkillDamage = attrValue * ConstDefine.CONFIG_PLACE_EX;;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtkMultiple_Rate:   // 技能伤害次数(倍数)加成比率
                        fightAttrVo.MagicTimes = (int) attrValue * ConstDefine.CONFIG_PLACE_EX;;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtkMultiple:    // 技能伤害次数(倍数)
                        fightAttrVo.MagicTimesAdd = (float) attrValue;
                        break;
                    case eBattleAttr.eBattleAttr_DropGold_Rate:    // 掉落金币加成比率
                        fightAttrVo.GoldAdd = (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_SkillCD_Rate:    // 技能冷却比率
                        fightAttrVo.SkillCd =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_AtkSpeed_Rate:   // 攻速比率
                        fightAttrVo.AtkSpeed =  (float)item.AttrValue*ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_ComboAtk_Rate:   // 连击比率
                        fightAttrVo.ComboAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CounterAtk_Rate:   // 反击比率
                        fightAttrVo.CounterAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_RareLoreEquipGen_Rate:    // 稀有传承概率
                        fightAttrVo.SkillCd =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_DropLoreEquip_Rate:   // 传承掉落概率
                        fightAttrVo.AtkSpeed =  (float)item.AttrValue*ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_OnlineStageAwaardMultiple:   // 关卡挂机奖励次数
                        fightAttrVo.ComboAtk =  (float)attrValue;
                        break;
                    case eBattleAttr.eBattleAttr_HomeTownProduce_ExtraLimit:   // 家园挂机时间上限
                        fightAttrVo.CounterAtk =  (float)attrValue;
                        break;
                }
            }
            */
            
            //大地图 buff遗迹的buff
            // ClaimBuff claimBuff = new ClaimBuff();
            // //buff列表  添加buff  根据这个 有没有值  做表现
            // foreach (var value in msg.BattleBuffList)
            // {
            //     BuffInfo buffInfo = new BuffInfo();
            //     foreach (var item in value.BattleAttrList)
            //     {
            //         double attrValue = double.Parse(item.AttrValue.ToString("f4"));
            //         BuffData buffData = new BuffData();
            //         buffData.battleAttr = item.AttrId;
            //         buffData.attrValue = attrValue;
            //         buffInfo.battleAttrList.Add(buffData);
            //     }
            //     buffInfo.startTime = value.StartTime;
            //     buffInfo.endTime = value.EndTime;
            //     buffInfo.cfgId = (int)value.CfgId;
            //     claimBuff.buffInfo = buffInfo;
            // }
            // DataManager.Instance.claimBuff = claimBuff;
            
            // 修改版：大地图遗迹buff
            foreach (var value in msg.BattleBuffList)
            {
                BuffInfo buffInfo = new BuffInfo();
                foreach (var item in value.BattleAttrList)
                {
                    double attrValue = double.Parse(item.AttrValue.ToString("f4"));
                    BuffData buffData = new BuffData();
                    buffData.battleAttr = item.AttrId;
                    buffData.attrValue = attrValue;
                    buffInfo.battleAttrList.Add(buffData);
                }
                buffInfo.startTime = value.StartTime;
                buffInfo.endTime = value.EndTime;
                buffInfo.cfgId = (int)value.CfgId;
                
                DataManager.Instance.AddBuffInfos(buffInfo);
            }
            
            //战斗内触发的buff  技能、天赋、宠物等触发的buff  做表现用，现在没有  值已经加到 BattleAttrList 里面
            // foreach (var value in msg.BattleBuf4StageList)
            // {
            // }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerBattleAttrInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
