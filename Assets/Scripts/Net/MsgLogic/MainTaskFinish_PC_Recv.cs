using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class MainTaskFinish_PC_Recv : IReceiver
    {
        public MainTaskFinish_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_MainTaskFinish_PC;
        }

        public void Process()
        {
            TaskInfoManager.Instance.SetCurTaskId((int)msg.FinishedTaskId,(int) msg.FinishedTaskProcess);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = MainTaskFinish_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
