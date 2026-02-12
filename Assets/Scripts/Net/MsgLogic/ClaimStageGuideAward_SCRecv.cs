using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public class ClaimStageGuideAward_SCRecv : IReceiver
    {
        public ClaimStageGuideAward_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_ClaimStageGuideAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ActivityManager.Instance.ClaimStageGuideAward((uint)msg.StageId);
                
                List<ItemData> ret = new List<ItemData>();
                // PlayerAttrUtils.GetItemData(msg.ClaimedItemsList.ToList(),ref ret);
                PlayerAttrUtils.GetItemData(msg.ClaimedItems.ItemsList.ToList(),ref ret);
                PlayerAttrUtils.UpdateFinance(msg.Finance);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CRAZY_GUIDE_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GUIDE_AWARD_GET);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
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
                    UIManager.Instance.ShowUIPanel("GetReward", ret,boxShowItems);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimStageGuideAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}