using Config;
using EngineBase;
using msg;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

namespace Engine
{
    /// <summary>
    /// 玩家在地图的状态
    /// </summary>
    public enum MapStatusType
    {
        /// <summary>
        /// 在关卡中
        /// </summary>
        InStage,
        /// <summary>
        /// 在地图上
        /// </summary>
        OnMap,
    }
    
    /// <summary>
    /// 章节关卡信息
    /// </summary>
    public class ChapterMapInfoData
    {
        /// <summary>
        /// 当前的章节id
        /// </summary>
        public int chapterId;
        /// <summary>
        /// 通过的关卡id
        /// </summary>
        public List<int> stageIdslist = new List<int>();
        /// <summary>
        /// //获得徽章的关卡id
        /// </summary>
        public List<int> badgeStageIdsList = new List<int>();
    }
    
    /// <summary>
    /// 随机事件内部生成的数据
    /// </summary>
    public class BatchStuff
    {
        public int id;   //内部维护id  1 ,2 ,3
        public int cfgId;     //配置id  针对不同事件 会是不同表内的配置id，详见时序说明
        public int amount;    //分配数量 或 当前进度
        public int endTime;  //结束时间  为0表示和随机事件结束时间相同
        public int extraCfgId;  //额外配置id 1 一般为0，备用，有启用会在时序中说明
        public int eventStatus;    //状态
        public BatchStuff()
        {

        }
        public BatchStuff(msg.BatchStuff data)
        {
            id = (int)data.Id;
            cfgId = (int)data.CfgId;
            amount = (int)data.Amount;
            endTime = (int)data.EndTime;
            extraCfgId = (int)data.ExtraCfgId1;
            eventStatus = (int)data.EventStatus;
        }
    }
    
    /// <summary>
    /// 随机任务
    /// </summary>
    public class RandomEventData
    {
        public ulong guid;   // guid 事件唯一id
        public int eventId;     //事件id，event表里的id
        public int eventType;    //事件类型，event表里的类型  宝箱  遗迹  任务等
        public int eventStatus;  //任务状态
        public ulong startTime;  //开始时间
        public ulong endTime;    //结束时间
        public List<BatchStuff> batchStuffList = new List<BatchStuff>();    //随机事件内部道具或事件数据
        public int typeGroup;  //event表的 TypeGroup 内容，用于标识固定点还是随机分布
        public List<int> extraIdList = new List<int>();     //参数 任务ID 委托任务id可能有多个 --宝箱id，遗迹id，任务id，bossid，宠物id，货币itemid
        public List<int> extraNumList = new List<int>();    //参数ID对应的数量 如获得货币的数量
    }

    /// <summary>
    /// 随机事件完成数量
    /// </summary>
    public class FinishEventCount
    {
        public int eventType;    //事件类型，event表里的类型  宝箱  遗迹  任务等
        public int finishCount;     //完成数量
    }
    
    public class BigMapObjectData
    {
        public int id;
        public ulong guid;
        public BigMapObject bigMapObject;
        public int monsterData;
    }

    /// <summary>
    /// 遗迹事件怪物或boss
    /// </summary>
    public class RandomEventMonster
    {
        public ulong guid;//事件guid
        public int eventStageId;//EventStage表中的id
        public int indexId;//怪物索引id，只会是 0,1,2
    }

    /// <summary>
    /// 遗迹buff三选一
    /// </summary>
    public class RuinBuffInfo
    {
        public ulong guid;//事件guid
        public uint monsterIndex;//战胜的monster  0,1,2
        public List<int> buffIds;//buffId列表
        public bool isGet = false;//是否领取,默认没有领取
    }

    public class MapChapterManager : TSingleton<MapChapterManager>
    {
        /// <summary>
        /// 角色在地图的状态
        /// </summary>
        public MapStatusType mapStatus;

        //章节地图信息
        public ChapterMapInfoData chapterMapInfo = new ChapterMapInfoData();

