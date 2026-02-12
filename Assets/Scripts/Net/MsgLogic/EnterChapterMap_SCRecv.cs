using System.Collections.Generic;
using System.Linq;
using Config;
using msg;
using UnityEngine;

namespace Engine
{
    using EngineBase;
    
    public class EnterChapterMap_SCRecv : IReceiver
    {
        public EnterChapterMap_SC msg;

        public int MsgID()
        {
            return (int)eMsgID.eMsg_EnterChapterMap_SC;
        }

        public void Process()
        {   // 进入新章节地图
            if (msg.Result == eErrCode.eErrCode_Success)
            {
                // 要开始的关卡  --- 为0--表示本章节无可挂机关卡，非0 表示可挂机的本章最高关卡，同步玩家属性里的 latest_passed_stage
                // DataManager.Instance.mRoleData.latestPassedStageId = (int) msg.StartStageId;
                // DataManager.Instance.mRoleData.stageId = (int) msg.StartStageId;
                // DataManager.Instance.mRoleData.chapterId = (int) msg.StartChapterId;

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
                // 重置 怪物词条
                RoleManager.Instance.SetMonsterEntry(true);

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
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_NEXT_CHAPTER_UPDATE);
            }
            else
            {
                UIManager.Instance.ToastByKey(5000+ (int) msg.Result);
            }
        }

        public bool Read(BaseStructRecv mRecv)
        {
            msg = EnterChapterMap_SC.ParseFrom(mRecv.obj);
            return true;
        }
    }
}
