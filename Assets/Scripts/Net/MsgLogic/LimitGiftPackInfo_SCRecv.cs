using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    /// <summary>
    /// 限制礼包购买信息请求
    /// </summary>
    public class LimitGiftPackInfo_SCRecv : IReceiver
    {
        public LimitGiftPackInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_LimitGiftPackInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.DailyLimitPacksList)
                {
                    ShopInfoManager.Instance.SetDailyPackFlag((int) item.PackId, (int) item.PurchaseCounter, item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
                }
                
                foreach (var item in msg.WeeklyLimitPacksList)
                {
                    ShopInfoManager.Instance.SetWeekPackFlag((int) item.PackId, (int) item.PurchaseCounter,item.ExpiredTime, item.ClaimAwardCounter, item.LatestClaimedTime);
                }
                
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VIP_INFO_UPDATE);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_GIFTPACK_INFO_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
            
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = LimitGiftPackInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
