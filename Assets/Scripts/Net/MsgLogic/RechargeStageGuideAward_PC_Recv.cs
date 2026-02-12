using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public class RechargeStageGuideAward_PC_Recv : IReceiver
    {
        public RechargeStageGuideAward_PC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_RechargeStageGuideAward_PC;
        }

        public void Process()
        {
            List<ItemData> ret = new List<ItemData>();
            PlayerAttrUtils.GetItemData(msg.PurchasedItems.ItemsList.ToList(), ref ret);
            ActivityManager.Instance.UpdateGuideBuyIdInfo(msg.PurchasedChapterList.ToList());
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CRAZY_GUIDE_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CRAZY_GUIDE_PAY_SUCCESS);
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
            msg = RechargeStageGuideAward_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}