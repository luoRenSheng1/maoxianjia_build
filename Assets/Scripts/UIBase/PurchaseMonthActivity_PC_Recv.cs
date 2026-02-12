using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PurchaseMonthActivity_PC_Recv : IReceiver
    {
        public PurchaseMonthActivity_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PurchaseMonthActivity_PC;
        }

        public void Process()
        {
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.PurchasedItems.ItemsList.ToList(), ref ret);
            if (msg.ActivityId == 1004)//英雄特权卡
            {
                ActivityManager.Instance.hasPurchaseMonth1004 = msg.RechargeStatus == eRechargeStatus.eRechargeStatus_Success;
                ActivityManager.Instance.GetTodayReward1004 = true;
                ActivityManager.Instance.Month1004_EndTime = msg.EndTimestamp;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS);
            }else if (msg.ActivityId == 1001 || msg.ActivityId == 1002 || msg.ActivityId == 1003)
            {
                ActivityManager.Instance.SetCardPurchase((CardType) msg.ActivityId, msg.RechargeStatus == eRechargeStatus.eRechargeStatus_Success);
                ActivityManager.Instance.SetCardTodayGet((CardType) msg.ActivityId, true);
                ActivityManager.Instance.SetCardPurchaseEndTime((CardType) msg.ActivityId, msg.EndTimestamp);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS);
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
            PlayerAttrUtils.UpdateFinance(msg.Finance);
            
            if (ret.Count > 0)
            {
                List<ItemData> boxShowItems = new List<ItemData>();
                foreach (var item in msg.PurchasedItems.BoxRandResultList)
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

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PurchaseMonthActivity_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
