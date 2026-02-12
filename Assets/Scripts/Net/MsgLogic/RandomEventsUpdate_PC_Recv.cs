using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class RandomEventsUpdate_PC_Recv : IReceiver
    {
        public RandomEventsUpdate_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_RandomEventsUpdate_PC;
        }

        public void Process()
        {   // 随机事件 部分事件内容发生变更
            if (msg != null)
            {
                //随机任务完成次数
                List<FinishEventCount> finishEventCountList = new List<FinishEventCount>();
                for (int i = 0; i < msg.FinishEventsList.Count; i++)
                {
                    FinishEventCount data = new FinishEventCount();
                    data.eventType = (int)msg.FinishEventsList[i].EventType;
                    data.finishCount = (int)msg.FinishEventsList[i].FinishCounter;
                    finishEventCountList.Add(data);
                }
                MapChapterManager.Instance.SetFinishEventCountList(finishEventCountList);

                //有新的事件
                bool newGoldEvent = false;//金币堆事件
                bool newTreasureEvent = false;//开箱子砍树
                bool newDiamondEvent = false;//钻石
                bool bossAndPetEvent = false;//boss 宠物
                bool taskEvent = false;//狩猎任务
                bool ruinEvent = false;//遗迹
                bool adventureBusinessMan = false;//奇遇商人
                //随机任务信息更新
                for (int i = 0; i < msg.EventsList.Count; i++)
                {
                    RandomEventData eventData = new RandomEventData();
                    eventData.guid = msg.EventsList[i].Guid;
                    eventData.eventId = (int)msg.EventsList[i].EventId;
                    eventData.eventType = (int)msg.EventsList[i].EventType;
                    eventData.eventStatus = (int)msg.EventsList[i].EventStatus;
                    eventData.startTime = msg.EventsList[i].StartTime;
                    eventData.endTime = msg.EventsList[i].EndTime;
                    for (int j = 0; j < msg.EventsList[i].BatchStuffList.Count; j++)
                    {
                        BatchStuff data = new BatchStuff();
                        data.id = (int)msg.EventsList[i].BatchStuffList[j].Id;
                        data.cfgId = (int)msg.EventsList[i].BatchStuffList[j].CfgId;
                        data.amount = (int)msg.EventsList[i].BatchStuffList[j].Amount;
                        data.endTime = (int)msg.EventsList[i].BatchStuffList[j].EndTime;
                        data.extraCfgId = (int)msg.EventsList[i].BatchStuffList[j].ExtraCfgId1;
                        data.eventStatus = (int)msg.EventsList[i].BatchStuffList[j].EventStatus;
                        eventData.batchStuffList.Add(data);
                    }
                    eventData.typeGroup = (int)msg.EventsList[i].TypeGroup;
                    for (int j = 0; j < msg.EventsList[i].ExtraIdList.Count; j++)
                    {
                        eventData.extraIdList.Add((int)msg.EventsList[i].ExtraIdList[j]);
                    }
                    for (int j = 0; j < msg.EventsList[i].ExtraNumList.Count; j++)
                    {
                        eventData.extraNumList.Add((int)msg.EventsList[i].ExtraNumList[j]);
                    }
                    MapChapterManager.Instance.UpdateRandomEventList(eventData,false);

                    if (!newGoldEvent && eventData.eventType == (int)eRandomEventType.eRandomEventType_RandomGold)
                    {
                        newGoldEvent = true;
                    }

                    if (!newTreasureEvent && eventData.eventType == (int)eRandomEventType.eRandomEventType_RandomEuip)
                    {
                        newTreasureEvent = true;
                    }
                    if (!newDiamondEvent && eventData.eventType == (int)eRandomEventType.eRandomEventType_RandomDiamond)
                    {
                        newDiamondEvent = true;
                    }
                    if (!bossAndPetEvent && (eventData.eventType == (int)eRandomEventType.eRandomEventType_NextChapterBoss || eventData.eventType == (int)eRandomEventType.eRandomEventType_StageDropPet))
                    {
                        bossAndPetEvent = true;
                    }
                    
                    if (!taskEvent && eventData.eventType == (int)eRandomEventType.eRandomEventType_NpcTask)
                    {
                        taskEvent = true;
                    }
                    if (!ruinEvent && eventData.eventType == (int)eRandomEventType.eRandomEventType_RuinsBuff)
                    {
                        ruinEvent = true;
                    }
                    if (!adventureBusinessMan && eventData.eventType == (int)eRandomEventType.eRandomEventType_AdventureBusinessMan)
                    {
                        adventureBusinessMan = true;
                    }
                }

                //激活监听器
                if (newGoldEvent)
                {//天女散花金币事件
                    Debug.Log("刷新金币事件");
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_GOLD_EVENTS_UPDATE);
                }
				//开箱子砍树
                if (newTreasureEvent)
                {
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_TREASURE_UPDATE);
                }
				
                if (newDiamondEvent)
                {//钻石矿事件
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_UPDATE);
                }
                if (bossAndPetEvent)
                {   //传承BOSS和宠物
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
                }

                if (taskEvent)
                {//狩猎任务
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
                }
                if (ruinEvent)
                {//遗迹
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RUIN_UPDATE);
                }

                if(adventureBusinessMan)
                {//奇遇
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_ADVENTURE_CAVE_SHOP);
                }
            }
            // CityInfo cityInfo = PlayerAttrUtils.GetBuildInfoByServer(msg.Build);
            // VillageInfoManager.Instance.SetBuildInfo(cityInfo.BuildType, cityInfo);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_VILLAGE_ONE_BUILD, cityInfo);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = RandomEventsUpdate_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
