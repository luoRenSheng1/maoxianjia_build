using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DailyReset_SCRecv : IReceiver
    {
        public DailyReset_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyReset_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ReddotSysManager.Instance.SendRedPointCS();
            }
           
            ServerTimeManager.Instance.SyncServerTime(msg.ServerTimeStamp);
            ServerTimeManager.Instance.SetNextToZeroServerTime(msg.ServerDailyResetTimeStamp);
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DAILY_RESET_UPDATE);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_SHOP_REDDOT);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyReset_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
