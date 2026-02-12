using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class HeroExpItemByFreeAD_SCRecv : IReceiver
    {
        public HeroExpItemByFreeAD_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_HeroExpItemByFreeAD_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                AdManager.Instance.SetAdFreeTime((int) ePlayerAttrID.ePlayerAttrID_HeroExpItemFreeTimes, (int) msg.RemainFreeTimes);
                List<ItemData> ret = new List<ItemData>();
                PlayerAttrUtils.GetItemData(msg.AwardItemsList.ToList(), ref ret);
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
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ITEM_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_WATCHADTOGET_HEROLEVELUP_Item);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }

        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = HeroExpItemByFreeAD_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
