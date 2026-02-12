using System.Collections.Generic;
using System.Linq;
using Config;
using EngineBase;
using msg;

namespace Engine
{
    public enum CardType
    {
        Permanent = 1001,
        Monthly = 1002,
        Weekly= 1003,
    }

    /// <summary>
    /// 埋点礼包ID
    /// </summary>
    public enum Gift_Bury
    {
        Gift_zzc = 5001,
        Gift_yxl = 5002,
        Gift_yxsj = 5003,
        Gift_yxtp = 5004,
        Gift_fb_jktz = 5005,
        Gift_fb_yxtz = 5006,
        Gift_fb_zbtz = 5007,
        Gift_fb_fstz = 5008,
        Gift_goldAdd = 5009,    
    }

    public class SevenDayInfo
    {
        public ConfigDailyCheckRewardUnit sevenUnit;
        /// <summary>
        /// //是否领奖   1--未领奖，2--已领奖
        /// </summary>
        public int Tag;
    }

    public class FirstPayDayOneRwInfo
    {
        public int itemId;
        public int num;
    }

    public class FirstPayInfo
    {
        // 第几天
        public int WhichDay;
        
        /// <summary>
        /// //是否领奖   0--未领奖，1--已领奖
        /// </summary>
        public int Tag;

        public ulong GetRwTime;
    }

    public class PassPortSubTask
    {
        public int TaskId;
        public ConfigPassTaskUnit PassSubTaskUnit;
        public ulong Progress;
        public bool HasGetReward;
    }
    public class PassPortInfo
    {
        public List<ConfigPassGiftUnit> PassGiftUnits;
        public int UnlockTier;
        public ulong StartTime;
        public ulong EndTime;
        public List<PassPortSubTask> DailySubTasks = new List<PassPortSubTask>();
        public List<PassPortSubTask> WeekSubTasks = new List<PassPortSubTask>();
        public int PassPortLv;
        public long PassPortExp;
        public List<int> NormalRewardGetLvs = new List<int>();
        public List<int> SuperRewardGetLvs = new List<int>();
        public int CounterAfterTopLevel;
        public int ClaimedCounterAfterTopLevel;
    }

    public class ActivityManager : TSingleton<ActivityManager>
    {
        private List<SevenDayInfo> _serverInfos;

        private List<FirstPayInfo> _firstPayInfos = new List<FirstPayInfo>();

        private Dictionary<int, int> continuousSaveDict = new Dictionary<int, int>();

        public bool HasSevenDay { get; set; }

        private ConfigCommonUnit _common701TXZ;
        private List<ConfigGuideRewardUnit> _guideRewardUnits;

        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ACTIVITY_UPDATE, this.OnUpdateActivity);

