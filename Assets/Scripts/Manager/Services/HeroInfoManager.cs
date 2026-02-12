using Config;
using EngineBase;
using msg;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Engine
{
    public enum HeroState
    {
        /// <summary>
        /// 待机
        /// </summary>
        [Description("idle")]
        idle,
        /// <summary>
        /// 行走
        /// </summary>
        [Description("run")]
        run,
        /// <summary>
        /// 攻击
        /// </summary>
        [Description("attack")]
        attack,
        /// <summary>
        /// 技能
        /// </summary>
        [Description("skill")]
        skill,
        /// <summary>
        /// 挖宝
        /// </summary>
        [Description("wabao")]
        wabao,
        /// <summary>
        /// 挖矿
        /// </summary>
        [Description("wakuang")]
        wakuang,
        /// <summary>
        /// 爱心
        /// </summary>
        [Description("aixing")]
        aixing,
        /// <summary>
        /// 钓鱼1
        /// </summary>
        [Description("diaoyv1")]
        diaoyv1,
        /// <summary>
        /// 钓鱼2
        /// </summary>
        [Description("diaoyv2")]
        diaoyv2,
        /// <summary>
        /// 钓鱼3
        /// </summary>
        [Description("diaoyv3")]
        diaoyv3,
        /// <summary>
        /// 钓鱼4
        /// </summary>
        [Description("diaoyv4")]
        diaoyv4,
        /// <summary>
        /// 抓宠
        /// </summary>
        [Description("zhuachong")]
        zhuachong,
        /// <summary>
        /// 落地
        /// </summary>
        [Description("luodi")]
        luodi,
        /// <summary>
        /// 推箱子
        /// </summary>
        [Description("tui")]
        tui,
    }

    public class HeroInfo
    {
        public int Level; //等级
        public ulong Exp; //经验
        public int BreakLevel; //突破等级
        public int BreakLevelLayer; //突破阶级
        public ConfigHeroUnit HeroUnit;
        public bool IsGet;
        public int SkillLevel;//主动技能等级

        public FightAttrVo FightAttrVo;
        public double SkillDamageRate;
    }

    public class HeroInfoManager : TSingleton<HeroInfoManager>
    {
        private bool _isOperating = true;

        public bool IsOperating
        {
            private get => _isOperating;
            set
            {
                if (_isOperating == value)
                    return;
                _isOperating = value;
                if (!_isOperating)
                    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE);
            }
        }

        private List<HeroInfo> _heroInfos = new List<HeroInfo>();

        private HeroInfo _myHero;
        private int FirstHeroID = 20001;

        private ConfigCommonUnit _common20007;
        private ConfigCommonUnit _common20008;
        private ConfigCommonUnit _common100003;

        private List<ConfigHeroUnit> _heroUnits;

        public void OnInit()
        {
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_SKILL_LIST, this.UpdateBattleSkillList);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_POWER_CHANGE, this.OnPowerChange);
            EventDispatcher.GameWorld.Regist(EventDefine.STR_SOUND_SETTING_CHANGE, this.SettingSoundChange);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_GO_TO_Lobby, this.OnLoginSuccess);
            _common20007 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200007);
            _common20008 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200008);
            _common100003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100003);
            FirstHeroID = int.Parse(_common100003.Param1);
            _heroUnits = ConfigDataGroup.GetInstance<ConfigHero>().Data.Values.ToList();
        }

        public override void Dispose()
        {
            _heroInfos.Clear();
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_SKILL_LIST, this.UpdateBattleSkillList);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_POWER_CHANGE, this.OnPowerChange);
            EventDispatcher.GameWorld.UnRegist(EventDefine.STR_SOUND_SETTING_CHANGE, this.SettingSoundChange);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_GO_TO_Lobby, this.OnLoginSuccess);
            base.Dispose();
        }

        public void UpdateBattleSkillList()
        {
            List<SkillInfo> skillInfos = SkillInfoManager.Instance.GetBattleSkillList();
            for (int i = 0; i < skillInfos.Count; i++)
            {
                RoleManager.Instance.SetSkillIdByIndex(skillInfos[i].SkillId, skillInfos[i].BattleIndex);
            }
            RoleManager.Instance.UpdateSkillIdList(skillInfos);
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_Skill_LIST_Lobby);
        }

        private void SettingSoundChange()
        {
            EngineBase.PlayerPrefs.SetFloat("music", GameManager.Instance.SoundManager.musicVolume);
            EngineBase.PlayerPrefs.SetFloat("sound", GameManager.Instance.SoundManager.soundVolume);
        }

        private void OnPowerChange()
        {
            if (IsOperating) return;
            double fight = FightUtils.GetTotalFight();
            double deltaFight = fight - RoleManager.Instance.TotalFight;
            RoleManager.Instance.TotalFight = FightUtils.GetTotalFight();

            if (deltaFight >= RoleManager.Instance.TotalFight * (int.Parse(_common20007.Param1) * ConstDefine.CONFIG_PLACE_EX))
            {
                bool isSuper = deltaFight > RoleManager.Instance.TotalFight *
                    (int.Parse(_common20008.Param1) * ConstDefine.CONFIG_PLACE_EX);
                UIManager.Instance.ShowUIPanel("PowerUp", deltaFight, RoleManager.Instance.TotalFight, isSuper);
                GameManager.Instance.TimerManager.ClearTimer(HidePowerUpView);
                GameManager.Instance.TimerManager.SetTimer(1.5f, this.HidePowerUpView);
            }
            else
            {
                if (FightUtils.IsWearEquip && deltaFight > 0)
                {
                    UIManager.Instance.ShowUIPanel("PowerUp", deltaFight, RoleManager.Instance.TotalFight, false);
                    GameManager.Instance.TimerManager.ClearTimer(HidePowerUpView);
                    GameManager.Instance.TimerManager.SetTimer(1.5f, this.HidePowerUpView);
                }
            }

            if ((PvpRankDataManager.Instance.YestdayMyRank > 0 || PvpRankDataManager.Instance.IsMyInRank() != null))
            {
                var builder = BattlePowerChange_CS.CreateBuilder();
                builder.BattlePower = RoleManager.Instance.TotalFight;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_BattlePowerChange_CS, builder.Build());
            }

            FightUtils.IsWearEquip = false;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_POWER_CHANGE_Update_lobby);
        }

        private void HidePowerUpView()
        {
            UIManager.Instance.CloseUIPanel("PowerUp");
        }

        public void UpdateMyHeroInfo(int heroId)
        {
            _myHero = GetThisHero(heroId);
            DataManager.Instance.GetRoleData().FightAttrVo.HarmType = _myHero.HeroUnit.Vocation;
            DataManager.Instance.GetRoleData().FightAttrVo.HarmAttrAtkAdd = _myHero.HeroUnit.VocationAttr;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_UPDATE_BATTLE_HERO);
        }

        public HeroInfo GetMyHero()
        {
            if (_myHero == null)
            {
                HeroInfo curHero = GetThisHero(DataManager.Instance.GetRoleData().heroId);
                if (curHero != null)
                {
                    _myHero = curHero;
                }
            }

            return _myHero;
        }

        public int GetHeroFirstID()
        {
            return FirstHeroID;
        }

        private void OnLoginSuccess()
        {
            TreasureChesManager.Instance.StartTreasureCd();
            //发送红点
            ReddotSysManager.Instance.SendRedPointCS();
            //发送充值信息
            ShopInfoManager.Instance.SendRechargeInfoCS();
            //限制礼包购买信息请求
            ShopInfoManager.Instance.SendLimitGiftPackInfoCS();
            //每日任务
            TaskInfoManager.Instance.SendToGetDailyTaskCS();
            //头像
            RoleManager.Instance.SendToGetAvatarListCS();

            GameManager.Instance.TimerManager.SetTimer(0.5f, () =>
            {
                var builder = GetHometown_CS.CreateBuilder();
                GetHometown_CS hometownCs = builder.Build();
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Hometown_CS, hometownCs);
            });

            var builder2 = GetHeroMonthActivityInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetHeroMonthActivityInfo_CS, builder2.Build());
            
            ActivityManager.Instance.SendToGetGuideAwardInfoCS();
            ActivityManager.Instance.SendToGetDailyAwardInfoCS();
            
            MailManager.Instance.SendMailListCS();

            var builder = GetTimeCardInfo_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetTimeCardInfo_CS, builder.Build());
        }

        public void UpdateHeroInfos(HeroInfo heroInfo)
        {
            bool isHas = false;
            foreach (var item in _heroInfos)
            {
                if (item.HeroUnit.Id == heroInfo.HeroUnit.Id)
                {
                    item.Exp = heroInfo.Exp;
                    item.Level = heroInfo.Level;
                    item.BreakLevel = heroInfo.BreakLevel;
                    item.BreakLevelLayer = heroInfo.BreakLevelLayer;
                    item.SkillDamageRate = heroInfo.SkillDamageRate;
                    item.IsGet = true;
                    isHas = true;
                    item.SkillLevel = heroInfo.SkillLevel;
                    break;
                }
            }

            if (!isHas)
            {
                heroInfo.IsGet = true;
                _heroInfos.Add(heroInfo);
            }
        }

        public HeroInfo GetThisHero(int heroId)
        {
            foreach (var item in _heroInfos)
            {
                if (item.HeroUnit.Id == heroId)
                    return item;
            }

            return null;
        }

        public List<HeroInfo> GetHeroInfos()
        {
            return _heroInfos;
        }

        public ConfigHeroAttrUnit HasUnlockHeroAttr(int heroId, int preLevel, int curLevel)
        {
            foreach (var item in ConfigDataGroup.GetInstance<ConfigHeroAttr>().Data)
            {
                if (item.Value.HeroId == heroId && preLevel < item.Value.Level && curLevel >= item.Value.Level &&
                    item.Value.Type == 1)
                {
                    return item.Value;
                }
            }

            return null;

        }

        public List<ConfigHeroUnit> GetAllHeroUnits()
        {
            return _heroUnits;
        }

        public bool IsShowRedDot()
        {
            return IsCanHeCheHero();
        }

        /// <summary>
        /// 有英雄可以合成
        /// </summary>
        /// <returns></returns>
        private bool IsCanHeCheHero()
        {
            foreach (var item in _heroUnits)
            {
                int needItemCnt = ItemInfoManager.Instance.GetItemCount(item.Item);
                if (needItemCnt >= item.Conflate && HeroInfoManager.Instance.GetThisHero(item.Id) == null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取所有英雄中等级最高的英雄等级
        /// </summary>
        /// <returns></returns>
        public int GetMaxHeroLv()
        {
            int maxLevel = 0;
            foreach (var hero in _heroInfos)
            {
                if (hero.Level > maxLevel)
                {
                    maxLevel = hero.Level; // 更新最高等级
                }
            }
            return maxLevel;
        }
        
        #region 主动技能

        private Dictionary<int,int> _skillDict = new Dictionary<int,int>(){};
        private int _skillDamageRate;
        
        /// <summary>
        /// 英雄主动技能升级
        /// </summary>
        /// <param name="heroId">英雄Id</param>
        /// <param name="skillLevel">主动技能等级</param>
        public void UpdataHeroSkillLevelUp(int heroId, int skillLevel)
        {
            _skillDict[heroId] = skillLevel;
        }

        public Dictionary<int, int> GetHeroSkillLevelUp()
        {
            return _skillDict;
        }

        public void UpdateHeroSkillDamageRate(int skillDamageRate)
        {
            _skillDamageRate = skillDamageRate;
        }

        public int GetHeroSkillDamageRate()
        {
            return _skillDamageRate;
        }

        #endregion
        
        #region 被动技能
        private List<int> passiveSkill = new List<int>(){};
        private int maxIndex = -1;
        ConfigHeroUnit heroUnit = null;
        public List<int> GetHeroUnlockPassiveSkill(int skillId)
        {
            maxIndex = -1;
            passiveSkill.Clear();
            
            ConfigCommonUnit _common100004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(100004);
            string[] lvs = _common100004.Param1.Split(',');
            for (int i = 0; i < lvs.Length; i++)
            {
                if (GetMyHero().Level >= int.Parse(lvs[i]))
                {
                    maxIndex = i;
                }
            }
            
            if (maxIndex >= 0)
            {
                heroUnit = ConfigUtils.GetHeroBySkillId(skillId);
                string[] skillBuffs = heroUnit.Passive.Split("|");
                for (int i = 0; i < skillBuffs.Length; i++)
                {
                    if (i >= maxIndex)
                    {
                        passiveSkill.Add(int.Parse(skillBuffs[i]));
                    }
                }
            }
            
            return passiveSkill;
        }
        #endregion


    }
}