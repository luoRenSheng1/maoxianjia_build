using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstPvPStart_PC_Recv : IReceiver
    {
        public GlobalFirstPvPStart_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstPvPStart_PC;
        }

        public void Process()
        {   //下发被挑战者数据，客户端pvp开始
            PvpRankVo pvpRankVo = new PvpRankVo();
            pvpRankVo.HeroId = (int) msg.OpponentData.HeroId;
            pvpRankVo.Fight = msg.OpponentData.BattlePower;
            pvpRankVo.PlayerId = msg.OpponentData.UserId.ToString();
            pvpRankVo.PlayerName = msg.OpponentData.UserName;
            pvpRankVo.Rank = (int) msg.OpponentData.Rank;
            // pvpRankVo.PlayerHeadIcon = msg.OpponentData.HeroId.ToString();
            pvpRankVo.PlayerHeadIcon = msg.OpponentData.AvatarId.ToString();//头像
            PvpRankDataManager.Instance.OtherPvpRankVo = pvpRankVo;
            
            FightAttrVo fightAttrVo = new FightAttrVo();
            /*
            foreach (var item in msg.OpponentBattleData.BattleAttrList)
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
                        fightAttrVo.SkillDamage = attrValue * ConstDefine.CONFIG_PLACE_EX;;
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
                }
            }
            PvpRankDataManager.Instance.OtherFightAttrVo = fightAttrVo;
            */
            PvpRankDataManager.Instance.OtherFightAttrVo = FightUtils.GetFightAttrVo(msg.OpponentBattleData.BattleAttrList, fightAttrVo);
            
            List<RuneInfo> runeList = new List<RuneInfo>();
            int runeIndex = -1;
            foreach (var item in msg.OpponentBattleData.RuneInSlotsList)
            {
                runeIndex++;
                RuneInfo runeInfo = new RuneInfo();
                runeInfo.Guid = item.Guid;
                runeInfo.ItemId = (int) item.ItemId;
                runeInfo.SkillId = (int) item.SkillId;
                runeInfo.Level = (int) item.Level;
                runeInfo.MagicTimes = (int) item.AtkCounter;
                runeInfo.BattleIndex = runeIndex;
                runeList.Add(runeInfo);
            }
            PvpRankDataManager.Instance.SetOtherBattleRuneInfoList(runeList);
            
            List<SkillInfo> skillList = new List<SkillInfo>();
            int index = -1;
            foreach (var item in msg.OpponentBattleData.SkillInSlotsList)
            {
                index++;
                SkillInfo skill = new SkillInfo();
                skill.Level = (int) item.Level;
                skill.SkillId = (int) item.SkillId;
                skill.SkillAtkValue = item.SkillAtkValue;
                skill.Level = (int) item.Level;
                skill.BattleIndex = index;
                skillList.Add(skill);
            }
            PvpRankDataManager.Instance.SetOtherBattleSkillInfoList(skillList);
            
            List<PetItemInfo> petList = new List<PetItemInfo>();
            int battleIndex = -1;
            foreach (var item in msg.OpponentBattleData.PetInSlotsList)
            {
                PetItemInfo pet = PetInfoManager.Instance.GetPetInfoBySever(item);
                battleIndex++;
                pet.BattleIndex = battleIndex;
                petList.Add(pet);
            }
            PvpRankDataManager.Instance.SetOtherBattlePetList(petList);
            PvpRankDataManager.Instance.OpponentData = msg.OpponentData;
            UIManager.Instance.ShowUIPanel("PVPMapMain", RoleManager.Instance.TotalFight, PvpRankDataManager.Instance.OtherPvpRankVo.Fight,DataManager.Instance.mRoleData.avatarID,PvpRankDataManager.Instance.OtherPvpRankVo.PlayerHeadIcon);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstPvPStart_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
