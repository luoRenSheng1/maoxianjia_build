using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public class DailyTaskInfo
    {
        public int Id;
        public int Progress;
        public int CanGetRw;
        public ConfigDailyTaskUnit DailyTaskUnit;
    }

    /// <summary>
    /// npc委托任务
    /// </summary>
    public class NpcTask
    {
        public List<NpcTaskData> TaskDatas = new List<NpcTaskData>();    //npc委托任务
        public List<PointExchangeInfo> ExchangeInfos = new List<PointExchangeInfo>();    //兑换信息
    }
    
    /// <summary>
    /// 积分兑换信息
    /// </summary>
    public class PointExchangeInfo
    {
        public int itemId;  //兑换的道具
        public int changeCounter;  //今日兑换次数
    }
    
    /// <summary>
    /// npc委托任务数据
    /// </summary>
    public class NpcTaskData
    {
        public int taskId;  //任务id，配置表中的主id
        public int progress;  //任务进度
        public ulong startTime;  //开始时间
        public ulong endTime;    //结束时间
        public ConfigEventTaskUnit taskUnit;
    }

    public enum TaskType
    {
        eMainTaskType_PassStage        = 1,        //  挑战关卡
        eMainTaskType_KillMonsters        = 2,        // 击杀怪物
        eMainTaskType_EquipMake        = 3,    // 打造装备
        eMainTaskType_PetSummon        = 5,    // 召唤宠物
        eMainTaskType_SellEquips        = 7,        // 出售装备
        eMainTaskType_EquipBoxLevelUp        = 8,        // 箱子升级
        eMainTaskType_GetQualityEquip        = 25,    // 获得一个X品质装备
        eMainTaskType_SkillSummon        = 26,    // 召唤技能
        eMainTaskType_HeroSummon        = 27,    // 召唤英雄
        eMainTaskType_AttackLvUp        = 28,    // 强化攻击
        eMainTaskType_LifeLvUp        = 29,    // 强化生命
        eMainTaskType_CriticalHitLvUp        = 30,    // 强化暴击
        eMainTaskType_BlastInjuryLvUp        = 31,    // 强化暴伤
        eMainTaskType_DailyTaskNum        = 32,    // 日常任务数
        eMainTaskType_WatchAD        = 33,    // 看广告
        eMainTaskType_OnlineTime        = 34,    // 在线时长
        eMainTaskType_HomeGain        = 35,    // 家园奖励收取
        eMainTaskType_login        = 36,    // 登录
        eMainTaskType_ArenaChallenge        = 37,    // 竞技场挑战
        eMainTaskType_GetOnlineRw        = 38,    // 领取在线奖励
        eMainTaskType_GoldCopy        = 40,    // 指定金币副本
        eMainTaskType_EquipCopy        = 41,    // 指定装备副本
        eMainTaskType_DiamondCopy        = 42,    // 指定钻石副本
        eMainTaskType_HeroExperienceCopy        = 43,    // 指定英雄经验副本
        eMainTaskType_StoneCopy        = 44,    // 指定符石副本
        eMainTaskType_AnyCopy        = 45,    // 任意副本
        eMainTaskType_RecoverLvUp        = 46,    // 强化恢复
    }

    public class TaskVo
    {
        public int MainTaskId;
        public int MainTaskProgress;
    }
    
    
    public class TaskInfoManager : TSingleton<TaskInfoManager>
    {
        private TaskVo _taskVo;

        #region 每日任务  活跃值
        private List<DailyTaskInfo> _dailyTaskList = new List<DailyTaskInfo>();
        private long _activeBox;
        private List<int> _activeBoxGetRwId = new List<int>();
        #endregion

        
        #region 委托任务 只有一个就是随机任务里面已经接受的任务
        private NpcTaskData npcTask = null;
        private List<PointExchangeInfo> _exchangeInfoList = new List<PointExchangeInfo>();//兑换信息列表
        #endregion
        
        public void OnInit()
        {

        }

        public override void Dispose()
        {
            _dailyTaskList.Clear();
            npcTask = null;
            base.Dispose();
        }
        
        private float _recoveryTicker = 0;
        private float _totalRecoverTimer = 1;
        public void Tick(float deltaSeconds)
        {
            _recoveryTicker += deltaSeconds;
            if (_recoveryTicker > _totalRecoverTimer)
            {
                _recoveryTicker -= 0;
                if (npcTask != null && npcTask.taskUnit != null)
                {
                    var lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    if (npcTask.endTime <= ServerTimeManager.Instance.CurServerTime && lobbyView != null)
                    {   //刚登录的情况等进战斗界面再弹
                        var loading = UIManager.Instance.FindByName("Loading") as LoadingView;
                        if ( loading == null || (loading != null && !loading.IsShow()))
                        {
                            // npcTask = null;
                            if (npcTask.taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || npcTask.taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || npcTask.taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets)
                            {
                                if (npcTask.progress < int.Parse(npcTask.taskUnit.Param.Split(',')[0]))
                                {
                                    npcTask = null;
                                    UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                                }
                            }
                            else
                            {
                                if (npcTask.progress < int.Parse(npcTask.taskUnit.Param))
                                {
                                    npcTask = null;
                                    UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                                }
                            }
                            // UIManager.Instance.CloseUIPanel("HuntingTaskMain");
                            // UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(8005));
                            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_DELEGATE_TASK_UPDATE);
                        }
                    }
                }
            }

        }

        public void SendToGetDailyTaskCS()
        {
            var builder = DailyTaskInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DailyTaskInfo_CS, builder.Build());
        }

        public List<DailyTaskInfo> GetDailyTaskInfos()
        {
            return _dailyTaskList;
        }

        public bool IsCanGetDailyReward()
        {
            foreach (var item in _dailyTaskList)
            {
                if (item.CanGetRw == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        /// <param name="canGetRw"> 0=能领取 1=不能领取 2=已领取</param>
        public void UpdateDailyTaskInfo(int id, bool canGetRw, int progress = -1)
        {
            bool isHasDaily = false;
            foreach (var item in _dailyTaskList)
            {
                if (item.Id == id)
                {
                    if (progress != -1)
                    {
                        item.CanGetRw = canGetRw ? 2 : (int.Parse(item.DailyTaskUnit.Param) > progress) ? 1 : 0;
                        item.Progress = progress;
                    }
                    else
                    {
                        item.CanGetRw = canGetRw ? 2 : (int.Parse(item.DailyTaskUnit.Param) > item.Progress) ? 0 : 1;
                    }

                    isHasDaily = true;
                    break;
                }
            }

            if (!isHasDaily)
            {
                ConfigDailyTaskUnit dailyTaskUnit = ConfigUtils.GetDailyTaskUnit(id);
                if(dailyTaskUnit != null)
                {
                    DailyTaskInfo dailyTaskInfo = new DailyTaskInfo();
                    dailyTaskInfo.Id = id;
                    dailyTaskInfo.CanGetRw =  canGetRw ? 2 : (int.Parse(dailyTaskUnit.Param) > progress) ? 1 : 0;
                    dailyTaskInfo.DailyTaskUnit = dailyTaskUnit;
                    dailyTaskInfo.Progress = progress;
                    _dailyTaskList.Add(dailyTaskInfo);
                }
            }
        }

        public void SetTotalActiveBox(long activeBox)
        {
            _activeBox = activeBox;
        }
        public long GetTotalActiveBox()
        {
            return _activeBox;
        }

        public void SetActiveBoxGetRwId(int id)
        {
            if(!_activeBoxGetRwId.Contains(id))
                _activeBoxGetRwId.Add(id);
        }

        public bool IsActiveBoxGetRw(int id)
        {
            foreach (var item in _activeBoxGetRwId)
            {
                if (item == id)
                    return true;
            }

            return false;
        }

        public void SkipToCompleteTask()
        {
            /*
                type=1，Param=关卡ID，表示通关X-X关
                type=2，Param=击杀数量
                type=3，Param=X表示开箱子数量
                type=4，Param=X表示英雄等级
                type=5，Param=X表示抽取数量
                type=6，Param=X表示宠物强化次数
                type=7，Param=X，其中X为出售件数
                type=8，Param=X表示箱子等级
             */
            ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());
            FightUtils.SkipToCompleteTask(taskUnit.Type);
        }

        public void SetCurMainTask(TaskVo task)
        {
            this._taskVo = task;
        }

        public void SetCurTaskId(int taskId, int taskProgress)
        {
            if (_taskVo != null && _taskVo.MainTaskId == taskId)
            {
                _taskVo.MainTaskProgress = taskProgress;
            }
        }
        
        public int GetCurTaskId()
        {
            return _taskVo.MainTaskId;
        }
        
        public TaskVo GetCurTask()
        {
            return _taskVo;
        }
        
        public long GetCurTaskNum()
        {
            return _taskVo.MainTaskProgress;
        }

        public void SetNpcTask(NpcTaskData npcTaskData)
        {
            npcTask = npcTaskData;
        }
        
        public NpcTaskData GetNpcTask()
        {
            return npcTask;
        }

        /// <summary>
        /// 兑换积分道具信息
        /// </summary>
        /// <param name="exchangeInfo"></param>
        public void UpdateNpcTaskExchangeInfo(PointExchangeInfo exchangeInfo)
        {
            bool isHas = false;
            foreach (var item in _exchangeInfoList)
            {
                if (item.itemId == exchangeInfo.itemId)
                {
                    item.changeCounter = exchangeInfo.changeCounter;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
            {
                _exchangeInfoList.Add(exchangeInfo);
            }
        }

        /// <summary>
        /// 获取某一兑换积分道具信息
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public PointExchangeInfo GetNpcTaskExchangeInfoByItemId(int itemId)
        {
            foreach (var item in _exchangeInfoList)
            {
                if (item.itemId == itemId)
                {
                    return item;
                }
            }
            return null;
        }

    }
}