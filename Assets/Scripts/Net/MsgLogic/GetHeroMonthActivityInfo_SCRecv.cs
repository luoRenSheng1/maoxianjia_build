using System.Collections.Generic;
using System.Linq;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GetHeroMonthActivityInfo_SCRecv : IReceiver
    {
        public GetHeroMonthActivityInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetHeroMonthActivityInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ActivityManager.Instance.hasPurchaseMonth1004 = msg.HasPurchase;
                ActivityManager.Instance.GetTodayReward1004 = msg.HasTodayClaimed;
                ActivityManager.Instance.Month1004_EndTime = msg.EndTime;
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GetHeroMonthActivityInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
