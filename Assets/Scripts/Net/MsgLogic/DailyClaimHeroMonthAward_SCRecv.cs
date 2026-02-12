using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DailyClaimHeroMonthAward_SCRecv : IReceiver
    {
        public DailyClaimHeroMonthAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyClaimHeroMonthAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.AwardItemsList.ToList(), ref ret);
            
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                if (ret.Count > 0)
                {
                    // foreach (var item in ret)
                    // {
                    //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                    //     if(itemTypeUnit != null)
                    //         TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                    // }
                    UIManager.Instance.ShowUIPanel("GetReward", ret);
                }

                if (msg.ActivityId == 1004)//英雄特权卡
                {
                    ActivityManager.Instance.GetTodayReward1004 = msg.HasTodayClaimed;
                    ActivityManager.Instance.Month1004_EndTime = msg.EndTime;
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS);
                }else if (msg.ActivityId == 1001 || msg.ActivityId == 1002 || msg.ActivityId == 1003)
                {
                    ActivityManager.Instance.SetCardTodayGet((CardType) msg.ActivityId, true);
                    ActivityManager.Instance.SetCardPurchaseEndTime((CardType) msg.ActivityId, msg.EndTime);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyClaimHeroMonthAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
