using System;
using System.Collections.Generic;
using msg;
using UnityEngine;

namespace Engine
{
    public class PlayerChapterMapInfo4Login_PC_Recv : IReceiver
    {
        public PlayerChapterMapInfo4Login_PC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_PlayerChapterMapInfo4Login_PC;
        }

        public void Process()
        {   //包括登录后所处的章节地图关卡信息 和 随机任务信息
            ChapterMapInfoData info = new ChapterMapInfoData();
            info.chapterId = (int)msg.ChapterStage.ChapterId;
            for (int j = 0; j < msg.ChapterStage.StageIdsList.Count; j++)
            {
                info.stageIdslist.Add((int)msg.ChapterStage.StageIdsList[j]);
            }
            for (int j = 0; j < msg.ChapterStage.BadgeStageIdsList.Count; j++)
            {
                info.badgeStageIdsList.Add((int)msg.ChapterStage.BadgeStageIdsList[j]);
            }
            
            MapChapterManager.Instance.SetChapterMapInfo(info);
            
            //随机任务信息
            List<RandomEventData> randomEventDataList = new List<RandomEventData>();
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
                randomEventDataList.Add(eventData);
            }
            MapChapterManager.Instance.SetRandomEventList(randomEventDataList);
            
            //随机任务完成次数
            List<FinishEventCount> finishEventCountList = new List<FinishEventCount>();
            for (int i = 0; i < msg.FinshedEventInfoList.Count; i++)
            {
                FinishEventCount data = new FinishEventCount();
                data.eventType = (int)msg.FinshedEventInfoList[i].EventType;
                data.finishCount = (int)msg.FinshedEventInfoList[i].FinishCounter;
                finishEventCountList.Add(data);
            }
            MapChapterManager.Instance.SetFinishEventCountList(finishEventCountList);
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = PlayerChapterMapInfo4Login_PC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
