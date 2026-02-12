using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class PassTaskProcess_PC_Recv : IReceiver
    {
        public PassTaskProcess_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PassTaskProcess_PC;
        }

        public void Process()
        {
            PassPortInfo passPortInfo = ActivityManager.Instance.GetPassPortInfo();
            if(passPortInfo == null) return;
            foreach (var item in passPortInfo.DailySubTasks)
            {
                if (item.TaskId == msg.SubTask.TaskId)
                {
                    item.Progress = (ulong) msg.SubTask.Process;
                    item.HasGetReward = msg.SubTask.HasClaimed;
                    break;
                }
            }
            
            foreach (var item in passPortInfo.WeekSubTasks)
            {
                if (item.TaskId == msg.SubTask.TaskId)
                {
                    item.Progress = (ulong) msg.SubTask.Process;
                    item.HasGetReward = msg.SubTask.HasClaimed;
                    break;
                }
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_PASSPORT);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PASSPORT_GetSubTask);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PassTaskProcess_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
