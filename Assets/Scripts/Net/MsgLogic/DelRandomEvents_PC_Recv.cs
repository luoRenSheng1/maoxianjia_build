using System.Collections.Generic;
using System.Linq;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class DelRandomEvents_PC_Recv : IReceiver
    {
        public DelRandomEvents_PC msg;
        public int MsgID()
        {
            return (int) eMsgID.eMsg_DelRandomEvents_PC;
        }

        public void Process()
        {
            ulong eventGuid = 0;
            eventGuid = msg.Guid;

            //将客户端事件设置为完成，不能删除，方便客户端其他数据检测使用
            FinishEventCount d = new FinishEventCount();
            d.eventType = (int)msg.FinishEvents.EventType;
            d.finishCount = (int)msg.FinishEvents.FinishCounter;
            MapChapterManager.Instance.UpdateFinishEventCountList(d);
            MapChapterManager.Instance.DeleteRandomEvent(eventGuid);

            Debug.Log($"删除guid={msg.Guid}, 事件={d.eventType}, 次数={msg.FinishEvents.FinishCounter}");

            //int eType = MapChapterManager.Instance.GetEventTypeByGuid(msg.Guid);
            //if(eType != -1)
            //随机事件只有一个，只有客户端有的数据才进行删除，否则会调用多次

            if (d.eventType == (int)eRandomEventType.eRandomEventType_RandomGold)
            {//天女散花金币事件
                Debug.Log("删除天女散花金币事件");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_GOLD_EVENTS_END);
            }
            else if (d.eventType == (int)eRandomEventType.eRandomEventType_RandomDiamond)
            {//钻石矿事件结束
                Debug.Log("删除钻石矿事件");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_END);
            }
            //砍树开箱子事件
            else if (d.eventType == (int)eRandomEventType.eRandomEventType_RandomEuip)
            {
                Debug.Log("删除砍树开箱子事件");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_TREASURE_END, eventGuid);
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_TREASURE_UPDATE);
            }
            else if (d.eventType == (int)eRandomEventType.eRandomEventType_NextChapterBoss || d.eventType == (int)eRandomEventType.eRandomEventType_StageDropPet)
            {
                MapChapterManager.Instance.DeleteMapObjectByGuid(eventGuid);
            }
            else if (d.eventType == (int)eRandomEventType.eRandomEventType_NpcTask)
            {//狩猎任务事件结束
                Debug.Log("删除狩猎任务事件");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_END);
            }
            else if (d.eventType == (int)eRandomEventType.eRandomEventType_RuinsBuff)
            {//遗迹事件结束
                Debug.Log("删除遗迹事件");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RUIN_END);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            LogUtils.LogWarning("DelRandomEvents_PC_Recv Read " + mRecv.Length);
            msg = DelRandomEvents_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}