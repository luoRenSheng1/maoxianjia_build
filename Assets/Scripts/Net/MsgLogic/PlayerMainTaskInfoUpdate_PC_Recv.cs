using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerMainTaskInfoUpdate_PC_Recv : IReceiver
    {
        public PlayerMainTaskInfoUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerMainTaskInfoUpdate_PC;
        }

        public void Process()
        {
            TaskInfoManager.Instance.SetCurTaskId((int)msg.MainTaskId,(int) msg.MainTaskProcess);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerMainTaskInfoUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
