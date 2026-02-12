using Common;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using AchievementInfo = Engine.AchievementInfo;
using EventDispatcher = EngineBase.EventDispatcher;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UI_BtnBuff = Lobby.UI_BtnBuff;
using UI_HeroAttrItem = CommonEx.UI_HeroAttrItem;
using UI_Main = Lobby.UI_Main;

public class SkillCDInfo
{
    public GTweener tween;
    public float time;
}

/// <summary>
/// 属性组数据项
/// </summary>
public class AttributeGroupItem
{
    public int GroupId { get; set; }
    public List<ClassicAttr> Attributes { get; set; }
}

public class LobbyView : UIViewBase
{
    private UI_Main lobbyMain => this.main as UI_Main;

    private int mCurGuankaIndex = 0;
    private int mStageMonsterWave = 0;

    private float _activeSkillBaseCD;

    private float orgBattleRootY = 154;//这边写死，获取不到fairygui的设计位置
    private int curSelectIndex = -1;
    private bool isMapOpen = false; // 跟踪地图界面的打开状态
    private Dictionary<int, int> _skillIdList = new Dictionary<int, int>(6);
    private ConfigCommonUnit _commonUnit30;
    private List<FunPrevInfo> _leftFuncPrevInfos = new List<FunPrevInfo>();

    private bool _isGuidingTask;
    private bool _isGuide;
    private bool _isInit = false;

    private List<ShuxingInfo> _shuxingInfos = new List<ShuxingInfo>();
    private ConfigCommonUnit _common200004;
    private ConfigCommonUnit _common200009;

    private List<ItemData> rewards = new List<ItemData>();

    private KeyCode hotkeyKey = KeyCode.F5;    // PM快捷键
    private bool requiresControl = false;    // 是否需要Ctrl键组合
    private bool isPMWindowOpen = false;    // PMWindow是否打开

    // 秘典
    private ConfigClassicsUnit configClassicsUnit;
    private List<ClassicInfo> _classicInfoList = new List<ClassicInfo>();
    private Dictionary<int, ClassicInfo> _classicsDictionary = new Dictionary<int, ClassicInfo>();
    // 新修改秘典
    private Google.Protobuf.Collections.MapField<int, ConfigClassicsUnit> _allClassicsMap;
    private Dictionary<int, ConfigClassicsUnit> _allClassicsDict;
    private Dictionary<int, ConfigClassicsUnit> _maxLevelClassicsDict;

    private Dictionary<int, int> _artifactsDictionary = new Dictionary<int, int>();//神器字典，由服务器下发
    private List<ConfigArtifactEquipUnit> _artifactEquipUnitList = new List<ConfigArtifactEquipUnit>();

    private List<ConfigSystemUnit> _leftFuncUnitList = new List<ConfigSystemUnit>();//左边栏位
    private List<ConfigSystemUnit> _unlockLeftFuncs = new List<ConfigSystemUnit>();//左边栏位-已解锁列表
    private List<ConfigSystemUnit> _rightFuncUnitList = new List<ConfigSystemUnit>();//右边栏位
    private List<ConfigSystemUnit> _unlockRightFuncs = new List<ConfigSystemUnit>();//右边栏位-已解锁列表
    private List<ConfigSystemUnit> _foldFuncUnitList = new List<ConfigSystemUnit>();//折叠栏位
    private List<ConfigSystemUnit> _unlockFoldFuncs = new List<ConfigSystemUnit>();//折叠栏位-已解锁列表

    private UI_BtnFucntionIcon homeBtn = null;//家园的入口-引导使用

    public class AttrData
    {
        public int id;
        public double number;
    }

