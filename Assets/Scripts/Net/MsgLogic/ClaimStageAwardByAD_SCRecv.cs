using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClaimStageAwardByAD_SCRecv : IReceiver
    {
        public ClaimStageAwardByAD_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClaimStageAwardByAD_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.RewardItems.ItemsList.ToList(), ref ret);

                PlayerAttrUtils.UpdateFinance(msg.Finance);
                if (ret.Count > 0)
                {
                    // foreach (var item in ret)
                    // {
                    //     ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(item.id);
                    //     if(itemTypeUnit != null)
                    //         TipsManger.Instance.ShowTip(UIResource.GetItemUrl(itemTypeUnit.Icon),itemTypeUnit.Name + "x"+StringUtils.FormatCurrency(item.count));
                    // }
                    // UIManager.Instance.ShowUIPanel("GetReward", ret);
                    
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
                
                AdManager.Instance.SetAdFreeTime((int) ePlayerAttrID.ePlayerAttrID_OfflineAwardFreeTimes, (int) msg.OfflineRemaindAdFreeTimes);
                AdManager.Instance.SetAdFreeTime((int) ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes, (int) msg.OnlineRemaindAdFreeTimes);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_ONLINEAWARD);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClaimStageAwardByAD_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
