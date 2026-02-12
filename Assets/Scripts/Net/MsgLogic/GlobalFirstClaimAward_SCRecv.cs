using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GlobalFirstClaimAward_SCRecv : IReceiver
    {
        public GlobalFirstClaimAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GlobalFirstClaimAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                PvpRankDataManager.Instance.FightLeftCount = msg.PlayerDailyData.ChallengeOpportunity;
                PvpRankDataManager.Instance.YestdayMyRank = msg.PlayerDailyData.YesterdayRank;
                PvpRankDataManager.Instance.YestdayMyRankAwardGet = msg.PlayerDailyData.HasClaimAward;
                PvpRankDataManager.Instance.YesterdayGFPlayCounter = (int) msg.PlayerDailyData.YesterdayPlayCounter;
                PvpRankDataManager.Instance.TodayGFPlayCounter = (int) msg.PlayerDailyData.TodayPlayCounter;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_FIGHT_COUNT_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PVP_GET_REWARD_UPDATE);
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ClaimedItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
                if (ret.Count > 0)
                {
                    List<ItemData> boxShowItems = new List<ItemData>();
                    foreach (var item in msg.ClaimedItems.BoxRandResultList)
                    {
                        boxShowItems.Add(new ItemData()
                        {
                            id = (int) item.ItemId,
                            count = item.ItemNum,
                            ItemGuid = item.ItemGuid
                        });
                    }
                    // foreach (var item in ret)
                    // {
                    //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                    //     if(itemTypeUnit != null)
                    //         TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                    // }
                    if (ret.Count == 1 && ret[0].count == 1)
                    {
                        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ret[0].id);
                        if (itemTypeUnit.Type == 8) //宝箱
                        {
                            UIManager.Instance.ShowUIPanel("GetReward", boxShowItems);
                            return;
                        }
                    }
                    UIManager.Instance.ShowUIPanel("GetReward", ret, boxShowItems);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GlobalFirstClaimAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
