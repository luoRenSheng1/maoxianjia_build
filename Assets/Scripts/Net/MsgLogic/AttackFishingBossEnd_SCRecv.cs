using System.Collections.Generic;
using System.Linq;
using Config;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class AttackFishingBossEnd_SCRecv : IReceiver
    {
        public AttackFishingBossEnd_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AttackFishingBossEnd_SC;
        }

        public void Process()
        {   // 钓鱼boss结束攻击回复
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                if (msg.IsWin)
                {
                    PlayerAttrUtils.UpdateFinance(msg.Finance);  //更新 最新账户余额
                    
                    // 今日钓鱼次数
                    // MapChapterManager.Instance.UpdateFishingCount((int)msg.TodayFishingCounter);
                    FinishEventCount finishEventCount = new FinishEventCount();
                    finishEventCount.eventType = (int)StageEventType.Fishing;
                    finishEventCount.finishCount = (int)msg.TodayFishingCounter;
                    MapChapterManager.Instance.UpdateFinishEventCountListInfo(finishEventCount);
                    
                    List<ItemData> ret = new List<ItemData>();
                    PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);
                    // PlayerAttrUtils.GetItemDataWithGuid(msg.RewardItems.ItemsList.ToList(), ref ret);
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
            msg = AttackFishingBossEnd_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}