    private float _timer = 0;
    public LobbyView()
    {
        this.type = UIType.Background;
        this.name = "Lobby";
        this.package = "Lobby";
        this.component = "Main";
        this.removePackage = false;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.lobbyMain.panel.bottomGroup.sortingOrder = 9999;
        // this.lobbyMain.panel.btnHelp.onClick.Set(OnClickHelp);
        this.lobbyMain.panel.battleRoot.comMessage.InitChatInfo();
        this.lobbyMain.panel.btnSpeed.onClick.Set(OnClickSpeed);
        this.lobbyMain.panel.battleRoot.btnToppedAni.onClick.Set(OnClickTopped);
        this.lobbyMain.panel.btnSetupAuto.onClick.Set(OnClickSetupAuto);
        this.lobbyMain.panel.listBottom.onClickItem.Set(OnClickBottomItem);
        this.lobbyMain.panel.comBox.btnBoxIcon.onClick.Set(OnClickBoxIcon);
        // this.lobbyMain.panel.comBox.btnBoxLv.onClick.Set(OnClickBoxLv);
        this.lobbyMain.panel.comBox.btnBoxLvUp.onClick.Set(OnClickBoxLv);
        this.lobbyMain.panel.battleRoot.btnStage.onClick.Set(OnClickNextStage);
        this.lobbyMain.panel.battleRoot.comSkills.btnSkillAuto.onClick.Set(OnClickAutoSkill);
        this.lobbyMain.panel.battleRoot.comSkills.listSkills.onClickItem.Add(OnClickSkillItem);

        this.lobbyMain.panel.battleRoot.comSkills.listSkills.itemRenderer = ItemListSkillRender;
        this.lobbyMain.panel.battleRoot.comSkills.activeSkill.onClick.Add(OnClickHeroSkill);

        this.lobbyMain.panel.tempEquipIcon.onClick.Add(OnClickTempEquipIcon);

        ((UI_MonsterGroupBar)this.lobbyMain.panel.battleRoot.monsterGroup).monsterGroupList.itemRenderer = ItemRendererGroupList;

        this.lobbyMain.panel.btnHelp.onClick.Add(this.OnClickToOpenAllHeroAttr);

        // 绑定PM按钮点击事件
        this.lobbyMain.panel.openPmBtn.onClick.Add(this.OnClickPmBtn);
        // this.lobbyMain.panel.dailyBtn.onClick.Add(this.OnClickDailyBtn);
        this.lobbyMain.panel.tujianBtn.onClick.Add(this.OnClickTujianBtn);
        // this.lobbyMain.panel.villageBtn.onClick.Add(this.OnclickVillageBtn);

        // 主线任务
        // this.lobbyMain.panel.comPandaInfo.rewardList.itemRenderer = MainTaskRewardListItemRender;
        this.lobbyMain.panel.comPandaInfo.onClick.Add(this.OnClickToGetTaskReward);

        // 秘典
        // configClassicsUnitList = ConfigDataGroup.GetInstance<ConfigClassics>().Data.Values.ToList();
        this.lobbyMain.panel.classicList.itemRenderer = ClassicListRender; // 秘典
        _allClassicsMap = ConfigDataGroup.GetInstance<ConfigClassics>().Data;
        _allClassicsDict = _allClassicsMap.ToDictionary(kv => kv.Key, kv => kv.Value);
        // 构建最高等级配置字典
        _maxLevelClassicsDict = _allClassicsDict.Values
            .Where(c => !string.IsNullOrEmpty(c.Attr2) && c.Attr2 != "0")
            .GroupBy(c => c.ClassicsID)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.Lv).First());

        //神器
        this.lobbyMain.panel.listEquip2.itemRenderer = ArtifactListRender;//神器列表
        this.lobbyMain.panel.upLvBtn.onClick.Add(this.OnClickUpLvBtn);// 神器升级

        this.lobbyMain.comBuff.buffList.itemRenderer = BuffListRender;

        //功能栏
        FunctionInfo();

        // 侧边栏
        this.lobbyMain.panel.showOtherBtn.onClick.Add(this.OnClickOtherBtn);
        this.lobbyMain.panel.otherList.itemRenderer = OtherListRender;
        this.lobbyMain.panel.colseOtherBtn.onClick.Add(this.OnClickColseBtnToOther);

        // 左侧功能栏
        this.lobbyMain.panel.leftFunctionList.itemRenderer = LeftFunctionListRender;
        // 右侧功能栏
        this.lobbyMain.panel.rightFunctionList.itemRenderer = RightFunctionListRender;

        this.lobbyMain.panel.firstChargeBtn.onClick.Add(this.OnClickFirstChargeBtn);
        this.lobbyMain.panel.superTXZBtn.onClick.Add(this.OnClickSuperTXZBtn);
        this.lobbyMain.panel.onlineBtn.onClick.Add(this.OnClickOnlineBtn);
        // this.lobbyMain.panel.mailBtn.onClick.Add(this.OnClickMailBtn);
        // this.lobbyMain.panel.sevenDayBtn.onClick.Add(this.OnClickSevenBtn);
        this.lobbyMain.panel.shopBtn.onClick.Add(this.OnClickShopBtn);
        this.lobbyMain.panel.loginGiftBtn.onClick.Add(this.OnClickLoginGiftBtn);
        this.lobbyMain.panel.achieveBtn.onClick.Add(this.OnClickAchieveBtn);
        // this.lobbyMain.panel.copyBtn.onClick.Add(this.OnClickCopyBtn);

        this.lobbyMain.panel.packageBtn.onClick.Add(this.OnClickInheritBagButton);
        this.lobbyMain.panel.closeBtn.onClick.Add(this.OnClickInheritBagCloseButton);
        this.lobbyMain.panel.recycleBtn.onClick.Add(this.OnClickRecycleButton);
        // this.lobbyMain.panel.inheritList.itemRenderer = InheritListRender;
        // this.lobbyMain.panel.inheritBagList.itemRenderer = InheritBagListRender;
        this.lobbyMain.panel.recycleBagList.itemRenderer = InheritBagItemListRender;

        this.lobbyMain.panel.shuxingList.itemRenderer = ShuxingListRender;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLASSIC_UPDATE, this.UpdateClassicInfo);// 秘典
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateClassicInfo);// 秘典
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);//神器
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, this.UpdateArtifactInfo);

        EventDispatcher.GameWorld.Regist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.Regist<Vector2, double>(EventDefine.FIGHT_DATA_MONSTER_DEAD, OnMonsterDead);
        EventDispatcher.GameWorld.Regist(EventDefine.FIGHT_DATA_MONSTER_BOSS_INFO, OnMonsterBossInfo);
        EventDispatcher.GameWorld.Regist<EquipData, bool, int, int>(EventDefine.EVENT_USE_TREASURE_CHES_RES, OnUseTreasureChes);
        EventDispatcher.GameWorld.Regist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PART_REPLACE_RES, OnPartReplaceSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.Regist<List<ConfigStageUnit>, int>(EventDefine.STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.Regist<int>(EventDefine.MONSTER_WAVE_DATA_INFO, OnUpdateMonsterWaveInfo);
        EventDispatcher.GameWorld.Regist<Vector2>(EventDefine.STAGE_COMPLETE_INFO, OnStageCompleteInfo);
        EventDispatcher.GameWorld.Regist<ConfigItemTypeUnit, int>(EventDefine.STAGE_COMPLETE_RWARD, OnStageCompleteReward);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PART_DECOMPOSE_RES, OnEquipPartDecomposeSucc);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.STAGE_FIGHT_LOSE, OnStageFightLose);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, UpdatePartAll);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_Skill_LIST_Lobby, UpdateSkillList);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Lobby, UpdateHeroSkillList);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateItemCount);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.EVENT_GOTO_DUNGEON_MAP_STATE, this.OnGotoDungeonMapState);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_TRANSFER_SUCCESS, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateBottomRedDot);

        // 注册更新主线任务事件
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_TASK_UPDATE, this.UpdateTaskInfo);

        EventDispatcher.GameWorld.Regist<int>(EventDefine.EVENT_UPDATE_ONLINEAWARD_lobby, this.UpdateOnlineReward);
        // GameManager.Instance.SoundManager.PlayMusic(2);
        PlaySceneBGM();//根据场景播放BGM
        
        Object.FindObjectOfType<NoOperationMono>().isOpenCheck = true;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFuncPreviewInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateOtherInfo);//折叠栏位
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateLeftFunctionList);//左侧
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateRightFunctionList);//右侧
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_POWER_CHANGE_Update_lobby, this.OnPowerChange);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateMailRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateDailyRedDot);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, UpdateTreasureRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_level_up, this.PlayLevelUpEff);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_REDPOINT_UPDATE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SEVENDAY_UPDATE, UpdateSevenDayReddot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_RingInfo, UpdateShuxingInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_FIGHT_INFO, this.UpdateFightInfo);
        // EventDispatcher.GameWorld.Regist<int, int>(EventDefine.EVENT_SHOW_FORCE_GUIDE, this.OnShowForceGuide); // TODO // 暂时不需要引导功能
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD, this.UpdateVillageGuideProduceGetReward);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHANGE_HERO_TO_BATTLE, this.OnChangeHeroToBattle);
        EventDispatcher.GameWorld.Regist<float>(EventDefine.EVENT_UPDATE_BATTLE_SKILL_CD, this.ResetAllSkillCD);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLoginGiftBtn);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT, this.UpdatePassportReddot);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateShopFKZNReddot);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.UpdateFirstPayReddot);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.UpdateFirstPayReddot);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.OnUpdatePvpReddot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.OnClickInheritBagButton);
        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_LORE_EQUIP_EQUIP, this.UpdateInheritBagListItem);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateInheritInfo);//传承装备
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.Regist<int>(EventDefine.EVENT_GAIN_LORE_EQUIP, this.GainNewLoreEquip);
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ACHIEVEMENT_UPDATE, this.UpdateAchievementRedPoint);// 成就红点
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateClaimBuffUI);//buff
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AVATARS_UPDATE, this.UpdateOtherInfo);//侧边栏
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHANGENAMECOUNTER, this.UpdateOtherInfo);//侧边栏
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SEVENDAY_UPDATE, this.UpdateLeftFunctionList);//左功能栏-七天签到
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateRightFunctionList);//右功能栏-日常任务
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateLeftFunctionList);//左功能栏-邮件
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.UpdateLeftFunctionList);//左功能栏-pvp
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLeftFunctionList);//左功能栏-每日礼包
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateLeftFunctionList);//左功能栏-商城
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT, this.UpdateRightFunctionList);//右功能栏-通行证
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.UpdateRightFunctionList);//右功能栏-新人礼包
        // EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.UpdateRightFunctionList);//右功能栏-新人礼包
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ACHIEVEMENT_UPDATE, this.UpdateRightFunctionList);//右功能栏-成就
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateRightFunctionList);//右功能栏-家园
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpdateRightFunctionList);//右功能栏-家园
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateRightFunctionList);//右功能栏-副本
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.UpdateGuideInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdateGuideInfo);

        ConfigServerUnit serverUnit = ConfigDataGroup.GetInstance<ConfigServer>().Get(GameManager.Instance.CurServerUnit.Id);
        this.lobbyMain.panel.openPmBtn.visible = (serverUnit != null && serverUnit.IsShowPm == 1);

        _commonUnit30 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(30);

        //敏感字
        List<string> illegalStrList = new List<string>(ConfigDataGroup.GetInstance<ConfigIllegalWord>().Data.Count);
        foreach (var item in ConfigDataGroup.GetInstance<ConfigIllegalWord>().Data)
        {
            illegalStrList.Add(item.Value.Word);
        }
        IllegalWordDetection.Init(illegalStrList.ToArray());
        illegalStrList.Clear();

        this.UpdateSevenDayReddot();

        _common200004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200004);
        _common200009 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(200009);

        FairyGUI.Stage.inst.onTouchEnd.AddCapture(__stageTouchEnd);

        UIGLoaderManager.Instance.RecordResCacheKey("ItemPath/");
        UIGLoaderManager.Instance.RecordResCacheKey("Pet/");

        // 跑马灯
        UIManager.Instance.ShowUIPanel("Marquee");

        this.lobbyMain.panel.PVPBtn.onClick.Add(this.OnClickTestPVP);
        // OpenBottomPanel(2, 0);//默认进入地图

        HideNextStageButton();
        JumpManager.Instance.OnInit();
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        EventDispatcher.GameWorld.UnRegist<int, EN_CAMP_TYPE>(EventDefine.FIGHT_DATA_HERO_CASTSKILL, OnCastSkill);
        EventDispatcher.GameWorld.UnRegist<Vector2, double>(EventDefine.FIGHT_DATA_MONSTER_DEAD, OnMonsterDead);
        EventDispatcher.GameWorld.UnRegist(EventDefine.FIGHT_DATA_MONSTER_BOSS_INFO, OnMonsterBossInfo);
        EventDispatcher.GameWorld.UnRegist<EquipData, bool, int, int>(EventDefine.EVENT_USE_TREASURE_CHES_RES, OnUseTreasureChes);
        EventDispatcher.GameWorld.UnRegist<int, string>(EventDefine.EVENT_TREASURE_LEVELUP_RES, OnTreasureChesLevelUpRes);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PART_REPLACE_RES, OnPartReplaceSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CLASSIC_UPDATE, this.UpdateClassicInfo);//秘典
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateClassicInfo);// 秘典
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ARTIFACT_UPDATE, this.UpdateArtifactInfo);//神器
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, this.UpdateArtifactInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, OnRoleUpdate);
        EventDispatcher.GameWorld.UnRegist<List<ConfigStageUnit>, int>(EventDefine.STAGE_DATA_INFO, OnUpdateStageInfo);
        EventDispatcher.GameWorld.UnRegist<int>(EventDefine.MONSTER_WAVE_DATA_INFO, OnUpdateMonsterWaveInfo);
        EventDispatcher.GameWorld.UnRegist<Vector2>(EventDefine.STAGE_COMPLETE_INFO, OnStageCompleteInfo);
        EventDispatcher.GameWorld.UnRegist<ConfigItemTypeUnit, int>(EventDefine.STAGE_COMPLETE_RWARD, OnStageCompleteReward);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PART_DECOMPOSE_RES, OnEquipPartDecomposeSucc);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.STAGE_FIGHT_LOSE, OnStageFightLose);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SIGLE_EQUIP_WEAR, OnEquipSingleWear);
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_EQUIP_CHANGE, UpdatePartAll);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_Skill_LIST_Lobby, UpdateSkillList);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_BATTLE_HERO_Lobby, UpdateHeroSkillList);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateItemCount);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.EVENT_GOTO_DUNGEON_MAP_STATE, this.OnGotoDungeonMapState);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_TRANSFER_SUCCESS, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.UpdateBottomRedDot);

        // 销毁更新注册任务事件
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_TASK_UPDATE, this.UpdateTaskInfo);

        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFuncPreviewInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateOtherInfo);//折叠栏位
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateLeftFunctionList);//左侧
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateRightFunctionList);//右侧
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_POWER_CHANGE_Update_lobby, this.OnPowerChange);
        EventDispatcher.GameWorld.UnRegist<int>(EventDefine.EVENT_UPDATE_ONLINEAWARD_lobby, this.UpdateOnlineReward);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateMailRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateDailyRedDot);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.EVENT_TREASURE_PROGRESS_UPSUCC, UpdateTreasureRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_level_up, this.PlayLevelUpEff);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_REDPOINT_UPDATE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SEVENDAY_UPDATE, UpdateSevenDayReddot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_RingInfo, UpdateShuxingInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_FIGHT_INFO, this.UpdateFightInfo);
        // EventDispatcher.GameWorld.UnRegist<int, int>(EventDefine.EVENT_SHOW_FORCE_GUIDE, this.OnShowForceGuide); // TODO // 暂时不需要引导功能
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateBottomRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_GUIDE_PRODUCE_GETREWARD, this.UpdateVillageGuideProduceGetReward);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHANGE_HERO_TO_BATTLE, this.OnChangeHeroToBattle);
        EventDispatcher.GameWorld.UnRegist<float>(EventDefine.EVENT_UPDATE_BATTLE_SKILL_CD, this.ResetAllSkillCD);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLoginGiftBtn);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT, this.UpdatePassportReddot);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateShopFKZNReddot);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.UpdateFirstPayReddot);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.UpdateFirstPayReddot);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.OnUpdatePvpReddot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LORE_EQUIP_REMOVE, this.OnClickInheritBagButton);
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_LORE_EQUIP_EQUIP, this.UpdateInheritBagListItem);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.UpdateInheritInfo);//传承装备
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOREEQUIP_UPDATE, this.RefreshInheritBag);
        EventDispatcher.GameWorld.UnRegist<int>(EventDefine.EVENT_GAIN_LORE_EQUIP, this.GainNewLoreEquip);
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ACHIEVEMENT_UPDATE, this.UpdateAchievementRedPoint);// 成就红点
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateClaimBuffUI);//buff
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_AVATARS_UPDATE, this.UpdateOtherInfo);//侧边栏
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHANGENAMECOUNTER, this.UpdateOtherInfo);//侧边栏
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SEVENDAY_UPDATE, this.UpdateLeftFunctionList);//左功能栏-七天签到
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateRightFunctionList);//右功能栏-日常任务
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateLeftFunctionList);//左功能栏-邮件
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_PVP_GET_REWARD_UPDATE, this.UpdateLeftFunctionList);//左功能栏-pvp
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLeftFunctionList);//左功能栏-每日礼包
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateLeftFunctionList);//左功能栏-商城
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT, this.UpdateRightFunctionList);//右功能栏-通行证
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.UpdateRightFunctionList);//右功能栏-新人礼包
        // EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.UpdateRightFunctionList);//右功能栏-新人礼包
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ACHIEVEMENT_UPDATE, this.UpdateRightFunctionList);//右功能栏-成就
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateRightFunctionList);//右功能栏-家园
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpdateRightFunctionList);//右功能栏-家园
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DUNGEON_STAGE_UPDATE, this.UpdateRightFunctionList);//右功能栏-副本

        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_SKILL_LOTTERY_SUCCESS, this.UpdateGuideInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PET_SC_SUCC, this.UpdateGuideInfo);

        FairyGUI.Stage.inst.onTouchEnd.RemoveCapture(__stageTouchEnd);
        JumpManager.Instance.Dispose();
    }

    private void __stageTouchEnd(EventContext context)
    {
        OnTouchEndItem();
        OnTouchEndClassicItem();
    }

    public override void BindAll()
    {
        base.BindAll();
        LobbyBinder.BindAll();
    }

    protected override void OnHide()
    {
        base.OnHide();

        for (int i = 0; i < this.lobbyMain.panel.listBottom.numItems; i++)
        {
            GButton btn = this.lobbyMain.panel.listBottom.GetChildAt(i) as GButton;
            if (btn != null)
                btn.selected = false;
        }

        GameManager.Instance.TimerManager.ClearTimerBySource(EN_TIMER_SOURCE.UI);

        Utils.ClearSpineModelOnFGUI(this.lobbyMain.panel.RestView.hero);
        
        // 重置状态
        curSelectIndex = -1;
        isMapOpen = false;
    }

    private void OnGotoDungeonMapState(bool inCopyMap)
    {
        this.lobbyMain.panel.comBox.btnBoxIcon.enabled = true;
        this.lobbyMain.visible = !inCopyMap;
        // this.lobbyMain.panel.battleRoot.fightBossBg.visible = false;
        this.lobbyMain.panel.battleRoot.fightBoss.visible = false;
        if (!inCopyMap)
        {
            var hero = MapObjectManager.Instance.GetLocalHero();

            if (hero != null)
            {
                hero.IsAutoXPAttack = true;
            }

            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                item.Value.IsAutoXPAttack = this.lobbyMain.panel.battleRoot.comSkills.btnSkillAuto.ctrlAuto.selectedIndex == 1;
            }
            UpdateUIInfo();

            for (int i = 0; i < this.lobbyMain.panel.battleRoot.comSkills.listSkills.numChildren; i++)
            {
                UI_BtnSkill btnSkill = (UI_BtnSkill)this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(i);
                if (null != btnSkill)
                {
                    btnSkill.touchable = true;
                    GTween.Kill(btnSkill);
                }
            }

            RoleManager.Instance.SetHeroSkillProxy();
        }


    }


    protected override void OnShow()
    {
        base.OnShow();

        homeBtn = null;

        // 获取屏幕安全区域
        //Rect safeArea = Screen.safeArea;
        // 屏幕总尺寸
        //float screenHeight = Screen.height;
        //Debug.Log($"安全区域 h = {safeArea.height}，刘海高度 y = {safeArea.y}，屏幕高度 h = {screenHeight}");

        PlaySceneBGM();//根据场景播放BGM
        this.lobbyMain.maskBG.visible = true;
        this.lobbyMain.panel.listBottom.EnsureBoundsCorrect();
        // OpenBottomPanel(2, 0);//默认进入地图
        //首次登录进入大地图
        // if (GameManager.Instance.isFirstLogin == 0)
        // {
        //     OpenBottomPanel(2, 0);//默认进入地图
        // }
        // else
        // {
        //     // 0:大地图场景  1:非大地图场景，即战斗场景
        //     if (GameManager.Instance.downLineScene == 0)
        //     {
        //         OpenBottomPanel(2, 0);//默认进入地图
        //         GameManager.Instance.downLineScene = 0;
        //     }
        //
        //     if (GameManager.Instance.downLineScene == 1)
        //     {
        //         //不做处理
        //     }
        // }

        this.lobbyMain.panel.zzcCtrl.selectedIndex = 0;
        this.lobbyMain.panel.battleRoot.skillCtrl.selectedIndex = 0;
        this.lobbyMain.panel.battleRoot.copyCtrl.selectedIndex = 0;
        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Circle)
        {
            ConfigChapterUnit chapterUnit = ConfigUtils.GetChapterUnitById(DataManager.Instance.GetRoleData().chapterId);
            if (chapterUnit != null)
            {
                var stageId = DataManager.Instance.GetRoleData().stageId;
                List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(stageId);  //胜利或失败 循环感觉服务器下发的子节点索引控制
                MapObjectManager.Instance.MaxGuanKaMonsterIndex = stageUnits.Count;  //DataManager.Instance.GetRoleData().subStageId;
                // EventDispatcher.GameWorld.DispatchEvent(EventDefine.STAGE_FIGHT_LOSE, true);
            }
        }

        // 读取 怪物词条
        RoleManager.Instance.SetMonsterEntry();

        this.lobbyMain.panel.RestView.visible = true;  //显示默认背景图
        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Normal || DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Circle)
        {
            this.lobbyMain.panel.RestView.visible = false;  //显示默认背景图
        }
        //初始化关卡状态机
        MapObjectManager.Instance.InitGuanKaFSM();
        UpdateSkillList();
        if (!_isInit)
        {
            _isInit = true;
            this.lobbyMain.panel.battleRoot.topGroup.y = Math.Min(this.lobbyMain.panel.battleRoot.topGroup.y, orgBattleRootY - this.lobbyMain.panel.battleRoot.y + this.lobbyMain.panel.battleRoot.topGroup.y);
        }

        UpdateUI();
        UpdateClassicInfo();//更新秘典

        // UpdateArtifactInfo();//神器

        UpdateClaimBuffUI();//buff

        this.lobbyMain.panel.isShowOther.selectedIndex = 0;
        // this.lobbyMain.panel.showOtherBtn.visible = true;
        UpdateOtherInfo();//侧边栏
        UpdateLeftFunctionList();//左功能栏
        UpdateRightFunctionList();//右功能栏
        this.lobbyMain.panel.xialaCtrl2.selectedIndex = 0;
        this.lobbyMain.panel.xialaCtrl.selectedIndex = 0;

        Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.spineEff, "idle", true);
        this.lobbyMain.panel.comBox.btnBoxIcon.spineEff.spineAnimation.state.Data.DefaultMix = 0.3f;

        OnClickAutoSkill();//默认开启自动战斗
        UpdateShuxingInfo();
        //更新任务
        UpdateTaskInfo();
        this.lobbyMain.panel.showTask.selectedIndex = 0;

        RoleManager.Instance.TotalFight = FightUtils.GetTotalFight();
        OnPowerChange();

        //没有穿上的装备
        List<EquipData> lstEq = EquipManager.Instance.GetNoWearEquip();
        if (lstEq.Count > 0)
        {
            bool isNew = EquipManager.Instance.IsNewPartEquip((int)lstEq[0].partType);//此部位上是否有装备
            EquipManager.Instance.isNewEquip = isNew;
            EquipManager.Instance.curNoEquipGuid = lstEq[0].guid;
            int sourceId = lstEq[0].id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            this.lobbyMain.panel.tempEquipIcon.visible = true;
            this.lobbyMain.panel.tempEquipIcon.tempEquipIcon.icon = UIResource.GetItemUrl(config.Icon);
        }
        this.lobbyMain.panel.comBox.finger.visible = false;

        // 根据战斗状态初始化地图界面
        var battleStatus = DataManager.Instance.GetRoleData().battleStatus;
        if (battleStatus == (int)eBattleStatus.eBattleStatus_None)
        {
            // 延迟一帧确保UI初始化完成
            GameManager.Instance.TimerManager.SetTimer(0.1f, () => {
                OpenMapIfNeeded();
            });
        }
        
        if (DataManager.Instance.GetRoleData().OfflineRewardData != null && GuideManager.Instance.IsLobbyComplete())
        {
            this.lobbyMain.panel.listBottom.touchable = false;
            GameManager.Instance.TimerManager.SetTimer(.5f, () =>
            {
                this.lobbyMain.panel.listBottom.touchable = true;
                if (!GuideManager.Instance.IsShowGuiding)
                {
                    UIManager.Instance.ShowUIPanel("OfflineReward", 0, DataManager.Instance.GetRoleData().OfflineRewardData);
                }
                DataManager.Instance.GetRoleData().OfflineRewardData = null;
            });

        }
        else
        {
            // if(GuideManager.Instance.IsLobbyComplete())
            // UIManager.Instance.ShowUIPanel("LotteryTask", false, 1);
            this.lobbyMain.panel.listBottom.touchable = true;
        }

        UpdateInheritInfo();  //传承装备

        UpdateMailRedDot();
        UpdateDailyRedDot();
        UpdateTreasureRedDot(false);
        // TestWords();

        this.lobbyMain.panel.battleRoot.touchPanel.visible = false;

        UpdateFightInfo();
        this.UpdateFuncPreviewInfo();
        UpdateLoginGiftBtn();
        UpdateVillageGuideProduceGetReward();
        UpdatePassportReddot();
        UpdateShopFKZNReddot();
        UpdateFirstPayReddot();
        OnUpdatePvpReddot();
        HeroInfoManager.Instance.IsOperating = false;

        GameManager.Instance.TimerManager.SetTimer(1.5f, () =>
        {
            HideMaskBG();
        });
    }
    public void HideMaskBG()
    {
        if(!IsShow() || !IsOnStage() || this.lobbyMain == null || this.lobbyMain.maskBG == null) { return; }
        this.lobbyMain.maskBG.visible = false;
        //重新登录，如果一些关闭引导没有完成，前置条件达成则认为都完成
        GuideManager.Instance.ChkSubsequentGuidance();
        ChkGuide();
    }
    private void UpdateItemCount()
    {
        UpdateBoxInfo();
    }
    
    //更新任务信息-优化版
    private void UpdateTaskInfo()
    {
        // 获取当前任务信息
        ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());
            
        // 检查任务信息是否有效
        if (taskUnit != null)
        {
            // 设置任务名字：示例：任务1
            this.lobbyMain.panel.comPandaInfo.taskName.text = ConfigUtils.GetTextById(taskUnit.Name,taskUnit.NameParam);
            // 设置奖励信息
            this.lobbyMain.panel.comPandaInfo.reward.text = StringUtils.FormatCurrency(taskUnit.Reward);
            
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(1000);
            this.lobbyMain.panel.comPandaInfo.rewardItem.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
            ((UI_ItemCom)this.lobbyMain.panel.comPandaInfo.rewardItem).hasCount.selectedIndex = 0;
            ((UI_ItemCom)this.lobbyMain.panel.comPandaInfo.rewardItem).txtLv.text = taskUnit.Reward.ToString();
            ((UI_ItemCom)this.lobbyMain.panel.comPandaInfo.rewardItem).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
            
            switch (taskUnit.Type)
            {
                case (int)eMainTaskType.eMainTaskType_KillMonster:
                case (int)eMainTaskType.eMainTaskType_GenEquipTimes:
                case (int)eMainTaskType.eMainTaskType_SoldEquipTimes:
                case (int)eMainTaskType.eMainTaskType_PlayPetLotto:
                case (int)eMainTaskType.eMainTaskType_PlaySkillLotto:
                case (int)eMainTaskType.eMainTaskType_PlayHeroLotto:
                case (int)eMainTaskType.eMainTaskType_EquipBoxLevelUp:
                case (int)eMainTaskType.eMainTaskType_ClassicAtkLevelUp:
                case (int)eMainTaskType.eMainTaskType_ClassicHPLevelUp:
                case (int)eMainTaskType.eMainTaskType_ClassicDefLevelUp:
                case (int)eMainTaskType.eMainTaskType_ClassicPetLevelUp:
                case (int)eMainTaskType.eMainTaskType_DailyTask:
                case (int)eMainTaskType.eMainTaskType_WatchADTimes:
                case (int)eMainTaskType.eMainTaskType_OnlineTime:
                case (int)eMainTaskType.eMainTaskType_ClaimHomeTownProduct:
                case (int)eMainTaskType.eMainTaskType_LoginDays:
                case (int)eMainTaskType.eMainTaskType_ArenaPlay:
                case (int)eMainTaskType.eMainTaskType_ClainOnlineAward:
                case (int)eMainTaskType.eMainTaskType_GoldCopyTimes:
                case (int)eMainTaskType.eMainTaskType_EquipCopyTimes:
                case (int)eMainTaskType.eMainTaskType_DiamondCopyTimes:
                case (int)eMainTaskType.eMainTaskType_HeroExpCopyTimes:
                case (int)eMainTaskType.eMainTaskType_RuneCopyTimes:
                case (int)eMainTaskType.eMainTaskType_AnyCopyTimes:
                case (int)eMainTaskType.eMainTaskType_CollectLoreEquips:
                case (int)eMainTaskType.eMainTaskType_PickUpGold:
                case (int)eMainTaskType.eMainTaskType_PickUpDiamond:
                case (int)eMainTaskType.eMainTaskType_FishingCounter:
                case (int)eMainTaskType.eMainTaskType_KillNormalBoss:
                case (int)eMainTaskType.eMainTaskType_KillLoreBoss:
                case (int)eMainTaskType.eMainTaskType_ArtifactLevels:
                case (int)eMainTaskType.eMainTaskType_FinishFixedEvents:
                case (int)eMainTaskType.eMainTaskType_ShufflePetTalents:
                case (int)eMainTaskType.eMainTaskType_PutOnPetSkillBook:
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.min = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.max = int.Parse(taskUnit.Param1);
                    this.lobbyMain.panel.comPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), int.Parse(taskUnit.Param1));

                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 1;
                        // this.lobbyMain.comPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                        // this.lobbyMain.comPandaInfo.txtProgress2.SetVar("min", Math.Min(int.Parse(taskUnit.Param1), TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", taskUnit.Param1).FlushVars();
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                    }
                    else
                    {
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 0;
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                        // this.lobbyMain.comPandaInfo.txtProgress.SetVar("min", Math.Min(int.Parse(taskUnit.Param1), TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", taskUnit.Param1).FlushVars();
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_PassStage:
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 1;
                    string[] nameParam = taskUnit.TypeNameParam.Split('|');
                    
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.min = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.max = 1;
                    int num = 0;
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        num = 1;
                    }
                    this.lobbyMain.panel.comPandaInfo.taskBar.value = Mathf.Min(num, 1);
                    
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        // this.lobbyMain.comPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName),nameParam[0], nameParam[1]);
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), nameParam[0], nameParam[1]);
                    }
                    else
                    {
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName),nameParam[0], nameParam[1]);
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_GenQualityEquip:
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 1;
                    int quality = int.Parse(taskUnit.Param1);
                    string qualityName = EquipManager.Instance.GetQualityName((QualityType)quality);
                    this.lobbyMain.panel.comPandaInfo.taskBar.min = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.max = 1;
                    this.lobbyMain.panel.comPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), 1);
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= 1)
                    {
                        // this.lobbyMain.comPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                    }
                    else
                    {
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_CollectQualityEquips:
                case (int)eMainTaskType.eMainTaskType_CollectQualityPets:
                case (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips:
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 0;
                    string[] param1 = taskUnit.Param1.Split(',');
                    int num1 = int.Parse(param1[0]);
                    int quality1 = int.Parse(param1[1]);
                    string qualityName1 = EquipManager.Instance.GetQualityName((QualityType)quality1);
                    
                    this.lobbyMain.panel.comPandaInfo.taskBar.min = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.max = num1;
                    this.lobbyMain.panel.comPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), num1);
                    
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(param1[0]))
                    {
                        // this.lobbyMain.comPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                        // this.lobbyMain.comPandaInfo.txtProgress2.SetVar("min", Math.Min(num1, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num1.ToString()).FlushVars();
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                    }
                    else
                    {
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                        // this.lobbyMain.comPandaInfo.txtProgress.SetVar("min", Math.Min(num1, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num1.ToString()).FlushVars();
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_QualitySkillReachLevels:
                    this.lobbyMain.panel.comPandaInfo.taskType.selectedIndex = 0;
                    string[] param = taskUnit.Param1.Split(',');
                    int quality2 = int.Parse(param[0]);
                    int num2 = int.Parse(param[1]);
                    string qualityName2 = EquipManager.Instance.GetQualityName((QualityType)quality2);
                    
                    this.lobbyMain.panel.comPandaInfo.taskBar.min = 0;
                    this.lobbyMain.panel.comPandaInfo.taskBar.max = num2;
                    this.lobbyMain.panel.comPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), num2);
                    
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= num2)
                    {
                        // this.lobbyMain.comPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                        // this.lobbyMain.comPandaInfo.txtProgress2.SetVar("min", Math.Min(num2, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num2.ToString()).FlushVars();
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                    }
                    else
                    {
                        this.lobbyMain.panel.comPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                        // this.lobbyMain.comPandaInfo.txtProgress.SetVar("min", Math.Min(num2, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num2.ToString()).FlushVars();
                        this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
            }
            
            
            if(this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 0)
            {
                this.lobbyMain.panel.comPandaInfo.showHandle.selectedIndex = 0;
            }
            
            //if (taskUnit.TaskStartGuideId > 0 || taskUnit.TaskCompleteGuideId > 0)
            //{
            //    EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SHOW_FORCE_GUIDE, this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 1 ? taskUnit.TaskCompleteGuideId : taskUnit.TaskStartGuideId, this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 1 ? 2 : 1);
            //}
        }
    }

    public void UpdateHeroSkillList()
    {
        var hero = MapObjectManager.Instance.GetLocalHero();

        if (hero != null)
        {
            hero.IsAutoXPAttack = true;
        }

        UpdateSkillList();
    }

    private void UpdateSkillList()
    {
        _skillIdList.Clear();
        // var hero = MapObjectManager.Instance.GetLocalHero();
        // if(hero != null)
        //     _skillIdList.Add(0,hero.Attr.skillXPID);
        
        UpdateActiveSkill();// 主动技能
        foreach (var skill in RoleManager.Instance.GetSkillProxyDict())
        {
            if(!_skillIdList.ContainsKey(skill.Key))
                _skillIdList.Add(skill.Key, skill.Value.ProxyAttr.skillXPID);
            skill.Value.IsAutoXPAttack =
                this.lobbyMain.panel.battleRoot.comSkills.btnSkillAuto.ctrlAuto.selectedIndex == 1;
        }
        // this.lobbyMain.panel.battleRoot.comSkills.listSkills.numItems = 6;//TODO 技能修改为5个
        this.lobbyMain.panel.battleRoot.comSkills.listSkills.numItems = 5;

        UpdateBottomRedDot();
    }

    private void ResetAllSkillCD(float cdTime)
    {
        if (cdTime == 0)
        {
            for (int i = 0; i < this.lobbyMain.panel.battleRoot.comSkills.listSkills.numChildren; i++)
            {
                UI_BtnSkill btnSkill = (UI_BtnSkill) this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(i);
                if (null != btnSkill)
                {
                    btnSkill.touchable = true;
                    btnSkill.mask2.fillAmount = 0;
                    GTween.Kill(btnSkill);
                }
            }
        
            // 处理主动技能CD
            UI_BtnSkill activeSkill = this.lobbyMain.panel.battleRoot.comSkills.activeSkill;
            activeSkill.mask2.fillAmount = 0;
            GTween.Kill(activeSkill);
        }
        else
        {
            foreach (var tweener in tweenerDic)
            {
                if (tweener.Value != null)
                {
                    float remainingTime = tweener.Value.time - tweener.Value.time * tweener.Value.tween.normalizedTime;
                    float targetTime = remainingTime - tweener.Value.time * cdTime;
                    float timeScale = 1;
                    // 使用反射获取 _timeScale 字段
                    FieldInfo timeScaleField = typeof(GTweener).GetField("_timeScale", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (timeScaleField != null)
                    {
                        float currentTimeScale = (float)timeScaleField.GetValue(tweener.Value.tween);
                        timeScale = currentTimeScale + remainingTime / targetTime;
                        if (timeScale < 1)
                        {
                            timeScale = 1;
                        }
                    }
                    tweener.Value.tween.SetTimeScale(timeScale);
                    // tweener.Value.tween.SetDuration(tweener.Value.tween.duration - tweener.Value.time * cdTime);
                }
            }
        }
    }
    
    private void OnChangeHeroToBattle()
    {
        for (int i = 0; i < this.lobbyMain.panel.battleRoot.comSkills.listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                GTween.Kill(btnSkill);
                // btnSkill.mask.fillAmount = 0;
                btnSkill.mask2.fillAmount = 0;
            }
        }

        GameManager.Instance.TimerManager.SetTimer(0.2f, () =>
        {
            foreach (var skill in RoleManager.Instance.GetSkillProxyDict())
            {
                skill.Value.IsAutoXPAttack =
                    this.lobbyMain.panel.battleRoot.comSkills.btnSkillAuto.ctrlAuto.selectedIndex == 1;
            }
        });

    }

    private void ItemListSkillRender(int index, GObject item)
    {
        if (_skillIdList.TryGetValue(index, out int skillId))
        {
            item.data = skillId;
            if (skillId > 0)
            {
                ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(skillId);
                ((UI_BtnSkill) item).skillIcon.skillIcon.url = UIResource.GetItemUrl(skillUnit.SkillIcon);
                ((UI_BtnSkill)item).qualityIcon.visible = true;
                ((UI_BtnSkill)item).qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
                ((UI_BtnSkill) item).skillIcon.skillIcon.fixMode = true;
            }
        }
        // if(SkillInfoManager.Instance.UnLockSkillPos > index)
        //     ((UI_BtnSkill) item).hasSkill.selectedIndex = skillId > 0 ? 0 : 1;
        // else
        //     ((UI_BtnSkill) item).hasSkill.selectedIndex = 2;
        if (SkillInfoManager.Instance.UnLockSkillPos > index)
        {
            ((UI_BtnSkill) item).hasSkill.selectedIndex = skillId > 0 ? 0 : 1;
            ((UI_BtnSkill)item).qualityIcon.visible = skillId > 0;
        }
        else
        {
            ((UI_BtnSkill) item).hasSkill.selectedIndex = 2;
            ((UI_BtnSkill)item).qualityIcon.visible = false;
        }

        // if (((UI_BtnSkill) item).hasSkill.selectedIndex == 1)
        //     ((UI_BtnSkill) item).redDot.visible = SkillInfoManager.Instance.IsHasNoUpLoadSkill();
        if (((UI_BtnSkill)item).hasSkill.selectedIndex == 1)
        {
            ((UI_BtnSkill) item).redDot.visible = SkillInfoManager.Instance.IsHasNoUpLoadSkill();
            ((UI_BtnSkill)item).qualityIcon.visible = false;
        }

    }

    private void ItemRendererGroupList(int index, GObject item)
    {
        ((UI_MonsterGroupItem) item).type.selectedIndex = (index == 0) ? 0 : 1;
        if (index <= mCurGuankaIndex)
        {
            ((UI_MonsterGroupItem) item).full.selectedIndex = 1;
            if (index == mCurGuankaIndex)
            {
                ((UI_MonsterGroupItem) item).fullBar.TweenValue(100, 2f);
            }
            else
            {
                ((UI_MonsterGroupItem) item).fullBar.value = 100;
            }
        }
        else
        {
            ((UI_MonsterGroupItem) item).fullBar.value = 0;
            ((UI_MonsterGroupItem) item).full.selectedIndex = 0;
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
#if UNITY_EDITOR
        CheckHotkey();
#endif
        this.lobbyMain.panel.battleRoot.barBossTime.visible = false;
        if (MapObjectManager.Instance.IsMonsterBoss)
        {
            this.lobbyMain.panel.battleRoot.btnToppedAni.visible = false;
            this.lobbyMain.panel.battleRoot.zctz.visible = false;
            this.lobbyMain.panel.battleRoot.copyBossTime.visible = true;
            ((UI_BossTime)((UI_battleRoot)this.lobbyMain.panel.battleRoot).copyBossTime).title.text = ConfigUtils.FormatStringByKey(36, (int)MapObjectManager.Instance.timerMonsterBoss.GetRemain());
            // this.lobbyMain.panel.battleRoot.barBossTime.visible = true;
            // ((UI_BlueBar3) this.lobbyMain.panel.battleRoot.barBossTime).title.text = StringUtils.GetTimeString((int) MapObjectManager.Instance.timerMonsterBoss.GetRemain());
            //UpdateBossTimeProgressBar();
        }
        else
        {
            //this.lobbyMain.panel.battleRoot.barBossTime.visible = false;
            this.lobbyMain.panel.battleRoot.copyBossTime.visible = false;
        }

        if (MapObjectManager.Instance.MaxGuanKaMonsterIndex != 0 && !MapObjectManager.Instance.IsMonsterBoss)
        {
            this.lobbyMain.panel.battleRoot.loopIcon.visible = true;
        }
        else
        {
            this.lobbyMain.panel.battleRoot.loopIcon.visible = false;
        }

        if (EquipManager.Instance.AutoUnpack && !this.lobbyMain.panel.btnSetupAuto.autoEff.playing)
        {
            this.lobbyMain.panel.btnSetupAuto.autoEff.Play(-1, 0, null);
        }else if (!EquipManager.Instance.AutoUnpack && this.lobbyMain.panel.btnSetupAuto.autoEff.playing)
        {
            this.lobbyMain.panel.btnSetupAuto.autoEff.Stop();
        }
        //
        // if (!UIManager.Instance.IsTopController("Lobby") && !UIManager.Instance.IsTopController("FightLose"))
        // {
        //     var fightLose = UIManager.Instance.FindByName("FightLose");
        //     fightLose?.SetVisible(false);
        // }
        // ulong endTime =  MapObjectManager.Instance.InvincibleEndTime;
        // int totalSecond = (int)(endTime - ServerTimeManager.Instance.CurServerTime);
        // if (totalSecond > 0 && _totalXPSkillCd > 0)
        // {
        //     this.lobbyMain.panel.battleRoot.comSkills.xpskill.mask.fillAmount = totalSecond/_totalXPSkillCd;
        // }
        // else
        // {
        //     this.lobbyMain.panel.battleRoot.comSkills.xpskill.mask.fillAmount = 0;
        // }
        
        
        if (this.lobbyMain.panel.superTXZBtn.visible)
        {
            PassPortInfo pass = ActivityManager.Instance.GetPassPortInfo();
            if (pass != null)
            {
                int totalSecond = (int)(pass.EndTime - ServerTimeManager.Instance.CurServerTime);
                this.lobbyMain.panel.superTXZBtn.text = StringUtils.GetTimeString2(totalSecond);
            }

        }

        if (this.lobbyMain.panel.loginGiftBtn.visible)
        {
            int loginPackId = ActivityManager.Instance.LoginGiftPackID;
            ConfigLoginGiftUnit loginGiftUnit = ConfigUtils.GetLoginGiftById(loginPackId);
            ulong endTime = (ulong)(ServerTimeManager.Instance.GetNextToZeroServerTime() - 86400f + loginGiftUnit.Time * 60f);
            int totalSecond = (int) (endTime - ServerTimeManager.Instance.CurServerTime);
            if (totalSecond <= 0)
                this.lobbyMain.panel.loginGiftBtn.visible = false;
            else
            {
                this.lobbyMain.panel.loginGiftBtn.text = StringUtils.GetTimeString(totalSecond);
            }
        }

        UpdateClaimBuffUI();//buff

        if (UIManager.Instance.IsTopController("Lobby"))
        {
            PlaySceneBGM();
        }

        if (this.lobbyMain.panel.battleRoot.fightBoss.visible)
        {
            this.lobbyMain.panel.comPandaInfo.visible = true;
            this.lobbyMain.panel.taskXialaBtn.visible = false;
        }
        else
        {
            this.lobbyMain.panel.comPandaInfo.visible = true;
            this.lobbyMain.panel.taskXialaBtn.visible = true;
        }

        if (this.lobbyMain.panel.RestView.visible)
        {
            this.lobbyMain.panel.taskPos.selectedIndex = 1;
            // this.lobbyMain.panel.comPandaInfo.visible = false;
            // this.lobbyMain.panel.taskXialaBtn.visible = false;
        }
        else
        {
            this.lobbyMain.panel.taskPos.selectedIndex = 0;
            if(DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Normal && !this.lobbyMain.panel.comPandaInfo.visible)
            {
                // this.lobbyMain.panel.comPandaInfo.visible = true;
                // this.lobbyMain.panel.taskXialaBtn.visible = true;
                ChkGuide();
            }
        }
    }

    public GComponent GetBattleRoot()
    {
        return this.lobbyMain.panel.battleRoot;
    }
    
    public GComponent GetRoot()
    {
        return this.lobbyMain;
    }

    public GComponent GetBattleBGRoot()
    {
        return this.lobbyMain.panel.battleRoot.bg;
    }

    public GComponent GetBattleSceneRoot()
    {
        return this.lobbyMain.panel.battleRoot.scene;
    }

    public GComponent GetBattleSceneObjRoot()
    {
        return this.lobbyMain.panel.battleRoot.scene.obj;
    }

    public GComponent GetBattleSceneFlyRoot()
    {
        return this.lobbyMain.panel.battleRoot.scene.fly;
    }

    protected void UpdateUI()
    {
        UpdateUIInfo();
        UpdatePartAll();
    }

    private void UpdateBossTimeProgressBar()
    {
        double v = ((UI_BlueBar3) this.lobbyMain.panel.battleRoot.barBossTime).bar.value;
        float curVelocity = 0;
        float maxValue = MapObjectManager.Instance.MonsterBossTime;
        ((UI_BlueBar3) this.lobbyMain.panel.battleRoot.barBossTime).bar.value = Mathf.SmoothDamp((float) v,
            MapObjectManager.Instance.timerMonsterBoss.GetRemain() / maxValue * 100f, ref curVelocity, Time.deltaTime);
    }

    private enum E_SYSTEM
    {
        Role,
        Home,
        Map,
        ZZC,
        Shop
    }
    
    private string spinePath = "";
    /// <summary>
    /// 关闭地图、显示休闲营地 
    /// </summary>
    public void OpenBottomMapPanel()
    {
        PlaySceneBGM();
        
        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_NoneInCamp && !GuideManager.Instance.IsShowGuiding) //没有战斗--在营地
        {   // 显示营地
            this.lobbyMain.panel.RestView.visible = true;
            
            HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
            if (myHero.HeroUnit.Model != spinePath)
            {
                spinePath = myHero.HeroUnit.Model;
                Utils.ClearSpineModelOnFGUI(this.lobbyMain.panel.RestView.hero);
                Utils.SetSpineModelOnFGUI(this.lobbyMain.panel.RestView.hero, spinePath, 80f, "idle", (o) => {});
            }
            HideMaskBG();
            return;
        }
        else if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_None && !GuideManager.Instance.IsShowGuiding)  //没有战斗--在地图
        {   //只打开地图
            GButton btn = this.lobbyMain.panel.listBottom.GetChildAt((int)E_SYSTEM.Map) as GButton;
            if(btn != null)
                btn.selected = true;
            isMapOpen = true;
            ChangeIndex(2, true,0);
            return;
        }
        this.lobbyMain.panel.RestView.visible = false;
        OpenBottomPanel(2, 0);
        HideNextStageButton();
        HideMaskBG();
        ChkGuide();
    }
    
    /// <summary>
    /// 跳转到主界面对应按钮
    /// </summary>
    /// <param name="index">索引 0-4</param>
    public void OnlyOpenBottomPanel(int index, object data)
    {
        (this.lobbyMain.panel.listBottom.GetChildAt(index) as UI_BtnBottom).data = data;
        (this.lobbyMain.panel.listBottom.GetChildAt(index) as UI_BtnBottom)?.FireClick(true, true);
    }
    
    /// <summary>
    /// 跳转到主界面对应按钮
    /// </summary>
    /// <param name="index">索引 0-4</param>
    public void OpenBottomPanel(int index, object data)
    {
        UIManager.Instance.CloseAllUIPanelExcept("Lobby");
        (this.lobbyMain.panel.listBottom.GetChildAt(index) as UI_BtnBottom).data = data;
        (this.lobbyMain.panel.listBottom.GetChildAt(index) as UI_BtnBottom)?.FireClick(true, true);
    }

    public void OpenBottomPanelToEquipMake()
    {
        UIManager.Instance.CloseAllUIPanelExcept("Lobby");
        this.lobbyMain.panel.zzcCtrl.selectedIndex = 1;
        this.lobbyMain.panel.battleRoot.skillCtrl.selectedIndex = 1;
        // (this.lobbyMain.panel.listBottom.GetChildAt((int)E_SYSTEM.ZZC) as UI_BtnBottom)?.FireClick(true, true);
        ShowGenEquipFinger();
    }

    public void OpenBottomPanelToSkill()
    {
        UIManager.Instance.CloseAllUIPanelExcept("Lobby");
        (this.lobbyMain.panel.listBottom.GetChildAt(0) as UI_BtnBottom).data = 1;
        (this.lobbyMain.panel.listBottom.GetChildAt(0) as UI_BtnBottom)?.FireClick(true, true);
    }
    
    public void OnClickBottomItem(EventContext context)
    {
        object param = null;
        GButton item = context.data as GButton;
        var index = this.lobbyMain.panel.listBottom.GetChildIndex(item);
        bool isMapButton = index == (int)E_SYSTEM.Map;
        param = item.data;
        
        // 处理地图按钮点击
        if (isMapButton)
        {
            // 切换地图状态
            bool newState = !isMapOpen;
            
            // 如果要打开地图且当前有非地图界面打开，先关闭非地图界面
            if (newState && curSelectIndex != -1)
            {
                GButton preBtn = this.lobbyMain.panel.listBottom.GetChildAt(curSelectIndex) as GButton;
                if (preBtn != null)
                {
                    preBtn.selected = false;
                    ChangeIndex(curSelectIndex, false, preBtn.data);
                    curSelectIndex = -1;
                }
            }
            // 关闭地图时需要同步关闭非地图界面
            else if (!newState && curSelectIndex != -1)
            {
                GButton nonMapBtn = this.lobbyMain.panel.listBottom.GetChildAt(curSelectIndex) as GButton;
                if (nonMapBtn != null)
                {
                    nonMapBtn.selected = false;
                    ChangeIndex(curSelectIndex, false, nonMapBtn.data);
                    curSelectIndex = -1;
                }
            }
            
            item.selected = newState;
            isMapOpen = newState;
            ChangeIndex(index, newState, param);
            return;
        }
        
        // 处理非地图按钮
        if (index == curSelectIndex)
        {
            // 点击已选中的按钮 - 关闭当前界面
            item.selected = false;
            curSelectIndex = -1;
            ChangeIndex(index, false, param);
            
            // 关闭后检查是否需要显示地图
            TryOpenMapAfterClose();
        }
        else
        {
            // 关闭之前打开的非地图界面
            if (curSelectIndex != -1)
            {
                GButton preBtn = this.lobbyMain.panel.listBottom.GetChildAt(curSelectIndex) as GButton;
                if (preBtn != null)
                {
                    preBtn.selected = false;
                    ChangeIndex(curSelectIndex, false, preBtn.data);
                }
            }
            
            // 打开新界面
            item.selected = true;
            curSelectIndex = index;
            ChangeIndex(index, true, param);
        }

        GuideToClickZZCSys(item.selected);
        if (!item.selected)
            curSelectIndex = -1;
        
        OnStageFightLose(this.lobbyMain.panel.battleRoot.btnToppedAni.visible);
        OnStageFightLose(this.lobbyMain.panel.battleRoot.zctz.visible);

        if(index == 1)
        {//点击装备的引导
            ChkGuide();
        }
    }
    
    // 非地图界面关闭后尝试打开地图
    private void TryOpenMapAfterClose()
    {
        var roleData = DataManager.Instance.GetRoleData();
        if (roleData != null && roleData.battleStatus == (int)eBattleStatus.eBattleStatus_None && !isMapOpen && curSelectIndex == -1)
        {
            OpenMapIfNeeded();
        }
    }
    
    // 按需打开地图界面
    private void OpenMapIfNeeded()
    {
        var roleData = DataManager.Instance.GetRoleData();
        if (roleData != null && roleData.battleStatus == (int)eBattleStatus.eBattleStatus_None && !isMapOpen)
        {
            int mapIndex = (int)E_SYSTEM.Map;
            GButton mapButton = this.lobbyMain.panel.listBottom.GetChildAt(mapIndex) as GButton;
            if (mapButton != null)
            {
                isMapOpen = true;
                mapButton.selected = true;
                ChangeIndex(mapIndex, true, mapButton.data);
            }
        }
    }

    public void ChangeIndex(int index, bool isSelected, object param)
    {
        // this.lobbyMain.panel.listBottom.touchable = true;
        UIManager.Instance.CloseUIPanel("Chat");

        if (this.lobbyMain.panel.leftFunctionList.numItems > 0)
        {
            this.lobbyMain.panel.leftFunctionList.ScrollToView(0);
        }
        if (this.lobbyMain.panel.rightFunctionList.numItems > 0)
        {
            this.lobbyMain.panel.rightFunctionList.ScrollToView(0);
        }
        
        switch (index)
        {
            case (int) E_SYSTEM.Role:
                if (isSelected)
                {
                    UIManager.Instance.ShowUIPanel("RoleSystem", param);
                    
                    MapChapterManager.Instance.HideMapObject();
                    var view = UIManager.Instance.FindByName("ChapterStageDetail") as ChapterStageDetailView;
                    if (view != null)
                    {
                        UIManager.Instance.CloseUIPanel("ChapterStageDetail");
                    }
                    GrimoireManager.Instance.isChangeUI = true;
                    UpdateClassicInfo();
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("RoleSystem");
                    GrimoireManager.Instance.isChangeUI = false;
                }
                break;
            case (int) E_SYSTEM.Home:
                var zhuzhaoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
                if (!zhuzhaoMap.Item1)
                {
                     UIManager.Instance.Toast(zhuzhaoMap.Item2);
                     ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).selected = false;
                      return;
                }
                if (isSelected)
                {
                    UIManager.Instance.ShowUIPanel("EquipSystem", param);
                    
                    MapChapterManager.Instance.HideMapObject();
                    var view = UIManager.Instance.FindByName("ChapterStageDetail") as ChapterStageDetailView;
                    if (view != null)
                    {
                        UIManager.Instance.CloseUIPanel("ChapterStageDetail");
                    }
                    GrimoireManager.Instance.isChangeUI = true;
                    UpdateClassicInfo();
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("EquipSystem");
                    GrimoireManager.Instance.isChangeUI = false;
                }
                break;
            case (int) E_SYSTEM.Map:
                if (isSelected)
                {
                    UIManager.Instance.ShowUIPanel("ChapterMap", param);
                    ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).title = ConfigUtils.GetStringByKey(5172);
                    HideMaskBG();
                    GrimoireManager.Instance.isChangeUI = true;
                    UpdateClassicInfo();
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("ChapterMap");
                    ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).title = ConfigUtils.GetStringByKey(5171);
                    GrimoireManager.Instance.isChangeUI = false;
                    if (param is JumpTypeEnum)
                    {
                        ClassicListScrollToView((JumpTypeEnum)param);
                    }
                }
                break;
            case (int) E_SYSTEM.ZZC:
                // var heroClassicMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroClassic);
                // if (!heroClassicMap.Item1)
                // {
                //     UIManager.Instance.Toast(heroClassicMap.Item2);
                //     ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).selected = false;
                //     return;
                // }
                // this.lobbyMain.panel.zzcCtrl.selectedIndex = isSelected ? 0 : 1;
                // this.lobbyMain.panel.zzcCtrl.selectedIndex = isSelected ? 1 : 0;
                // this.lobbyMain.panel.battleRoot.skillCtrl.selectedIndex = isSelected ? 1 : 0;
                // if(isSelected)
                //     this.lobbyMain.panel.shuxingList.ScrollToView(0);
                // if (isSelected)
                // {
                //     UpdateClassicInfo();
                // }
                
                var summonPetMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPet);
                if (!summonPetMap.Item1)
                {
                    UIManager.Instance.Toast(summonPetMap.Item2);
                    ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).selected = false;
                    return;
                }
                if (isSelected)
                {
                    UIManager.Instance.ShowUIPanel("PetSystem", param);
                    
                    MapChapterManager.Instance.HideMapObject();
                    var view = UIManager.Instance.FindByName("ChapterStageDetail") as ChapterStageDetailView;
                    if (view != null)
                    {
                        UIManager.Instance.CloseUIPanel("ChapterStageDetail");
                    }
                    GrimoireManager.Instance.isChangeUI = true;
                    UpdateClassicInfo();
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("PetSystem");
                    GrimoireManager.Instance.isChangeUI = false;
                }
                break;
            case (int) E_SYSTEM.Shop:
                var summonMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
                if (!summonMap.Item1)
                {
                    UIManager.Instance.Toast(summonMap.Item2);
                    ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(index)).selected = false;
                    return;
                }
                if (isSelected)
                {
                    UIManager.Instance.ShowUIPanel("SummonSystem", param);
                    ((UI_BtnBottom)this.lobbyMain.panel.listBottom.GetChildAt(index)).data = null;
                    
                    MapChapterManager.Instance.HideMapObject();
                    var view = UIManager.Instance.FindByName("ChapterStageDetail") as ChapterStageDetailView;
                    if (view != null)
                    {
                        UIManager.Instance.CloseUIPanel("ChapterStageDetail");
                    }
                    GrimoireManager.Instance.isChangeUI = true;
                    UpdateClassicInfo();
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("SummonSystem");
                    GrimoireManager.Instance.isChangeUI = false;
                }
                break;
        }

    }

    private void OnClickTujianBtn()
    {
        UIManager.Instance.ShowUIPanel("TuJianMain");
    }

    private void OnclickVillageBtn()
    {
        UIManager.Instance.ShowUIPanel("VillageHome");
    }

    private void OnClickDailyBtn()
    {
        UIManager.Instance.ShowUIPanel("DailyTask");
    }

    private void OnClickMailBtn()
    {
        // UIManager.Instance.ToastByKey(206);
        UIManager.Instance.ShowUIPanel("MailMain");
    }

    private void OnClickFirstChargeBtn()
    {
        UIManager.Instance.ShowUIPanel("FirstPay");
    }

    private void OnClickOnlineBtn()
    {
        var builder = OnlineAwardClick_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_OnlineAwardClick_CS, builder.Build());
    }

    private void OnClickSetupAuto()
    {
        var autoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.AutoPack);
        if (!autoMap.Item1)
        {
            UIManager.Instance.Toast(autoMap.Item2);
            return;
        }
        if (EquipManager.Instance.AutoUnpack)
        {
            EquipManager.Instance.AutoUnpack = false;
            this.lobbyMain.panel.btnSetupAuto.autoEff.Stop();
            return;
        }

        UIManager.Instance.ShowUIPanel("AutoUnpack");
    }

    // private void OnClickHelp()
    // {
    //     UIManager.Instance.ShowUIPanel("LotteryTask");
    // }

    private void OnClickSpeed()
    {
    }

    private void OnClickTopped()
    {
        // this.lobbyMain.panel.battleRoot.btnToppedAni.t0.Play();
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralChallengeBossClickSE);
        MapObjectManager.Instance.InitGuanKaFSM(MapObjectManager.Instance.MaxGuanKaMonsterIndex);
        OnStageFightLose(false);
        GuideManager.Instance.HideGuide();
    }
    
    private void OnClickNextStage()
    {
        // GButton btn = this.lobbyMain.panel.listBottom.GetChildAt((int)E_SYSTEM.Map) as GButton;
        // if(btn != null)
        //     btn.selected = true;
        // ChangeIndex(2, true, MapObjectManager.Instance.GuanKaStageId + 1);
        
        // 自动挑战下一关  ---- 当前为1-1，挑战1-2
        // GButton btn = this.lobbyMain.panel.listBottom.GetChildAt((int)E_SYSTEM.Map) as GButton;
        // if (btn != null)
        // {
        //     btn.data = MapObjectManager.Instance.GuanKaStageId + 1;
        //     EventContext context = new EventContext();
        //     context.data = btn;
        //     OnClickBottomItem(context);
        // }
        
        // 自动挑战关卡 -----  挑战大地图上有黄色标记的关卡
        GButton btn = this.lobbyMain.panel.listBottom.GetChildAt((int)E_SYSTEM.Map) as GButton;
        if (btn != null)
        {
            EventContext context = new EventContext();
            context.data = btn;
            OnClickBottomItem(context);

            float time = 0.1f;
            if (!MapChapterManager.Instance.isLoadComplete) time = 3f;
            
            // 延迟等待界面加载完
            GameManager.Instance.TimerManager.SetTimer(time, () =>
            {
                var mapView = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                if (mapView != null)
                {
                    mapView?.OnClickFightCurStageBtn();
                }
            });
        }

        GuideManager.Instance.HideGuide();
    }

    private void OnClickBoxIcon()
    {
        // 是否解锁铸造功能
        var zhuzhaoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
        if (!zhuzhaoMap.Item1)
        {
            UIManager.Instance.Toast(zhuzhaoMap.Item2);
            return;
        }
        
        this.lobbyMain.panel.comBox.btnBoxIcon.enabled = false;
        GameManager.Instance.TimerManager.SetTimer(2f, () =>
        {
            this.lobbyMain.panel.comBox.btnBoxIcon.enabled = true;
        });
        if (EquipManager.Instance.AutoUnpack)
        {
            EquipManager.Instance.AutoUnpack = false;
            EquipManager.Instance.HasNewEquipToStopAutopack = false;
            return;
        }

        if (EquipManager.Instance.curNoEquipGuid != 0)
        {
            UIManager.Instance.ShowUIPanel("Equip", EquipManager.Instance.curNoEquipGuid, EquipManager.Instance.isNewEquip);
            return;
        }
        TreasureChesManager.Instance.UseTreasureChes(DataManager.Instance.GetTreasureData().id, 0, 0);
    }

    private void OnClickBoxLv()
    {
        UIManager.Instance.ShowUIPanel("TreasureLevelup");
    }

    private bool IsAutoXPAttack = false;
    private void OnClickAutoSkill()
    {
        // var hero = MapObjectManager.Instance.GetLocalHero();
        //
        // if (hero != null)
        // {
            // hero.IsAutoXPAttack = !hero.IsAutoXPAttack;
            IsAutoXPAttack = !IsAutoXPAttack;
            this.lobbyMain.panel.battleRoot.comSkills.btnSkillAuto.ctrlAuto.selectedIndex = IsAutoXPAttack ? 1 : 0;
           
            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                item.Value.IsAutoXPAttack = IsAutoXPAttack;
            }
            if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
                RoleManager.Instance.GetMapHeroSkillProxy().IsAutoXPAttack = IsAutoXPAttack;
        // }
        
    }

    private void OnClickHeroSkill(EventContext context)
    {
        if (RoleManager.Instance.GetMapHeroSkillProxy() != null)
        {
            RoleManager.Instance.GetMapHeroSkillProxy().CastXPSkill();
        }
    }
    
    private void OnClickSkillItem(EventContext context)
    {
        var index = this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildIndex((GObject) context.data);

        if (index != -1)
        {
            bool isHasSkill = false;
            foreach (var item in RoleManager.Instance.GetSkillProxyDict())
            {
                if (item.Key == index && item.Value.ProxyAttr.skillXPID > 0)
                {
                    item.Value.CastXPSkill();
                    isHasSkill = true;
                    break;
                }
            }

            if (!isHasSkill && index < SkillInfoManager.Instance.UnLockSkillPos)
            {
                OpenBottomPanelToSkill();
            }
            else if(!isHasSkill)
            {
                var stateMap =  FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType) (index+1901));
                UIManager.Instance.Toast(stateMap.Item2);
            }
        }
    }

    private void UpdateUIInfo()
    {
        UpdateRoleInfo();

        this.lobbyMain.panel.comBox.btnBoxLv.txtBoxLv.text = DataManager.Instance.GetTreasureData().id.ToString();
        this.lobbyMain.panel.comBox.btnBoxKey.txtKeyValue.text = DataManager.Instance.GetMagicKeys().ToString();

        for (int i = 0; i < this.lobbyMain.panel.battleRoot.comSkills.listSkills.numChildren; i++)
        {
            UI_BtnSkill btnSkill = (UI_BtnSkill) this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(i);
            if (null != btnSkill)
            {
                // btnSkill.mask.fillAmount = 0;
                btnSkill.mask2.fillAmount = 0;
            }
        }
        
        // 处理主动技能CD
        UI_BtnSkill activeSkill = this.lobbyMain.panel.battleRoot.comSkills.activeSkill;
        activeSkill.mask2.fillAmount = 0;
    
        // 计算实际冷却时间（应用冷却缩减）
        float time = DataManager.Instance.GetRoleData().FightAttrVo.SkillCd;
        float skillCdReduction = DataManager.Instance.GetRoleData().FightAttrVo.SkillCd * ConstDefine.CONFIG_PLACE_EX;
        float actualCD = _activeSkillBaseCD * (1 - skillCdReduction);

        //刚进战斗 默认为0，可释放
        actualCD = 0;

        CastSkillCD(activeSkill, actualCD);
        
    }

    private void UpdateRoleInfo()
    {
        ((UI_ComUserInfo)this.lobbyMain.panel.userInfo).UpdateUserInfo();
        ((UI_ComUserInfo)this.lobbyMain.userInfo).UpdateUserInfo();//修改主界面顶部栏位、测试
        //红点
        UpdateBottomRedDot();
        OnPowerChange();
    }

    private void OnPowerChange()
    {
        this.lobbyMain.panel.fightValLb.text = StringUtils.FormatCurrency(RoleManager.Instance.TotalFight);
    }

    private void OnRoleUpdate()
    {
        if (IsShow() && IsOnStage())
        {
            UpdateRoleInfo();
            UpdateSkillList();
            UpdateTreasureRedDot(false);
            UpdateShuxingInfo();
            // UpdateClassicInfo();
        }
    }

    #region 战斗

    private GTweener tweener;
    private Dictionary<int, SkillCDInfo> tweenerDic = new Dictionary<int, SkillCDInfo>();
    private void CastSkillCD(UI_BtnSkill item, float time)
    {
        GTween.Kill(item);
        tweener = GTween.To(1, 0, time)
            .SetTarget(item)
            .SetEase(EaseType.Linear)
            .OnStart(() =>
            {
                // item.mask.fillAmount = 0;
                item.mask2.fillAmount = 0;
                item.touchable = false;
            })
            .OnUpdate((fillnum) =>
            {
                item.mask2.fillAmount = fillnum.value.x;
                //tweenerDic[item.GID].tween.SetDuration(tweenerDic[item.GID].tween.duration); //重新设置 倒计时时间
            })
            .OnComplete(() =>
            {
                // item.mask.fillAmount = 0;
                item.mask2.fillAmount = 0;
                item.touchable = true;
                tweener = null;
                tweenerDic[item.GID] = null;
            });
        
        SkillCDInfo info = new SkillCDInfo();
        info.tween = tweener;
        info.time = time;
        tweenerDic[item.GID] = info;
    }

    /// <summary>
    /// 施法技能
    /// </summary>
    /// <param name="skillID"></param>
    /// <param name="campType"></param>
    private void OnCastSkill(int skillID, EN_CAMP_TYPE campType)
    {
        if (IsShow() && IsOnStage())
        {
            var skillType = ConfigUtils.GetSkillById(skillID);
            int skillIndex = -1;
            foreach (var item in _skillIdList)
            {
                if (item.Value == skillID)
                {
                    skillIndex = item.Key;
                    break;
                }
            }

            if (skillIndex >= ConstDefine.PetSkillIndex)
                return; //宠物技能  不需要UI表现
            
            // 判断是否为英雄主动技能
            bool isActiveSkill = false;//(skillType.SkillType == (int)EN_SKILL_TYPE.XP);
            if (skillType.SkillType == (int)EN_SKILL_TYPE.XP)
            {
                MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
                if (heroObject != null)
                {
                    int activeSkillId = heroObject.HeroAttr.HeroUnit.ActiveSkill;
                    if (activeSkillId == skillType.Id)
                    {
                        isActiveSkill = true;
                    }
                }
            }
            
            if(skillType != null && (skillType.SkillType == (int) EN_SKILL_TYPE.ChoukaSkill || skillType.SkillType == (int) EN_SKILL_TYPE.XP))
                SceneEffect();
            
            float actualCD;
            if (isActiveSkill)
            {
                actualCD = _activeSkillBaseCD * (1 - DataManager.Instance.GetRoleData().FightAttrVo.SkillCd * ConstDefine.CONFIG_PLACE_EX);
                CastSkillCD(this.lobbyMain.panel.battleRoot.comSkills.activeSkill, actualCD);
            }

            if (skillType != null && skillIndex != -1)
            {
                float fCDTime = skillType.Cd * ConstDefine.CONFIG_PLACE_EX/MapObjectManager.Instance.Speed;
                fCDTime = Math.Max(0, fCDTime * (1 - DataManager.Instance.GetRoleData().FightAttrVo.SkillCd));
                var skillBtn = this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(skillIndex) as UI_BtnSkill;

                if (skillBtn != null)
                {
                    if (skillBtn.tapSpine != null)
                    {
                        skillBtn.tapSpine.visible = true;
                        Utils.PlaySpineAnim(skillBtn.tapSpine, "chuxian", false, () =>
                        {
                            skillBtn.tapSpine.visible = false;
                        });
                    }
 
                    CastSkillCD(skillBtn, fCDTime);
                }
            }
        }
    }

    /// <summary>
    /// 怪死亡飘金币
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gold"></param>
    public void OnMonsterDead(Vector2 pos, double gold)
    {
        if (!UIManager.Instance.IsTopController("Lobby")) return;
        // 飘金币
        UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();//new UIItemsGain();
        // UI_ComCurrency comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.panel.userInfo).comCurrency;
        UI_ComCurrency comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.userInfo).comCurrency;//修改主界面顶部栏位、测试
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_GoldId);
        itemsGain.ApplyItemSourceToDestination(comCurrency.btnGold.asCom, itemTypeUnit.Icon);
        itemsGain.StartItemFly(pos, gold);
        DataManager.Instance.GetRoleData().gold += gold;
        double startValue = DataManager.Instance.GetRoleData().gold - gold;
        double endValue = DataManager.Instance.GetRoleData().gold;
        itemsGain.flyComplete = () =>
        {
            GTween.ToDouble(startValue, endValue, 0.3f).SetEase(EaseType.Linear)
                .OnUpdate((GTweener tweener) =>
                {
                    comCurrency.btnGold.txtValue.text = StringUtils.FormatCurrency(Math.Ceiling(tweener.value.d));
                }).OnComplete((() =>
                {
                    comCurrency.btnGold.txtValue.text =
                        StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
                }));
        };
    }

    public void OnMonsterBossInfo()
    {
        //this.lobbyMain.panel.battleRoot.barBossTime.visible = MapObjectManager.Instance.IsMonsterBoss;
        this.lobbyMain.panel.battleRoot.copyBossTime.visible = MapObjectManager.Instance.IsMonsterBoss;
    }

    #endregion

    #region 开箱

    private void OnUseTreasureChes(EquipData equipData, bool isNew, int deltaTime, int type)
    {
        if (IsShow() && IsOnStage())
        {
            GuideManager.Instance.HideGuide();
            ulong guid = equipData.guid;
            // Debug.Log("isNew:"+isNew);
            EquipManager.Instance.curNoEquipGuid = guid;
            EquipManager.Instance.isNewEquip = isNew;
            EquipManager.Instance.GetEquipById(guid).isNew = true;
            int sourceId = equipData.id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            int quality = config.Quality >= 5 ? 5 : config.Quality;
            Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.spineEff, "lv1", false, delegate()
            {
                Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.spineEff, "idle", true);
            });
            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 2, 0.2f, () =>
            {
                // if(!VillageInfoManager.Instance.IsInVillageHome)
                    // GameManager.Instance.SoundManager.PlayEffectWithoutLoop(14);
                Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.fx, "skill", false);
                Utils.PlaySpineAnim(this.lobbyMain.panel.comBox.btnBoxIcon.fx1, "skill", false);
                this.lobbyMain.panel.comBox.btnBoxIcon.enabled = true;
            });

            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,3, 0.3f, () =>
            {
                this.lobbyMain.panel.tempEquipIcon.visible = true;
                this.lobbyMain.panel.tempEquipIcon.tempEquipIcon.icon = UIResource.GetItemUrl(config.Icon);// UIResource.GetPartURL(config.Parts);
                GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 4,0.3f, () =>
                {
                    UpdateBoxInfo();
                    if (EquipManager.Instance.AutoUnpack)
                    {
                        bool isStopAutopack = true;
                        bool isEntry1 = true;
                        bool isEntry2 = true;
                        AutoOpenEquipStruct openEquipStruct = EquipManager.Instance.OpenEquipStruct;
                        // foreach (var lstEntryId in equipData.lstEntrysID)
                        // {
                        //     Debug.LogFormat("lstEntryId={0}", lstEntryId);
                        // }
                        // Debug.LogFormat("openEquipStruct.entryId1={0}  {1}", openEquipStruct.entryId1, equipData.lstEntrysID.Contains(openEquipStruct.entryId1));
                        // Debug.LogFormat("openEquipStruct.entryId2={0}  {1}", openEquipStruct.entryId2,equipData.lstEntrysID.Contains(openEquipStruct.entryId2));
                        // Debug.LogFormat("openEquipStruct.entryId3={0}  {1}", openEquipStruct.entryId3,equipData.lstEntrysID.Contains(openEquipStruct.entryId3));
                        // Debug.LogFormat("openEquipStruct.entryId4={0}  {1}", openEquipStruct.entryId4,equipData.lstEntrysID.Contains(openEquipStruct.entryId4));
                        if(equipData.quality < openEquipStruct.equipQuality)
                        {
                            isStopAutopack = false;
                        }
                        
                        if (openEquipStruct.isEntry1)
                        {
                            if (openEquipStruct.entryId1 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId1))
                            {
                                isEntry1 = false;
                            }

                            if (isEntry1)
                            {
                                if(openEquipStruct.entryId2 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId2))
                                {
                                    isEntry1 = false;
                                }
                            }
       
                        }

                        if (openEquipStruct.isEntry2)
                        {
                            if (openEquipStruct.entryId3 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId3))
                            {
                                isEntry2 = false;
                            }

                            if (isEntry2)
                            {
                                if(openEquipStruct.entryId4 > 0 && !equipData.lstEntrysID.Contains(openEquipStruct.entryId4))
                                {
                                    isEntry2 = false;
                                }
                            }
                            
                        }

                        // Debug.LogFormat("{0}   {1}", (isStopAutopack && isEntry1), (isStopAutopack && isEntry2));
                        if (isStopAutopack && isEntry1 && isEntry2)
                        {
                            EquipManager.Instance.HasNewEquipToStopAutopack = true;
                            // UIManager.Instance.ShowUIPanel("Equip", guid, isNew);
                            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 11, deltaTime,  () =>
                            {
                                UIManager.Instance.ShowUIPanel("Equip", guid, isNew, type);
                            });
                        }
                        else
                        {
                            GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 8, 0.5f, () =>
                            {
                                if (!EquipManager.Instance.HasNewEquipToStopAutopack)
                                {
                                    this.lobbyMain.panel.tempEquipIcon.visible = false;
                                    EquipManager.Instance.DecomposeEquip(guid);  
                                }

                                // UIManager.Instance.ToastByKey(StringDefine.STRING_USE_DECOM_SUCC);
                            });
                        }
                 

                    }
                    else
                    {
                        // UIManager.Instance.ShowUIPanel("Equip", guid, isNew);
                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 11, deltaTime, () =>
                        {
                            UIManager.Instance.ShowUIPanel("Equip", guid, isNew, type);
                        });
                    }
                });
            });
        }
    }

    private void OnPartReplaceSucc()
    {
        if (IsShow() && IsOnStage())
        {
            UpdatePartAll();
        }
    }

    private void UpdateBoxInfo()
    {
        this.lobbyMain.panel.comBox.btnBoxLv.txtBoxLv.text = DataManager.Instance.GetTreasureData().id.ToString();
        this.lobbyMain.panel.comBox.btnBoxKey.txtKeyValue.text = DataManager.Instance.GetMagicKeys().ToString();
    }

    private void UpdatePartAll(ulong equipId = 0)
    {
        for (int i = (int) EN_EQUIP_PARTS.Weapon; i < 5; i++)
        {
            int index = i - 1;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
            UpdatePartData(index, partEquipData);
        }

        // for (int i = (int) EN_Other_PARTS.Weapon; i < (int) EN_Other_PARTS.Count; i++)
        // {
        //     int index = i - 1;
        //     GObject gObject = (GButton) this.lobbyMain.panel.listEquip2.GetChildAt(index);
        //     gObject.onClick.Set(() =>
        //     {
        //         UIManager.Instance.ToastByKey(206);
        //     });
        // }
        
        if (equipId > 0)
        {
            int sourceId = EquipManager.Instance.GetEquipById(equipId).id;
            ConfigItemTypeUnit config = ConfigUtils.GetConfigItemTypeUnitById(sourceId);
            this.lobbyMain.panel.tempEquipIcon.visible = true;
            this.lobbyMain.panel.tempEquipIcon.tempEquipIcon.icon = UIResource.GetItemUrl(config.Icon);
        }

    }

    private void UpdatePartData(int index, EquipData partEquipData)
    {
        if (index < this.lobbyMain.panel.listEquip1.numChildren)
        {
            UI_BtnEquip gObject = (UI_BtnEquip) this.lobbyMain.panel.listEquip1.GetChildAt(index);
            if (null != gObject)
            {
                gObject.onClick.Clear();
                gObject.qualityIcon.visible = true;
                if (null != partEquipData && !partEquipData.IsNull())
                {
                    var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                    if (null != config)
                    {
                        gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                        gObject.ctrQuality.selectedIndex = partEquipData.quality - 1;
                        gObject.qualityIcon.visible = true;
                        gObject.hasCnt.selectedIndex = 1;
                        gObject.lvLb.SetVar("value",partEquipData.lv.ToString()).FlushVars();
                    }

                    gObject.onClick.Set(() =>
                    {
                        UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    });
                    // if (partEquipData.quality < 4)
                    // {
                    //     gObject.equipSpineEff.visible = false;
                    // }
                    // else
                    // {
                    //     gObject.equipSpineEff.visible = true;
                    //     gObject.equipSpineEff.spineAnimation.AnimationState.AddAnimation(0, "idle_Q"+partEquipData.quality.ToString(), true, 0);
                    // }

                }
                else
                {
                    gObject.icon = "";
                    // gObject.equipSpineEff.visible = false;
                    gObject.ctrbg.selectedIndex = index + 8;
                    gObject.qualityIcon.visible = false;
                    gObject.hasCnt.selectedIndex = 0;
                }
            }
        }
    }

    #endregion

    #region 传承装备
    /// <summary>
    /// 传承装备信息
    /// </summary>
    private void UpdateInheritInfo()
    {
        for (int i = 5; i < 9; i++)
        {
            int index = i - 5;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
            SetInheritData(index, partEquipData, this.lobbyMain.panel.inheritList);
        }
    }

    /// <summary>
    /// 传承装备数据设置
    /// </summary>
    /// <param name="index">装备列表位置索引</param>
    /// <param name="partEquipData">装备部位数据</param>
    /// <param name="equipList">装备列表数据</param>
    private void SetInheritData(int index, EquipData partEquipData,GList equipList)
    {
        //UI_BtnEquip gObject = (UI_BtnEquip) this.lobbyMain.panel.inheritList.GetChildAt(index);
        UI_BtnEquip gObject = (UI_BtnEquip) equipList.GetChildAt(index);
        if (null != gObject)
        {
            gObject.onClick.Clear();
            gObject.qualityIcon.visible = true;
            if (null != partEquipData && !partEquipData.IsNull())
            {
                var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                if (null != config)
                {
                    gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                    gObject.ctrQuality.selectedIndex = partEquipData.quality - 1;
                    gObject.qualityIcon.visible = true;
                    gObject.hasCnt.selectedIndex = 1;
                    gObject.lvLb.SetVar("value",partEquipData.lv.ToString()).FlushVars();
                }

                gObject.onClick.Set(() =>
                {
                    // UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    //                                 选中装备的guid    是否新装备 新装备只显示新装备  旧装备 会显示两个装备的信息
                    EquipManager.Instance.isInheritEquip = true;
                    UIManager.Instance.ShowUIPanel("Equip",partEquipData.guid, true);//partEquipData);
                });
            }
            else
            {
                gObject.icon = "";
                // gObject.equipSpineEff.visible = false;
                gObject.ctrbg.selectedIndex = index;
                gObject.qualityIcon.visible = false;
                gObject.hasCnt.selectedIndex = 0;
            }
        }
    }
    
    /// <summary>
    /// 点击传承背包按钮  卸下通知也调用这个
    /// </summary>
    private void OnClickInheritBagButton()
    {
        var inheritMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Inherit);
        if (!inheritMap.Item1)
        {
            UIManager.Instance.Toast(inheritMap.Item2);
            return;
        }
        
        this.lobbyMain.panel.packageCtrl.selectedIndex = 1;
        // for (int i = 5; i < 9; i++)
        // {
        //     int index = i - 5;
        //     var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
        //     SetInheritData(index, partEquipData, this.lobbyMain.panel.inheritBagList);
        // }

        // 传承背包列表 刷新
        // GetLoreEquipList();
        // this.lobbyMain.panel.recycleBagList.numItems = equipBagList.Count;
        RefreshInheritBag();
    }
    
    // 传承背包列表 刷新
    private void RefreshInheritBag()
    {
        for (int i = 5; i < 9; i++)
        {
            int index = i - 5;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
            SetInheritData(index, partEquipData, this.lobbyMain.panel.inheritBagList);
        }
        
        GetLoreEquipList();
        
        this.lobbyMain.panel.recycleBagList.numItems = equipBagList.Count;

        ChkGuide();
    }

    /// <summary>
    /// 点击关闭传承背包
    /// </summary>
    private void OnClickInheritBagCloseButton()
    {
        this.lobbyMain.panel.packageCtrl.selectedIndex = 0;
        for (int i = (int)EN_EQUIP_PARTS.Cloak; i < 9; i++)
        {
            int index = i - (int)EN_EQUIP_PARTS.Cloak;
            var partEquipData = DataManager.Instance.FindEquip((EN_EQUIP_PARTS) i);
            SetInheritData(index, partEquipData, this.lobbyMain.panel.inheritList);
        }
    }
    
    /// <summary>
    /// 点击一键回收
    /// </summary>
    private void OnClickRecycleButton()
    {
        UIManager.Instance.ShowUIPanel("InheritRecycle");
    }
    
    //穿戴传承装备列表
    // private void InheritListRender(int index, GObject item)
    // {
    // }
    // //背包穿戴传承装备列表
    // private void InheritBagListRender(int index, GObject item)
    // {
    // }
    
    //背包传承装备列表
    private void InheritBagItemListRender(int index, GObject item)
    {
        UI_BtnEquip gObject = (UI_BtnEquip)item;
        if (null != gObject)
        {
            gObject.onClick.Clear();
            gObject.qualityIcon.visible = true;
            var partEquipData = equipBagList[index];
            if (null != partEquipData && !partEquipData.IsNull())
            {
                var config = ConfigUtils.GetConfigItemTypeUnitById(partEquipData.GetSourceId());
                if (null != config)
                {
                    gObject.icon = UIResource.GetItemUrl(config.Icon); //UIResource.GetPartURL(config.Parts); //config.Icon;
                    gObject.ctrQuality.selectedIndex = partEquipData.quality - 1;
                    gObject.qualityIcon.visible = true;
                    gObject.hasCnt.selectedIndex = 1;
                    gObject.lvLb.SetVar("value",partEquipData.lv.ToString()).FlushVars();
                }

                gObject.onClick.Set(() =>
                {
                    // UIManager.Instance.ShowUIPanel("PopupEquipAttribute", partEquipData);
                    //                                         选中装备的guid     是否新装备 新装备只显示新装备  旧装备 会显示两个装备的信息
                    EquipManager.Instance.isInheritEquip = true;
                    UIManager.Instance.ShowUIPanel("Equip",partEquipData.guid, false);
                });
            }
            else
            {
                gObject.icon = "";
                gObject.ctrQuality.selectedIndex = 0;
                gObject.hasCnt.selectedIndex = 0;
            }
        }
    }

    //装备时背包装备列表item显示光效
    private void UpdateInheritBagListItem(ulong guid)
    {
        //传承装备面板  
        if (RoleManager.Instance.GetEquipSlotInfoByGuid(guid) == null) return;
        EquipSlotInfo slotInfo = RoleManager.Instance.GetEquipSlotInfoByGuid(guid);
        if (slotInfo == null) return;

        int tempIndex = 0;
        for (int i = 5; i < 9; i++)
        {
            if (slotInfo.SlotType == i)
            {
                tempIndex = i - 5;
                break;  
            }
        }
        
        // UI_BtnEquip item = this.lobbyMain.panel.inheritList.GetChildAt(0) as UI_BtnEquip;
        UI_BtnEquip item = this.lobbyMain.panel.inheritBagList.GetChildAt(tempIndex) as UI_BtnEquip;
        item.equipSpineEff.visible = true;
        item.equipSpineEff.frame = 0;
        Utils.PlaySpineAnim(item.equipSpineEff, "show_Q4", false, () =>
        {
            item.equipSpineEff.visible = false;
        });
    }

    
    private List<EquipData> equipBagList = new List<EquipData>();
    /// <summary>
    /// 获取背包传承装备列表
    /// </summary>
    private void GetLoreEquipList()
    {
        equipBagList.Clear();
        foreach (var equipItem in EquipManager.Instance.GetNoWearLoreEquip())
        {
            // 未穿戴的传承装备都显示
            equipBagList.Add(equipItem);
        }

        if (equipBagList == null)
            return;
        
        equipBagList.Sort((item1,item2) =>
        {
            if(item1 == null || item2 == null) 
                return 0;
            if (item1.quality < item2.quality)
            {
                return 1;
            }else if (item1.quality == item2.quality)
            {
                if (item1.lv > item2.lv)
                {
                    return 1;
                }else if (item1.lv == item2.lv)
                {
                    return item1.id > item2.id ? 1 : -1;
                }
            }
            return -1;
        });
    }

    
    private Vector2 equipBagPos = Vector2.zero;
    /// <summary>
    /// 获得传承装备飞的动画
    /// </summary>
    /// <param name="itemId"></param>
    private void GainNewLoreEquip(int itemId)
    {
        if (!UIManager.Instance.IsTopController("Lobby")) return;
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(itemId);
        
        if (equipBagPos == Vector2.zero)
            equipBagPos = new Vector2((Screen.width / 2) + 200, (Screen.height / 2) - 200);
        
        UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase();
        itemsGain2.ApplyItemSourceToDestinationEx(this.lobbyMain.panel.equipFlyPos.asCom, UIResource.GetItemUrl(itemTypeUnit.Icon));
        itemsGain2.StartItemFly(equipBagPos, 1);
    }
    #endregion
    
    #region 宝箱升级

    private void OnTreasureChesLevelUpRes(int msgCode, string msgData)
    {
        if (IsShow() && IsOnStage())
        {
            JsonObject res = (JsonObject) SimpleJson.DeserializeObject(msgData);
            if (null == res)
                return;

            switch (msgCode)
            {
                case MsgCode.SUCCESS:
                {
                    // var comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.panel.userInfo).comCurrency;
                    var comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.userInfo).comCurrency;//修改主界面顶部栏位、测试
                    comCurrency.btnGold.txtValue.text = StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().gold);
                    this.lobbyMain.panel.comBox.btnBoxLv.txtBoxLv.text =
                        DataManager.Instance.GetTreasureData().id.ToString();
                }
                    break;
                case MsgCode.FAIL:
                {
                    int errCode = res.GetInt("errCode");
                    switch (errCode)
                    {
                        case 1000:
                            UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_LIMITE_LEVEL_ERROR);
                            break;
                        case 1001:
                            UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_CD_ERROR);
                            break;
                        case 1002:
                            UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_CURRENCY_ERROR);
                            break;
                        case 1003:
                            UIManager.Instance.ToastByKey(StringDefine.STRING_UPGRADE_LEVEL_ERROR);
                            break;
                    }
                }
                    break;
            }
        }
    }

    #endregion

    #region 关卡信息更新

    private void OnUpdateStageInfo(List<ConfigStageUnit> stageUnits, int guankaIndex)
    {
        // this.lobbyMain.panel.battleRoot.stageName.text = stageUnits[guankaIndex].Name;
        // this.lobbyMain.panel.battleRoot.stageName.text = ConfigUtils.GetTextById(stageUnits[guankaIndex].Name,stageUnits[guankaIndex].NameParam);
        this.lobbyMain.panel.battleRoot.stageName.text = string.Format(ConfigUtils.GetTextById(stageUnits[guankaIndex].Name), stageUnits[guankaIndex].Chapter, stageUnits[guankaIndex].LevelId % 100);
        this.mCurGuankaIndex = guankaIndex;
        this.mStageMonsterWave = stageUnits.Count;
        ((UI_MonsterGroupBar) this.lobbyMain.panel.battleRoot.monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
        ((UI_MonsterGroupBar) this.lobbyMain.panel.battleRoot.monsterGroup).monsterGroupList.ResizeToFit();
        ((UI_MonsterGroupBar)this.lobbyMain.panel.battleRoot.monsterGroup).ctrlStatus.selectedIndex = guankaIndex;
    }

    private void OnUpdateMonsterWaveInfo(int guankaIndex)
    {
        this.mCurGuankaIndex = guankaIndex;
        ((UI_MonsterGroupBar) this.lobbyMain.panel.battleRoot.monsterGroup).monsterGroupList.numItems = this.mStageMonsterWave;
        ((UI_MonsterGroupBar) this.lobbyMain.panel.battleRoot.monsterGroup).monsterGroupList.ResizeToFit();
        ((UI_MonsterGroupBar)this.lobbyMain.panel.battleRoot.monsterGroup).ctrlStatus.selectedIndex = guankaIndex;
    }

    #endregion

    #region 关卡信息

    /// <summary>
    /// 通关stage的消息   TODO 怪死亡 胜利界面
    /// </summary>
    /// <param name="pos"></param>
    private void OnStageCompleteInfo(Vector2 pos)
    {
        //通关stage的消息
        var builderE = Stage_End_CS.CreateBuilder();
        builderE.PosX = pos.x;
        builderE.PosY = pos.y;
        Stage_End_CS endCs = builderE.Build();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_Stage_End_CS, endCs);
        // MapObjectManager.Instance.StageComplete = true;
        // GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 9,5f, () =>
        // {
        //     if(MapObjectManager.Instance.StageComplete)
        //         MapObjectManager.Instance.StageComplete = false;
        // });

        this.lobbyMain.panel.battleRoot.btnStage.visible = true;
        if (!UIManager.Instance.IsTopController("Lobby"))
        {
            return;
        }

        UIManager.Instance.CloseUIPanel("FightWin");//防止多个胜利界面
        bool isShowWinUI = IsShowFightWin();
        if (isShowWinUI)
        {
            if (GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_MPBuy1) && GuideManager.Instance.GuideIsComplete(GuideID.Click_EquipSkillItem))
            {
                UIManager.Instance.ShowUIPanel("FightWin");
            }
        }
        
        // UIManager.Instance.CloseUIPanel("FightWin");//防止多个胜利界面
        // UIManager.Instance.ShowUIPanel("FightWin");
        // if(!VillageInfoManager.Instance.IsInVillageHome)
        //     GameManager.Instance.SoundManager.PlayEffectWithoutLoop(9);
        // GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 10,2.5f,() =>
        // {
        //     UIManager.Instance.CloseUIPanel("FightWin");
        // });

        UpdateShopFKZNReddot();
    }

    /// <summary>
    /// 是否展示胜利界面-进入关卡循环时不展示界面
    /// </summary>
    /// <returns></returns>
    private bool IsShowFightWin()
    {
        if (MapChapterManager.Instance.theLastChallengeStageId == 0)
        {
            MapChapterManager.Instance.theLastChallengeStageId = DataManager.Instance.GetRoleData().stageId;
            return true;
        }
        
        if (MapChapterManager.Instance.theLastChallengeStageId == DataManager.Instance.GetRoleData().stageId)
        {
            return false;
        }
        
        if (MapChapterManager.Instance.theLastChallengeStageId != DataManager.Instance.GetRoleData().stageId)
        {
            MapChapterManager.Instance.theLastChallengeStageId = DataManager.Instance.GetRoleData().stageId;
            return true;
        }
        
        return false;
    }

    public void HideNextStageButton()
    {
        this.lobbyMain.panel.battleRoot.btnStage.visible = false;
    }
    
    private void OnStageCompleteReward(ConfigItemTypeUnit itemCfg, int num)
    {
        // EquipManager.Instance.AddItem(itemCfg.Id, num);
        UpdateUIInfo();
    }

    private void OnStageFightLose(bool isShowTopped)
    {
        this.lobbyMain.panel.battleRoot.monsterGroup.visible = !isShowTopped;
        //this.lobbyMain.panel.battleRoot.barBossTime.visible = false;
        this.lobbyMain.panel.battleRoot.copyBossTime.visible = false;
        this.lobbyMain.panel.battleRoot.btnToppedAni.visible = isShowTopped;
        this.lobbyMain.panel.battleRoot.zctz.visible = isShowTopped;
        if (isShowTopped)
        {
            HideNextStageButton();
        }

        ShowBtnToppedAniEff(isShowTopped);
    }

    // 挑战按钮收缩效果
    private void ShowBtnToppedAniEff(bool isPlay)
    {
        if (isPlay)
        {
            this.lobbyMain.panel.battleRoot.btnToppedAni.t0.Play(9999999,0,null);
        }
        else
        {
            this.lobbyMain.panel.battleRoot.btnToppedAni.t0.Stop();
        }
        
    }

    #endregion

    #region 装备分解

    private void OnEquipPartDecomposeSucc()
    {
        this.lobbyMain.panel.tempEquipIcon.visible = false;
        this.lobbyMain.panel.equipSplitEff.visible = true;
        this.lobbyMain.panel.equipSplitEff.splitSpine.spineAnimation.state.SetAnimation(0, "idle", false);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,5,1.0f, () => { this.lobbyMain.panel.equipSplitEff.visible = false; });
    }

    #endregion

    private void OnClickTempEquipIcon()
    {
        if (EquipManager.Instance.curNoEquipGuid != 0)
        {
            EquipManager.Instance.HasNewEquipToStopAutopack = true;
            UIManager.Instance.ShowUIPanel("Equip", EquipManager.Instance.curNoEquipGuid, EquipManager.Instance.isNewEquip);
        }
    }

    private void OnEquipSingleWear()
    {
        this.UpdatePartAll();
        this.lobbyMain.panel.tempEquipIcon.visible = false;
    }

    private void OnClickToOpenAllHeroAttr()
    {
        // UIManager.Instance.ShowUIPanel("AllHeroAttrPanel",DataManager.Instance.GetRoleData().FightAttrVo);
        UIManager.Instance.ShowUIPanel("AllHeroAttrPanel",ObjType.HERO, DataManager.Instance.GetRoleData().FightAttrVo);
    }

    public void PlayCommonTimeSpine(bool isLose)
    {
        this.lobbyMain.panel.battleRoot.commonTimeSp.visible = true;
        if (!isLose)
        {
            Utils.ShowUIPrefab(this.lobbyMain.panel.battleRoot.commonTimeSp, "CommonEx_time", 100f);
        }
        else
        {
            for (int i = 0; i < this.lobbyMain.panel.battleRoot.comSkills.listSkills.numChildren; i++)
            {
                UI_BtnSkill btnSkill = (UI_BtnSkill) this.lobbyMain.panel.battleRoot.comSkills.listSkills.GetChildAt(i);
                if (null != btnSkill)
                {
                    //GTween.Kill(btnSkill);
                    btnSkill.touchable = true;
                    // btnSkill.mask.fillAmount = 0;
                }
            }
            GTween.Kill(this);
            this.lobbyMain.panel.battleRoot.touchPanel.visible = true;
            Utils.ShowUIPrefab(this.lobbyMain.panel.battleRoot.commonTimeSp, "UI_BattleLoseEff", 100f);
        }
    }

    public void StopCommonTimeSpine()
    {
        this.lobbyMain.panel.battleRoot.touchPanel.visible = false;
        this.lobbyMain.panel.battleRoot.commonTimeSp.visible = false;
        Utils.HideUIPrefab(this.lobbyMain.panel.battleRoot.commonTimeSp);
    }

    private void OnClickPmBtn()
    {
        UIManager.Instance.ShowUIPanel("PMWindow");
        
        // GetPetRewardView.GetPetParam getPetParam = new GetPetRewardView.GetPetParam();
        // getPetParam.Id = 20031;
        // getPetParam.IsRole = true;
        // getPetParam.CloseCallback = null;
        // UIManager.Instance.ShowUIPanel("GetPetReward", getPetParam);
    }

    private void OnClickTestPVP()
    {
        UIManager.Instance.ShowUIPanel("AthleticsMain");
    }

    // 检查快捷键是否被按下，并打开PMWindow，支持F5打开和Ctrl+F5打开
    private void CheckHotkey()
    {
        // 如果需要Ctrl键组合，则检查Ctrl是否被按下
        if (requiresControl && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            if (Input.GetKeyDown(hotkeyKey))
            {
                // 切换PMWindow的显示状态
                isPMWindowOpen = !isPMWindowOpen;
                // UIManager.Instance.ShowUIPanel("PMWindow");
                if (isPMWindowOpen)
                {
                    UIManager.Instance.ShowUIPanel("PMWindow");
                }
                else
                {
                    UIManager.Instance.CloseUIPanel("PMWindow");
                }
            }
        }
        else if (!requiresControl && Input.GetKeyDown(hotkeyKey))
        {
            // 切换PMWindow的显示状态
            isPMWindowOpen = !isPMWindowOpen;
            // UIManager.Instance.ShowUIPanel("PMWindow");
            if (isPMWindowOpen)
            {
                UIManager.Instance.ShowUIPanel("PMWindow");
            }
            else
            {
                UIManager.Instance.CloseUIPanel("PMWindow");
            }
        }
    }

    private void UpdateBottomRedDot()
    {
        if(!IsShow() ||  !IsOnStage()) return;
        this.lobbyMain.panel.listBottom.EnsureBoundsCorrect();
        
        // 装备
        var zhuzhaoMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
        if (zhuzhaoMap.Item1)
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(1)).lockCtrl.selectedIndex = 0;
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(1)).hasRed.selectedIndex = TreasureChesManager.Instance.TreasureBoxRedPoint() || EquipManager.Instance.LoreRedPoint() || EquipManager.Instance.ArtifactRedPointHandler() ? 1 : 0;
        }
        else
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(1)).lockCtrl.selectedIndex = 1;
        }
        
        // 宠物
        var summonPetMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPet);
        if (summonPetMap.Item1)
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).lockCtrl.selectedIndex = 0;
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).hasRed.selectedIndex = PetInfoManager.Instance.PetRedPointHandle() ? 1 : 0;
        }
        else
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).lockCtrl.selectedIndex = 1;
        }
        
        // 秘典
        // var heroClassicMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroClassic);
        // if (heroClassicMap.Item1)
        // {
        //     ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).lockCtrl.selectedIndex = 0;
        //     // 秘典红点
        //     bool classicHasRed = ClassicHasRed();
        //     ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).hasRed.selectedIndex = classicHasRed ? 1 : 0;
        // }
        // else
        // {
        //     ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(3)).lockCtrl.selectedIndex = 1;
        // }

        //召唤
        var summonMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonSkill);
        if (summonMap.Item1)
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(4)).lockCtrl.selectedIndex = 0;
            bool summonHasRed = ActivityManager.Instance.SummonSpecialRedDot();
            bool summonPetAndSkillRed = ShopInfoManager.Instance.SummonPetAndSkillRedPoint();
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(4)).hasRed.selectedIndex = summonHasRed || summonPetAndSkillRed ? 1 : 0;
        }
        else
        {
            ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(4)).lockCtrl.selectedIndex = 1;
        }
        
        var autoPackMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.AutoPack);
        this.lobbyMain.panel.btnSetupAuto.lockCtrl.selectedIndex = autoPackMap.Item1 ? 0 : 1;

        bool hasRed = SkillInfoManager.Instance.IsCanUpLevel() || SkillInfoManager.Instance.IsCanUpLoadSkill() || HeroInfoManager.Instance.IsShowRedDot();
        ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(0)).hasRed.selectedIndex = hasRed ? 1 : 0;
    }
    
    // 领取任务奖励
    private void OnClickToGetTaskReward()
    {
        if (this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 1) 
        {
            var builder = ClaimTaskAward_CS.CreateBuilder();
            ClaimTaskAward_CS taskAwardCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimTaskAward_CS, taskAwardCs);
            
            int diamond = Random.Range(2, 9);
            TaskRewardGetShow(diamond);
        }
        else
        {
            // UIManager.Instance.ToastByKey(10173);
            JumpManager.Instance.ClickMainTaskJump();
        }
        
        if(GuideManager.Instance.IsShowGuiding)
            GuideManager.Instance.HideGuide();

        ChkGuide();
    }

    private Vector2 _pos = Vector2.zero;
    // 任务奖励获得表现效果：需要进行有无金币或钻石的判断，有的话才表现出飘的效果
    public void TaskRewardGetShow(double diamond)
    {
        // if(_pos == Vector2.zero)
        //     _pos = this.lobbyMain.panel.comPandaInfo.LocalToGlobal(Vector2.zero);
        
        if (_pos == Vector2.zero)
        {
            GComponent mainComPanda = this.lobbyMain.panel.comPandaInfo;
            Vector2 centerLocal = new Vector2(mainComPanda.width / 2, mainComPanda.height / 2);
            _pos = this.lobbyMain.panel.comPandaInfo.LocalToGlobal(centerLocal);
        }
        
        // 创建金币动画对象
        // UIItemsGain itemsGain1 = new UIItemsGain();

        // 获取金币和钻石显示控件
        UI_ComCurrency comCurrency = (UI_ComCurrency) ((UI_ComUserInfo)this.lobbyMain.userInfo).comCurrency;
    
        // 应用金币来源到目标
        // itemsGain1.ApplyItemSourceToDestination(comCurrency.btnGold.asCom, 0);
    
        // 开始金币飘动动画
        // itemsGain1.StartItemFly(_pos, gold);

        // 钻石 
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_Diamond);
        UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase();//new UIItemsGain();
        itemsGain2.ApplyItemSourceToDestination(comCurrency.btnDia.asCom, itemTypeUnit.Icon);
        itemsGain2.StartItemFly(_pos, diamond);

        // 其他物品
        // UIItemsGain itemsGain3 = new UIItemsGain();
        // itemsGain3.ApplyItemSourceToDestination(((UI_ComUserInfo)this.lobbyMain.panel.userInfo).headIcon, 3);
        // itemsGain3.StartItemFly(_pos, 1);
    }

    private void UpdateFuncPreviewInfo()
    {
        var chatMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Chat);
        this.lobbyMain.panel.battleRoot.comMessage.visible = chatMap.Item1;
        
        var firstChargeMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.FirstCharge);
        var firstPayReward = ActivityManager.Instance.HasAllGetFirstPayReward();
        this.lobbyMain.panel.firstChargeBtn.visible = false;//firstChargeMap.Item1 && (firstPayReward && DataManager.Instance.GetRoleData().IsFirstCharge || !DataManager.Instance.GetRoleData().IsFirstCharge);

        // var shopMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.OnlineReward);
        // this.lobbyMain.panel.onlineBtn.visible = onlineMap.Item1;
        
        var txzMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SuperTXZ);
        if (txzMap.Item1)
        {
            ActivityManager.Instance.GetPassTaskInfoCS();
            // int totalSecond = (int)(Utils.GetSundayEndTime() - (long) ServerTimeManager.Instance.CurServerTime);
            // this.lobbyMain.panel.superTXZBtn.text = StringUtils.GetTimeString2(totalSecond);
        }
        
        //传承
        var inheritMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Inherit);
        if (inheritMap.Item1)
        {
            this.lobbyMain.panel.packageLockIcon.visible = false;
            this.lobbyMain.panel.packageBtn.grayed = false;
        }
        else
        {
            this.lobbyMain.panel.packageLockIcon.visible = true;
            this.lobbyMain.panel.packageBtn.grayed = true;
        }
        
        //神器
        var artifactMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Artifact);
        if (artifactMap.Item1)
        {
            this.lobbyMain.panel.lvUpLockIcon.visible = false;
            this.lobbyMain.panel.upLvBtn.grayed = false;
        }
        else
        {
            this.lobbyMain.panel.lvUpLockIcon.visible = true;
            this.lobbyMain.panel.upLvBtn.grayed = true;
        }
        
        UpdateSevenDayReddot();
        UpdateBottomRedDot();
        UpdateGuideInfo();
        UpdateLoginGiftBtn();
        UpdateAchievementRedPoint();//成就红点
        
    }

    public void ShowGenEquipFinger()
    {
        this.lobbyMain.panel.comBox.finger.visible = true;
        this.lobbyMain.panel.comBox.t0.Play(4, 0, null);
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,6,3.0f, () =>
        {
            this.lobbyMain.panel.comBox.finger.visible = false;
            this.lobbyMain.panel.comBox.t0.Stop();
        });
    }

    public void ShowClickAttrFinger()
    {
        if (this.lobbyMain.panel.zzcCtrl.selectedIndex == 0)
        {
            (this.lobbyMain.panel.listBottom.GetChildAt(3) as UI_BtnBottom)?.FireClick(true,true);
        }
        this.lobbyMain.panel.clickAttr.visible = true;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI,6,2.0f, () =>
        {
            this.lobbyMain.panel.clickAttr.visible = false;
        });
    }

    private bool onlineRwFalg = false;
    private void UpdateOnlineReward(int onlineTime)
    {
        // this.lobbyMain.panel.onlineBtn.timeLb.SetVar("value", StringUtils.GetTimeString((int)onlineTime)).FlushVars();
        if (onlineTime > int.Parse(_commonUnit30.Param1))
        {
            // this.lobbyMain.panel.onlineBtn.openCtrl.selectedIndex = 1;
            onlineRwFalg = true;
        }
        else
        {
            // this.lobbyMain.panel.onlineBtn.openCtrl.selectedIndex = 0;
            onlineRwFalg = false;
        }
        this.lobbyMain.panel.leftFunctionList.numItems = _unlockLeftFuncs.Count;
    }

    private void UpdateMailRedDot()
    {
        UI_BtnFucntionIcon mailFunItem = this.lobbyMain.panel.mailBtn;
        if(mailFunItem != null)
            mailFunItem.redDot.visible = MailManager.Instance.HasNotReadMail();
    }
    
    private void UpdateDailyRedDot()
    {
        var dailyMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.DailyTask);
        this.lobbyMain.panel.dailyBtn.visible = dailyMap.Item1;
        this.lobbyMain.panel.dailyBtn.redDot.visible = TaskInfoManager.Instance.IsCanGetDailyReward();

    }

    private void UpdateTreasureRedDot(bool isLevelUp)
    {
        var zhuzhaiMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Zhuzhao);
        if (!zhuzhaiMap.Item1)
        {
            return;
        }

        bool canUpLv = false;
        int currLv = DataManager.Instance.GetTreasureData().id;
        int maxLv = TreasureChesManager.Instance.GetTreasureMaxLv();
        if (currLv < maxLv)
        {
            if (TreasureChesManager.Instance.TreasureBoxOpenStatus != TreasureBoxOpenStatus.LevelUp)
            {
                var treasureChestUnit = ConfigUtils.GetTreasureChestUnitById(DataManager.Instance.GetTreasureData().id);
                if (treasureChestUnit != null)
                {

                    canUpLv = DataManager.Instance.GetRoleData().gold >= double.Parse(treasureChestUnit.GoldCoins);
                }

            }
        }
        int totalSecond = (int)(DataManager.Instance.GetTreasureData().lastTargetTime - ServerTimeManager.Instance.CurServerTime);
        bool isPro = totalSecond > 0;
        var data = TreasureChesManager.Instance.CheckTreasureChes();
        // this.lobbyMain.panel.comBox.btnBoxLv.redDot.visible = (canUpLv && data.Item1 == 0 && !isPro);
        this.lobbyMain.panel.comBox.btnBoxLvUp.redDot.visible = (canUpLv && data.Item1 == 0 && !isPro);
        // ((UI_BtnBottom) this.lobbyMain.panel.listBottom.GetChildAt(4)).hasRed.selectedIndex = (canUpLv && data.Item1 == 0 && !isPro) ? 1 : 0;
    }

    public bool IsClickBottom()
    {
        return this.lobbyMain.panel.listBottom.selectedIndex != -1;
    }
    private void PlayLevelUpEff()
    {
        Utils.ShowUIPrefab(this.lobbyMain.lvPos, "UI_Player_LvUp");
    }
    

    private void OnClickSevenBtn()
    {
        UIManager.Instance.ShowUIPanel("SevenDay");
    }

    private void OnClickShopBtn()
    {
        UIManager.Instance.ShowUIPanel("ShopMain");
    }

    private void OnClickAchieveBtn()
    {
        AchievementManager.Instance.SendToGetAchievement();
        // UIManager.Instance.ShowUIPanel("Achievement");
    }

    private void OnClickCopyBtn()
    {
        UIManager.Instance.ShowUIPanel("DungeonStage",null);
    }

    private void UpdateAchievementRedPoint()
    {
        this.lobbyMain.panel.achieveBtn.redDot.visible = false;
        List<AchievementInfo> _curInfo = new List<AchievementInfo>();
        _curInfo = AchievementManager.Instance.GetAchievementList();
        foreach (var info in _curInfo)
        {
            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
            {
                this.lobbyMain.panel.achieveBtn.redDot.visible = true;
                break;
            }
        }
    }

    private bool UpdateAchievementRedPoint2()
    {
        // 登录时，未请求成就数据。根据服务器发下的红点信息，判断成就系统是否需要展示红点
        RedPointInfo redPointInfo = ReddotSysManager.Instance.GetRedPointByType(eRedPointType.eRedPointType_AchievementNotClaim);
        if (redPointInfo != null && !redPointInfo.IsRead)
        {
            return true;
        }
        
        List<AchievementInfo> _curInfo = new List<AchievementInfo>();
        _curInfo = AchievementManager.Instance.GetAchievementList();
        foreach (var info in _curInfo)
        {
            if (info.Status == (int)eAchievementStatusType.eAchievementStatusType_FinNotClaim)
            {
                return true;
            }
        }
        return false;
    }

    private void OnClickLoginGiftBtn()
    {
        UIManager.Instance.ShowUIPanel("LoginGift");
    }

    private void UpdateLoginGiftBtn()
    {
        var loginGiftMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.LoginGift);
        if (loginGiftMap.Item1)
        {
            int loginPackId = ActivityManager.Instance.LoginGiftPackID;
            if (loginPackId > 0)
            {
                ConfigLoginGiftUnit loginGiftUnit = ConfigUtils.GetLoginGiftById(loginPackId);
                ulong endTime = (ulong)(ServerTimeManager.Instance.GetNextToZeroServerTime() - 86400f + loginGiftUnit.Time * 60f);
                int totalSecond = (int) (endTime - ServerTimeManager.Instance.CurServerTime);
                this.lobbyMain.panel.loginGiftBtn.visible = false; //loginPackId > 0 && totalSecond > 0;
            }
            else
            {
                this.lobbyMain.panel.loginGiftBtn.visible = false;
            } 
        }
        else
        {
            this.lobbyMain.panel.loginGiftBtn.visible = false;
        }


    }
    
    private void UpdateSevenDayReddot()
    {
        var sevenMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SevenDay);
        this.lobbyMain.panel.sevenDayBtn.visible =sevenMap.Item1 &&  ActivityManager.Instance.HasSevenDay && !ActivityManager.Instance.HasGetAllSevenDayReward();
        this.lobbyMain.panel.sevenDayBtn.redDot.visible = ActivityManager.Instance.HasSevenDayReddot();
    }

    // private float _totalXPSkillCd;
    // private void UpdateXPSkill()
    // {
    //     MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
    //     if (heroObject != null)
    //     {
    //         ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(heroObject.HeroAttr.HeroUnit.SkillXp);
    //         _totalXPSkillCd = skillUnit.Cd * ConstDefine.CONFIG_PLACE_TIME;
    //         this.lobbyMain.panel.battleRoot.comSkills.xpskill.skillIcon.skillIcon.url = UIResource.GetSkillIcon(skillUnit.SkillIcon);
    //     }
    // }
    
    // 主动技能
    private void UpdateActiveSkill()
    {
        MapHeroObject heroObject = MapObjectManager.Instance.GetLocalHero();
        if (heroObject != null)
        {
            int activeSkillId = heroObject.HeroAttr.HeroUnit.ActiveSkill;
            ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(activeSkillId);
            // this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = UIResource.GetItemUrl(skillUnit.SkillIcon);
            this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = UIResource.GetHeroSkillIcon(skillUnit.SkillIcon);//暂时注释
            // this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = UIResource.GetHeroSkillIcon(2004602.ToString());//美术要求暂时写死
            this.lobbyMain.panel.battleRoot.comSkills.activeSkill.qualityIcon.visible = true;
            this.lobbyMain.panel.battleRoot.comSkills.activeSkill.qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
            if (skillUnit != null)
            {
                // 更新技能图标
                // this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = 
                //     UIResource.GetItemUrl(skillUnit.SkillIcon);
                this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = 
                UIResource.GetHeroSkillIcon(skillUnit.SkillIcon);//暂时注释
                // this.lobbyMain.panel.battleRoot.comSkills.activeSkill.skillIcon.skillIcon.url = 
                //     UIResource.GetHeroSkillIcon(2004602.ToString());//美术要求暂时写死
                this.lobbyMain.panel.battleRoot.comSkills.activeSkill.qualityIcon.visible = true;
                this.lobbyMain.panel.battleRoot.comSkills.activeSkill.qualityCtrl.selectedIndex = skillUnit.SkillQuality - 1;
            
                // 缓存基础CD
                _activeSkillBaseCD = skillUnit.Cd * ConstDefine.CONFIG_PLACE_EX;
            }
        }
    }

    /// <summary>
    /// 更新主场景属性列表信息
    /// </summary>
    private void UpdateShuxingInfo()
    {
        _isReceived = true;
        _shuxingInfos = RoleManager.Instance.GetShuxingInfos();
        foreach (var item in _shuxingInfos)
        {
            item.MaxTag = GetMaxTag(item);
        }
        _shuxingInfos.Sort((a, b) =>
        {
            int result = a.MaxTag > b.MaxTag ? 1 : (a.MaxTag == b.MaxTag ?  0 : -1);
            if (result == 0)
                result = a.Type > b.Type ? 1 : -1;
            return result;

        });
        this.lobbyMain.panel.shuxingList.numItems = _shuxingInfos.Count;
        this.lobbyMain.panel.shuxingList.EnsureBoundsCorrect();
    }

    private int GetMaxTag(ShuxingInfo info)
    {
        if (int.Parse(_common200004.Param1) == (int) info.Type)
        {
            if (info.Val < int.Parse(_common200004.Param2))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (info.Type == ShuxingType.HP)
            {
                if (info.Lv >= int.Parse(_common200009.Param1))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (info.Type == ShuxingType.ATK)
            {
                if (info.Lv >= int.Parse(_common200009.Param2))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }else if (info.Type == ShuxingType.Recovery)
            {
                if (info.Lv >= int.Parse(_common200009.Param3))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }else if (info.Type == ShuxingType.CriticalInjury)
            {
                if (info.Lv >= int.Parse(_common200009.Param4))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }

        return 0;

    }

    //属性列表 方法
    private void ShuxingListRender(int index, GObject item)
    {
        ShuxingInfo info = _shuxingInfos[index];
        FightAttrVo attrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        ((UI_LobbyHeroAttrAdd) item).attrCtrl.selectedIndex = (int)info.Type - 1;
        switch (info.Type)
        {
            case ShuxingType.ATK:
                ((UI_LobbyHeroAttrAdd) item).attrName.text = ConfigUtils.GetStringByKey(208);
                ((UI_LobbyHeroAttrAdd) item).attrVal.shuxingCtrl.selectedIndex = 0;
                ((UI_LobbyHeroAttrAdd) item).attrVal.attrLb.SetVar("value", StringUtils.FormatCurrency(attrVo.Atk)).FlushVars();
                break;
            case ShuxingType.HP:
                ((UI_LobbyHeroAttrAdd) item).attrName.text = ConfigUtils.GetStringByKey(207);
                ((UI_LobbyHeroAttrAdd) item).attrVal.shuxingCtrl.selectedIndex = 1;
                ((UI_LobbyHeroAttrAdd) item).attrVal.attrLb.SetVar("value", StringUtils.FormatCurrency(attrVo.HP)).FlushVars();
                break;
            case ShuxingType.CriticalStrike:
                ((UI_LobbyHeroAttrAdd) item).attrName.text = ConfigUtils.GetStringByKey(210);
                ((UI_LobbyHeroAttrAdd) item).attrVal.shuxingCtrl.selectedIndex = 2;
                ((UI_LobbyHeroAttrAdd) item).attrVal.attrLb.SetVar("value", StringUtils.FormatCurrency(double.Parse((attrVo.CriticalStrike*100f).ToString("f2")))).FlushVars();
                break;
            case ShuxingType.CriticalInjury:
                ((UI_LobbyHeroAttrAdd) item).attrName.text = ConfigUtils.GetStringByKey(211);
                ((UI_LobbyHeroAttrAdd) item).attrVal.shuxingCtrl.selectedIndex = 3;
                ((UI_LobbyHeroAttrAdd) item).attrVal.attrLb.SetVar("value", StringUtils.FormatCurrency(double.Parse((attrVo.CriticalInjury*100f).ToString("f2")))).FlushVars();
                break;
            case ShuxingType.Recovery:
                ((UI_LobbyHeroAttrAdd) item).attrName.text = ConfigUtils.GetStringByKey(209);
                ((UI_LobbyHeroAttrAdd) item).attrVal.shuxingCtrl.selectedIndex = 4;
                ((UI_LobbyHeroAttrAdd) item).attrVal.attrLb.SetVar("value", StringUtils.FormatCurrency(attrVo.Recovery)).FlushVars();
                break;
        }
        
        if (info.MaxTag == 1)
        {
            ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).heroBtn.enabled = false;
            ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).heroBtn.ctrl.selectedIndex = 1;
        }
        else
        {
            double cost = ConfigUtils.GetRingStrengthCost(info.Lv+1, info.Type);
            ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).heroBtn.costLb.text = StringUtils.FormatCurrency(cost);
            ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).heroBtn.grayed = DataManager.Instance.GetRoleData().gold < cost;
        }

        ((UI_LobbyHeroAttrAdd) item).lvLb.SetVar("lv", info.Lv.ToString()).FlushVars();
        ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).onTouchBegin.Add(OnTouchBeginItem);
        ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).onTouchEnd.Add(OnTouchEndItem);
        if(!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_BossDeadUpLevel) || !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon1))
            ((UI_HeroAttrAniBtn) ((UI_LobbyHeroAttrAdd) item).strengthBtn).onTouchEnd.Add(ChkGuide);
    }
    private Coroutine _delayCoroutine;
    private WaitForSeconds _wait = new WaitForSeconds(.1f);
    private bool _isTouchStart;
    private bool _isReceived;
    private float _longPressTimer = 0f;
    private void OnTouchBeginItem(EventContext context)
    {
        HeroInfoManager.Instance.IsOperating = true;
        _longPressTimer = 0;
        UI_HeroAttrAniBtn heroAttrBtn = (context.sender as UI_HeroAttrAniBtn);
        UI_LobbyHeroAttrAdd item = heroAttrBtn.parent as UI_LobbyHeroAttrAdd;
        int index = this.lobbyMain.panel.shuxingList.GetChildIndex(item);
        _isTouchStart = true;
        _delayCoroutine = GameManager.Instance.StartCoroutine(DoDelayActionCoroutine(index, heroAttrBtn, item)); 
    }

    private IEnumerator DoDelayActionCoroutine(int index, UI_HeroAttrAniBtn heroAttrBtn, UI_LobbyHeroAttrAdd item)
    {
        while (_isTouchStart)
        {
            ShuxingInfo info = _shuxingInfos[index];
            double cost = ConfigUtils.GetRingStrengthCost(info.Lv, info.Type);
            if (DataManager.Instance.GetRoleData().gold >= cost)
            {
                heroAttrBtn.t0.Play();
                GameManager.Instance.SoundManager.PlayEffect(4);
                item.showHandle.selectedIndex = 0;
                if(GuideManager.Instance.IsShowGuiding)
                    GuideManager.Instance.HideGuide();
                float clickNum = 1;
                if (_longPressTimer > 3f)
                {
                    clickNum = Mathf.Min(1000, Mathf.Pow(4, Mathf.RoundToInt(_longPressTimer / 3)));
                }
                item.attrVal.t0.Play();
                SendToStrengthShuxing(index, (int) clickNum);
            }
            else
            {
                UIManager.Instance.ToastByKey(10012);
                yield break;
            }
            yield return _isReceived;
            yield return _wait;
            _longPressTimer += 0.1f;
        }
    }

    private void OnTouchEndItem()
    {
        HeroInfoManager.Instance.IsOperating = false;
        _isTouchStart = false;
        if (_delayCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_delayCoroutine);
        }

        _longPressTimer = 0;


    }
    /// <summary>
    /// 外部调用的引导关闭
    /// </summary>
    public void ChkGuide()
    {
        if (GuideManager.Instance.IsShowGuiding || this.lobbyMain.maskBG.visible) { return; }
        if (this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 1)//任务是要可以领取的
        {
            //领取任务
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                //bid = GuideID.NewAccount_ClickEquip,
                uiName = "Lobby",
                giding = GuideID.NewAccount_ClickChallenge,
                gid = GuideID.NewAccount_ClickClaimTask,
                tui = this.lobbyMain.panel.comPandaInfo,
                isForce = true,
                isSend = true,
                npcTxt = "Beginner_Doc_004",
                npcPosType = PosType.Down,
            });
        }
        if (_classicInfoList.Count == 1 && _classicInfoList[0].level < 3 && this.lobbyMain.panel.listBottom.visible && this.lobbyMain.panel.classicList != null && lobbyMain.panel.classicList.numItems == 1 && !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon1))
        {
            if (_classicInfoList[0].level == 1)
            {
                //第一次获得秘典引导触发//因为有一个解锁提示，所以需要等它消失
                GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    bid = GuideID.NewAccount_ClickClaimTask,
                    giding = GuideID.NewAccount_ClickClaimTask,
                    gid = GuideID.NewAccount_BossDeadUpLevel,
                    tui = ((UI_GrimoireItem)this.lobbyMain.panel.classicList.GetChildAt(0)).upAniBtn,
                    isForce = true,
                    isSend = true,
                    npcTxt = "Beginner_Doc_005",
                    npcPosType = PosType.Down,
                });
            }
            else if (_classicInfoList[0].level > 1)
            {
                //引导-第2次点击秘典
                if (GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    bid = GuideID.NewAccount_ClickChallenge,
                    giding = GuideID.NewAccount_BossDeadUpLevel,
                    gid = GuideID.NewAccount_ClickSecretCanon1,
                    tui = ((UI_GrimoireItem)this.lobbyMain.panel.classicList.GetChildAt(0)).upAniBtn,
                    isForce = true,
                    isSend = true,
                })) { }
            }
        }
        else if (_classicInfoList.Count == 1 && FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroClassic).Item1)
        {
            if (_classicInfoList[0].level > 3 && !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon1))
            {
                List<GuideID> temp = new List<GuideID>();
                if (!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_BossDeadUpLevel))
                    temp.Add(GuideID.NewAccount_BossDeadUpLevel);

                GuideManager.Instance.HideGuide();
                GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickSecretCanon1, temp);
            }
            
            //去装备界面
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                //giding = GuideID.NewAccount_ClickSecretCanon2,
                pid = GuideID.NewAccount_MPUpLevel1,//已经完成ID不能继续
                bid = GuideID.NewAccount_BossDeadUpLevel,//前置需完成ID
                //giding = GuideID.NewAccount_ClickSecretCanon1,//上个执行ID
                gid = GuideID.NewAccount_UIEquip0,//当前执行ID
                tui = this.lobbyMain.panel.listBottom.GetChildAt(1),//目标对象
                pType = PosType.Down,
                isForce = true,//强制引导（其他地方点击不了）
                isSend = false//是否发送记录
            });
        }

        //关闭装备界面
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            uiName = "EquipSystem",
            giding = GuideID.NewAccount_MPClose,
            bid = GuideID.NewAccount_MPUpLevel1,
            gid = GuideID.NewAccount_UIEquipClose,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(1),
            pType = PosType.Down,
            isForce = true,
            isSend = true
        })) { return; }

        //引导-去下一关
        if (UIManager.Instance.IsTopController("Lobby") && this.lobbyMain.panel.battleRoot.btnStage.visible && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.NewAccount_UIEquipClose,
            bid = GuideID.NewAccount_MPUpLevel1,
            gid = GuideID.NewAccount_NextGate,
            tui = this.lobbyMain.panel.battleRoot.btnStage,
            isForce = true,
            isSend = true,
            npcTxt = "Beginner_Doc_008",
            npcPosType = PosType.Down,
            isOver = true
        })) { return; }


        ///引导-关闭角色界面
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            uiName = "RoleSystem",
            giding = GuideID.Click_EquipSkillItem,
            bid = GuideID.Click_EquipSkillItem,
            gid = GuideID.Click_UIRoleClose,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(0),
            pType = PosType.Down,
            isForce = true,
            isSend = true,
            isOver = true
        })) { return; }

        ///引导-点击打开装备界面
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Inherit,
            gid = GuideID.Click_UIEquip,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(1),
            pType = PosType.Down,
            isForce = true,
            isSend = false
        })) {
            UIManager.Instance.CloseUIPanel("FightWin");//防止多个胜利界面
            return;
        }

        ///引导-关闭装备界面
        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            uiName = "EquipSystem",
            fid = FuncOpenType.Inherit,
            giding = GuideID.Click_UIClothingEquip,
            bid = GuideID.Click_UIClothingEquip,
            gid = GuideID.Click_UIEquipClose3,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(1),
            pType = PosType.Down,
            isForce = true,
            isSend = true,
            isOver = true
        })) { return; }
        //if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_EquipSkillItem) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIEquipClose))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Click_UIEquipClose, PosType.Left, true, false);
        //}
        ////引导-关闭装备界面
        //else if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIClothingEquip) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIEquipClose3))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Click_UIEquipClose3, PosType.Left, true, false);
        //}

        UpdateVillageGuideProduceGetReward();
    }

    private void SendToStrengthShuxing(int index, int clickNum)
    {
        ShuxingInfo info = _shuxingInfos[index];
        if (info.Type == ShuxingType.CriticalStrike && info.MaxTag == 1)
        {
            UIManager.Instance.ToastByKey(10188);
            return;
        }
        _isReceived = false;
        // Debug.LogWarningFormat("SendToStrengthShuxing _longPressTimer = {0}    clickNum = {1}", _longPressTimer,  clickNum);
        var builder = RingAttrLevelUp_CS.CreateBuilder();
        builder.AttrId = (eBattleAttr) info.Type;
        builder.ClickTimes = (uint) clickNum;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_RingAttrLevelUp_CS, builder.Build());
    }

    private void UpdateFightInfo()
    {
        FightAttrVo fightAttrVo = DataManager.Instance.GetRoleData().FightAttrVo;
        // this.lobbyMain.panel.atkLb.text = StringUtils.FormatCurrency(fightAttrVo.Atk);
        // this.lobbyMain.panel.hpLb.text = StringUtils.FormatCurrency(fightAttrVo.HP);
        // this.lobbyMain.panel.criticalStrikeLb.text = StringUtils.FormatCurrency(double.Parse((fightAttrVo.CriticalStrike*100f).ToString("f2"))) + "%";
        // this.lobbyMain.panel.criticalInjuryLb.text = StringUtils.FormatCurrency(double.Parse((fightAttrVo.CriticalInjury*100f).ToString("f2"))) +"%";
        
        //伤害
        this.lobbyMain.panel.hpLb.icon = UIResource.GetAttrIconById(101.ToString());
        this.lobbyMain.panel.hpLb.text = StringUtils.FormatCurrency(fightAttrVo.Atk);//总攻击力
        ClassicAttr hpData = new ClassicAttr();
        hpData.AttrId = 101;
        hpData.AttrVal = fightAttrVo.Atk;
        this.lobbyMain.panel.hpLb.data = hpData;
        this.lobbyMain.panel.hpLb.onClick.Add(ShowFightInfoTips);
        LogUtils.LogWarning("伤害：" + fightAttrVo.Atk);
        
        //宠物伤害
        this.lobbyMain.panel.atkLb.icon = UIResource.GetAttrIconById(9.ToString());
        this.lobbyMain.panel.atkLb.text = StringUtils.FormatCurrency(fightAttrVo.PetAtk);
        ClassicAttr atkData = new ClassicAttr();
        atkData.AttrId = 9;
        atkData.AttrVal = fightAttrVo.PetAtk;
        this.lobbyMain.panel.atkLb.data = atkData;
        this.lobbyMain.panel.atkLb.onClick.Add(ShowFightInfoTips);
        LogUtils.LogWarning("宠物伤害：" + fightAttrVo.PetAtk);
        
        //防御
        this.lobbyMain.panel.criticalStrikeLb.icon = UIResource.GetAttrIconById(102.ToString());
        this.lobbyMain.panel.criticalStrikeLb.text = StringUtils.FormatCurrency(fightAttrVo.Def);//总防御力
        ClassicAttr criticalStrikeData = new ClassicAttr();
        criticalStrikeData.AttrId = 102;
        criticalStrikeData.AttrVal = fightAttrVo.Def;
        this.lobbyMain.panel.criticalStrikeLb.data = criticalStrikeData;
        this.lobbyMain.panel.criticalStrikeLb.onClick.Add(ShowFightInfoTips);
        LogUtils.LogWarning("防御：" + fightAttrVo.Def);
        
        //生命
        this.lobbyMain.panel.criticalInjuryLb.icon = UIResource.GetAttrIconById(4.ToString());
        this.lobbyMain.panel.criticalInjuryLb.text = StringUtils.FormatCurrency(fightAttrVo.HP);
        ClassicAttr criticalInjuryData = new ClassicAttr();
        criticalInjuryData.AttrId = 4;
        criticalInjuryData.AttrVal = fightAttrVo.HP;
        this.lobbyMain.panel.criticalInjuryLb.data = criticalInjuryData;
        this.lobbyMain.panel.criticalInjuryLb.onClick.Add(ShowFightInfoTips);
        LogUtils.LogWarning("生命：" + fightAttrVo.HP);
       
    }

    private void ShowFightInfoTips(EventContext context)
    {
        ClassicAttr data = ((UI_HeroAttrItem)context.sender)?.data as ClassicAttr;
        List<ClassicAttr> attrs = new List<ClassicAttr>();
        attrs.Add(data);

        int xPos1 = 0;
        int xPos2 = 0;

        if (data.AttrId == 101)
        {
            xPos1 = 200;
            xPos2 = 30;
        }

        if (data.AttrId == 9)
        {
            xPos1 = 40;
            xPos2 = 190;
        }

        if (data.AttrId == 102)
        {
            xPos1 = 30;
            xPos2 = 190;
        }

        if (data.AttrId == 4)
        {
            xPos1 = -40;
            xPos2 = 260;
        }
        
        TipsManger.Instance.ShowPopupTip((UI_HeroAttrItem)context.sender, Tipstype.Classic, attrs, xPos1, xPos2);
    }

    public GLabel GetUserHeadIcon()
    {
        // return ((UI_ComUserInfo) this.lobbyMain.panel.userInfo).headIcon;
        return ((UI_ComUserInfo) this.lobbyMain.userInfo).headIcon;//修改主界面顶部栏位、测试
    }

    /// <summary>
    /// //TODO 震动屏幕效果
    /// </summary>
    public void SceneEffect()
    {
        int time = Random.Range(1, 3);
        this.lobbyMain.panel.battleRoot.bg.sceneT.timeScale = this.lobbyMain.panel.battleRoot.scene.sceneT.timeScale = 1.5f * MapObjectManager.Instance.Speed;
        this.lobbyMain.panel.battleRoot.scene.sceneT.Play(time, 0.5f, null);
        this.lobbyMain.panel.battleRoot.bg.sceneT.Play(time, 0.5f, null);
    }

    private void OnClickSuperTXZBtn()
    {
        UIManager.Instance.ShowUIPanel("Passport");
    }

    private void OnShowForceGuide(int guideId,int guideFlag)
    {
        if (GuideManager.Instance.IsShowGuiding) return;
        UpdateGuideInfo();
        if (guideId == 999999)
        {
            
            if (this.lobbyMain.panel.comPandaInfo.rewardCtrl.selectedIndex == 1)
            {
                this.lobbyMain.panel.comPandaInfo.showHandle.selectedIndex = 1;
            }
        }
        //bool isGuideComplete = true;
        //switch ((GuideID)guideId)
        //{
            //case GuideID.GetFirstReward:
            //case GuideID.GetReward_AddAtk5:
            //case GuideID.GetReward_AddHp5:
            //case GuideID.GetReward_1007:
            //case GuideID.GetReward_1011:
            //case GuideID.GuideCompleteGetReward:
            //    GotoLobbyView();
            //    if (GuideManager.Instance.NotShowThisGuide(guideId))
            //    {
            //        this.lobbyMain.comPandaInfo.showHandle.selectedIndex = 0;
            //        GuideManager.Instance.HideGuide();
            //        return;
            //    }
            //    ConfigGuideUnit guideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get((int) guideId);
            //    if (guideUnit.GuideType == 1)
            //    {
            //        GuideManager.Instance.HideGuide();
                    
            //        GuideManager.Instance.StartGuide(this.lobbyMain.comPandaInfo, (GuideID) guideId, PosType.Left, true, true);
            //    }
            //    else
            //    {
            //        if (this.lobbyMain.comPandaInfo.rewardCtrl.selectedIndex == 1)  
            //        {
            //            this.lobbyMain.comPandaInfo.showHandle.selectedIndex = 1;
            //            if (guideId == (int) GuideID.GuideCompleteGetReward)
            //            {
            //                GuideManager.Instance.SendToCompleteGuide(guideId);
            //            }
            //        }
            //    }
            //    break;
            //case GuideID.Click_AddAtk5:
            //case GuideID.Click_AddAtk5_2:
            //case GuideID.Common_ClickAddAtk:
            //    int atkGuideId = guideId;
            //    if (GuideManager.Instance.NotShowThisGuide(guideId))
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            if (!GuideManager.Instance.NotShowThisGuide(guideId + i))
            //            {
            //                isGuideComplete = false;
            //                atkGuideId = guideId + i;
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        isGuideComplete = false;
            //    }

            //    if (isGuideComplete)
            //    {
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(1) as UI_LobbyHeroAttrAdd;
            //        attrAdd.showHandle.selectedIndex = 0;
            //        return;
            //    }
            //    ConfigGuideUnit atkGuideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get((int) atkGuideId);
            //    if (atkGuideUnit.GuideType == 1)
            //    {
            //        GuideManager.Instance.HideGuide();
            //        this.lobbyMain.panel.shuxingList.ScrollToView(0);
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(1) as UI_LobbyHeroAttrAdd;
            //        GuideManager.Instance.StartGuide(attrAdd.strengthBtn, (GuideID) guideId, PosType.Left, true, true);
            //    }
            //    else
            //    {
            //        UI_LobbyHeroAttrAdd attrAdd0 = this.lobbyMain.panel.shuxingList.GetChildAt(0) as UI_LobbyHeroAttrAdd;
            //        attrAdd0.showHandle.selectedIndex = 0;
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(1) as UI_LobbyHeroAttrAdd;
            //        attrAdd.showHandle.selectedIndex = guideFlag == 1 ? 1 : 0;
            //    }
            //    break;
            //case GuideID.Click_AddHp5:
            //case GuideID.Click_AddHp5_2:
            //case GuideID.Common_ClickAddHp:
            //    int hpGuideId = guideId;
            //    if (GuideManager.Instance.NotShowThisGuide(guideId))
            //    {
            //        for (int i = 0; i < 5; i++)
            //        {
            //            if (!GuideManager.Instance.NotShowThisGuide(guideId + i))
            //            {
            //                isGuideComplete = false;
            //                hpGuideId = guideId + i;
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        isGuideComplete = false;
            //    }

            //    if (isGuideComplete)
            //    {
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(0) as UI_LobbyHeroAttrAdd;
            //        attrAdd.showHandle.selectedIndex = 0;
            //        return;
            //    }
            //    ConfigGuideUnit hpGuideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get((int) hpGuideId);
            //    if (hpGuideUnit.GuideType == 1)
            //    {
            //        GuideManager.Instance.HideGuide();
            //        this.lobbyMain.panel.shuxingList.ScrollToView(0);
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(0) as UI_LobbyHeroAttrAdd;
            //        GuideManager.Instance.StartGuide(attrAdd.strengthBtn, (GuideID) guideId, PosType.Left, true, true);
            //    }
            //    else
            //    {
            //        //1=接收领取 2=完成领取
            //        UI_LobbyHeroAttrAdd attrAdd = this.lobbyMain.panel.shuxingList.GetChildAt(0) as UI_LobbyHeroAttrAdd;
            //        attrAdd.showHandle.selectedIndex = guideFlag == 1 ? 1 : 0;
            //        UI_LobbyHeroAttrAdd attrAdd1 = this.lobbyMain.panel.shuxingList.GetChildAt(1) as UI_LobbyHeroAttrAdd;
            //        attrAdd1.showHandle.selectedIndex = 0;
            //    }
            //    break;
            //case GuideID.Click_zhuzhaoBtn:
            //    if (!GuideManager.Instance.NotShowThisGuide(guideId))
            //    {
            //        GuideManager.Instance.HideGuide();
            //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)guideId, PosType.Down, true, false);
            //    }
            //    break;
            //case GuideID.Click_Summon://召唤引导
            //case GuideID.Click_Summon2:
            //    GuideManager.Instance.StarGuideByData(new GuideData()
            //    {
            //        gid = (GuideID)guideId,
            //        tui = this.lobbyMain.panel.listBottom.GetChildAt(4),
            //        isForce = true,
            //        isSend = false,
            //        pType = PosType.Down
            //    });
            //    break;
        //}
    }

    private void GuideToClickZZCSys(bool isSelected)
    {
        //if (isSelected && !GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_zhuzhaoBtn))
        //{
        //    GuideManager.Instance.HideGuide();
        //}

        //if (curSelectIndex == 3 && isSelected)
        //{
        //    if (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_GetEquip))
        //    {
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.comBox.btnBoxIcon, GuideID.Click_GetEquip,
        //            PosType.Down, true, false);
        //    }
        //}
    }
    /// <summary>
    /// 引导开始
    /// </summary>
    public void ChkGuideStart(int passId, FuncOpenType type)
    {
        try
        {
            FunPrevInfo o = FuncPreviewManger.Instance.GetFunInfo((int)type);
            if (int.Parse(o.SystemUnit.Condition) == passId)
            {
                var drawSkill = FuncPreviewManger.Instance.GetFuncOpenState(type);
                if (drawSkill.Item1)
                {
                    //激活引导
                    switch (type)
                    {
                        case FuncOpenType.SummonSkill://技能开放
                            OnShowForceGuide((int)GuideID.Click_Summon, 0);
                            break;
                        case FuncOpenType.SummonPet://宠物开放
                            OnShowForceGuide((int)GuideID.Click_Summon2, 0);
                            break;
                        case FuncOpenType.Inherit://传承装备开放
                            OnShowForceGuide((int)GuideID.Click_UIEquip, 0);
                            break;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }


    public void UpdateGuideInfo()
    {
        this.lobbyMain.panel.listBottom.EnsureBoundsCorrect();

        //引导-去角色界面
        if(RoleManager.Instance.GetSkillSlotInfoCount() <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            giding = GuideID.Click_SummonSkill_Close,
            pid = GuideID.Click_EquipSkillItem,
            bid = GuideID.Click_SummonSkill,
            gid = GuideID.Click_OpenHeroSys,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(0),
            pType = PosType.Down,
            isForce = true,
            isSend = false
        }) ||
        //引导-去宠物界面
        ((UI_BtnBottom)this.lobbyMain.panel.listBottom.GetChildAt(3)).lockCtrl.selectedIndex == 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.SummonPet,
            giding = GuideID.Click_SummonPet_Close,
            bid = GuideID.Click_SummonPet,
            gid = GuideID.Click_OpenPet,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(3),
            pType = PosType.Down,
            isForce = true,
            isSend = true
        }) ||
        //引导-关闭宠物界面
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            uiName = "PetSystem",
            giding = GuideID.Click_EquipPetItem,
            bid = GuideID.Click_EquipPetItem,
            gid = GuideID.Click_UIPetClose2,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(3),
            pType = PosType.Down,
            isForce = true,
            isSend = true,
            isOver = true
        })
        ) { }
        var zh = ((UI_BtnBottom)this.lobbyMain.panel.listBottom.GetChildAt(4));
        //召唤技能引导
        if (zh.lockCtrl.selectedIndex == 0 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.SummonSkill,
            gid = GuideID.Click_Summon,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(4),
            isForce = true,
            isSend = false,
            pType = PosType.Down
        }))
        {
            UIManager.Instance.CloseUIPanel("FightWin");//防止多个胜利界面
            return;
        }
        //召唤宠物引导
        if(zh.lockCtrl.selectedIndex == 0 && !UIManager.Instance.IsShowByName("SummonSystem") && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.SummonPet,
            gid = GuideID.Click_Summon2,
            tui = this.lobbyMain.panel.listBottom.GetChildAt(4),
            isForce = true,
            isSend = false,
            pType = PosType.Down
        }))
        {
            UIManager.Instance.CloseUIPanel("FightWin");//防止多个胜利界面
            return;
        }


        //if (GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_Summon) &&
        //    (!GuideManager.Instance.NotShowThisGuide((int) GuideID.Click_OpenHeroSys)))
        //{
        //    GuideManager.Instance.HideGuide();
        //    //主UI下面的角色按钮
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(0), (GuideID)GuideID.Click_OpenHeroSys, PosType.Left, true, false);
        //}
        //引导-关闭装备界面
        //else if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_EquipSkillItem) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIEquipClose))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Click_UIEquipClose, PosType.Left, true, false);
        //}
        //引导-关闭装备界面
        //else if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIClothingEquip) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIEquipClose3))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Click_UIEquipClose3, PosType.Left, true, false);
        //}
        //else if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_Summon2) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_OpenPet))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Click_OpenPet, PosType.Left, true, false);
        //}
        //else if (GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_Summon2) &&
        //    !GuideManager.Instance.NotShowThisGuide((int)GuideID.Click_UIPetClose2))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Click_UIPetClose2, PosType.Left, true, false);
        //}

        // if(!GuideManager.Instance.NotShowThisGuide((int) GuideID.GuideCompleteGetReward)) return;

        //var petMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.SummonPetPos2);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_OpenHeroSys1) && petMap.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(0), GuideID.Trigger_Click_OpenHeroSys1, PosType.Left, true, false);
        //}

        //var roleMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroLevelUp);
        //if (!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_OpenHeroSys2) && roleMap.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(0), GuideID.Trigger_Click_OpenHeroSys2, PosType.Left, true, false);
        //}

        //var dungeonMap1 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_gold);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_DungeonBtn) && dungeonMap1.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Trigger_Click_DungeonBtn, PosType.Left, true, false);
        //}
        //var dungeonMap2 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_zhuzhao);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_DungeonBtn2) && dungeonMap2.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Trigger_Click_DungeonBtn2, PosType.Left, true, false);
        //}
        //var dungeonMap3 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_dimoand);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_DungeonBtn3) && dungeonMap3.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Trigger_Click_DungeonBtn3, PosType.Left, true, false);
        //}
        //var dungeonMap4 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_exp);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_DungeonBtn4) && dungeonMap4.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Trigger_Click_DungeonBtn4, PosType.Left, true, false);
        //}
        //var dungeonMap5 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Dungeon_petMatial);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_DungeonBtn5) && dungeonMap5.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(3), (GuideID)GuideID.Trigger_Click_DungeonBtn5, PosType.Left, true, false);
        //}

        //var villageMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildStone);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn) && villageMap.Item1)
        //{
        //    GotoLobbyView();
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.StartGuide(this.homeBtn, (GuideID)GuideID.Trigger_Click_VillageBtn, PosType.Left, true, false);
        //}
        

        // var runeMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Rune);
        // if(!GuideManager.Instance.GuideIsComplete((int) GuideID.Trigger_Click_OpenHeroSys3) && runeMap.Item1)
        // {
        //     GotoLobbyView();
        //     GuideManager.Instance.HideGuide();
        //     GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(0), (GuideID)GuideID.Trigger_Click_OpenHeroSys3, PosType.Left, true, false);
        // }
    }
    
    public void GotoLobbyView()
    {
        UIManager.Instance.CloseAllUIPanelExcept("Lobby");
        if(curSelectIndex != -1)
            (this.lobbyMain.panel.listBottom.GetChildAt(curSelectIndex) as UI_BtnBottom)?.FireClick(true, true);
    }

    private void UpdateVillageGuideProduceGetReward()
    {
        if (!UIManager.Instance.IsTopController("Lobby")) { return; }
        this.lobbyMain.panel.listBottom.EnsureBoundsCorrect();

        var status = (eBattleStatus)DataManager.Instance.GetRoleData().battleStatus;
        if (this.homeBtn != null && this.lobbyMain.panel.rightBtn.visible && !UIManager.Instance.IsShowByName("ChapterMap") && !GuideManager.Instance.IsShowGuiding && (status == eBattleStatus.eBattleStatus_NoneInCamp || status == eBattleStatus.eBattleStatus_Normal || status == eBattleStatus.eBattleStatus_Circle))
        {
            //引导-点击家园按钮（石头矿区）
            if (GuideManager.Instance.StarGuideByData(new GuideData()
            {
                fid = FuncOpenType.BuildStone,
                gid = GuideID.Trigger_Click_VillageBtn,
                tui = this.homeBtn,
                isForce = true,
                isSend = false,
                //npcTxt = "Beginner_Doc_002",
                //npcPosType = PosType.Down,
            })){}
            //引导-点击家园按钮（加工厂驻扎）
            else if (VillageInfoManager.Instance.GetVillagePetByBuild(VillageBuildType.Factory).Count <= 0 && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildFactory,
                    gid = GuideID.guideId_4200,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                    npcTxt = "Beginner_Doc_031",
                    npcPosType = PosType.Down,
            })){ }
            //引导-点击家园按钮（加工厂领奖）
            else if (VillageInfoManager.Instance.GetVillagePetByBuild(VillageBuildType.Factory).Count > 0 && VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Factory).ItemNum > 0
                && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildFactory,
                    gid = GuideID.Trigger_Click_VillageBtn2,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                })){ }
            //引导-点击家园按钮（训练场领奖）
            else if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Training).ItemNum > 0
                && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildTrain,
                    gid = GuideID.Trigger_Click_VillageBtn3,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                })){}
            //引导-点击家园按钮（窝棚领奖）
            else if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Shack).ItemNum > 0
                && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildPet,
                    gid = GuideID.Trigger_Click_VillageBtn4,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                })){}
            //引导-点击家园按钮（探索营地领奖）
            else if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Explore).ItemNum > 0
                && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildExplore,
                    gid = GuideID.Trigger_Click_VillageBtn5,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                })){}
            //引导-点击家园按钮（粮食工坊领奖）
            else if (VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.FoodWorkshop).ItemNum > 0
                && GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.BuildFood,
                    gid = GuideID.Trigger_Click_VillageBtn6,
                    tui = this.homeBtn,
                    isForce = true,
                    isSend = false,
                })){}
        }
        //this.lobbyMain.panel.listBottom.EnsureBoundsCorrect();
        //if(GuideManager.Instance.IsShowGuiding) return;
        //var villageMap2 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildFactory);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn2) && villageMap2.Item1)
        //{
        //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Factory);
        //    if (cityInfo.ItemNum > 0)
        //    {
        //        GotoLobbyView();
        //        GuideManager.Instance.HideGuide();
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Trigger_Click_VillageBtn2, PosType.Left, true, false);
        //    }
        //}


        //var villageMap3 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildFood);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn6) && villageMap3.Item1)
        //{
        //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.FoodWorkshop);
        //    if (cityInfo.ItemNum > 0)
        //    {
        //        GotoLobbyView();
        //        GuideManager.Instance.HideGuide();
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Trigger_Click_VillageBtn6, PosType.Left, true, false);
        //    }
        //}

        //var villageMap4 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildTrain);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn3) && villageMap4.Item1)
        //{
        //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Training);
        //    if (cityInfo.ItemNum > 0)
        //    {
        //        GotoLobbyView();
        //        GuideManager.Instance.HideGuide();
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Trigger_Click_VillageBtn3, PosType.Left, true, false);
        //    }
        //}
        //var villageMap5 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildPet);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn4) && villageMap5.Item1)
        //{
        //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Shack);
        //    if (cityInfo.ItemNum > 0)
        //    {
        //        GotoLobbyView();
        //        GuideManager.Instance.HideGuide();
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Trigger_Click_VillageBtn4, PosType.Left, true, false);
        //    }
        //}

        //var villageMap6 = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.BuildExplore);
        //if (!GuideManager.Instance.GuideIsComplete((int)GuideID.Trigger_Click_VillageBtn5) && villageMap6.Item1)
        //{
        //    CityInfo cityInfo = VillageInfoManager.Instance.GetBuildInfoByType(VillageBuildType.Explore);
        //    if (cityInfo.ItemNum > 0)
        //    {
        //        GotoLobbyView();
        //        GuideManager.Instance.HideGuide();
        //        GuideManager.Instance.StartGuide(this.lobbyMain.panel.listBottom.GetChildAt(1), (GuideID)GuideID.Trigger_Click_VillageBtn5, PosType.Left, true, false);
        //    }
        //}
    }

    private void UpdatePassportReddot()
    {
        this.lobbyMain.panel.superTXZBtn.redDot.visible = ActivityManager.Instance.HasPassportRewardGet() || ActivityManager.Instance.HasPassportTaskRewardGet();
    }
    
    private void UpdateShopFKZNReddot()
    {
        bool hasFKZNReward = ActivityManager.Instance.UpdateFkznReddot(1) ||
                             ActivityManager.Instance.UpdateFkznReddot(2) ||
                             ActivityManager.Instance.UpdateFkznReddot(3);
        bool hasFree = ActivityManager.Instance.Shop_FreeDailyPack == 0 || ActivityManager.Instance.Shop_FreeTTHLPack == 0 ||
                       ActivityManager.Instance.Shop_FreeTQKPack == 0;
        bool dailyReddot = ActivityManager.Instance.GetDailyReddot();
        this.lobbyMain.panel.shopBtn.redDot.visible = hasFKZNReward || hasFree || dailyReddot;
    }

    private void UpdateFirstPayReddot()
    {
        this.lobbyMain.panel.firstChargeBtn.redDot.visible = ActivityManager.Instance.CanGetFirstPayInfo().Item2;
    }

    private void OnUpdatePvpReddot()
    {
        this.lobbyMain.panel.PVPBtn.redDot.visible = (!PvpRankDataManager.Instance.YestdayMyRankAwardGet &&
                                                      PvpRankDataManager.Instance.YestdayMyRank >= 1 && PvpRankDataManager.Instance.YesterdayGFPlayCounter>0);
    }

    #region 秘典
    
    // 记录每个秘典的索引位置（仅在 Lobby 界面下使用）
    private Dictionary<int, int> _classicIndexCache = new Dictionary<int, int>();
    
    private void UpdateClassicIndexCache()
    {
        _classicIndexCache.Clear();
        for (int i = 0; i < _classicInfoList.Count; i++)
        {
            _classicIndexCache[_classicInfoList[i].classicId] = i;
        }
    }
    
    //新修改秘典
    private ConfigClassicsUnit GetCalssicsById(int id)
    {
        return _allClassicsDict.TryGetValue(id, out var unit) ? unit : null;
    }
    private ConfigClassicsUnit GetClassicsByClassicsID(int classicsID)
    {
        return _maxLevelClassicsDict.TryGetValue(classicsID, out var unit) ? unit : null;
    }
    
    private void UpdateClassicInfo()
    {
        _classicsDictionary = GrimoireManager.Instance.GetGrimoireDict();
        
        //秘典空状态
        if (_classicsDictionary.Count == 0)
        {
            this.lobbyMain.panel.noClassicTips.visible = true;
            this.lobbyMain.panel.noClassicTips.text = ConfigUtils.GetStringByKey(8059);
            this.lobbyMain.panel.listBottom.visible = false;
        }
        else
        {
            this.lobbyMain.panel.noClassicTips.visible = false;
            this.lobbyMain.panel.listBottom.visible = true;
        }
        
        if (GrimoireManager.Instance.firstSortClassic)
        {
            HandleClassicDict(); 
            GrimoireManager.Instance.firstSortClassic = false;
            UpdateClassicIndexCache();
        }
        else
        {
            // 非首次时：
            if (GrimoireManager.Instance.isChangeUI)
            {
                // 非Lobby界面，清空字典并排序
                HandleClassicDict();
                UpdateClassicIndexCache();
            }
            else
            {
                // Lobby界面，保持升级过的秘典在原位置
                HandleClassicDict(useCache: true);
            }
        }
        
        this.lobbyMain.panel.classicList.numItems = _classicInfoList.Count;

        // this.lobbyMain.panel.classicList.ScrollToView(0);
        
    }
    
    private void HandleClassicDict(bool useCache = false)
    {
        _classicInfoList.Clear();
        
        List<ClassicInfo> tempList = new List<ClassicInfo>(_classicsDictionary.Values);
        
        if (useCache && _classicIndexCache.Count > 0)
        {
            // 使用缓存顺序
            _classicInfoList.AddRange(tempList.OrderBy(x =>
            {
                return _classicIndexCache.TryGetValue(x.classicId, out var idx) ? idx : int.MaxValue;
            }));
        }
        else
        {
            // 按规则排序：未满级优先 > 当前等级降序
            tempList.Sort((a, b) =>
            {
                bool aIsMax = (a.nextLevel == 0);
                bool bIsMax = (b.nextLevel == 0);
                if (aIsMax != bIsMax)
                    return aIsMax ? 1 : -1;
                return b.level.CompareTo(a.level);
            });

            _classicInfoList.AddRange(tempList);
        }
    }

    private void ClassicListRender(int index, GObject item)
    {
        ClassicInfo classicInfo = _classicInfoList[index];
        var maxLvUnit = GetClassicsByClassicsID(classicInfo.classicId);//暂时没用-显示升级按钮用到
        bool isMaxLevel = classicInfo.nextLevel == 0;//为0表示满级

        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeByParam(classicInfo.classicId);
        // ((UI_GrimoireItem) item).name.text = ConfigUtils.GetTextById(itemTypeUnit.Name);
        
        if (maxLvUnit.Type == 1)
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.title = ConfigUtils.GetStringByKey(8074);
        }
        if (maxLvUnit.Type == 2)
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.title = ConfigUtils.GetStringByKey(8076);
        }
        if (maxLvUnit.Type == 3)
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.title = ConfigUtils.GetStringByKey(8075);
        }
        if (maxLvUnit.Type == 4)
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.title = ConfigUtils.GetStringByKey(8077);
        }
        
        ((UI_GrimoireItem)item).grimoireBtn.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());//获取秘典图标

        // ((UI_GrimoireItem) item).Lv.SetVar("cur",classicInfo.level.ToString()).SetVar("total",maxLvUnit.Lv.ToString()).FlushVars();
        ((UI_GrimoireItem)item).Lv.SetVar("cur",classicInfo.level.ToString()).FlushVars();
        if(_classicInfoList.Count == 1 && (classicInfo.level >= 1 && classicInfo.level <= 4) && classicInfo.classicId == 1001)
        {
            ChkGuide();
        }
        List<object> maxAttrs = null;//满级属性
        if (isMaxLevel)//当前的秘典ID和该类型的秘典的最大等级ID相等时，表示已满级
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.enabled = false;
            ((UI_GrimoireItem) item).upAniBtn.upBtn.status.selectedIndex = 1;
            maxAttrs = GroupAttributes(classicInfo.maxAttrs);//满级属性
        }
        else
        {
            ((UI_GrimoireItem)item).upAniBtn.upBtn.enabled = true;
            ((UI_GrimoireItem) item).upAniBtn.upBtn.status.selectedIndex = 0;
        }
        
        long costGold = classicInfo.nextLevelCostGold;//升级消耗金币数量
        ((UI_GrimoireItem) item).upAniBtn.upBtn.goldNum.text = costGold.ToString();
        
        ((UI_GrimoireItem) item).upAniBtn.upBtn.enough.selectedIndex = DataManager.Instance.GetRoleData().gold < costGold ? 0 : 1;
        // ((UI_GrimoireItem) item).upAniBtn.upBtn.data = classicInfo;
        ((UI_GrimoireItem) item).upAniBtn.data = classicInfo;
        // ((UI_GrimoireItem) item).upAniBtn.upBtn.onClick.Set(this.OnClickUpAttr);
        ((UI_UpAniBtn) ((UI_GrimoireItem)item).upAniBtn).onTouchBegin.Add(OnTouchBeginClassicItem);
        ((UI_UpAniBtn) ((UI_GrimoireItem)item).upAniBtn).onTouchEnd.Add(OnTouchEndClassicItem);
        
        ((UI_GrimoireItem)item).grimoireBtn.data = classicInfo;
        ((UI_GrimoireItem)item).grimoireBtn.onClick.Set(this.ShowClassicTip);//点击秘典图标展示出满级秘典信息
        
        //满级属性
        // ((UI_GrimoireItem) item).attr2List.itemRenderer = AttrItem2Render;
        // List<ClassicAttr> maxAttrsList = classicInfo.maxAttrs;
        // ((UI_GrimoireItem)item).attr2List.data = classicInfo;
        // ((UI_GrimoireItem)item).attr2List.numItems = maxAttrsList.Count;
        
        // 主属性列表 - 旧逻辑
        // ((UI_GrimoireItem) item).attrList.itemRenderer = AttrItemRender;
        // List<ClassicAttr> attrsList = classicInfo.attr;
        // ((UI_GrimoireItem) item).attrList.data = attrsList;
        // ((UI_GrimoireItem) item).attrList.numItems = attrsList.Count;
        
        // 主属性列表 - 使用分组后的属性数据
        var groupedAttrs = GroupAttributes(classicInfo.attr);
        if (maxAttrs != null)
        {
            groupedAttrs.AddRange(maxAttrs);
        }
        ((UI_GrimoireItem) item).attrList.itemRenderer = AttrItemRender;
        ((UI_GrimoireItem) item).attrList.data = groupedAttrs;
        ((UI_GrimoireItem) item).attrList.numItems = groupedAttrs.Count;

    }

    private Coroutine _delayCoroutineOfClassic;
    private WaitForSeconds _waitOfClassic = new WaitForSeconds(.1f);
    private bool _isTouchStartOfClassic;
    private bool _isReceivedOfClassic;
    private float _longPressTimerOfClassic = 0f;
    private void OnTouchBeginClassicItem(EventContext context)
    {
        HeroInfoManager.Instance.IsOperating = true;
        _longPressTimerOfClassic = 0;
        UI_UpAniBtn upBtn = context.sender as UI_UpAniBtn;
        UI_GrimoireItem item = upBtn.parent as UI_GrimoireItem;
        
        ClassicInfo classicInfo = upBtn.data as ClassicInfo;
        
        int index = this.lobbyMain.panel.classicList.GetChildIndex(item);
        _isTouchStartOfClassic = true;
        _delayCoroutineOfClassic = GameManager.Instance.StartCoroutine(DoDelayActionCoroutineOfClassic(index, upBtn, item));
    }

    private IEnumerator DoDelayActionCoroutineOfClassic(int index, UI_UpAniBtn upBtn, UI_GrimoireItem item)
    {
        while (_isTouchStartOfClassic)
        {
            ClassicInfo classicInfo = _classicInfoList[index];
            double cost = classicInfo.nextLevelCostGold;//升到下一等级所需金币   为0表示没有下一等级
            if (DataManager.Instance.GetRoleData().gold >= cost && cost > 0)
            {
                upBtn.t0.Play();
                GameManager.Instance.SoundManager.PlayEffect((int)SoundType.UpLvSE);
                float clickNum = 1;
                if (_longPressTimerOfClassic > 3f)
                {
                    clickNum = Mathf.Min(1000, Mathf.Pow(4, Mathf.RoundToInt(_longPressTimerOfClassic / 3)));
                }

                SendToStrengthClassic(index, (int)clickNum);
            }
            else
            {
                if (classicInfo.nextLevel == 0)
                {
                    UIManager.Instance.ToastByKey(10014);
                }
                else
                {
                    UIManager.Instance.ToastByKey(10012);
                }
                
                yield break;
            }

            yield return _isReceivedOfClassic;
            yield return _waitOfClassic;
            _longPressTimerOfClassic += 0.1f;
        }
    }

    private void OnTouchEndClassicItem()
    {
        HeroInfoManager.Instance.IsOperating = false;
        _isTouchStartOfClassic = false;
        if (_delayCoroutineOfClassic != null)
        {
            GameManager.Instance.StopCoroutine(_delayCoroutineOfClassic);
        }
        
        _longPressTimerOfClassic = 0;
        
    }

    private void SendToStrengthClassic(int index, int clickNum)
    {
        ClassicInfo classicInfo = _classicInfoList[index];
        _isReceivedOfClassic = false;
        
        Debug.LogWarningFormat("点击了秘典次数：clickNum={0}", clickNum);
        var builder = ClassicsLevelUp_CS.CreateBuilder();
        builder.ClassicId = classicInfo.classicId;
        builder.Levels = clickNum;//要升的等级数， 为0或为1则默认升1级。 大于1则表示升多个等级，服务器根据实际资源升具体的等级数
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClassicsLevelUp_CS, builder.Build());

        //引导强制结束
        //if(!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_BossDeadUpLevel))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_BossDeadUpLevel);
        //}
        //if (_classicInfoList[0].level > 2 && !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon1))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickSecretCanon1);
        //}
        //if (_classicInfoList[0].level > 3 && !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon2))
        //{
        //    GuideManager.Instance.HideGuide();
        //    GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickSecretCanon2);
        //}
        if (_classicInfoList[0].level >= 3)
        {
            if(!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickSecretCanon1))
            {
                List<GuideID> temp = new List<GuideID>();
                if (!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_BossDeadUpLevel))
                    temp.Add(GuideID.NewAccount_BossDeadUpLevel);

                GuideManager.Instance.HideGuide();
                GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickSecretCanon1, temp);
                ChkGuide();
            }
        }
    }

    // 属性分组配置
    private static readonly Dictionary<int, List<int>> AttributeGroups = new Dictionary<int, List<int>>()
    {
        { 101, new List<int> { 1, 2, 3 } },
        { 102, new List<int> { 5, 6, 7 } },
        { 501, new List<int> { 10, 11, 12 } },
        { 502, new List<int> { 14, 15, 16 } },
        { 503, new List<int> { 17, 18, 19, 20 } }
    };
    
    /// <summary>
    /// 将属性按照规则分组
    /// </summary>
    /// <param name="originalAttrs"></param>
    /// <returns></returns>
    private List<object> GroupAttributes(List<ClassicAttr> originalAttrs)
    {
        var result = new List<object>();
        var remainingAttrs = new List<ClassicAttr>(originalAttrs);
        
        // 检查并处理完整的分组
        foreach (var group in AttributeGroups)
        {
            int groupId = group.Key;
            var requiredAttrIds = group.Value;
        
            // 检查是否包含该组所有属性
            bool hasAllAttributes = true;
            var foundAttrs = new List<ClassicAttr>();
        
            foreach (int attrId in requiredAttrIds)
            {
                var attr = originalAttrs.Find(a => a.AttrId == attrId);
                if (attr == null)
                {
                    hasAllAttributes = false;
                    break;
                }
                foundAttrs.Add(attr);
            }
        
            // 如果包含完整分组，则创建整合项
            if (hasAllAttributes)
            {
                // 按属性ID排序确保显示顺序一致
                foundAttrs.Sort((a, b) => a.AttrId.CompareTo(b.AttrId));
            
                result.Add(new AttributeGroupItem
                {
                    GroupId = groupId,
                    Attributes = foundAttrs
                });
            
                // 从剩余属性中移除已整合的属性
                remainingAttrs.RemoveAll(a => requiredAttrIds.Contains(a.AttrId));
            }
        }

        // 添加未整合的剩余属性
        foreach (var attr in remainingAttrs)
        {
            result.Add(attr);
        }
    
        return result;
    }
    
    // 主属性列表渲染
    private void AttrItemRender(int index, GObject item)
    {
        // 新规则:
        //当服务端下发秘典属性中属性ID=1、2、3时，属性图标资源ID使用101
        //当服务端下发秘典属性中属性ID=5、6、7时，属性图标资源ID使用102
        //当服务端下发秘典属性中属性ID=10、11、12时，属性图标资源ID使用501
        //当服务端下发秘典属性中属性ID=14、15、16时，属性图标资源ID使用502
        //当服务端下发秘典属性中属性ID=17、18、19、20时，属性图标资源ID使用503
        //其余属性ID依然根据属性ID映射对应属性图标资源ID，以上5种整合图标点击图标时展示所有整合属性ID的属性值，示意图如下
        
        // 旧逻辑
        // List<ClassicAttr> list = item.parent.data as List<ClassicAttr>;
        // ((UI_AttrItem1)item).icon.url = UIResource.GetAttrIconById(list[index].AttrId.ToString());//属性图标
        // string lastAttrValue = EquipManager.Instance.SetAttributeValue(list[index].AttrId, list[index].AttrVal,true);
        // ((UI_AttrItem1) item).num.SetVar("value",lastAttrValue).FlushVars();
        // ((UI_AttrItem1) item).data = list[index];
        // ((UI_AttrItem1) item).onClick.Set(this.OnClickAttrItem1);
        
        var dataList = (List<object>)item.parent.data;
        var data  = dataList[index];

        if (data is AttributeGroupItem groupItem)
        {
            // 渲染属性组整合项
            ((UI_AttrItem1)item).icon.url = UIResource.GetAttrIconById(groupItem.GroupId.ToString());
            string value = EquipManager.Instance.SetAttributeValue(groupItem.Attributes[0].AttrId, groupItem.Attributes[0].AttrVal, true);
            ((UI_AttrItem1) item).num.SetVar("value", value).FlushVars();
            ((UI_AttrItem1) item).data = groupItem;
        }
        else if (data is ClassicAttr singleAttr)
        {
            // 渲染单个属性项
            int resourceId = singleAttr.AttrId;
            ((UI_AttrItem1)item).icon.url = UIResource.GetAttrIconById(resourceId.ToString());
            string value = EquipManager.Instance.SetAttributeValue(singleAttr.AttrId, singleAttr.AttrVal, true);
            ((UI_AttrItem1) item).num.SetVar("value", value).FlushVars();
            ((UI_AttrItem1) item).data = singleAttr;
        }
        
        ((UI_AttrItem1) item).onClick.Set(this.OnClickAttrItem1);
    }

    // 点击属性图标展示tips
    private void OnClickAttrItem1(EventContext context)
    {
        // 旧逻辑
        // ClassicAttr attrData = (context.sender as UI_AttrItem1).data as ClassicAttr;//EN_BUFF_ADD_TYPE
        // string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(attrData.AttrId).AttrName);
        // string lastAttrValue = EquipManager.Instance.SetAttributeValue(attrData.AttrId, attrData.AttrVal, true);
        // TipsManger.Instance.ShowPopupTip((UI_AttrItem1)context.sender, Tipstype.Classic, attrName, lastAttrValue, 50);
        
        var sender = (UI_AttrItem1)context.sender;
        if (sender.data is AttributeGroupItem groupItem)
        {
            // 显示分组属性的所有值
            List<ClassicAttr> attrs = new List<ClassicAttr>();
            foreach (var attribute in groupItem.Attributes)
            {
                attrs.Add(attribute);
            }
            TipsManger.Instance.ShowPopupTip((UI_AttrItem1)context.sender, Tipstype.Classic, attrs, 50);
        }
        else if (sender.data is ClassicAttr singleAttr)
        {
            // 显示单个属性值
            // string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(singleAttr.AttrId).AttrName);
            // string value = EquipManager.Instance.SetAttributeValue(singleAttr.AttrId, singleAttr.AttrVal, true);
            List<ClassicAttr> attrs = new List<ClassicAttr>();
            attrs.Add(singleAttr);
            TipsManger.Instance.ShowPopupTip((UI_AttrItem1)context.sender, Tipstype.Classic, attrs, 50);
        }
        
    }

    //满级属性
    private void AttrItem2Render(int index, GObject item)
    {
        // ClassicInfo info = item.parent.data as ClassicInfo;
        // ((UI_AttrItem2)item).icon.url = UIResource.GetAttrIconById(info.maxAttrs[index].AttrId.ToString());//属性图标
        // if (info.nextLevel == 0)//满级
        // {
        //     ((UI_AttrItem2)item).status.selectedIndex = 0;
        // }
        // else
        // {
        //     ((UI_AttrItem2)item).status.selectedIndex = 1;
        // }
        // string lastAttrValue = EquipManager.Instance.SetAttributeValue(info.maxAttrs[index].AttrId, info.maxAttrs[index].AttrVal,true);
        // ((UI_AttrItem2) item).num.SetVar("value",lastAttrValue).FlushVars();
        // ((UI_AttrItem2) item).data = info.maxAttrs[index];
        // ((UI_AttrItem2) item).onClick.Set(this.OnClickAttrItem2);
    }
    
    private void OnClickAttrItem2(EventContext context)
    {
        // ClassicAttr attrData = (context.sender as UI_AttrItem2).data as ClassicAttr;//EN_BUFF_ADD_TYPE
        // string attrName = ConfigUtils.GetTextById(ConfigUtils.GetAttrEnumerationUnitByAttrId(attrData.AttrId).AttrName);
        // string lastAttrValue = EquipManager.Instance.SetAttributeValue(attrData.AttrId, attrData.AttrVal, true);
        // TipsManger.Instance.ShowPopupTip((UI_AttrItem2)context.sender, Tipstype.Classic, attrName, lastAttrValue, -120);
    }

    private void ShowClassicTip(EventContext context)
    {
        ClassicInfo classicInfo = (context.sender as GButton).data as ClassicInfo;
        if (classicInfo != null)
        {
            bool isMaxLevel = classicInfo.nextLevel == 0;//为0表示满级
            TipsManger.Instance.ShowPopupTip((GButton)context.sender,Tipstype.ClassicInfo,classicInfo,isMaxLevel);
        }
    }
    
    private void OnClickUpAttr(EventContext context)
    {
        ClassicInfo classicInfo = (context.sender as GButton).data as ClassicInfo;
        
        if (classicInfo != null && classicInfo.nextLevel != 0)
        {
            if (DataManager.Instance.GetRoleData().gold >= classicInfo.nextLevelCostGold)
            {
                var builder = ClassicsLevelUp_CS.CreateBuilder();
                builder.ClassicId = classicInfo.classicId;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClassicsLevelUp_CS, builder.Build());
            }
            else
            {
                UIManager.Instance.ToastByKey(10012);
            }
        }
        else
        {
            UIManager.Instance.ToastByKey(10014);
        }
        
    }

    private bool ClassicHasRed()
    {
        foreach (var item in _classicInfoList)
        {
            long costGold;//升级消耗金币数量
            // 是否为满级
            if (item.nextLevel != 0)
            {
                costGold = item.nextLevelCostGold;//升级消耗的金币数量
                if (DataManager.Instance.mRoleData.gold >= costGold)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region 神器

    // 神器升级界面
    private void OnClickUpLvBtn()
    {
        var artifactMap = FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.Artifact);
        if (!artifactMap.Item1)
        {
            UIManager.Instance.Toast(artifactMap.Item2);
            return;
        }
        
        UIManager.Instance.ShowUIPanel("Artifact");
    }
    
    private void UpdateArtifactInfo()
    {
        _artifactsDictionary = EquipManager.Instance.GetArtifactDict();
        _artifactEquipUnitList.Clear();
        foreach (var artifact in _artifactsDictionary)
        {
            ConfigArtifactEquipUnit artifactEquipUnit = ConfigUtils.GetArtifactEquipUnitByTypeAndLv(artifact.Key, artifact.Value);
            _artifactEquipUnitList.Add(artifactEquipUnit);
        }
        
        this.lobbyMain.panel.red.visible = false;
        foreach (var item in _artifactEquipUnitList)
        {
            var nextUnit = ConfigUtils.GetNextArtifactEquipUnitByTypeAndLv(item.Type, item.ArtifactLevel);
            if (nextUnit == null)
                continue;
            
            string[] itemCostParts = nextUnit.ItemCost.Split(',');
            int costItemId = int.Parse(itemCostParts[0]);
            int costItemNum = int.Parse(itemCostParts[1]);
    
            // 钻石消耗
            int costDiaNum = nextUnit.DiamondsCost;
    
            // 玩家当前拥有的材料数量
            int curCostItemNum = ItemInfoManager.Instance.GetItemCount(costItemId);
            // 玩家当前钻石数量
            long curCostDiaNum = DataManager.Instance.mRoleData.dia;
            
            if (curCostItemNum >= costItemNum && curCostDiaNum >= costDiaNum && GetMaxHeroLv() > item.ArtifactLevel)
            {
                this.lobbyMain.panel.red.visible = true;
                break;
            }
        }
        
        this.lobbyMain.panel.listEquip2.numItems =  _artifactsDictionary.Count;
    }

    private void ArtifactListRender(int index, GObject item)
    {
        ConfigArtifactEquipUnit artifactUnit = _artifactEquipUnitList[index];
        ((UI_BtnEquip)item).qualityIcon.visible = true;
        ((UI_BtnEquip)item).ctrQuality.selectedIndex = ConfigUtils.GetConfigItemTypeUnitById(artifactUnit.ItemId).Quality - 1;
        ((UI_BtnEquip) item).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeUnitById(artifactUnit.ItemId).Icon);
        ((UI_BtnEquip) item).hasCnt.selectedIndex = 1;
        ((UI_BtnEquip) item).lvLb.SetVar("value",artifactUnit.ArtifactLevel.ToString()).FlushVars();
        ((UI_BtnEquip) item).data = artifactUnit;
        ((UI_BtnEquip) item).onClick.Set(this.OnClickArtifactItem);
    }
    
    // 获取拥有的英雄中等级最高的英雄等级
    private int GetMaxHeroLv()
    {
        List<HeroInfo> heros = HeroInfoManager.Instance.GetHeroInfos();//拥有的英雄列表
        int maxLevel = 0;
        if (heros != null)
        {
            foreach (var hero in heros)
            {
                if (hero.Level > maxLevel)
                {
                    maxLevel = hero.Level; // 更新最高等级
                }
            }
        }
        return maxLevel;
    }

    //展示神器信息
    private void OnClickArtifactItem(EventContext context)
    {
        ConfigArtifactEquipUnit data = (context.sender as GButton).data as ConfigArtifactEquipUnit;
        UIManager.Instance.ShowUIPanel("ArtifactAttrTip",data);
    }

    #endregion

    #region Buff

    //更新buffUI
    private List<BuffInfo> _showBuffInfos = new List<BuffInfo>();
    private void UpdateClaimBuffUI()
    {
        _showBuffInfos.Clear();
        List<BuffInfo> buffInfoList = DataManager.Instance.GetBuffInfos();
        foreach (var buffInfo in buffInfoList)
        {
            ulong serverTime = ServerTimeManager.Instance.CurServerTime;//服务器当前时间

            if (serverTime < buffInfo.endTime)
            {
                _showBuffInfos.Add(buffInfo);
            }
        }
        _showBuffInfos.Sort((a, b) => b.startTime.CompareTo(a.startTime));//没有特殊意义，只是为了飞行效果而已
            
        this.lobbyMain.comBuff.buffList.numItems = _showBuffInfos.Count;
    }
    
    private void BuffListRender(int index, GObject item)
    {
        BuffInfo buffInfo = _showBuffInfos[index];
        ConfigRuinsBuffUnit ruinsBuffUnit = ConfigUtils.GetRuinsBuffDataById(buffInfo.cfgId);
        ((UI_BtnBuff)item).icon = UIResource.GetTalentUrl(ruinsBuffUnit.Icon.ToString());
        ((UI_BtnBuff)item).touchable = true;
        ((UI_BtnBuff)item).data = buffInfo;
        ((UI_BtnBuff)item).onClick.Add(OnClickShwoBuffInfo);

    }
    
    private void OnClickShwoBuffInfo(EventContext context)
    {
        BuffInfo buffInfo = (context.sender as UI_BtnBuff)?.data as BuffInfo;
        if (buffInfo == null) return;

        UIManager.Instance.ShowUIPanel("RuinBuffTips", buffInfo);

    }

    #endregion


    #region 功能栏

    private void FunctionInfo()
    {
        _foldFuncUnitList = ConfigUtils.GetFunPreUnitsByPosition((int)FunctionPosition.FOLD);//折叠功能栏
        _leftFuncUnitList = ConfigUtils.GetFunPreUnitsByPosition((int)FunctionPosition.LEFT);//左功能栏
        _rightFuncUnitList = ConfigUtils.GetFunPreUnitsByPosition((int)FunctionPosition.RIGHT);//右功能栏
    }
    
    //过滤未解锁的功能栏位信息
    private List<ConfigSystemUnit> FilterOtherUnit(List<ConfigSystemUnit> funcUnitList)
    {
        // 暂时注释
        // List<ConfigSystemUnit> unlockFunctionList = new List<ConfigSystemUnit>();//已解锁的功能列表
        // foreach (var unit in funcUnitList)
        // {
        //     if (FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)unit.Id).Item1)
        //     {
        //         unlockFunctionList.Add(unit);
        //     }
        // }
        // return unlockFunctionList;
        
        // 关闭商业化入口
        List<ConfigSystemUnit> unlockFunctionList = new List<ConfigSystemUnit>();//已解锁的功能列表
        foreach (var unit in funcUnitList)
        {
            if (FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)unit.Id).Item1)
            {
                if (unit.Id == 700 || unit.Id == 701 || unit.Id == 702 || unit.Id == 703 || unit.Id == 1100 || unit.Id == 1300 || unit.Id == 2500)
                {
                    continue;
                }
                unlockFunctionList.Add(unit);
            }
        }
        return unlockFunctionList;
        
    }

    // 折叠栏位-策划取消了
    private void OnClickOtherBtn()
    {
        this.lobbyMain.panel.isShowOther.selectedIndex = 1;
        // this.lobbyMain.panel.showOtherBtn.visible = false;
    }

    private void OnClickColseBtnToOther()
    {
        this.lobbyMain.panel.isShowOther.selectedIndex = 0;
        // this.lobbyMain.panel.showOtherBtn.visible = true;
    }

    private void UpdateOtherInfo()
    {
        var roleData = DataManager.Instance.GetRoleData();
        this.lobbyMain.panel.headIcon.icon = UIResource.GetItemUrl(roleData.GetAvatarUrl());
        this.lobbyMain.panel.playerName.text = roleData.userName;
        
        _unlockFoldFuncs = FilterOtherUnit(_foldFuncUnitList);
        this.lobbyMain.panel.otherList.numItems = _unlockFoldFuncs.Count;
    }

    private void OtherListRender(int index, GObject item)
    {
        ConfigSystemUnit unit = _unlockFoldFuncs[index];
        
        FuncOpenType funcOpenType = (FuncOpenType)unit.Id;
        
        switch (funcOpenType)
        {
            case FuncOpenType.SevenDay://七日签到
                ((UI_LanweiBtn)item).redDot.visible = ActivityManager.Instance.HasSevenDayReddot();
                break;
            case FuncOpenType.DailyTask://日常任务
                ((UI_LanweiBtn)item).redDot.visible = TaskInfoManager.Instance.IsCanGetDailyReward();
                break;
            case FuncOpenType.Village://家园
                ((UI_LanweiBtn)item).redDot.visible = VillageInfoManager.Instance.VillageRedDot();
                break;
            case FuncOpenType.Email://邮箱
                ((UI_LanweiBtn)item).redDot.visible = MailManager.Instance.HasNotReadMail();
                break;
            case FuncOpenType.TuJian://图鉴
                break;
        }
        
        ((UI_LanweiBtn)item).icon = UIResource.GetFuncPreIcon(unit.Icon.ToString());
        ((UI_LanweiBtn)item).title = ConfigUtils.GetTextById(unit.Name);
        ((UI_LanweiBtn)item).data = funcOpenType;
        item.onClick.Set(this.OnClickOtherItemBtn);
    }

    private void OnClickOtherItemBtn(EventContext context)
    {
        FuncOpenType funcId = (FuncOpenType)(context.sender as UI_LanweiBtn)?.data;

        ShowFunctionUI(funcId);
    }

    
    //左侧功能栏
    private void UpdateLeftFunctionList()
    {
        _unlockLeftFuncs = FilterOtherUnit(_leftFuncUnitList);

        if (_unlockLeftFuncs != null && _unlockLeftFuncs.Count > 0)
        {//目前按照降序规则刚好满足需求，后续需要改动的话，要求策划在System表中加入排序字段
            _unlockLeftFuncs = _unlockLeftFuncs.OrderByDescending(x => x.Id).ToList();
        }
        
        if (_unlockLeftFuncs.Count == 0 || _unlockLeftFuncs == null)
        {
            // 不展示
            this.lobbyMain.panel.leftBtn.visible = false;
            this.lobbyMain.panel.leftBg.visible = false;
        }
        else
        {
            this.lobbyMain.panel.leftBtn.visible = true;
            this.lobbyMain.panel.leftBg.visible = true;
        }
        
        this.lobbyMain.panel.leftFunctionList.numItems = _unlockLeftFuncs.Count;
    }

    private void LeftFunctionListRender(int index, GObject item)
    {
        ConfigSystemUnit unit = _unlockLeftFuncs[index];
        
        FuncOpenType funcOpenType = (FuncOpenType)unit.Id;

        ShowFunctionRedDot(funcOpenType, ((UI_BtnFucntionIcon)item));

        ((UI_BtnFucntionIcon)item).icon = UIResource.GetFuncPreIcon(unit.Icon.ToString());
        ((UI_BtnFucntionIcon)item).title = ConfigUtils.GetTextById(unit.Name);
        ((UI_BtnFucntionIcon) item).data = funcOpenType;
        ((UI_BtnFucntionIcon) item).onClick.Set(this.OnClickLeftFunctionItem);
    }

    //商城红点
    private bool ShopRedPointFalg()
    {
        bool flag = false;
        
        bool hasFKZNReward = ActivityManager.Instance.UpdateFkznReddot(1) ||
                             ActivityManager.Instance.UpdateFkznReddot(2) ||
                             ActivityManager.Instance.UpdateFkznReddot(3);
        bool hasFree = ActivityManager.Instance.Shop_FreeDailyPack == 0 || ActivityManager.Instance.Shop_FreeTTHLPack == 0 ||
                       ActivityManager.Instance.Shop_FreeTQKPack == 0;
        bool dailyReddot = ActivityManager.Instance.GetDailyReddot();
        flag = hasFKZNReward || hasFree || dailyReddot;
        return flag;
    }

    private void OnClickLeftFunctionItem(EventContext context)
    {
        FuncOpenType funcId = (FuncOpenType)(context.sender as UI_BtnFucntionIcon)?.data;

        ShowFunctionUI(funcId);
        
    }


    // 右侧功能栏
    private void UpdateRightFunctionList()
    {
        _unlockRightFuncs = FilterOtherUnit(_rightFuncUnitList);
        
        if (_unlockRightFuncs.Count == 0 || _unlockRightFuncs == null)
        {
            // 不展示
            this.lobbyMain.panel.rightBtn.visible = false;
            this.lobbyMain.panel.rightBg.visible = false;
        }
        else
        {
            this.lobbyMain.panel.rightBtn.visible = true;
            this.lobbyMain.panel.rightBg.visible = true;
        }
        
        this.lobbyMain.panel.rightFunctionList.numItems = _unlockRightFuncs.Count;
    }
    
    private void RightFunctionListRender(int index, GObject item)
    {
        ConfigSystemUnit unit = _unlockRightFuncs[index];

        FuncOpenType funcOpenType = (FuncOpenType)unit.Id;
        
        ShowFunctionRedDot(funcOpenType, ((UI_BtnFucntionIcon)item));

        ((UI_BtnFucntionIcon)item).icon = UIResource.GetFuncPreIcon(unit.Icon.ToString());
        ((UI_BtnFucntionIcon)item).title = ConfigUtils.GetTextById(unit.Name);
        ((UI_BtnFucntionIcon) item).data = funcOpenType;
        ((UI_BtnFucntionIcon) item).onClick.Set(this.OnClickRightFunctionItem);
    }

    private void OnClickRightFunctionItem(EventContext context)
    {
        FuncOpenType funcId = (FuncOpenType)(context.sender as UI_BtnFucntionIcon)?.data;

        ShowFunctionUI(funcId);
    }

    private void ShowFunctionUI(FuncOpenType funcOpenType)
    {
        switch (funcOpenType)
        {
            case FuncOpenType.SuperTXZ://通行证
                UIManager.Instance.ShowUIPanel("Passport");
                break;
            case FuncOpenType.Shop://商城
                UIManager.Instance.ShowUIPanel("ShopMain");
                break;
            case FuncOpenType.LoginGift://登录礼包
                UIManager.Instance.ShowUIPanel("LoginGift");
                break;
            case FuncOpenType.TuJian://图鉴
                UIManager.Instance.ShowUIPanel("TuJianMain");
                break;
            case FuncOpenType.Email://邮箱
                UIManager.Instance.ShowUIPanel("MailMain");
                break;
            case FuncOpenType.FirstCharge://新人礼包
                UIManager.Instance.ShowUIPanel("FirstPay");
                break;
            case FuncOpenType.SevenDay://七天签到
                UIManager.Instance.ShowUIPanel("SevenDay");
                break;
            case FuncOpenType.DailyTask://日常任务
                UIManager.Instance.ShowUIPanel("DailyTask");
                break;
            case FuncOpenType.OnlineReward://挂机奖励
                var builder = OnlineAwardClick_CS.CreateBuilder();
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_OnlineAwardClick_CS, builder.Build());
                break;
            case FuncOpenType.Dungeon://副本
                UIManager.Instance.ShowUIPanel("DungeonStage",null);
                break;
            case FuncOpenType.Village://家园
                UIManager.Instance.ShowUIPanel("VillageHome");
                GrimoireManager.Instance.isChangeUI = true;
                UpdateClassicInfo();
                break;
            case FuncOpenType.PVP://竞技
                UIManager.Instance.ShowUIPanel("AthleticsMain");
                break;
            case FuncOpenType.Achievement://成就
                AchievementManager.Instance.SendToGetAchievement();
                break;
            default:
                LogUtils.Log("错误的功能解锁Id");
                break;
        }
    }

    private void ShowFunctionRedDot(FuncOpenType funcOpenType, UI_BtnFucntionIcon item)
    {
        switch (funcOpenType)
        {
            case FuncOpenType.SuperTXZ://通行证
                ((UI_BtnFucntionIcon)item).redDot.visible = ActivityManager.Instance.HasPassportRewardGet() || ActivityManager.Instance.HasPassportTaskRewardGet();;
                break;
            case FuncOpenType.Shop://商城
                ((UI_BtnFucntionIcon)item).redDot.visible = ShopRedPointFalg();
                break;
            case FuncOpenType.LoginGift://登录礼包
                break;
            case FuncOpenType.TuJian://图鉴
                break;
            case FuncOpenType.Email://邮箱
                ((UI_BtnFucntionIcon)item).redDot.visible = MailManager.Instance.HasNotReadMail();
                break;
            case FuncOpenType.FirstCharge://新人礼包
                ((UI_BtnFucntionIcon)item).redDot.visible = ActivityManager.Instance.CanGetFirstPayInfo().Item2;
                break;
            case FuncOpenType.SevenDay://七天签到
                ((UI_BtnFucntionIcon)item).redDot.visible = ActivityManager.Instance.HasSevenDayReddot();
                break;
            case FuncOpenType.DailyTask://日常任务
                ((UI_BtnFucntionIcon)item).redDot.visible = TaskInfoManager.Instance.IsCanGetDailyReward();
                break;
            case FuncOpenType.OnlineReward://挂机奖励
                int onlineFreeTime = AdManager.Instance.GetAdFreeTimes((int)ePlayerAttrID.ePlayerAttrID_OnlineAwardFreeTimes);
                ((UI_BtnFucntionIcon)item).redDot.visible = (onlineRwFalg || onlineFreeTime > 0);
                break;
            case FuncOpenType.Dungeon://副本
                ((UI_BtnFucntionIcon)item).redDot.visible = DungeonMapManager.Instance.DungeonRedPoint();
                break;
            case FuncOpenType.Village://家园
                homeBtn = ((UI_BtnFucntionIcon)item);
                homeBtn.redDot.visible = VillageInfoManager.Instance.VillageRedDot();
                if (DataManager.Instance.GetRoleData().stageId == 1010)
                {
                    UpdateVillageGuideProduceGetReward();
                }
                break;
            case FuncOpenType.PVP://竞技
                ((UI_BtnFucntionIcon)item).redDot.visible = (!PvpRankDataManager.Instance.YestdayMyRankAwardGet &&
                                                             PvpRankDataManager.Instance.YestdayMyRank >= 1 && PvpRankDataManager.Instance.YesterdayGFPlayCounter>0);
                break;
            case FuncOpenType.Achievement://成就
                ((UI_BtnFucntionIcon)item).redDot.visible = UpdateAchievementRedPoint2();
                break;
            default:
                LogUtils.Log("错误的功能解锁Id");
                break;
        }
    }

    #endregion
    
    public GList GetBottomList()
    {
        return this.lobbyMain.panel.listBottom;
    }

    #region 场景BGM

    /// <summary>
    /// 播放不同的场景BGM
    /// </summary>
    public void PlaySceneBGM()
    {
        int x = DataManager.Instance.GetRoleData().battleStatus;
        
        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_NoneInCamp)
        {
            //营地BGM
            GameManager.Instance.SoundManager.PlayMusic((int)SoundType.CampBGM);
        }

        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Normal || DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_None || DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Circle)
        {
            // 战斗中
            ChapterMapInfoData chapterMapData = MapChapterManager.Instance.GetChapterMapInfoData();
            int chapterId = chapterMapData.chapterId;
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
    }

    #endregion
    
    public void ChangeMapBtnTitle(bool isMap)
    {
        if (!IsShow() || !IsOnStage()) { return; }
        ((UI_BtnBottom)this.lobbyMain.panel.listBottom.GetChildAt(2)).title = ConfigUtils.GetStringByKey(isMap ? 5171 : 5172);
    }

    /// <summary>
    /// 推送的位置
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPushPos()
    {
        var pos = lobbyMain.panel.btnHelp.xy;
        //pos.y -= 0;
        return pos;
    }

    private int classicTypeIndex = 0;
    private int classicItemIndex = -1;
    private int classicId = 0;
    public void ClassicListScrollToView(JumpTypeEnum jumpTypeEnum)
    {
        if (jumpTypeEnum != JumpTypeEnum.MiDianAtk && jumpTypeEnum != JumpTypeEnum.MiDianDef && jumpTypeEnum != JumpTypeEnum.MiDianLife && jumpTypeEnum != JumpTypeEnum.MiDianPet) return;
        
        classicId = 0;
        classicTypeIndex = 0;
        if (jumpTypeEnum == JumpTypeEnum.MiDianAtk)
            classicTypeIndex = 1;
        else if (jumpTypeEnum == JumpTypeEnum.MiDianDef)
            classicTypeIndex = 2;
        else if (jumpTypeEnum == JumpTypeEnum.MiDianLife)
            classicTypeIndex = 3;
        else if (jumpTypeEnum == JumpTypeEnum.MiDianPet)
            classicTypeIndex = 4; 
        
        if (classicTypeIndex == 0) return;

        classicItemIndex = -1;
        for (int i = 0; i < _classicInfoList.Count; i++)
        {
            var maxLvUnit = GetClassicsByClassicsID(_classicInfoList[i].classicId);//暂时没用-显示升级按钮用到
            if (maxLvUnit.Type == classicTypeIndex)  //伤害升级
            {
                if (classicId ==0 || classicId < _classicInfoList[i].classicId)
                {
                    classicId = _classicInfoList[i].classicId;
                    classicItemIndex = i;
                }
            }
        }
        
        if (classicItemIndex != -1)
        {
            this.lobbyMain.panel.classicList.ScrollToView(classicItemIndex);
            
            var globalPos = ((UI_GrimoireItem)this.lobbyMain.panel.classicList.GetChildAt(classicItemIndex)).upAniBtn.LocalToGlobal(Vector2.zero);
            Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(globalPos);
            JumpManager.Instance.ShowFinger(JumpTypeEnum.WatchAd, ((UI_GrimoireItem)this.lobbyMain.panel.classicList.GetChildAt(classicItemIndex)).upAniBtn, (int)logicScreenPos.x, (int)logicScreenPos.y);
        }
    }
}