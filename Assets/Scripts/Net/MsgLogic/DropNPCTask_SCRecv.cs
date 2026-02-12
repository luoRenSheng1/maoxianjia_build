using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class DropNPCTask_SCRecv : IReceiver
    {
        public DropNPCTask_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_DropNPCTask_SC;
        }

        public void Process()
        {   // NPC委托任务  放弃任务
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                //现在只有一条委托任务，放弃后就删除
                // NpcTaskData taskData = new NpcTaskData();
                // taskData.taskId = (int)msg.NpcTask.TaskId;
                // taskData.progress = (int)msg.NpcTask.Process;
                // taskData.startTime = (ulong)msg.NpcTask.StartTime;
                // taskData.endTime = (ulong)msg.NpcTask.EndTime;
                
                TaskInfoManager.Instance.SetNpcTask(null);
                
                //事件
                RandomEventData randomEventData = new RandomEventData();
                randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);
                if (randomEventData != null)
                {
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

                    if (MapChapterManager.Instance.isShowTaskSelectUI)
                    {
                        UIManager.Instance.ShowUIPanel("ChapterTaskStageDetail", randomEventData);
                    }
                    
                }
                
                //通知刷新
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
                
                // 放弃任务后，切换回任务领取界面
                UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                UIManager.Instance.CloseUIPanel("SubmitGoldAndDiaMain");
                // UIManager.Instance.ShowUIPanel("ChapterTaskStageDetail", randomEventData);

            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = DropNPCTask_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
