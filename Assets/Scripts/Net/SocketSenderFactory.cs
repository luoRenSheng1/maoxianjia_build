using msg;
using UnityEngine;

namespace Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public class SocketSenderFactory
    {
        public ISender CreateSender(int msgID)
        {
            return _GetSender(msgID);
        }

        private ISender _GetSender(int msgID)
        {
            switch (msgID)
            {
                case (int)eMsgID.eTestMsg_Proto2_CS:
                    return new TestMsg_Proto2_CSSend();
                case (int)eMsgID.eMsg_AccountCheck_CS:
                    return new AccountCheck_CSSend();
                case (int)eMsgID.eMsg_PlayerLogin_CS:
                    return new PlayerLogin_CSSend();
                case (int)eMsgID.eMsg_LoginFinish_CS:
                    return new LoginFinish_CSSend();
                case (int)eMsgID.eMsg_ClientPing_CS:
                    return new ClientPing_CSSend();
                case (int) eMsgID.eMsg_Buy_Item_CS:
                    return new Buy_Item_CSSend();
                case (int) eMsgID.eMsg_PlayerChangeHero_CS:
                    return new PlayerChangeHero_CSSend();
                case (int) eMsgID.eMsg_Stage_AwardCommit_CS:
                    return new Stage_AwardCommit_CSSend();
                case (int) eMsgID.eMsg_Stage_Begin_CS:
                    return new Stage_Begin_CSSend();
                case (int) eMsgID.eMsg_Stage_End_CS:
                    return new Stage_End_CSSend();
                case (int) eMsgID.eMsg_Pm_CS:
                    return new PM_CSSend();
                case (int) eMsgID.eMsg_EquipBox_LevelupMilestone_CS:
                    return new EquipBox_LevelupMilestone_CSSend();
                case (int) eMsgID.eMsg_EquipBox_Levelup_CS:
                    return new EquipBox_Levelup_CSSend();
                case (int) eMsgID.eMsg_GenEquips_CS:
                    return new GenEquips_CSSend();
                case (int) eMsgID.eMsg_ReplaceEquip_CS:
                    return new ReplaceEquip_CSSend();
                case (int) eMsgID.eMsg_SellEquip_CS:
                    return new SellEquip_CSSend();
                case (int) eMsgID.eMsg_PetInSlot_CS:
                    return new PetInSlot_CSSend();
                case (int) eMsgID.eMsg_RemovePetFromSlot_CS:
                    return new RemovePetFromSlot_CSSend();
                case (int) eMsgID.eMsg_PetsSwitchInSlots_CS:
                    return new PetsSwitchInSlots_CSSend();
                case (int) eMsgID.eMsg_EquipBox_StartLevelup_CS:
                    return new EquipBox_StartLevelup_CSSend();
                case (int) eMsgID.eMsg_PetLevelUp_CS:
                    return  new PetLevelUp_CSSend();
                case (int) eMsgID.eMsg_ClaimTaskAward_CS:
                    return new ClaimTaskAward_CSSend();
                case (int) eMsgID.eMsg_Copy_Begin_CS:
                    return new Copy_Begin_CSSend();
                case (int) eMsgID.eMsg_Copy_End_CS:
                    return new Copy_End_CSSend();
                case (int) eMsgID.eMsg_DailyReset_CS:
                    return new DailyReset_CSSend();
                case (int) eMsgID.eMsg_GetRedPoints_CS:
                    return new GetRedPoints_CSSend();
                case (int) eMsgID.eMsg_ReadRedPoints_CS:
                    return new ReadRedPoints_CSSend();
                case (int) eMsgID.eMsg_CopyInfo_CS:
                    return new CopyInfo_CSSend();
                case (int) eMsgID.eMsg_Hometown_CS:
                    return new GetHometown_CSSend();
                case (int) eMsgID.eMsg_PetDispatched_CS:
                    return new PetDispatched_CSSend();
                case (int) eMsgID.eMsg_BatchReplacePetInSlot_CS:
                    return new BatchReplacePetInSlot_CSSend();
                case (int) eMsgID.eMsg_ItemUse4Acceleration_CS:
                    return new ItemUse4Acceleration_CSSend();
                case (int) eMsgID.eMsg_BuyGiftPack_CS:
                    return new BuyGiftPack_CSSend();
                case (int) eMsgID.eMsg_RechargeInfo_CS:
                    return new RechargeInfo_CSSend();
                case (int) eMsgID.eMsg_LimitGiftPackInfo_CS:
                    return new LimitGiftPackInfo_CSSend();
                case (int) eMsgID.eMsg_DailyTaskInfo_CS:
                    return new DailyTaskInfo_CSSend();
                case (int) eMsgID.eMsg_ClaimDailyTaskAward_CS:
                    return new ClaimDailyTaskAward_CSSend();
                case (int) eMsgID.eMsg_ClaimOnlineAward_CS:
                    return new ClaimOnlineAward_CSSend();
                case (int) eMsgID.eMsg_OnlineAwardClick_CS:
                    return new OnlineAwardClick_CSSend();
                case (int) eMsgID.eMsg_MailSend_CS:
                    return new MailSend_CSSend();
                case (int) eMsgID.eMsg_MailList_CS:
                    return new MailList_CSSend();
                case (int) eMsgID.eMsg_MailExtract_CS:
                    return new MailExtract_CSSend();
                case (int) eMsgID.eMsg_MailRemove_CS:
                    return new MailRemove_CSSend();
                case (int) eMsgID.eMsg_MailMark_CS:
                    return new MailMark_CSSend();
                case (int) eMsgID.eMsg_ChangePlayerName_CS:
                    return new ChangePlayerName_CSSend();
                case (int) eMsgID.eMsg_AwardCode_CS:
                    return new AwardCode_CSSend();
                case (int) eMsgID.eMsg_DiamondExchange4Gold_CS:
                    return new DiamondExchange4Gold_CSSend();
                case (int) eMsgID.eMsg_FinishedGuideID_CS:
                    return new FinishedGuideID_CSSend();
                case (int) eMsgID.eMsg_DiamondExchange4AccItem_CS:
                    return new DiamondExchange4AccItem_CSSend();
                case (int) eMsgID.eMsg_Chat_CS:
                    return new Chat_CSSend();
                case (int) eMsgID.eMsg_ReplaceEquipAutoSell_CS:
                    return new ReplaceEquipAutoSell_CSSend();
                case (int) eMsgID.eMsg_NewPlayerSignInClaimAward_CS:
                    return new NewPlayerSignInClaimAward_CSSend();
                case (int) eMsgID.eMsg_ApplyRechargeGameOrder_CS:
                    return  new ApplyRechargeGameOrder_CSSend();
                case (int) eMsgID.eMsg_CommitGameOrder_CS:
                    return  new CommitGameOrder_CSSend();
                case (int) eMsgID.eMsg_ClaimStageAwardByAD_CS:
                    return new ClaimStageAwardByAD_CSSend();
                case (int) eMsgID.eMsg_RingAttrLevelUp_CS:
                    return new RingAttrLevelUp_CSSend();
                case (int) eMsgID.eMsg_Stage_Failed_CS:
                    return new Stage_Failed_CSSend();
                case (int) eMsgID.eMsg_HeroAddExp_CS:
                    return new HeroAddExp_CSSend();
                case (int) eMsgID.eMsg_HeroBreak_CS:
                    return new HeroBreak_CSSend();
                case (int) eMsgID.eMsg_HeroPiecesMerge_CS:
                    return new HeroPiecesMerge_CSSend();
                case (int) eMsgID.eMsg_HeroReplaceInBattle_CS:
                    return new HeroReplaceInBattle_CSSend();
                case (int) eMsgID.eMsg_HeroExpItemByFreeAD_CS:
                    return new HeroExpItemByFreeAD_CSSend();
                // case (int) eMsgID.eMsg_AllPetLevelUp_CS:
                //     return new AllPetLevelUp_CSSend();
                case (int) eMsgID.eMsg_SkillInSlot_CS:
                    return new SkillInSlot_CSSend();
                case (int) eMsgID.eMsg_RemoveSkillFromSlot_CS:
                    return new RemoveSkillFromSlot_CSSend();
                case (int) eMsgID.eMsg_AllSkillLevelUp_CS:
                    return new AllSkillLevelUp_CSSend();
                case (int) eMsgID.eMsg_RuneInSlot_CS:
                    return new RuneInSlot_CSSend();
                case (int) eMsgID.eMsg_RemoveRuneFromSlot_CS:
                    return new RemoveRuneFromSlot_CSSend();
                case (int) eMsgID.eMsg_RunesSell_CS:
                    return new RunesSell_CSSend();
                case (int) eMsgID.eMsg_HeroDailyFreeLottery_CS:
                    return new HeroDailyFreeLottery_CSSend();
                case (int) eMsgID.eMsg_HeroLottery_CS:
                    return new HeroLottery_CSSend();
                case (int) eMsgID.eMsg_PetLottery_CS:
                    return new PetLottery_CSSend();
                case (int) eMsgID.eMsg_SkillLottery_CS:
                    return new SkillLottery_CSSend();
                case (int) eMsgID.eMsg_GetHeroMonthActivityInfo_CS:
                    return new GetHeroMonthActivityInfo_CSSend();
                case (int) eMsgID.eMsg_PetSwitchBetweenBuilds_CS:
                    return new PetSwitchBetweenBuilds_CSSend();
                case (int) eMsgID.eMsg_ObtainProduct_CS:
                    return new ObtainProduct_CSSend();
                case (int) eMsgID.eMsg_LeaveHomeTown_CS:
                    return new LeaveHomeTown_CSSend();
                case (int) eMsgID.eMsg_DailyClaimHeroMonthAward_CS:
                    return new DailyClaimHeroMonthAward_CSSend();
                case (int) eMsgID.eMsg_NewPlayerRechargeClaimAward_CS:
                    return new NewPlayerRechargeClaimAward_CSSend();
                case (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_CS:
                    return new DailyAwardOfRechargeInfo_CSSend();
                case (int) eMsgID.eMsg_ClaimDailyAwardOfRechargeAward_CS:
                    return new ClaimDailyAwardOfRechargeAward_CSSend();
                case (int) eMsgID.eMsg_StageGuideAwardInfo_CS:
                    return new StageGuideAwardInfo_CSSend();
                case (int) eMsgID.eMsg_ClaimStageGuideAward_CS:
                    return new ClaimStageGuideAward_CSSend();
                case (int) eMsgID.eMsg_PassTaskInfo_CS:
                    return new PassTaskInfo_CSSend();
                case (int) eMsgID.eMsg_ClaimPassTaskAward_CS:
                    return new ClaimPassTaskAward_CSSend();
                case (int) eMsgID.eMsg_ClaimPassSubTaskExp_CS:
                    return new ClaimPassSubTaskExp_CSSend();
                case (int) eMsgID.eMsg_ClaimGiftAward_CS:
                    return new ClaimGiftAward_CSSend();
                case (int) eMsgID.eMsg_ClaimMallFreeAward_CS:
                    return new ClaimMallFreeAward_CSSend();
                case (int) eMsgID.eMsg_GetTimeCardInfo_CS:
                    return new GetTimeCardInfo_CSSend();
                case (int) eMsgID.eMsg_EquipBox_AccLevelupByAD_CS:
                    return new EquipBox_AccLevelupByAD_CSSend();
                case (int) eMsgID.eMsg_GetCopyFreeTimesByAD_CS:
                    return new GetCopyFreeTimesByAD_CSSend();
                case (int) eMsgID.eMsg_GlobalFirstRank_CS:
                    return new GlobalFirstRank_CSSend();
                case (int) eMsgID.eMsg_GlobalFirstTakeOverRank_CS:
                    return new GlobalFirstTakeOverRank_CSSend();
                case (int) eMsgID.eMsg_GlobalFirstChallengeReq_CS:
                    return new GlobalFirstChallengeReq_CSSend();
                case (int) eMsgID.eMsg_GlobalFirstPvPBattleFinish_CS:
                    return new GlobalFirstPvPBattleFinish_CSSend();
                case (int) eMsgID.eMsg_GlobalFirstClaimAward_CS:
                    return new GlobalFirstClaimAward_CSSend();
                case (int) eMsgID.eMsg_GlobalBuyChallengeCounter_CS:
                    return new GlobalBuyChallengeCounter_CSSend();
                case (int) eMsgID.eMsg_BattlePowerChange_CS:
                    return new BattlePowerChange_CSSend();
                case (int) eMsgID.eMsg_AvatarList_CS:
                    return new AvatarList_CSSend();
                case (int) eMsgID.eMsg_AvatarFrameList_CS:
                    return new AvatarFrameList_CSSend();
                case (int) eMsgID.eMsg_SetAvatar_CS:
                    return new SetAvatar_CSSend();
                case (int) eMsgID.eMsg_SetAvatarFrame_CS:
                    return new SetAvatarFrame_CSSend();
                case (int) eMsgID.eMsg_ChooseStage_CS:
                    return new ChooseStage_CSSend();
                case (int) eMsgID.eMsg_ClassicsLevelUp_CS:
                    return new ClassicsLevelUp_CSSend();
                case (int) eMsgID.eMsg_EnterChapterMap_CS:
                    return new EnterChapterMap_CSSend();
                case (int) eMsgID.eMsg_ClaimRandomBox_CS:
                    return new ClaimRandomBox_CSSend();
                case (int) eMsgID.eMsg_ClaimBuff_CS:
                    return new ClaimBuff_CSSend();
                case (int) eMsgID.eMsg_ClaimRandomFinance_CS:
                    return new ClaimRandomFinance_CSSend();
                case (int) eMsgID.eMsg_AttackWildBossBegin_CS:
                    return new AttackWildBossBegin_CSSend();
                case (int) eMsgID.eMsg_AttackWildBossEnd_CS:
                    return new AttackWildBossEnd_CSSend();
                case (int) eMsgID.eMsg_AttackWildPetBegin_CS:
                    return new AttackWildPetBegin_CSSend();
                case (int) eMsgID.eMsg_AttackWildPetEnd_CS:
                    return new AttackWildPetEnd_CSSend();
                case (int) eMsgID.eMsg_AcceptNPCTask_CS:
                    return new AcceptNPCTask_CSSend();
                case (int) eMsgID.eMsg_ClaimNPCTaskAward_CS:
                    return new ClaimNPCTaskAward_CSSend();
                case (int) eMsgID.eMsg_DropNPCTask_CS:
                    return new DropNPCTask_CSSend();
				case (int) eMsgID.eMsg_BatchTalentsLevelUp_CS:
                    return new BatchTalentsLevelUp_CSSend();
                case (int) eMsgID.eMsg_UnlockTalent_CS:
                    return new UnlockTalent_CSSend();
                case (int) eMsgID.eMsg_ResetTalents_CS:
                    return new ResetTalents_CSSend();
                case (int) eMsgID.eMsg_HeroSkillLevelUp_CS:
                    return new HeroSkillLevelUp_CSSend();
                case (int) eMsgID.eMsg_ReachNodeInStageChallenge_CS:
                    return new ReachNodeInStageChallenge_CSSend();
                case (int) eMsgID.eMsg_ArtifactLevelUp_CS:
                    return new ArtifactLevelUp_CSSend();
                case (int) eMsgID.eMsg_SellLoreEquip_CS:
                    return new SellLoreEquip_CSSend();
                case (int) eMsgID.eMsg_LoreEquipOnPlayer_CS:
                    return new LoreEquipOnPlayer_CSSend();
                case (int) eMsgID.eMsg_TakeOffLoreEquip_CS:
                    return new TakeOffLoreEquip_CSSend();
                case (int) eMsgID.eMsg_ReplaceLoreEquip_CS:
                    return new ReplaceLoreEquip_CSSend();
                case (int) eMsgID.eMsg_BatchSellLoreEquip_CS:
                    return new BatchSellLoreEquip_CSSend();
                case (int) eMsgID.eMsg_PetSkillBookInfo_CS:
                    return new PetSkillBookInfo_CSSend();
                case (int) eMsgID.eMsg_PetSkillLevelUp_CS:
                    return new PetSkillLevelUp_CSSend();
                case (int) eMsgID.eMsg_PetShuffleTalent_CS:
                    return new PetShuffleTalent_CSSend();
                case (int) eMsgID.eMsg_UnlockPetBookSlot_CS:
                    return new UnlockPetBookSlot_CSSend();
                case (int) eMsgID.eMsg_MakeNewSkillBook4Slot_CS:
                    return new MakeNewSkillBook4Slot_CSSend();
                case (int) eMsgID.eMsg_RecyclePets_CS:
                    return new RecyclePets_CSSend();
                case (int) eMsgID.eMsg_BatchRecyclePets_CS:
                    return new BatchRecyclePets_CSSend();
                case (int) eMsgID.eMsg_RecyclePetBooks_CS:
                    return new RecyclePetBooks_CSSend();
                case (int) eMsgID.eMsg_BatchRecyclePetBooks_CS:
                    return new BatchRecyclePetBooks_CSSend();
                case (int) eMsgID.eMsg_PutOnPetBooks_CS:
                    return new PutOnPetBooks_CSSend();
                case (int) eMsgID.eMsg_AchievementList_CS:
                    return new AchievementList_CSSend();
                case (int) eMsgID.eMsg_ClaimAchievementAward_CS:
                    return new ClaimAchievementAward_CSSend();
                case (int) eMsgID.eMsg_HolyItemLevelUp_CS:
                    return new HolyItemLevelUp_CSSend();
                case (int) eMsgID.eMsg_ReplaceHolyItemInSlot_CS:
                    return new ReplaceHolyItemInSlot_CSSend();
                case (int) eMsgID.eMsg_BatchReplaceHolyItemInSlot_CS:
                    return new BatchReplaceHolyItemInSlot_CSSend();
                case (int) eMsgID.eMsg_RemoveHolyItemFromSlot_CS:
                    return new RemoveHolyItemFromSlot_CSSend();
                case (int) eMsgID.eMsg_SwitchHolyItemInSlot_CS:
                    return new SwitchHolyItemInSlot_CSSend();
                case (int) eMsgID.eMsg_TriggerStageBuff_CS:
                    return new TriggerStageBuff_CSSend();
                case (int) eMsgID.eMsg_UseBookItem_CS:
                    return new UseBookItem_CSSend();
                case (int) eMsgID.eMsg_Stage_Exit_CS:
                    return new StageExit_CSSend();
                case (int) eMsgID.eMsg_Camp_Exit_CS:
                    return new CampExit_CSSend();
                case (int) eMsgID.eMsg_Camp_Enter_CS:
                    return new CampEnter_CSSend();
                case (int) eMsgID.eMsg_ShuffleNpcTasks_CS:
                    return new ShuffleNpcTasks_CSSend();
                case (int) eMsgID.eMsg_ExchangeItemByNPCPoints_CS:
                    return new ExchangeItemByNPCPoints_CSSend();
                case (int) eMsgID.eMsg_AcceptWildPet_CS:
                    return new AcceptWildPet_CSSend();
                case (int) eMsgID.eMsg_CatchWildPetSuccess_CS:
                    return new CatchWildPetSuccess_CSSend();
                case (int) eMsgID.eMsg_OfferItem4Task_CS:
                    return new OfferItem4Task_CSSend();
                case (int) eMsgID.eMsg_EnterRuin_CS:
                    return new EnterRuin_CSSend();
                case (int) eMsgID.eMsg_BeginRuinMosterFight_CS:
                    return new BeginRuinMosterFight_CSSend();
                case (int) eMsgID.eMsg_EndRuinMosterFight_CS:
                    return new EndRuinMosterFight_CSSend();
                case (int) eMsgID.eMsg_ExitRuin_CS:
                    return new ExitRuin_CSSend();
                case (int) eMsgID.eMsg_EnterCave_CS:
                    return new EnterCave_CSSend();
                case (int) eMsgID.eMsg_PurchaseInCave_CS:
                    return new PurchaseInCave_CSSend();
                case (int) eMsgID.eMsg_ExitCave_CS:
                    return new ExitCave_CSSend();
				case (int) eMsgID.eMsg_ClaimWildPetAward_CS:
                    return new ClaimWildPetAward_CSSend();
				case (int) eMsgID.eMsg_BeginFishing_CS:
                    return new BeginFishing_CSSend();
                case (int) eMsgID.eMsg_AttackFishingBossBegin_CS:
                    return new AttackFishingBossBegin_CSSend();
                case (int) eMsgID.eMsg_AttackFishingBossEnd_CS:
                    return new AttackFishingBossEnd_CSSend();
                case (int) eMsgID.eMsg_FishingClaimAward_CS:
                    return new FishingClaimAward_CSSend();
                case (int) eMsgID.eMsg_EnterBoxMap_CS:
                    return new EnterBoxMap_CSSend();
                case (int) eMsgID.eMsg_AttackBoxBossBegin_CS:
                    return new AttackBoxBossBegin_CSSend();
                case (int) eMsgID.eMsg_AttackBoxBossEnd_CS:
                    return new AttackBoxBossEnd_CSSend();
                case (int) eMsgID.eMsg_ExitBoxMap_CS:
                    return new ExitBoxMap_CSSend();
                case (int) eMsgID.eMsg_FishingFailed_CS:
                    return new FishingFailed_CSSend();
                case (int)eMsgID.eMsg_FreeSkillLottery4Guide_CS:
                    return new FreeSkillLottery4Guide_CSSend();
                case (int)eMsgID.eMsg_FreePetLottery4Guide_CS:
                    return new FreePetLottery4Guide_CSSend();
                default:
                    Debug.LogErrorFormat("Msg No Define {0}", msgID);
                    break;
            }

            return null;
        }
    }
}

