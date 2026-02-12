using System.Collections.Generic;
using Config;
using EngineBase;
using msg;
using UnityEngine;

namespace Engine
{
    public class LimitPackVo
    {
        public int PackId;
        public int BuyCounter;
        public ulong EndTime;
        public int GetAwardCounter;
        public ulong LastGetAwardTime;
    }
    public class ShopInfoManager : TSingleton<ShopInfoManager>
    {
        private List<LimitPackVo> _dailyPackFlagList = new List<LimitPackVo>();
        private List<LimitPackVo> _weekPackFlagList = new List<LimitPackVo>();
        private List<LimitPackVo> _doublePackFlagList = new List<LimitPackVo>();
        private List<LimitPackVo> _monthlyPackFlagList = new List<LimitPackVo>();

        private ConfigCommonUnit _commonUnit700005;

        public void OnInit()
        {
            _commonUnit700005 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(700005);
        }
        public override void Dispose()
        {
            _dailyPackFlagList.Clear();
            _weekPackFlagList.Clear();
            base.Dispose();
        }

        public void SetDailyPackFlag(int packId, int buyCounter, ulong endTime, int getAwardCounter, ulong lastGetAwardTime)
        {
            bool isHas = false;
            for (int i = 0; i < _dailyPackFlagList.Count; i++)
            {
                if (_dailyPackFlagList[i].PackId == packId)
                {
                    _dailyPackFlagList[i].BuyCounter = buyCounter;
                    _dailyPackFlagList[i].EndTime = endTime;
                    _dailyPackFlagList[i].GetAwardCounter = getAwardCounter;
                    _dailyPackFlagList[i].LastGetAwardTime = lastGetAwardTime;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
            {
                _dailyPackFlagList.Add(new LimitPackVo()
                {
                    PackId = packId,
                    BuyCounter = buyCounter,
                    EndTime = endTime,
                    GetAwardCounter = getAwardCounter,
                    LastGetAwardTime = lastGetAwardTime
                });
            }

        }

        public void SetWeekPackFlag(int packId, int buyCounter, ulong endTime, int getAwardCounter, ulong lastGetAwardTime)
        {
            bool isHas = false;
            for (int i = 0; i < _weekPackFlagList.Count; i++)
            {
                if (_weekPackFlagList[i].PackId == packId)
                {
                    _weekPackFlagList[i].BuyCounter = buyCounter;
                    _weekPackFlagList[i].EndTime = endTime;
                    _weekPackFlagList[i].GetAwardCounter = getAwardCounter;
                    _weekPackFlagList[i].LastGetAwardTime = lastGetAwardTime;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
                _weekPackFlagList.Add(new LimitPackVo()
                {
                    PackId = packId,
                    BuyCounter = buyCounter,
                    EndTime = endTime,
                    GetAwardCounter = getAwardCounter,
                    LastGetAwardTime = lastGetAwardTime
                });

        }

        public void SetDoublePackFlag(int packId, int buyCounter, ulong endTime, int getAwardCounter, ulong lastGetAwardTime)
        {
            bool isHas = false;
            for (int i = 0; i < _doublePackFlagList.Count; i++)
            {
                if (_doublePackFlagList[i].PackId == packId)
                {
                    _doublePackFlagList[i].BuyCounter = buyCounter;
                    _doublePackFlagList[i].EndTime = endTime;
                    _doublePackFlagList[i].GetAwardCounter = getAwardCounter;
                    _doublePackFlagList[i].LastGetAwardTime = lastGetAwardTime;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
                _doublePackFlagList.Add(new LimitPackVo()
                {
                    PackId = packId,
                    BuyCounter = buyCounter,
                    EndTime = endTime,
                    GetAwardCounter = getAwardCounter,
                    LastGetAwardTime = lastGetAwardTime
                });

        }

        public void SetMonthlyPackFlag(int packId, int buyCounter, ulong endTime, int getAwardCounter, ulong lastGetAwardTime)
        {
            bool isHas = false;
            for (int i = 0; i < _monthlyPackFlagList.Count; i++)
            {
                if (_monthlyPackFlagList[i].PackId == packId)
                {
                    _monthlyPackFlagList[i].BuyCounter = buyCounter;
                    _monthlyPackFlagList[i].EndTime = endTime;
                    _monthlyPackFlagList[i].GetAwardCounter = getAwardCounter;
                    _monthlyPackFlagList[i].LastGetAwardTime = lastGetAwardTime;
                    isHas = true;
                    break;
                }
            }

            if (!isHas)
                _monthlyPackFlagList.Add(new LimitPackVo()
                {
                    PackId = packId,
                    BuyCounter = buyCounter,
                    EndTime = endTime,
                    GetAwardCounter = getAwardCounter,
                    LastGetAwardTime = lastGetAwardTime
                });

        }

        public LimitPackVo GetPackNum(int giftId)
        {
            ConfigGiftUnit giftUnit = ConfigUtils.GetGiftUnitsById(giftId);
            List<LimitPackVo> tempList = new List<LimitPackVo>();
            // 0=不限制
            // 1=每日重置
            // 2=7天重置
            // 3=14天重置
            // 4=30日重置
            if (giftUnit.Buy == 0)
            {
                return new LimitPackVo();
            }
            else if (giftUnit.Buy == 1)
            {
                tempList = _dailyPackFlagList;
            }
            else if (giftUnit.Buy == 2)
            {
                tempList = _weekPackFlagList;
            }
            else if (giftUnit.Buy == 3)
            {
                tempList = _doublePackFlagList;
            }
            else if (giftUnit.Buy == 4)
            {
                tempList = _monthlyPackFlagList;
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                if (_dailyPackFlagList[i].PackId == giftId)
                {
                    return _dailyPackFlagList[i];
                    break;
                }
            }

            return new LimitPackVo();
        }

        public void SendRechargeInfoCS()
        {
            //发送获取充值信息请求
            var builder = RechargeInfo_CS.CreateBuilder();
            RechargeInfo_CS infoCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_RechargeInfo_CS, infoCs);
        }

        public void SendLimitGiftPackInfoCS()
        {
            var builder = LimitGiftPackInfo_CS.CreateBuilder();
            LimitGiftPackInfo_CS infoCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_LimitGiftPackInfo_CS, infoCs);
        }

        public int GetSkillLotteryOne()
        {
            return int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700008).Param1);
        }

        public int GetPetLotteryOne()
        {
            return int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700007).Param1);
        }