        //随机事件列表
        public List<RandomEventData> randomEventList = new List<RandomEventData>();

        //随机事件完成数量列表
        public List<FinishEventCount> finishEventCountList = new List<FinishEventCount>();

        /// <summary>
        /// 地图上的boss、宠物
        /// </summary>
        public Dictionary<string, BigMapObjectData> mapObjDict = new Dictionary<string, BigMapObjectData>();
        
        public bool isShowTaskSelectUI = true;//是否展示狩猎任务选择面板

        public int theLastChallengeStageId = 0;//上一次挑战的关卡Id,仅用于关卡界面胜利是否展示时用到

        public bool autoSkipFindCurStage = false;//自动跳转到下一章节寻找第一个未通过的普通关卡

        public int showMapOrCamp = 0;//展示大地图还是营地：0为大地图，1为营地
        
        public bool isLoadComplete = false;//大地图是否加载完成-目前用于战斗界面下一关挑战按钮
        
        public void OnInit()
        {
            //EventDispatcher.GameWorld.Regist(EventDefine.EVENT_NEXT_CHAPTER_UPDATE, this.OnUpdateChapterMap);
        }

        public override void Dispose()
        {
            base.Dispose();
            randomEventList.Clear();
            finishEventCountList.Clear();
            //EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_NEXT_CHAPTER_UPDATE, this.OnUpdateChapterMap);
        }

        public void Tick(float deltaSeconds)
        {
            if (mapObjDict.Count <= 0) return;

            foreach (var item in mapObjDict)
            {
                item.Value.bigMapObject.Tick(deltaSeconds);
            }
        }

        public void AddMapObject(string key, BigMapObjectData objectData)
        {
            if (objectData == null)
            {
                return;
            }

            mapObjDict.Add(key, objectData);
        }

        public void DestroyMapObject(BigMapObject obj)
        {
            if (obj == null)
            {
                return;
            }

            var keysToRemove = mapObjDict.Where(kv => kv.Value.bigMapObject == obj).Select(kv => kv.Key).ToList();
            foreach (var key in keysToRemove)
            {
                mapObjDict.Remove(key);
            }
        }

        public void PauseMapObject()
        {
            foreach (var obj in mapObjDict)
            {
                obj.Value.bigMapObject.Pause();
            }
        }
        public void HideMapObject()
        {
            foreach (var obj in mapObjDict)
            {
                obj.Value.bigMapObject.Hide();
            }
        }

        /// <summary>
        /// 事件过期，删除改事件的所有怪物
        /// </summary>
        public void DeleteMapObjectByGuid(ulong guid)
        {
            foreach (var obj in mapObjDict)
            {
                if (obj.Value.guid == guid)
                {
                    obj.Value.bigMapObject.Destroy();
                }
            }
        }

        public Dictionary<string, BigMapObjectData> GetMapObjects()
        {
            return mapObjDict;
        }

        /// <summary>
        /// 切换地图
        /// </summary>
        public void OnUpdateChapterMap()
        {
            /*
            UIManager.Instance.DestroyUIPanel("ChapterMap");
            UIManager.Instance.DestroyUIPanel("ChapterEventBossStageDetail");
            UIManager.Instance.DestroyUIPanel("ChapterEventStageDetail");
            UIManager.Instance.DestroyUIPanel("ChapterStageDetail");
            UIManager.Instance.DestroyUIPanel("ChapterTaskStageDetail");

            // UIManager.Instance.AddPackageIfNot("BigMap", () => { });
            GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
            {
                //loading后
                // UIManager.Instance.ShowLoadingUI(() =>
                // {
                // 显示主界面  
                UIManager.Instance.ShowUIPanel("ChapterMap", true);
                // });
            });
*/
        }

        public void SetChapterMapInfo(ChapterMapInfoData chapterMapInfo)
        {
            if (chapterMapInfo.chapterId == 0)
            {
                Debug.Log("=========");
            }

            this.chapterMapInfo = chapterMapInfo;
        }

