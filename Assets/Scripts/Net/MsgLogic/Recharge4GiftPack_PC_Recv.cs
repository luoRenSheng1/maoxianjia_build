using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 礼包专用 PC包
    /// </summary>
    public class Recharge4GiftPack_PC_Recv : IReceiver
    {
        public Recharge4GiftPack_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_Recharge4GiftPack_PC;
        }

        public void Process()
        {
            foreach (var item in msg.DailyLimitPacksList)
            {
                ShopInfoManager.Instance.SetDailyPackFlag((int) item.PackId, (int) item.PurchaseCounter, item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
                
            foreach (var item in msg.WeeklyLimitPacksList)
            {
                ShopInfoManager.Instance.SetWeekPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            foreach (var item in msg.DoubleWeeklyLimitPacksList)
            {
                ShopInfoManager.Instance.SetDoublePackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            foreach (var item in msg.MonthlyLimitPacksList)
            {
                ShopInfoManager.Instance.SetMonthlyPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
            }
            
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.PurchasedItems.ItemsList.ToList(), ref ret);
            
            PlayerAttrUtils.UpdateFinance(msg.Finance);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_LOGINGIFT_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RECHARGE_GIFT_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
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
            msg = Recharge4GiftPack_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
