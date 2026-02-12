using System.Runtime.Remoting;
using msg;
using UnityEngine;

namespace Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public class SocketReceiverFactory
    {
        private object threadReceiverLock = new object();
        private Dictionary<int, Queue<IReceiver>> dicReceiver = new Dictionary<int, Queue<IReceiver>>();

        public void Dispose()
        {
            lock (threadReceiverLock)
            {
                try
                {
                    foreach (var item in dicReceiver)
                    {
                        if (item.Value != null)
                        {
                            item.Value.Clear();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtils.LogException(e);
                }

                dicReceiver.Clear();
            }
        }

        public void RecycleReceiver(int msgID, IReceiver handler)
        {
            lock (threadReceiverLock)
            {
                Queue<IReceiver> outArray = null;
                if (!dicReceiver.TryGetValue(msgID, out outArray))
                {
                    outArray = new Queue<IReceiver>();
                    dicReceiver.Add(msgID, outArray);
                }

                if (outArray == null)
                {
                    LogUtils.LogErrorFormat("RecycleReceiver Queue Error {0}", msgID);
                    
                    outArray = new Queue<IReceiver>();
                    dicReceiver.Add(msgID, outArray);
                }
                
                outArray.Enqueue(handler);
            }
        }

        public IReceiver CreateReceiver(int msgID)
        {
            lock (threadReceiverLock)
            {
                IReceiver ret = null;
                Queue<IReceiver> outArray = null;

                if (dicReceiver.TryGetValue(msgID, out outArray))
                {
                    if (outArray != null && outArray.Count > 0)
                    {
                        outArray.TryDequeue(out ret);
                    }
                }
                
                if (ret == null)
                {
                    switch (msgID)
                    {
                        case (int)eMsgID.eTestMsg_Proto2_SC:
                            ret = new TestMsg_Proto2_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_AccountCheck_SC:
                            ret = new AccountCheck_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_PlayerLogin_SC:
                            ret = new PlayerLogin_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_LoginFinish_SC:
                            ret = new LoginFinish_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_ClientPing_SC:
                            ret = new ClientPing_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_PlayerBasicInfo4Login_PC:
                            ret = new PlayerBasicInfo4Login_PC_Recv();
                            break;
                        case (int)eMsgID.eMsg_PlayerEquipsInfo4Login_PC:
                            ret = new PlayerEquipsInfo4Login_PC_Recv();
                            break;
                        case (int)eMsgID.eMsg_PlayerItemsInfo4Login_PC:
                            ret = new PlayerItemsInfo4Login_PC_Recv();
                            break;
                        case (int)eMsgID.eMsg_PlayerPetsInfo4Login_PC:
                            ret = new PlayerPetsInfo4Login_PC_Recv();
                            break;
                        case (int)eMsgID.eMsg_PlayerChangeHero_SC:
                            ret = new PlayerChangeHero_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_Buy_Item_SC:
                            ret = new Buy_Item_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_PlayerInfoUpdate_PC:
                            ret = new PlayerInfoUpdate_PC_Recv();
                            break;
                        case (int)eMsgID.eMsg_Pm_SC:
                            ret = new PM_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_Stage_AwardCommit_SC:
                            ret = new Stage_AwardCommit_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_Stage_Begin_SC:
                            ret = new Stage_Begin_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_Stage_End_SC:
                            ret = new Stage_End_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EquipBox_LevelupMilestone_SC:
                            ret = new EquipBox_LevelupMilestone_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EquipBox_Levelup_SC:
                            ret = new EquipBox_Levelup_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerTaskInfo4Login_PC:
                            ret = new PlayerTaskInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerPetSlotUpdate_PC:
                            ret = new PlayerPetSlotUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GenEquips_SC:
                            ret = new GenEquips_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EquipBox_Info_PC:
                            ret = new EquipBox_Info_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ReplaceEquip_SC:
                            ret = new ReplaceEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_SellEquip_SC:
                            ret = new SellEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewItems_PC:
                            ret = new NewItems_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPets_PC:
                            ret = new NewPets_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PetInSlot_SC:
                            ret = new PetInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RemovePetFromSlot_SC:
                            ret = new RemovePetFromSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetsSwitchInSlots_SC:
                            ret = new PetsSwitchInSlots_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_KickPlayer_SC:
                            ret = new KickPlayer_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EquipBox_StartLevelup_SC:
                            ret = new EquipBox_StartLevelup_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetLevelUp_SC:
                            ret = new PetLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerMainTaskInfoUpdate_PC:
                            ret = new PlayerMainTaskInfoUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_MainTaskFinish_PC:
                            ret = new MainTaskFinish_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PetUpdate_PC:
                            ret = new PetUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimTaskAward_SC:
                            ret = new ClaimTaskAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Copy_Begin_SC:
                            ret = new Copy_Begin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Copy_End_SC:
                            ret = new Copy_End_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DailyReset_SC:
                            ret = new DailyReset_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GetRedPoints_SC:
                            ret = new GetRedPoints_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ReadRedPoints_SC:
                            ret = new ReadRedPoints_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_SyncRedPoints_PC:
                            ret = new SyncRedPoints_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_CopyInfo_SC:
                            ret = new CopyInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Hometown_SC:
                            ret = new GetHometown_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BuildInfo_PC:
                            ret = new BuildInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PetDispatched_SC:
                            ret = new PetDispatched_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerUnlockFunction4Login_PC:
                            ret = new PlayerUnlockFunction4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerUnlockFunction_PC:
                            ret= new PlayerUnlockFunction_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_BatchReplacePetInSlot_SC:
                            ret = new BatchReplacePetInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ItemUse4Acceleration_SC:
                            ret = new ItemUse4Acceleration_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BuyGiftPack_SC:
                            ret = new BuyGiftPack_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_LimitGiftPackInfo_SC:
                            ret = new LimitGiftPackInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RechargeInfo_SC:
                            ret = new RechargeInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Recharge_PC:
                            ret = new Recharge_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DailyTaskInfo_SC:
                            ret = new DailyTaskInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimDailyTaskAward_SC:
                            ret = new ClaimDailyTaskAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DailyTaskProcess_PC:
                            ret = new DailyTaskProcess_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_OfflineAward_PC:
                            ret = new OfflineAward_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimOnlineAward_SC:
                            ret = new ClaimOnlineAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_OnlineAwardClick_SC:
                            ret = new OnlineAwardClick_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailSend_SC:
                            ret = new MailSend_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailList_SC:
                            ret = new MailList_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailExtract_SC:
                            ret = new MailExtract_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailRemove_SC:
                            ret = new MailRemove_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailRemove_PC:
                            ret = new MailRemove_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_MailMark_SC:
                            ret = new MailMark_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MailNew_PC:
                            ret = new MailNew_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ChangePlayerName_SC:
                            ret = new ChangePlayerName_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AwardCode_SC:
                            ret = new AwardCode_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DiamondExchange4Gold_SC:
                            ret = new DiamondExchange4Gold_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerGuideID4Login_PC:
                            ret = new PlayerGuideID4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_FinishedGuideID_SC:
                            ret = new FinishedGuideID_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DiamondExchange4AccItem_SC:
                            ret = new DiamondExchange4AccItem_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Chat_PC:
                            ret = new Chat_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_Chat_SC:
                            ret = new Chat_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ReplaceEquipAutoSell_SC:
                            ret = new ReplaceEquipAutoSell_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewDayReset_PC:
                            ret = new NewDayReset_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPlayerSignInInfo_PC:
                            ret = new NewPlayerSignInInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPlayerSignInClaimAward_SC:
                            ret = new NewPlayerSignInClaimAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_CommitGameOrder_SC:
                            ret = new CommitGameOrder_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ApplyRechargeGameOrder_SC:
                            ret = new ApplyRechargeGameOrder_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_LimitGiftPackInfo_PC:
                            ret = new LimitGiftPackInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimStageAwardByAD_SC:
                            ret = new ClaimStageAwardByAD_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerBattleAttrInfo4Login_PC:
                            ret = new PlayerBattleAttrInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerHeroRolesInfo4Login_PC:
                            ret = new PlayerHeroRolesInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerRunesInfo4Login_PC:
                            ret = new PlayerRunesInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerRuneSlotUpdate_PC:
                            ret = new PlayerRuneSlotUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerSkillInfo4Login_PC:
                            ret = new PlayerSkillInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerSkillSlotUpdate_PC:
                            ret = new PlayerSkillSlotUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerRingInfo4Login_PC:
                            ret = new PlayerRingInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerRingUpdate_PC:
                            ret = new PlayerRingUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerBattleAttrUpdate_PC:
                            ret = new PlayerBattleAttrUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerHeroRoleUpdate_PC:
                            ret = new PlayerHeroRoleUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_RingAttrLevelUp_SC:
                            ret = new RingAttrLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Stage_Failed_SC:
                            ret = new Stage_Failed_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroAddExp_SC:
                            ret = new HeroAddExp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroBreak_SC:
                            ret = new HeroBreak_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroPiecesMerge_SC:
                            ret = new HeroPiecesMerge_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroReplaceInBattle_SC:
                            ret = new HeroReplaceInBattle_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroExpItemByFreeAD_SC:
                            ret = new HeroExpItemByFreeAD_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewHero_PC:
                            ret = new NewHero_PC_Recv();
                            break;
                        // case (int) eMsgID.eMsg_AllPetLevelUp_SC:
                        //     ret = new AllPetLevelUp_SCRecv();
                        //     break;
                        case (int) eMsgID.eMsg_NewSkills_PC:
                            ret = new NewSkills_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_SkillUpdate_PC:
                            ret = new SkillUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_SkillInSlot_SC:
                            ret = new SkillInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RemoveSkillFromSlot_SC:
                            ret = new RemoveSkillFromSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AllSkillLevelUp_SC:
                            ret = new AllSkillLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerEquipSlotUpdate_PC:
                            ret = new PlayerEquipSlotUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelEquips_PC:
                            ret = new DelEquips_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewRune_PC:
                            ret = new NewRune_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_RuneAutoSell_PC:
                            ret = new RuneAutoSell_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_RuneInSlot_SC:
                            ret = new RuneInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RemoveRuneFromSlot_SC:
                            ret = new RemoveRuneFromSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RunesSell_SC:
                            ret = new RunesSell_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroDailyFreeLottery_SC:
                            ret = new HeroDailyFreeLottery_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_HeroLottery_SC:
                            ret = new HeroLottery_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetLottery_SC:
                            ret = new PetLottery_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_SkillLottery_SC:
                            ret = new SkillLottery_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GetHeroMonthActivityInfo_SC:
                            ret = new GetHeroMonthActivityInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PurchaseMonthActivity_PC:
                            ret = new PurchaseMonthActivity_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_LeaveHomeTown_SC:
                            ret = new LeaveHomeTown_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetSwitchBetweenBuilds_SC:
                            ret = new PetSwitchBetweenBuilds_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ObtainProduct_SC:
                            ret = new ObtainProduct_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ProduceEnd_PC:
                            ret = new ProduceEnd_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DailyClaimHeroMonthAward_SC:
                            ret = new DailyClaimHeroMonthAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BuildUnlock_PC:
                            ret = new BuildUnlock_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ProduceIsFull_PC:
                            ret = new ProduceIsFull_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPlayerRechargeClaimAward_SC:
                            ret = new NewPlayerRechargeClaimAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewPlayerRechargeInfo_PC:
                            ret = new NewPlayerRechargeInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PassTaskInfo_SC:
                            ret = new PassTaskInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PassTaskInfo_PC:
                            ret = new PassTaskInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PassTaskProcess_PC:
                            ret = new PassTaskProcess_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PassTaskFinish_PC:
                            ret = new PassTaskFinish_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPassTaskInfo_PC:
                            ret = new NewPassTaskInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PassTaskEnd_PC:
                            ret= new PassTaskEnd_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimPassTaskAward_SC:
                            ret = new ClaimPassTaskAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimPassSubTaskExp_SC:
                            ret = new ClaimPassSubTaskExp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_SC:
                            ret = new DailyAwardOfRechargeInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimDailyAwardOfRechargeAward_SC:
                            ret = new ClaimDailyAwardOfRechargeAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DailyAwardOfRechargeInfo_PC:
                            ret = new DailyAwardOfRechargeInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_StageGuideAwardInfo_SC:
                            ret = new StageGuideAwardInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_StageGuideAwardInfo_PC:
                            ret = new StageGuideAwardInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_RechargeStageGuideAward_PC:
                            ret = new RechargeStageGuideAward_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimStageGuideAward_SC:
                            ret = new ClaimStageGuideAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Recharge4GiftPack_PC:
                            ret = new Recharge4GiftPack_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimMallFreeAward_SC:
                            ret = new ClaimMallFreeAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimGiftAward_SC:
                            ret = new ClaimGiftAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GetTimeCardInfo_SC:
                            ret = new GetTimeCardInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PayUnlockAdvance_PC:
                            ret = new PayUnlockAdvance_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_EquipBox_AccLevelupByAD_SC:
                            ret = new EquipBox_AccLevelupByAD_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GetCopyFreeTimesByAD_SC:
                            ret = new GetCopyFreeTimesByAD_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstRank_SC:
                            ret = new GlobalFirstRank_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstWarReports_PC:
                            ret = new GlobalFirstWarReports_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstRank_PC:
                            ret = new GlobalFirstRank_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstPlayerInfo_PC:
                            ret = new GlobalFirstPlayerInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstRankNewWarReport_PC:
                            ret = new GlobalFirstRankNewWarReport_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstTakeOverRank_SC:
                            ret = new GlobalFirstTakeOverRank_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstChallengeReq_SC:
                            ret = new GlobalFirstChallengeReq_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstPvPStart_PC:
                            ret = new GlobalFirstPvPStart_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstPvPBattleFinish_SC:
                            ret = new GlobalFirstPvPBattleFinish_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstDefeatedInfo_PC:
                            ret = new GlobalFirstDefeatedInfo_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_GlobalFirstClaimAward_SC:
                            ret = new GlobalFirstClaimAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_GlobalBuyChallengeCounter_SC:
                            ret = new GlobalBuyChallengeCounter_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BattlePowerChange_SC:
                            ret = new BattlePowerChange_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AvatarList_SC:
                            ret = new AvatarList_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AvatarFrameList_SC:
                            ret = new AvatarFrameList_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewAvatars_PC:
                            ret = new NewAvatars_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewAvatarFrames_PC:
                            ret = new NewAvatarFrames_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelAvatars_PC:
                            ret = new DelAvatars_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelAvatarFrames_PC:
                            ret = new DelAvatarFrames_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_SetAvatar_SC:
                            ret = new SetAvatar_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_SetAvatarFrame_SC:
                            ret = new SetAvatarFrame_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ChooseStage_SC:
                            ret = new ChooseStage_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClassicsLevelUp_SC:
                            ret = new ClassicsLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerOccupation2Classics4Login_PC:
                            ret = new PlayerOccupation2Classics4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerOccupation2ClassicsUpdate_PC:
                            ret = new PlayerOccupation2ClassicsUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerUnlockNewClassics_PC:
                            ret = new PlayerUnlockNewClassics_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerChapterMapInfo4Login_PC: //包括登录后所处的章节地图关卡信息 和 随机任务信息
                            ret = new PlayerChapterMapInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerNPCTask4Login_PC:  //委托任务信息--如果数组为0或 任务id为0或结束时间大于当前时间，表示没有委托任务
                            ret = new PlayerNPCTask4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_RandomEventsUpdate_PC: // 随机事件 部分事件内容发生变更
                            ret = new RandomEventsUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewRandomEvents_PC:  // 随机事件 过期服务器端重新下发新的
                            ret = new NewRandomEvents_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimRandomBox_SC:  // 领取随机宝箱
                            ret = new ClaimRandomBox_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EnterChapterMap_SC:  // 进入新章节地图
                            ret = new EnterChapterMap_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimBuff_SC:  // 获取buff
                            ret = new ClaimBuff_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimRandomFinance_SC:  // 领取金币或钻石随机事件
                            ret = new ClaimRandomFinance_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackWildBossBegin_SC:  // 野外boss  开始攻击
                            ret = new AttackWildBossBegin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackWildBossEnd_SC:  // 野外boss  战斗结束
                            ret = new AttackWildBossEnd_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackWildPetBegin_SC:  // 逃跑的宠物  开始攻击
                            ret = new AttackWildPetBegin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackWildPetEnd_SC:  // 逃跑的宠物  战斗结束
                            ret = new AttackWildPetEnd_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AcceptNPCTask_SC:  // NPC委托任务  接受委托任务
                            ret = new AcceptNPCTask_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ClaimNPCTaskAward_SC:  // NPC委托任务  领取任务奖励
                            ret = new ClaimNPCTaskAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DropNPCTask_SC:  // NPC委托任务  领取任务奖励
                            ret = new DropNPCTask_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_UpdateNPCTask_PC:  // NPC委托任务  任务进度与状态更新
                            ret = new UpdateNPCTask_PC_Recv();
                            break;
						case (int) eMsgID.eMsg_PlayerTalents4Login_PC:
                            ret = new PlayerTalents4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewTalents_PC:
                            ret = new NewTalents_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelTalents_PC:
                            ret = new DelTalents_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_BatchTalentsLevelUp_SC:
                            ret = new BatchTalentsLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_UnlockTalent_SC:
                            ret = new UnlockTalent_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ResetTalents_SC:
                            ret = new ResetTalents_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_TalentUpdate_PC:
                            ret = new TalentUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_HeroSkillLevelUp_SC:
                            ret = new HeroSkillLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ReachNodeInStageChallenge_SC:
                            ret = new ReachNodeInStageChallenge_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ArtifactLevelUp_SC:
                            ret = new ArtifactLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerLoreEquipSlotUpdate_PC:
                            ret = new PlayerLoreEquipSlotUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerLoreEquipsInfo4Login_PC:
                            ret = new PlayerLoreEquipsInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewLoreEquips_PC:
                            ret = new NewLoreEquips_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelLoreEquips_PC:
                            ret = new DelLoreEquips_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_SellLoreEquip_SC:
                            ret = new SellLoreEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_LoreEquipOnPlayer_SC:
                            ret = new LoreEquipOnPlayer_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_TakeOffLoreEquip_SC:
                            ret = new TakeOffLoreEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ReplaceLoreEquip_SC:
                            ret = new ReplaceLoreEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BatchSellLoreEquip_SC:
                            ret = new BatchSellLoreEquip_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetSkillBookInfo_SC:
                            ret = new PetSkillBookInfo_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewPetSkillBook_PC:
                            ret = new NewPetSkillBook_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_UpdatePetSkillBook_PC:
                            ret = new UpdatePetSkillBook_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PetSkillLevelUp_SC:
                            ret = new PetSkillLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PetShuffleTalent_SC:
                            ret = new PetShuffleTalent_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_UnlockPetBookSlot_SC:
                            ret = new UnlockPetBookSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_MakeNewSkillBook4Slot_SC:
                            ret = new MakeNewSkillBook4Slot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RecyclePets_SC:
                            ret = new RecyclePets_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BatchRecyclePets_SC:
                            ret = new BatchRecyclePets_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RecyclePetBooks_SC:
                            ret = new RecyclePetBooks_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BatchRecyclePetBooks_SC:
                            ret = new BatchRecyclePetBooks_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PutOnPetBooks_SC:
                            ret = new PutOnPetBooks_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DelPet_PC:
                            ret = new DelPet_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelPetBooks_PC:
                            ret = new DelPetBooks_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_AchievementList_SC:
                            ret = new AchievementList_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_NewAchievement_PC:
                            ret = new NewAchievement_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_AchievementUpdate_PC:
                            ret = new AchievementUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_ClaimAchievementAward_SC:
                            ret = new ClaimAchievementAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerHolyItemSlotInfoUpdate_PC:
                            ret = new PlayerHolyItemSlotInfoUpdate_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_NewPlayerHolyItem_PC:
                            ret = new NewPlayerHolyItem_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_UpdatePlayerHolyItem_PC:
                            ret = new UpdatePlayerHolyItem_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_DelPlayerHolyItem_PC:
                            ret = new DelPlayerHolyItem_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_HolyItemLevelUp_SC:
                            ret = new HolyItemLevelUp_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ReplaceHolyItemInSlot_SC:
                            ret = new ReplaceHolyItemInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BatchReplaceHolyItemInSlot_SC:
                            ret = new BatchReplaceHolyItemInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_RemoveHolyItemFromSlot_SC:
                            ret = new RemoveHolyItemFromSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_SwitchHolyItemInSlot_SC:
                            ret = new SwitchHolyItemInSlot_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PlayerHolyItemsInfo4Login_PC:
                            ret = new PlayerHolyItemsInfo4Login_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerBattleAttrAddBuff_PC:
                            ret = new PlayerBattleAttrAddBuff_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_PlayerBattleAttrDelBuff_PC:
                            ret = new PlayerBattleAttrDelBuff_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_TriggerStageBuff_SC:
                            ret = new TriggerStageBuff_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_UseBookItem_SC:
                            ret = new UseBookItem_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Stage_Exit_SC:
                            ret = new StageExit_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Camp_Exit_SC:
                            ret = new CampExit_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_Camp_Enter_SC:
                            ret = new CampEnter_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ShuffleNpcTasks_SC:
                            ret = new ShuffleNpcTasks_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ExchangeItemByNPCPoints_SC:
                            ret = new ExchangeItemByNPCPoints_SCRecv();
                            break;
						case (int) eMsgID.eMsg_AcceptWildPet_SC:
                            ret = new AcceptWildPet_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_CatchWildPetSuccess_SC:
                            ret = new CatchWildPetSuccess_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_DelRandomEvents_PC:
                            ret = new DelRandomEvents_PC_Recv();
                            break;
                        case (int) eMsgID.eMsg_OfferItem4Task_SC:
                            ret = new OfferItem4Task_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EnterRuin_SC:
                            ret = new EnterRuin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_BeginRuinMosterFight_SC:
                            ret = new BeginRuinMosterFight_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EndRuinMosterFight_SC:
                            ret = new EndRuinMosterFight_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ExitRuin_SC:
                            ret = new ExitRuin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EnterCave_SC:
                            ret = new EnterCave_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_PurchaseInCave_SC:
                            ret = new PurchaseInCave_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ExitCave_SC:
                            ret = new ExitCave_SCRecv();
                            break;
						case (int) eMsgID.eMsg_ClaimWildPetAward_SC:
                            ret = new ClaimWildPetAward_SCRecv();
                            break;
						case (int) eMsgID.eMsg_BeginFishing_SC:
                            ret = new BeginFishing_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackFishingBossBegin_SC:
                            ret = new AttackFishingBossBegin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackFishingBossEnd_SC:
                            ret = new AttackFishingBossEnd_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_FishingClaimAward_SC:
                            ret = new FishingClaimAward_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_EnterBoxMap_SC:
                            ret = new EnterBoxMap_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackBoxBossBegin_SC:
                            ret = new AttackBoxBossBegin_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_AttackBoxBossEnd_SC:
                            ret = new AttackBoxBossEnd_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_ExitBoxMap_SC:
                            ret = new ExitBoxMap_SCRecv();
                            break;
						case (int) eMsgID.eMsg_FishingFailed_SC:
                            ret = new FishingFailed_SCRecv();
                            break;
                        case (int) eMsgID.eMsg_FreePetLottery4Guide_SC:
                            ret = new FreePetLottery4Guide_SCRecv();
                            break;
                        case (int)eMsgID.eMsg_FreeSkillLottery4Guide_SC:
                            ret = new FreeSkillLottery4Guide_SCRecv();
                            break;
                        default:  
                            Debug.LogErrorFormat("Msg No Define {0}", msgID);
                            break;
                    }
                }

                return ret;
            }
        }
    }
}