        public ChapterMapInfoData GetChapterMapInfoData()
        {
            return chapterMapInfo;
        }

        public void SetRandomEventList(List<RandomEventData> randomEventData)
        {
            randomEventList.Clear();
            randomEventList.AddRange(randomEventData);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_UPDATE);
        }

        public void UpdateRandomEventList(RandomEventData randomEventData, bool isAdd = true)
        {
            bool isEixt = false;
            foreach (var taskData in randomEventList)
            {
                if (taskData.eventType == randomEventData.eventType && taskData.guid == randomEventData.guid)
                {
                    taskData.guid = randomEventData.guid;
                    taskData.eventId = randomEventData.eventId;
                    taskData.eventType = randomEventData.eventType;
                    taskData.eventStatus = randomEventData.eventStatus;
                    taskData.startTime = randomEventData.startTime;
                    taskData.endTime = randomEventData.endTime;
                    taskData.batchStuffList = randomEventData.batchStuffList;
                    taskData.typeGroup = randomEventData.typeGroup;
                    taskData.extraIdList = randomEventData.extraIdList;
                    taskData.extraNumList = randomEventData.extraNumList;
                    isEixt = true;
                }
            }

            if (!isEixt && isAdd)
            {
                randomEventList.Add(randomEventData);
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_UPDATE);
        }

        public void UpdateBatchStuffList(ulong guid, List<BatchStuff> batchStuffList)
        {
            bool isEixt = false;
            foreach (var taskData in randomEventList)
            {
                if (taskData.guid == guid)
                {
                    taskData.batchStuffList = batchStuffList;
                }
            }
        }

        public void DeleteRandomEvent(ulong guid)
        {
            int index = -1;
            for (int i = 0; i < randomEventList.Count; i++)
            {
                if (randomEventList[i].guid == guid)
                {
                    index = i;
                }
            }

            if (index >= 0)
            {
                randomEventList.RemoveAt(index);
            }
        }

        /// <summary>
        /// 根据guid获取事件类型
        /// </summary>
        /// <param name="eventGuid"></param>
        /// <returns></returns>
        public int GetEventTypeByGuid(ulong eventGuid)
        {
            foreach (var tesk in randomEventList)
            {
                if (tesk.guid == eventGuid)
                {
                    return tesk.eventType;
                }
            }

            return -1;
        }

