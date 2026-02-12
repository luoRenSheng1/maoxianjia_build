using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GetTimeCardInfo_SCRecv : IReceiver
    {
        public GetTimeCardInfo_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetTimeCardInfo_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                foreach (var item in msg.TimeCardList)
                {
                    ActivityManager.Instance.SetCardPurchase((CardType) item.Id, item.EndTime>ServerTimeManager.Instance.CurServerTime);
                    ActivityManager.Instance.SetCardTodayGet((CardType) item.Id, item.HasTodayClaimed);
                    ActivityManager.Instance.SetCardPurchaseEndTime((CardType) item.Id, item.EndTime);
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = GetTimeCardInfo_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
