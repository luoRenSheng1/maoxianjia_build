using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerBattleAttrUpdate_PC_Recv : IReceiver
    {
        public PlayerBattleAttrUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerBattleAttrUpdate_PC;
        }

        public void Process()
        {
            FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
            /*
            foreach (var item in msg.BattleAttrList)
            {
                double attrValue = double.Parse(item.AttrValue.ToString("f4"));
                switch (item.AttrId)
                {
                    case eBattleAttr.eBattleAttr_Atk:
                        fightAttrVo.Atk = Math.Ceiling(attrValue);
                        break;
                    case eBattleAttr.eBattleAttr_HP:
                        fightAttrVo.HP = Math.Ceiling(attrValue);;
                        break;
                    case eBattleAttr.eBattleAttr_HP_Recovery:
                        fightAttrVo.Recovery = attrValue;;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_HP_Rate:
                        fightAttrVo.HPADD = (double)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_Atk_Rate:
                        fightAttrVo.ATKADD = (double) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalStrike_Rate:
                        fightAttrVo.CriticalStrike =  attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CriticalInjury_Rate:
                        fightAttrVo.CriticalInjury = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_InjuryBoss_Rate:
                        fightAttrVo.BossDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_Extra_InjuryMonster_Rate:
                        fightAttrVo.MonsterDamageAdd =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_ReduceHurt_Rate:
                        fightAttrVo.Mitigation =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_AbsorptionEnemyHP_Rate:
                        fightAttrVo.Bloodsucking =  (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtk_Rate:
                        fightAttrVo.SkillDamage = attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtkMultiple_Rate:
                        fightAttrVo.MagicTimes = (int) attrValue * ConstDefine.CONFIG_PLACE_EX;;
                        break;
                    case eBattleAttr.eBattleAttr_SkillAtkMultiple:
                        fightAttrVo.MagicTimesAdd = (float) attrValue;
                        break;
                    case eBattleAttr.eBattleAttr_DropGold_Rate:
                        fightAttrVo.GoldAdd = (float) attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_SkillCD_Rate:
                        fightAttrVo.SkillCd =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_AtkSpeed_Rate:
                        fightAttrVo.AtkSpeed =  (float)item.AttrValue*ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_ComboAtk_Rate:
                        fightAttrVo.ComboAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    case eBattleAttr.eBattleAttr_CounterAtk_Rate:
                        fightAttrVo.CounterAtk =  (float)attrValue * ConstDefine.CONFIG_PLACE_EX;
                        break;
                    
                    //TODO 添加新buff
                }
            }
            RoleManager.Instance.UpdateFightInfo(fightAttrVo);
            */
            FightAttrVo fightAttrVo1 = FightUtils.GetFightAttrVo(msg.BattleAttrList,fightAttrVo);
            LogUtils.LogWarning((int)eMsgID.eMsg_PlayerBattleAttrUpdate_PC + "：服务器更新-伤害:" + fightAttrVo1.Atk + "-----服务器更新-宠物伤害:" + fightAttrVo1.PetAtk + "-----服务器更新-防御:" + fightAttrVo1.Def + "-----服务器更新-生命:" + fightAttrVo1.HP);
            RoleManager.Instance.UpdateFightInfo(FightUtils.GetFightAttrVo(msg.BattleAttrList,fightAttrVo));
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_HERO_Attr);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ATTR_CHANGE_UPATE_PET);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerBattleAttrUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
