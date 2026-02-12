using System.Collections.Generic;
using msg;

namespace Engine
{
    using EngineBase;
    
    public class ShuffleNpcTasks_SCRecv : IReceiver
    {
        public ShuffleNpcTasks_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_ShuffleNpcTasks_SC;
        }

        public void Process()
        {
            //刷新npc反馈
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                RandomEventData randomEventData = new RandomEventData();
                randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(msg.EventGuid);

                randomEventData.guid = msg.RandomEvent.Guid;
                randomEventData.eventId = (int)msg.RandomEvent.EventId;
                randomEventData.eventType = (int)msg.RandomEvent.EventType;
                randomEventData.eventStatus = (int)msg.RandomEvent.EventStatus;
                randomEventData.startTime = msg.RandomEvent.StartTime;
                randomEventData.endTime = msg.RandomEvent.EndTime;
                
                List<BatchStuff> batchStuffList = new List<BatchStuff>();
                foreach (var item in msg.RandomEvent.BatchStuffList)
                {
                    BatchStuff stuff = new BatchStuff();
                    stuff.id = (int)item.Id;
                    stuff.amount = (int)item.Amount;
                    stuff.eventStatus = (int)item.EventStatus;
                    stuff.cfgId = (int)item.CfgId;
                    batchStuffList.Add(stuff);
                }
                randomEventData.batchStuffList = batchStuffList;
                
                PlayerAttrUtils.UpdateFinance(msg.Finance);

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
            msg = ShuffleNpcTasks_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}