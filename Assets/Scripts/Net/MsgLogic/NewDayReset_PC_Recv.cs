using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class NewDayReset_PC_Recv : IReceiver
    {
        public NewDayReset_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_NewDayReset_PC;
        }

        public void Process()
        {
            ServerTimeManager.Instance.SyncServerTime(msg.TimeStamp);
            ServerTimeManager.Instance.SetNextToZeroServerTime(msg.NextResetTimeStamp);
            ServerTimeManager.Instance.SetNextWeekServerTime(msg.ServerWeeklyResetTimeStamp);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = NewDayReset_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
