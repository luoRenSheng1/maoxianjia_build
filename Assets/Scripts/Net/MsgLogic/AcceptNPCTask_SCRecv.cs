using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class AcceptNPCTask_SCRecv : IReceiver
    {
        public AcceptNPCTask_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_AcceptNPCTask_SC;
        }

        public void Process()
        {
            // NPC委托任务 接受委托任务
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // foreach (var taskItem in TaskInfoManager.Instance.GetNpcTaskList())
                // {
                //     if (taskItem.taskId == msg.NpcTask.TaskId)
                //     {
                //         taskItem.progress = (int)msg.NpcTask.Process;
                //         taskItem.startTime = (ulong)msg.NpcTask.StartTime;
                //         taskItem.endTime = (ulong)msg.NpcTask.EndTime;
                //     }
                // }
                NpcTaskData taskData = new NpcTaskData();
                taskData.taskId = (int)msg.NpcTask.TaskId;
                taskData.progress = (int)msg.NpcTask.Process;
                taskData.startTime = (ulong)msg.NpcTask.StartTime;
                taskData.endTime = (ulong)msg.NpcTask.EndTime;
                taskData.taskUnit = ConfigUtils.GetEventTaskUnitById((int)msg.NpcTask.TaskId);

                TaskInfoManager.Instance.SetNpcTask(taskData);
                
                
                //事件
                RandomEventData randomEventData = new RandomEventData();
                randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                
                BatchStuff stuff = new BatchStuff();
                stuff.id = (int)msg.BatchStuff.Id;//服务器维护id
                stuff.amount = (int)msg.BatchStuff.Amount;
                stuff.eventStatus = (int)msg.BatchStuff.EventStatus;//状态
                stuff.cfgId = (int)msg.BatchStuff.CfgId;//EventTask表的 主id

                for (int i = 0; i < randomEventData.batchStuffList.Count; i++)
                {
                    if (randomEventData.batchStuffList[i].id == stuff.id)
                    {
                        randomEventData.batchStuffList[i] = stuff;
                        break;
                    }
                }
                
                //切换成已领取任务界面状态
                // UIManager.Instance.ShowUIPanel("HuntingTaskMain", msg.EventGuid, msg.NpcTask.TaskId);
                UIManager.Instance.ShowUIPanel("HuntingTaskMain", (int)msg.NpcTask.TaskId, true);
                
                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = AcceptNPCTask_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
