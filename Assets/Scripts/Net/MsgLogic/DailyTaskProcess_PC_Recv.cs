using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DailyTaskProcess_PC_Recv : IReceiver
    {
        public DailyTaskProcess_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DailyTaskProcess_PC;
        }

        public void Process()
        {
            TaskInfoManager.Instance.UpdateDailyTaskInfo((int) msg.DailyTask.TaskId, msg.DailyTask.IsClaimAward, (int) msg.DailyTask.TaskProcess);
            TaskInfoManager.Instance.SetTotalActiveBox(msg.ActiveValue);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DAILY_TASK_UPDATE);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DailyTaskProcess_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
