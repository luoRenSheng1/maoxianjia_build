using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewPlayerSignInClaimAward_SCRecv : IReceiver
    {
        public NewPlayerSignInClaimAward_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewPlayerSignInClaimAward_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ActivityManager.Instance.UpdateSevenDay(msg.SignInfo.Id, msg.SignInfo.ClaimStatus+1, msg.SignInfo.SigninTime);
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
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewPlayerSignInClaimAward_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
