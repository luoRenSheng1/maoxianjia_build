using CommonEx;
using Config;
using msg;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class Stage_End_SCRecv : IReceiver
    {
        public Stage_End_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Stage_End_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                int preChapterId = DataManager.Instance.mRoleData.chapterId;
                int preStageId = DataManager.Instance.mRoleData.stageId;
                // DataManager.Instance.mRoleData.chapterId = (int) msg.NewChapterId;  // TODO 胜利后不再进入下一关还是在当前关卡
                // DataManager.Instance.mRoleData.stageId = (int) msg.NewStageId;
                DataManager.Instance.GetRoleData().subStageId = (int) msg.LatestNode;  //最后挂机节点
                DataManager.Instance.mRoleData.latestPassedStageId = (int) msg.LatestPassedStage;
                DataManager.Instance.GetRoleData().battleStatus = (int) msg.Status;  //战场状态
                
                
                //所处的章节地图关卡信息
                ChapterMapInfoData info = new ChapterMapInfoData();
                info.chapterId = (int)msg.ChapterStage.ChapterId;
                for (int j = 0; j < msg.ChapterStage.StageIdsList.Count; j++)
                {
                    info.stageIdslist.Add((int)msg.ChapterStage.StageIdsList[j]);
                }
                for (int j = 0; j < msg.ChapterStage.BadgeStageIdsList.Count; j++)
                {
                    info.badgeStageIdsList.Add((int)msg.ChapterStage.BadgeStageIdsList[j]);
                }
                MapChapterManager.Instance.SetChapterMapInfo(info);
                
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.GetItemData(msg.RadnomRewardItems.ItemsList.ToList(), ref ret);
                float x = msg.PosX, y = msg.PosY;
                Vector2 pos = new Vector2(x, y);
                for (int i = 0; i < ret.Count; i++)
                {
                    ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ret[i].id);
                    LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    if (UIManager.Instance.IsTopController("Lobby"))
                    {
                        UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();//new UIItemsGain();
                        itemsGain.ApplyItemSourceToDestination(lobbyView?.GetUserHeadIcon(), itemTypeUnit.Icon); //type=3 通关拿到钥匙
                        if (itemTypeUnit.Id == ConstDefine.Item_GoldId)
                        {
                            itemsGain.StartItemFly(pos, ret[i].count);
                        }
                        else
                        {
                            itemsGain.StartItemFly(pos, Random.Range(5, 8));
                        }
                    }

                }
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
                ConfigCommonUnit commonUnit1002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(1002);
                if (!DungeonMapManager.Instance.IsInCopy)
                {
                    GameManager.Instance.TimerManager.SetTimer(int.Parse(commonUnit1002.Param1), () =>
                    {
                        // ConfigChapterUnit chapterUnit1 = ConfigUtils.GetChapterUnitById(preChapterId);
                        // int preIndex = Utils.GetStageIdInChapterIndex(preStageId, chapterUnit1);
                        // ConfigChapterUnit chapterUnit2 = ConfigUtils.GetChapterUnitById(DataManager.Instance.mRoleData.chapterId);
                        // int curIndex = Utils.GetStageIdInChapterIndex(DataManager.Instance.mRoleData.stageId, chapterUnit2);
                        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                        if (lobbyView != null && !lobbyView.IsClickBottom())
                        {
                            //todo 
                            // LogUtils.LogFormat("preChapterId={0} preStageId={1}  ChapterId={2}  StageId={3}", preChapterId,preStageId, DataManager.Instance.mRoleData.chapterId, DataManager.Instance.mRoleData.stageId);
                            // UIManager.Instance.ShowUIPanel("StageBigMap", chapterUnit2.BigMap, curIndex, chapterUnit1.BigMap, preIndex);
                            EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
                        }
                        else
                        {
                            if(!DungeonMapManager.Instance.IsInCopy)
                                EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
                        }
                    });
                }

                if(msg.IsFirst)
                {//首次通关，检测关卡ID，然后进行引导
                    var passId = info.stageIdslist[info.stageIdslist.Count - 1];
                    Debug.Log($"首次通过关卡 = {passId}");
                    var lobby = UIManager.Instance.FindByName("Lobby") as LobbyView;

                    //技能开放
                    lobby?.ChkGuideStart(passId, FuncOpenType.SummonSkill);
                    //宠物开放
                    lobby?.ChkGuideStart(passId, FuncOpenType.SummonPet);
                    //传承装备开放
                    lobby?.ChkGuideStart(passId, FuncOpenType.Inherit);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("Stage_End_SCRecv Read " + mRecv.Length);
            
            msg = Stage_End_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
