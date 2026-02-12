using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class ReadRedPoints_SCRecv : IReceiver
    {
        public ReadRedPoints_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ReadRedPoints_SC;
        }

        public void Process()
        {
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                ReddotSysManager.Instance.ReadRedPoint(msg.RedPoints.Type);
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_REDPOINT_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
 
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = ReadRedPoints_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