            _common701TXZ = ConfigDataGroup.GetInstance<ConfigCommon>().Get(701);
            DailyAwardInfos();
            _guideRewardUnits = ConfigDataGroup.GetInstance<ConfigGuideReward>().Data.Values.ToList();
        }

        public override void Dispose()
        {
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ACTIVITY_UPDATE, this.OnUpdateActivity);
            base.Dispose();
        }

        private void OnUpdateActivity()
        {

        }

        public List<ConfigGuideRewardUnit> GuideRewardUnits()
        {
            return _guideRewardUnits;
        }

    public List<SevenDayInfo> GetSevenDayInfos()
        {
            if (_serverInfos == null)
            {
                _serverInfos = new List<SevenDayInfo>();
                for (int i = 0; i < 7; i++)
                {
                    SevenDayInfo sevenDayInfo = new SevenDayInfo();
                    sevenDayInfo.sevenUnit = ConfigUtils.GetSevenDayUnit(i + 1);
                    sevenDayInfo.Tag = 0;
                    _serverInfos.Add(sevenDayInfo);
                }
            }

            return _serverInfos;
        }

        public void UpdateSevenDay(uint itemId, uint itemClaimStatus, ulong itemSigninTime)
        {
            _serverInfos = GetSevenDayInfos();
            foreach (var item in _serverInfos)
            {
                if (item.sevenUnit.DailyCheckId == itemId)
                    item.Tag = (int) itemClaimStatus;
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SEVENDAY_UPDATE);
        }

        public bool HasSevenDayReddot()
        {
            _serverInfos = GetSevenDayInfos();
            foreach (var item in _serverInfos)
            {
                if (item.Tag == 1)
                {
                    return true;
                }
            }

            return false;
        }
        public bool HasGetAllSevenDayReward()
        {
            _serverInfos = GetSevenDayInfos();
            foreach (var item in _serverInfos)
            {
                if (item.Tag != 2)
                {
                    return false;
                }
            }

            return true;
        }
        public bool HasSignAllSevenDay()
        {
            _serverInfos = GetSevenDayInfos();
            foreach (var item in _serverInfos)
            {
                if (item.Tag == 0)
                {
                    return false;
                }
            }

            return true;
        }

        #region 英雄特权卡

        public bool hasPurchaseMonth1004;
        public bool GetTodayReward1004;
        public double Month1004_EndTime;

        #endregion

        #region 首充
        
        private List<FirstPayDayOneRwInfo> _firstPayDayOneRwInfos;//首充奖励列表

        public ConfigCommonUnit GetConfigCommonUnitById(int id)
        {
            ConfigCommonUnit commonUnit = ConfigDataGroup.GetInstance<ConfigCommon>().Get(id);
            if (commonUnit == null)
            {
                LogUtils.LogErrorFormat("在Common配置表中找不到,id={0}",id);
            }
            return commonUnit;
        }

        // 获取首充第一天奖励信息
        public List<FirstPayDayOneRwInfo> GetFirstPayDayOneRws()
        {
            string[] rewardGroups = GetConfigCommonUnitById(800007).Param1.Split('|');
            if (rewardGroups == null)
            {
                LogUtils.LogErrorFormat("Common配置表Param1参数有误");
            }
        
            if (_firstPayDayOneRwInfos == null)
            {
                // 初始化列表
                _firstPayDayOneRwInfos = new List<FirstPayDayOneRwInfo>();
                
                foreach (var rewardGroup in rewardGroups)
                {
                    string[] rewardItemList = rewardGroup.Split(',');
        
                    if (rewardItemList.Length == 2)
                    {
                        int itemId = int.Parse(rewardItemList[0]);
                        int itemNum = int.Parse(rewardItemList[1]);
        
                        // 创建 FirstPayDayOneRwInfo 对象并添加到列表中
                        _firstPayDayOneRwInfos.Add(new FirstPayDayOneRwInfo
                        {
                            itemId = itemId,
                            num = itemNum
                        });
                    }
                }
            }
            return _firstPayDayOneRwInfos;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <param name="status">是否领奖   0--未领奖，1--已领奖</param>
        public void UpdateFirstPay(int day, int status, ulong getRwTime)
        {
            bool has = false;
            for (int i = 0; i < _firstPayInfos.Count; i++)
            {
                if (_firstPayInfos[i].WhichDay == day)
                {
                    _firstPayInfos[i].WhichDay = day;
                    _firstPayInfos[i].Tag = status;
                    _firstPayInfos[i].GetRwTime = getRwTime;
                    has = true;
                    break;
                }
            }
            
            if (!has)
            {
                _firstPayInfos.Add(new FirstPayInfo()
                {
                    WhichDay = day,
                    Tag = status,
                    GetRwTime = getRwTime
                });
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_FIRST_PAY_UPDATE);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <returns>第一个参数代表天数，第二个参数代表可领取的状态</returns>
        public (int, bool) CanGetFirstPayInfo()
        {
            foreach (var item in _firstPayInfos)
            {
                if (item.GetRwTime <= ServerTimeManager.Instance.CurServerTime && item.Tag == 0)
                    return (item.WhichDay, true);
            }

            return (-1, false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <returns>第一个参数代表天数，第二个参数代表可领取的状态</returns>
        public bool HasAllGetFirstPayReward()
        {
            foreach (var item in _firstPayInfos)
            {
                if (item.Tag == 0)
                    return true;
            }

            return false;
        }

        public List<FirstPayInfo> GetFirstPayInfos()
        {
            return _firstPayInfos;
        }

        #endregion

        public bool SummonSpecialRedDot()
        {
            var summonHeroMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonHero);
            if (summonHeroMap.Item1)
            {
                bool hasRed = SpecialRedDot();
               if (!hasRed)
                   hasRed = DataManager.Instance.mRoleData.HeroFreeLottery == 0;
               return hasRed;
            }

            return false;
        }

        public bool SpecialRedDot()
        {
            int totalSecond =  (int) (ActivityManager.Instance.Month1004_EndTime - ServerTimeManager.Instance.CurServerTime);
            if (totalSecond > 0)
            {
                return (ActivityManager.Instance.hasPurchaseMonth1004 && !ActivityManager.Instance.GetTodayReward1004);
            }

            return false;
        }

        #region 特权卡

        private Dictionary<CardType, bool> _hasPurchaseCardDict = new Dictionary<CardType, bool>();
        private Dictionary<CardType, bool> _getTodayRewardDict = new Dictionary<CardType, bool>();
        private Dictionary<CardType, double> _cardEndTimeDict = new Dictionary<CardType, double>();

        public void SetCardPurchase(CardType type, bool hasBuy)
        {
            if (_hasPurchaseCardDict.ContainsKey(type))
            {
                _hasPurchaseCardDict[type] = hasBuy;
            }
            else
            {
                _hasPurchaseCardDict.Add(type, hasBuy);
            }
        }

        public bool GetCardPurchase(CardType type)
        {
            if(_hasPurchaseCardDict.TryGetValue(type, out bool hasPurchase))
            {
                return hasPurchase;
            }

            return false;
        }
        
        public void SetCardTodayGet(CardType type, bool hasGet)
        {
            if (_getTodayRewardDict.ContainsKey(type))
            {
                _getTodayRewardDict[type] = hasGet;
            }
            else
            {
                _getTodayRewardDict.Add(type, hasGet);
            }
        }

        public bool GetCardTodayGet(CardType type)
        {
            if(_getTodayRewardDict.TryGetValue(type, out bool hasGet))
            {
                return hasGet;
            }

            return false;
        }
        
        public void SetCardPurchaseEndTime(CardType type, double endTime)
        {
            if (_cardEndTimeDict.ContainsKey(type))
            {
                _cardEndTimeDict[type] = endTime;
            }
            else
            {
                _cardEndTimeDict.Add(type, endTime);
            }
        }

        public double GetCardPurchaseEndTime(CardType type)
        {
            if(_cardEndTimeDict.TryGetValue(type, out double endtime))
            {
                return endtime;
            }

            return 0;
        }

        #endregion

        #region 通行证

        private PassPortInfo _passPortInfo;

        public void SetPassPortInfo(PassPortInfo portInfo)
        {
            this._passPortInfo = portInfo;
        }

        public PassPortInfo GetPassPortInfo()
        {
            return this._passPortInfo;
        }

        public int GetPassPortBoxMaxCnt()
        {
            return int.Parse(_common701TXZ.Param2);
        }

        public int GetPassportMaxLv()
        {
            return int.Parse(_common701TXZ.Param3);
        }
        
        public int GetPassportBoxMaxExp()
        {
            return int.Parse(_common701TXZ.Param1);
        }
        
        public void GetPassTaskInfoCS()
        {
            var builder = PassTaskInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_PassTaskInfo_CS, builder.Build());
        }
        
        public bool IsGetPassportReward(int lv, bool isSuper)
        {
            if (!isSuper)
            {
                return _passPortInfo.NormalRewardGetLvs.Contains(lv);
            }

            return _passPortInfo.SuperRewardGetLvs.Contains(lv);
        }

        public void UpdatePassSubTask(int taskId, long progress, bool hasGet)
        {
            foreach (var item in _passPortInfo.DailySubTasks)
            {
                if (item.TaskId == taskId)
                {
                    item.Progress = (ulong)progress;
                    item.HasGetReward = hasGet;
                    break;
                }
            }
            
            foreach (var item in _passPortInfo.WeekSubTasks)
            {
                if (item.TaskId == taskId)
                {
                    item.Progress = (ulong)progress;
                    item.HasGetReward = hasGet;
                    break;
                }
            }
        }

        public PassPortSubTask GetPassSubTask(int taskId, int cd)
        {
            if (cd == 1)
            {
                foreach (var item in _passPortInfo.DailySubTasks)
                {
                    if (item.TaskId == taskId)
                    {
                        return item;
                    }
                }
            }
            else if (cd == 2)
            {
                foreach (var item in _passPortInfo.WeekSubTasks)
                {
                    if (item.TaskId == taskId)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public bool HasPassportRewardGet()
        {
            return HasAllNormalPassRewardReddot() || HasAllSuperPassRewardReddot();
        }

        private bool HasAllNormalPassRewardReddot()
        {
            PassPortInfo passPortInfo = GetPassPortInfo();
            if (passPortInfo == null) return false;
            bool isNormalGet = false;
            foreach (var item in passPortInfo.PassGiftUnits)
            {
                isNormalGet = _passPortInfo.PassPortLv >= item.Level;
                if (isNormalGet)
                {
                    isNormalGet = !IsGetPassportReward(item.Level, false);
                }
                if(isNormalGet) break;
            }
            return isNormalGet;
        }

        private bool HasAllSuperPassRewardReddot()
        {
            PassPortInfo passPortInfo = GetPassPortInfo();
            if (passPortInfo == null) return false;
            if (passPortInfo.UnlockTier == 0) return false;
            bool isSuperGet = false;
            foreach (var item in passPortInfo.PassGiftUnits)
            {
                isSuperGet = _passPortInfo.PassPortLv >= item.Level;
                if (isSuperGet)
                {
                    isSuperGet = !IsGetPassportReward(item.Level, true);
                }
                if(isSuperGet) break;
            }
            return isSuperGet;
        }

        public bool HasPassportTaskRewardGet()
        {
            PassPortInfo passPortInfo = GetPassPortInfo();
            if (passPortInfo == null) return false;
            foreach (var item in passPortInfo.DailySubTasks)
            {
                if (item.Progress == ulong.Parse(item.PassSubTaskUnit.Param) && !item.HasGetReward && (item.PassSubTaskUnit.PayType==0|| (item.PassSubTaskUnit.PayType == 1&& passPortInfo.UnlockTier == 1)))
                {
                    return true;
                }
            }

            foreach (var item in passPortInfo.WeekSubTasks)
            {
                if (item.Progress == ulong.Parse(item.PassSubTaskUnit.Param) && !item.HasGetReward && (item.PassSubTaskUnit.PayType==0|| (item.PassSubTaskUnit.PayType == 1&& passPortInfo.UnlockTier == 1)))
                {
                    return true;
                }
            }
            
            return false;
        }

        #endregion

        #region 天天好礼

        // 获取天天好礼信息请求
        public void SendToGetDailyAwardInfoCS()
        {
            var builder = DailyAwardOfRechargeInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DailyAwardOfRechargeInfo_CS, builder.Build());
        }

        public void DailyAwardInfos()
        {
            List<ConfigContinuousSaveUnit> continuousSaveUnit = ConfigDataGroup.GetInstance<ConfigContinuousSave>().Data.Values.ToList();
            foreach (var item in continuousSaveUnit)
            {
                continuousSaveDict.Add(item.Day, 0);
            }
        }

        /// <summary>
        /// 天天好礼信息反馈
        /// </summary>
        /// <param name="day">哪一天</param>
        /// <param name="getTag">领取标识：0-未达成，1-以达成，未领取，2-已领取</param>
        public void UpdateDailyAwardInfo(int day, int getTag)
        {
            foreach (var key in continuousSaveDict.Keys.ToList())
            {
                if (key == day)
                {
                    continuousSaveDict[key] = getTag;
                }
            }

        }
        
        public Dictionary<int,int> GetContinuousSaveDict()
        {
            return continuousSaveDict;
        }

        private int wDay;
        public int GetWaitingAssignDays(int day)
        {
            wDay = day;
            return day;
        }

        public int GetDay()
        {
            return wDay;
        }

        #endregion

        #region 疯狂指南

        // 获取疯狂指南信息请求
        public void SendToGetGuideAwardInfoCS()
        {
            var builder = StageGuideAwardInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_StageGuideAwardInfo_CS, builder.Build());
        }

        List<uint> buyList = new List<uint>();
        List<uint> getList = new List<uint>();
        // 获取已购买到账关卡引导礼包对应的章节id
        public void UpdateGuideBuyIdInfo(List<uint> buyIdList)
        {
            buyList = buyIdList;
        }
        public List<uint> BuyIdsInfo()
        {
            return buyList;
        }

        // 获取已领取的关卡引导礼包的关卡Id
        public void UpdateGuideGetIdInfo(List<uint> getIdList)
        {
            getList = getIdList;
        }
        public List<uint> GetIdsInfo()
        {
            return getList;
        }

        // 领取疯狂指南奖励反馈
        public void ClaimStageGuideAward(uint stageId)
        {
            if(!getList.Contains(stageId))
                getList.Add(stageId);
        }
        
        public bool UpdateFkznReddot(int chapterId)
        {
            List<uint> chapterBuyIdList = ActivityManager.Instance.BuyIdsInfo();
            List<uint> chapterGetIdList = ActivityManager.Instance.GetIdsInfo();
            List<ConfigGuideRewardUnit> fkznChapterUnits = _guideRewardUnits.Where(unit => unit.ChapterId == chapterId).ToList();
            bool hasReward = false;
            foreach (var guideRewardUnit in fkznChapterUnits)
            {
                if (DataManager.Instance.GetRoleData().latestPassedStageId > guideRewardUnit.Levelid)
                {
                    // 有无购买高级疯狂指南，解锁条件为配置表GuideReward中的type
                    // 解锁类型为自动解锁：1
                    if (guideRewardUnit.Type == 1)
                    {
                        // 已领取
                        if (!chapterGetIdList.Contains((uint)guideRewardUnit.Levelid))
                        {
                            hasReward = true;
                            break;
                        }
                    }
                    // 解锁类型为购买高级指南解锁：2
                    else
                    {
                        // 已购买高级指南
                        if (chapterBuyIdList.Contains((uint)guideRewardUnit.ChapterId))
                        {
                            // 已领取
                            if (!chapterGetIdList.Contains((uint)guideRewardUnit.Levelid))
                            {
                                hasReward = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            return hasReward;
        }

        #endregion

        #region 登录礼包

        public int LoginGiftPackID;

        #endregion

        #region 商店免费

        public int Shop_FreeDailyPack;//每日礼包免费
        public int Shop_FreeTTHLPack;//天天好礼免费
        public int Shop_FreeTQKPack;//特权卡免费
        public int IsBuyPermanent;//是否已购买永久卡--为0表示未购买，1表示已购买
        public int HasGetPermanentTodayReward;
        public int GetPermanentDays;

        public bool GetDailyReddot()
        {
            bool dailyReddot = false;
            var _dailyGifts = ConfigUtils.GetGiftUnitsByType(GiftType.Daily);
            foreach (var item in _dailyGifts)
            {
                LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(item.Id);
                if (limitPackVo.BuyCounter == 0 || limitPackVo.EndTime < ServerTimeManager.Instance.CurServerTime)
                {
                    dailyReddot = false;
                }
                else
                {
                    bool todayGet = limitPackVo.GetAwardCounter > 0 &&
                                    limitPackVo.LastGetAwardTime < ServerTimeManager.Instance.GetNextToZeroServerTime();
                    dailyReddot = !todayGet;
                }
                if(dailyReddot) break;
            }

            return dailyReddot;
        }
        #endregion
    }
}