        public ConfigCommonUnit GetHeroLotteryCfg()
        {
            return _commonUnit700005;
        }

        public void Clear()
        {
            _dailyPackFlagList.Clear();
            _weekPackFlagList.Clear();
        }

        /// <summary>
        /// （广告次数, 抽奖次数）
        /// </summary>
        /// <returns></returns>
        public (int, int) GetSkillLotterFreeAdTime()
        {
            return (int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700004).Param1),
                int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700010).Param1));
        }

        /// <summary>
        /// （广告次数, 抽奖次数）
        /// </summary>
        /// <returns></returns>
        public (int, int) GetPetLotterFreeAdTime()
        {
            return (int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700003).Param1),
                int.Parse(ConfigDataGroup.GetInstance<ConfigCommon>().Get(700009).Param1));
        }

        public int GetQualityGet(int lv, int type)
        {
            ConfigRaffleLevelUnit raffleUnit = ConfigUtils.GetRaffleUnit(lv, type);
            if (raffleUnit.Quality7Pro != "0")
            {
                return 6;
            }

            if (raffleUnit.Quality6Pro != "0")
            {
                return 5;
            }

            return 4;
        }
        
        //

        #region 召唤红点

        public bool ChkSummonPetLimit(int num)
        {
            var petList = PetInfoManager.Instance.GetAllHavePetList();
            if (petList.Count + num > int.Parse(ItemInfoManager.Instance.common300008.Param1))
            {
                return true;
            }
            return false;
        }
        
        public bool SummonPetRedPoint()
        {
            var petMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPet);
            if (petMap.Item1)
            {
                bool petLimit = !ChkSummonPetLimit(30);
                bool petFlag = GetPetLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia;//砖石召唤宠物30次
                bool petAdFlag = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_PetLottoTimesByAd) > 0;//宠物看广告
                return (petFlag || petAdFlag) && petLimit;
            }
            
            return false;
        }

        public bool SummonSkillRedPoint()
        {
            var skillMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
            if (skillMap.Item1)
            {
                bool skillFlag = GetSkillLotteryOne() * 30 <= DataManager.Instance.GetRoleData().dia;//砖石召唤技能30次
                bool skillAdFlag = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_SkillLottoTimesByAd) > 0;//技能看广告
                return skillFlag || skillAdFlag;
            }
            
            return false;
        }

        // 召唤宠物和技能
        public bool SummonPetAndSkillRedPoint()
        {
            bool petFlag = SummonPetRedPoint();
            
            bool skillFlag = SummonSkillRedPoint();
            
            return petFlag || skillFlag;
        }

        #endregion
        
    }
}