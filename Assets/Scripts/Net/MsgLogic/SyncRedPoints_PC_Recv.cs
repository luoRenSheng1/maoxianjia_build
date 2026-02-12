using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class SyncRedPoints_PC_Recv : IReceiver
    {
        public SyncRedPoints_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_SyncRedPoints_PC;
        }

        public void Process()
        {
            foreach (var item in msg.RedPointsList)
            {
                RedPointInfo redPointInfo = new RedPointInfo();
                redPointInfo.Type = item.Type;
                redPointInfo.Param = item.Param;
                redPointInfo.IsRead = false;
                ReddotSysManager.Instance.SyncRedPointInfo(item.Type, redPointInfo);
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = SyncRedPoints_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
