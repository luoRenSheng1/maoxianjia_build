using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BeginFishing_SCRecv : IReceiver
    {
        public BeginFishing_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BeginFishing_SC;
        }

        public void Process()
        {   // 开始挥杆返回
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // 钓鱼结果为直接奖励时：
                // 客户端根据客户端的钓鱼结果判断是否需要展示出奖励界面，服务器不管是否成功都会下发奖励信息。
                // 客户端的钓鱼结果为成功，则直接发起领奖请求，失败则不发领奖请求
                
                // 钓鱼结果为普通boss挑战时：
                // msg.BossId是stage表的id
                
                
                // 钓鱼结果为传承boss挑战时：
                // msg.BossId是 EventStage表中的id
                
                
                switch (msg.FishingResult)
                {
                    case eFishingResult.eFishingResult_Nothing:
                        Debug.Log("啥也没有。。。");
                        UIManager.Instance.ShowUIPanel("FishingFail");
                        break;
                    case eFishingResult.eFishingResult_AwardGold:
                        if (MapChapterManager.Instance._fishingSuccess)
                        {
                            // 成功
                            HandlerReward(msg);
                            MapChapterManager.Instance.SendFishingClaimAeard();
                        }
                        break;
                    case eFishingResult.eFishingResult_AwardDiamond:
                        if (MapChapterManager.Instance._fishingSuccess)
                        {
                            // 成功
                            HandlerReward(msg);
                            MapChapterManager.Instance.SendFishingClaimAeard();
                        }
                        break;
                    case eFishingResult.eFishingResult_StageBoss:
                        if (MapChapterManager.Instance._fishingSuccess)
                        {
                            //成功
                            var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                            if (view != null)
                            {
                                view?._bubbleUI.RemoveFromParent();
                                view?._bubbleUI.Dispose();
                            }
                            UIManager.Instance.ShowUIPanel("FishingBossStageDetail", (int)msg.BossId, FishingBossType.CommomBoss);
                        }
                        break;
                    case eFishingResult.eFishingResult_LoreBoss:
                        if (MapChapterManager.Instance._fishingSuccess)
                        {
                            //成功
                            var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                            if (view != null)
                            {
                                view?._bubbleUI.RemoveFromParent();
                                view?._bubbleUI.Dispose();
                            }
                            UIManager.Instance.ShowUIPanel("FishingBossStageDetail", (int)msg.BossId, FishingBossType.InheritBoss);
                        }
                        break;
                    case eFishingResult.eFishingResult_MixedItems:
                        if (MapChapterManager.Instance._fishingSuccess)
                        {
                            // 成功
                            HandlerReward(msg);
                            MapChapterManager.Instance.SendFishingClaimAeard();
                        }
                        break;
                }
                
                
                //通知刷新
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BeginFishing_SC.ParseFrom(mRecv.obj);
            return true;
        }

        private void HandlerReward(BeginFishing_SC msg)
        {
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
            if (ret.Count > 0)
            {
                List<ItemData> boxShowItems = new List<ItemData>();
                foreach (var item in msg.RewardItems.BoxRandResultList)
                {
                    boxShowItems.Add(new ItemData()
                    {
                        id = (int) item.ItemId,
                        count = item.ItemNum,
                        ItemGuid = item.ItemGuid
                    });
                }
                
                if (ret.Count == 1 && ret[0].count == 1)
                {
                    ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ret[0].id);
                    if (itemTypeUnit.Type == 8) //宝箱
                    {
                        UIManager.Instance.ShowUIPanel("FishingReward", boxShowItems);
                        return;
                    }
                }
                UIManager.Instance.ShowUIPanel("FishingReward", ret, boxShowItems);
                
            }
        }
    }
}