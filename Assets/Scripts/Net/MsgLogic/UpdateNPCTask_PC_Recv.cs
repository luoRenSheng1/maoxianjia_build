using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class UpdateNPCTask_PC_Recv : IReceiver
    {
        public UpdateNPCTask_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_UpdateNPCTask_PC;
        }

        public void Process()
        {   // 随机事件 过期服务器端重新下发新的
            if (msg != null)
            {
                
                if (msg.NpcTask.TaskId == 0 || msg.NpcTask.EndTime < ServerTimeManager.Instance.CurServerTime)
                {//委托任务信息--如果数组为0或 任务id为0或结束时间大于当前时间，表示没有委托任务
                    TaskInfoManager.Instance.SetNpcTask(null);
                }
                else
                {
                    NpcTaskData taskData = new NpcTaskData();
                    taskData.taskId = (int)msg.NpcTask.TaskId;
                    taskData.progress = (int)msg.NpcTask.Process;
                    taskData.startTime = (ulong)msg.NpcTask.StartTime;
                    taskData.endTime = (ulong)msg.NpcTask.EndTime;
                    taskData.taskUnit = ConfigUtils.GetEventTaskUnitById((int)msg.NpcTask.TaskId);
                    TaskInfoManager.Instance.SetNpcTask(taskData);
                }
                
                // 服务器说：不管该事件任务是否过期，都下发该事件任务信息，客户端显示出任务信息
                // if (msg.NpcTask.TaskId > 0)
                // {
                //     NpcTaskData taskData = new NpcTaskData();
                //     taskData.taskId = (int)msg.NpcTask.TaskId;
                //     taskData.progress = (int)msg.NpcTask.Process;
                //     taskData.startTime = (ulong)msg.NpcTask.StartTime;
                //     taskData.endTime = (ulong)msg.NpcTask.EndTime;
                //     TaskInfoManager.Instance.SetNpcTask(taskData);
                // }
                // else
                // {
                //     TaskInfoManager.Instance.SetNpcTask(null);
                // }
                
                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = UpdateNPCTask_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
