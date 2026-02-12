using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class GetRedPoints_SCRecv : IReceiver
    {
        public GetRedPoints_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_GetRedPoints_SC;
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
            msg = GetRedPoints_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