        /// <summary>
        /// 标记随机事件子项完成
        /// </summary>
        /// <param name="eventGuid"></param>
        /// <param name="batchStuffId"></param>
        public void MarkingRandomEventCompleted(ulong eventGuid, int batchStuffId)
        {
            foreach (var task in randomEventList)
            {
                if (task.guid == eventGuid)
                {
                    foreach (var item in task.batchStuffList)
                    {
                        if (item.id == batchStuffId)
                        {
                            item.eventStatus = (int)eRandomEventStatus.eRandomEventStatus_Finished;
                            //记录完成数量
                            AddFinishEventCount(task.eventType);
                            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_DATA_UPDATE);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        public List<RandomEventData> GetRandomEventList()
        {
            return randomEventList;
        }

        /// <summary>
        /// 根据事件的guid获取到该事件
        /// </summary>
        /// <returns></returns>
        public RandomEventData GetRandomEventDataByGuid(ulong guid)
        {
            foreach (var item in randomEventList)
            {
                if (item.guid == guid)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据事件类型获取对应的事件列表
        /// </summary>
        /// <param name="type">事件类型</param>
        /// <returns></returns>
        public List<RandomEventData> GetRandomEventListByType(int type)
        {
            List<RandomEventData> randomEventDatas = new List<RandomEventData>();
            foreach (var item in randomEventList)
            {
                if (item.eventType == type)
                {
                    randomEventDatas.Add(item);
                }
            }

            return randomEventDatas;
        }

        /// <summary>
        /// 获取地图固定点位事件数据
        /// </summary>
        public List<RandomEventData> GetMapStageEventList()
        {
            List<RandomEventData> stageEventDatas = new List<RandomEventData>();
            foreach (var item in randomEventList)
            {
                ConfigEventUnit eventUnit = ConfigUtils.GetEventDataById(item.eventId);
                if (eventUnit != null && eventUnit.Rate > 0)
                {
                    stageEventDatas.Add(item);
                }
            }

            return stageEventDatas;
        }

        #region 随机事件完成次数

        public void SetFinishEventCountList(List<FinishEventCount> finishEventData)
        {
            finishEventCountList.Clear();
            finishEventCountList.AddRange(finishEventData);
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_UPDATE);
        }

        public void UpdateFinishEventCountList(FinishEventCount finishEventData)
        {
            foreach (var taskData in finishEventCountList)
            {
                if (taskData.eventType == finishEventData.eventType)
                {
                    //taskData.eventType = finishEventData.eventType;
                    taskData.finishCount = finishEventData.finishCount;
                    break;
                }
            }
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_UPDATE);
        }

        // 目前只用于钓鱼，后续可扩展
        public void UpdateFinishEventCountListInfo(FinishEventCount finishEventData)
        {
            bool isEixt = false;
            foreach (var taskData in finishEventCountList)
            {
                if (taskData.eventType == (int)StageEventType.Fishing)//finishEventData.eventType
                {
                    //taskData.eventType = finishEventData.eventType;
                    taskData.finishCount = finishEventData.finishCount;
                    isEixt = true;
                    break;
                }
            }

            if (!isEixt)
            {
                finishEventCountList.Add(finishEventData);
            }
            
        }

        /// <summary>
        /// 完成的事件次数累加
        /// </summary>
        /// <param name="eventType"></param>
        public void AddFinishEventCount(int eventType)
        {
            foreach (var taskData in finishEventCountList)
            {
                if (taskData.eventType == eventType)
                {
                    ++taskData.finishCount;
                    return;
                }
            }
            finishEventCountList.Add(new FinishEventCount() { eventType = eventType, finishCount = 1 });
            // EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_RANDOM_EVENTS_UPDATE);
        }

        public List<FinishEventCount> GetFinishEventCountList()
        {
            return finishEventCountList;
        }

        /// <summary>
        /// 获取事件完成的次数
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public int GetFinishEventCount(int eventType)
        {
            foreach (var item in finishEventCountList)
            {
                if (item.eventType == eventType)
                {
                    return item.finishCount;
                }
            }

            return 0;
        }

        #endregion

        public void SendEnterChapterMap(int chapterId)
        {
            var builder = EnterChapterMap_CS.CreateBuilder();
            builder.StartChapterId = (ulong)(chapterId);
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EnterChapterMap_CS, builder.Build());
        }

        public void SendEnterLastChapterMap(int chapterId)
        {
            var builder = EnterChapterMap_CS.CreateBuilder();
            builder.StartChapterId = (ulong)(chapterId); //(DataManager.Instance.mRoleData.chapterId - 1000);
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_EnterChapterMap_CS, builder.Build());
        }

        public void EnterRandomEventCopyBattle()
        {
            DungeonMapManager.Instance.GuanKaStage = 1;

            // ConfigDungeonChapterUnit dungeonChapterUnit = ConfigUtils.GetDungeonChapterById(DungeonType.BossWithPet);

            if (DungeonMapManager.Instance.mapEventData != null)
            {
                int cfgId = 0;

                if (DungeonMapManager.Instance.mapEventData.eventType == (int)eRandomEventType.eRandomEventType_RandomBox)
                {
                    foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                    {
                        if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                        {
                            cfgId = stuffData.extraCfgId;
                            break;
                        }
                    }
                }
                else if (DungeonMapManager.Instance.mapEventData.eventType == (int)StageEventType.Relic)
                {
                    cfgId = DungeonMapManager.Instance.eventMonsterData.eventStageId;
                }
                else
                {
                    foreach (var stuffData in DungeonMapManager.Instance.mapEventData.batchStuffList)
                    {
                        if (stuffData.id == DungeonMapManager.Instance.batchStuffId)
                        {
                            cfgId = stuffData.cfgId;
                            break;
                        }
                    }
                }
            
                ConfigEventStageUnit stageUnit = ConfigUtils.GetEventStageUnitById(cfgId);
                UIManager.Instance.ShowUIPanel("DungeonMap", stageUnit);
            }
        }

        #region 遗迹

        public List<RandomEventMonster> _randomEventMonsterList = new List<RandomEventMonster>();
        public RuinBuffInfo _ruinBuffInfo { get; set; } //遗迹buff三选一
        public int _canFightMonsterIndex = 0; //可以挑战的boss的index
        public int _defeatMonsterIndex = -1; //击败的boss的index
        public bool _canExitRuinMap { get; set; } = false; //所有奖励领取完毕自动退出遗迹地图标识

        public bool exitFlag { get; set; } = false;//用于标签栏展示效果

        public RandomEventData _ruinEventData { get; set; } = null;
        
        public void AddRuinBossInfo(List<RandomEventMonster> randomEventMonsterList)
        {
            _randomEventMonsterList.Clear();
            _randomEventMonsterList.AddRange(randomEventMonsterList);
        }

        public RandomEventMonster GetRandomEventMonsterByIndexId(int indexId)
        {
            foreach (var randomEventMonster in _randomEventMonsterList)
            {
                if (randomEventMonster.indexId == indexId)
                {
                    return randomEventMonster;
                }
            }

            return null;
        }

        public void AddRuinBuffInfo(RuinBuffInfo ruinBuffInfo)
        {
            _ruinBuffInfo = null; //清空数据
            _ruinBuffInfo = ruinBuffInfo;
        }

        public void ResetRuinState()
        {
            _ruinBuffInfo = null;
            _randomEventMonsterList.Clear();
            _canFightMonsterIndex = 0;
            _defeatMonsterIndex = -1;
            _canExitRuinMap = false;
            exitFlag = false;
            _ruinEventData = null;
        }

        #endregion

        #region 钓鱼

        public bool _fishingSuccess = false;//本次钓鱼是否成功，默认不成功
        public int _fishingCount = 0;//今日最新钓鱼次数
        
        /// <summary>
        /// 开始挥杆申请
        /// </summary>
        public void SendBeginFishing()
        {
            var builder = BeginFishing_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_BeginFishing_CS, builder.Build());
        }

        /// <summary>
        /// 钓鱼boss开始攻击请求
        /// </summary>
        public void SendAttackFishingBossBegin()
        {
            var builder = AttackFishingBossBegin_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_AttackFishingBossBegin_CS, builder.Build());
        }

        /// <summary>
        /// 钓鱼结果是直接奖励领取请求
        /// </summary>
        public void SendFishingClaimAeard()
        {
            var builder = FishingClaimAward_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FishingClaimAward_CS, builder.Build());
        }

        /// <summary>
        /// 钓鱼次数
        /// </summary>
        /// <param name="fishingCount"></param>
        public void UpdateFishingCount(int fishingCount)
        {
            _fishingCount = fishingCount;
        }

        public void SendFishingFailed()
        {
            var builder = FishingFailed_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FishingFailed_CS, builder.Build());
        }

        #endregion


        #region 大地图BGM

        public void PlayBigMapBGM(int chapterId)
        {
            switch (chapterId)
            {
                case 1:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM1);
                    break;
                case 2:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM2);
                    break;
                case 3:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM3);
                    break;
                case 4:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM4);
                    break;
                case 5:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM5);
                    break;
                case 6:
                    GameManager.Instance.SoundManager.PlayMusic((int)SoundType.ChapterBGM6);
                    break;
            }
        }

        #endregion
        
    }
}