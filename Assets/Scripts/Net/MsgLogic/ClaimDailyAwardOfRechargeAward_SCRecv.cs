using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;
using UnityEngine.UIElements;
using EventDispatcher = EngineBase.EventDispatcher;

namespace Engine
{
    public class ClaimDailyAwardOfRechargeAward_SCRecv : IReceiver
    {
        public ClaimDailyAwardOfRechargeAward_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ClaimDailyAwardOfRechargeAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ActivityManager.Instance.UpdateDailyAwardInfo(msg.DayId, 2);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DAILY_AWARD_UPDATE);

                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.ClaimedItems.ItemsList.ToList(), ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_FREE_GETITEM);
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
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimDailyAwardOfRechargeAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}