using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;

namespace Engine
{
    public class NewPlayerRechargeClaimAward_SCRecv : IReceiver
    {
        public NewPlayerRechargeClaimAward_SC msg;

        public int MsgID()
        {
            return (int) eMsgID.eMsg_NewPlayerRechargeClaimAward_SC;
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
                
                ActivityManager.Instance.UpdateFirstPay((int) msg.SignInfo.Id, (int) msg.SignInfo.ClaimStatus, msg.SignInfo.SigninTime);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FUN_PREVIEW_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FIRST_PAY_UI_UPDATE);// 首充领取状态刷新
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPlayerRechargeClaimAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}