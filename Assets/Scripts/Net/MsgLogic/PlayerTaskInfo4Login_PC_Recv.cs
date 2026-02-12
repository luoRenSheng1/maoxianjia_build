using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PlayerTaskInfo4Login_PC_Recv : IReceiver
    {
        public PlayerTaskInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerTaskInfo4Login_PC;
        }

        public void Process()
        {
            TaskVo taskVo = new TaskVo();
            taskVo.MainTaskId = (int) msg.MainTaskId;
            taskVo.MainTaskProgress = (int) msg.MainTaskProcess;
            TaskInfoManager.Instance.SetCurMainTask(taskVo);

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_TASK_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerTaskInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
