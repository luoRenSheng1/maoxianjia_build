using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ClientPing_SCRecv : IReceiver
    {
        public ClientPing_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ClientPing_SC;
        }

        public void Process()
        {
            var conn = GameManager.Instance.Connection;
            
            if (conn != null && conn.EventHandler != null)
            {
                conn.EventHandler.OnHeartBeatRecv(0);
            }
            //同步服务器时间
            ServerTimeManager.Instance.SyncServerTime(msg.TimeStamp);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ClientPing_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
