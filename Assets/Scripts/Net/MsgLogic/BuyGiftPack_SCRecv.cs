using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class BuyGiftPack_SCRecv : IReceiver
    {
        public BuyGiftPack_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_BuyGiftPack_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.PurchasedItems.ItemsList.ToList(), ref ret);

                foreach (var item in msg.DailyLimitPacksList)
                {
                    ShopInfoManager.Instance.SetDailyPackFlag((int) item.PackId, (int) item.PurchaseCounter, item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
                }
                
                foreach (var item in msg.WeeklyLimitPacksList)
                {
                    ShopInfoManager.Instance.SetWeekPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
                }

                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GIFTPACK_INFO_UPDATE);
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
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = BuyGiftPack_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
