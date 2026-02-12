using BigMap;
using Common;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Lobby;
using msg;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using BatchStuff = Engine.BatchStuff;
using BlendMode = FairyGUI.BlendMode;
using Color = UnityEngine.Color;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;
using UI_BtnBuff = BigMap.UI_BtnBuff;
using UI_TaskBarExp = BigMap.UI_TaskBarExp;

/// <summary>
/// 事件枚举
/// </summary>
public enum StageEventType
{
    /// <summary>
    /// 深埋宝藏
    /// </summary>
    Box = 1,
    /// <summary>
    /// 遗迹建筑
    /// </summary>
    Relic,
    /// <summary>
    /// 狩猎任务
    /// </summary>
    Task,
    /// <summary>
    /// 搜寻宠物
    /// </summary>
    Pet,
    /// <summary>
    /// 传承Boss
    /// </summary>
    Boss,
    /// <summary>
    /// 钻石矿
    /// </summary>
    Diamond,
    /// <summary>
    /// 天女散花
    /// </summary>
    Gold,
    /// <summary>
    /// 开箱子砍树（挖宝）
    /// </summary>
    OpenBox,
    /// <summary>
    /// 钓鱼
    /// </summary>
    Fishing,
    /// <summary>
    /// 奇遇商人
    /// </summary>
    Merchant,
}

public enum MapPosType
{
    /// <summary>
    /// 普通过路点
    /// </summary>
    Normal,
    /// <summary>
    /// 随机事件点
    /// </summary>
    Event,
    /// <summary>
    /// 关卡点
    /// </summary>
    Stage,
    /// <summary>
    /// 隐藏boss点
    /// </summary>
    Boss,
    /// <summary>
    /// 上一章
    /// </summary>
    LastChapter,
    /// <summary>
    /// 下一章
    /// </summary>
    NextChapter,
}

public class MapStageData
{
    public int posIndex { get; set; }
    public MapPosType posType { get; set; }
    public GLoader stageLoader { get; set; }
    public GLoader moveLoader { get; set; }
    public int mapStageId { get; set; }
    public RandomEventData stageTaskData { get; set; }
}

//金币事件飞入数据
public class StorehouseData
{
    /// <summary>
    /// 控件
    /// </summary>
    public GObject com { get; set; }

    /// <summary>
    /// 分割坐标(缓存用)
    /// </summary>
    //public Vector2 pos { get; set; }

    /// <summary>
    /// 实际坐标(缓存用)
    /// </summary>
    public Vector2 postion { get; set; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    public ulong guId { get; set; }
    /// <summary>
    /// 事件数据
    /// </summary>
    public int eventId { get; set; }
    /// <summary>
    /// 事件类型
    /// </summary>
    public eRandomEventType eventType { get; set; }
    /// <summary>
    /// 事件的内部数据ID
    /// </summary>
    public int batchStuffId { get; set; }
}

/// <summary>
/// 用来缓存数据使用的结构
/// </summary>
[System.Serializable]
public class StorehousePosSaveData
{
    /// <summary>
    /// 分割坐标(缓存用)
    /// </summary>
    //public Vector2 pos;

    /// <summary>
    /// 实际坐标(缓存用)
    /// </summary>
    public Vector2 postion;

    /// <summary>
    /// 唯一ID
    /// </summary>
    public ulong guId;
    /// <summary>
    /// 事件ID
    /// </summary>
    public int eventId;
    /// <summary>
    /// 事件类型
    /// </summary>
    public int eventType;
    /// <summary>
    /// 事件的内部数据ID
    /// </summary>
    public int batchStuffId;
}

public class StorehousePosSave
{
    public List<StorehousePosSaveData> data = new List<StorehousePosSaveData>();
}

/// <summary>
/// 开箱子砍树（挖宝）
/// </summary>
public class TreasureItemData
{
    /// <summary>
    /// 每棵树的唯一标识
    /// </summary>
    public string uniqueID { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public GComponent com { get; set; }

    /// <summary>
    /// 坐标
    /// </summary>
    public Vector2 pos { get; set; }

    /// <summary>
    /// 事件guid
    /// </summary>
    public ulong eventGuid { get; set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    public int type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int stuffId { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int amounts { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int eventStatus { get; set; }
}

[System.Serializable]
public class TreasureItemDataAndPos
{
    /// <summary>
    /// 每棵树的唯一标识
    /// </summary>
    public string uniqueID;

    /// <summary>
    /// 坐标x
    /// </summary>
    public float X;

    /// <summary>
    /// 坐标y
    /// </summary>
    public float Y;

}

[System.Serializable]
public class TreasureItemListWrapper
{
    public List<TreasureItemDataAndPos> treasureDatas = new List<TreasureItemDataAndPos>();
}

public class FishingPosData
{
    /// <summary>
    /// 钓点id
    /// </summary>
    public int id;
    
    /// <summary>
    /// 钓位地点
    /// </summary>
    public Vector2 fishingPos;
    
    /// <summary>
    /// 用于判断钓鱼方向
    /// </summary>
    public Vector2 fishingDirection;

    /// <summary>
    /// 节点
    /// </summary>
    public GLoader fishingNode;
    public GLoader fishingNode2;
}

public class ChapterMapView : UIViewBase
{
    private UI_ChapterMap chapterMap => this.main as UI_ChapterMap;

    private GComponent _mapPanel;//地图层
    private GLoader walkMap;//可行走区域图
    private GComponent mapbg;//地图
    private GComponent btnLastChapter;//上一章传送门按钮
    private GLoader3D spineLastChapter;//上一章传送门按钮上的特效
    private GLoader3D spineStageNext;//下一章传送门按钮上的特效
    private GComponent stageNext;//下一章传送门按钮
    private GLoader posNext;//下一章传送门坐标
    private GLoader posLast;//上一章传送门坐标

    private MapPosType chapterPosType;//切换地图是上一章还是下一章；

    #region 变量
    private int selectStageIndex = -1;  // 当前选中的关卡索引
    private AStarPathFinder aStarPathFinder;
    List<Vector2> posList = new List<Vector2>();
    private int currentMovePosIndex = 0;
    private Texture2D walkMapTexture;  //图集纹理
    private float gridSizeTextture = 20f;  // 纹理网格切图格子大小
    public float moveSpeed = 0.1f;   // 每个格子之间移动的速度
    private GLoader3D cursorSpine;//光标
    private Vector2 cursorLocalPos;// walkMap 上的点击局部坐标

    private int resourceLoadNum = 0;
    private int nextStageId = 0; //跳转 寻路到下一个关卡
    
    private float clickIntervalTime = 0;  //点击间隔时间

    //private Vector2 moveLocalPos = Vector2.zero;

    //关卡数据表
    private List<MapStageData> stageDataList = new List<MapStageData>();

    //角色移动时间配置
    private ConfigCommonUnit commonUnit2006;

    // 移动
    private GTweener tweener = null;
    /// <summary>
    /// 挖矿
    /// </summary>
    private GTweener WKTweener = null;

    //角色动画spine
    private SkeletonAnimation _heroSpine;

    private UI_VillagePetTalk UIContainerPaopao;

    /// <summary>
    /// 章节数据
    /// </summary>
    /// <returns></returns>
    private ChapterMapInfoData chapterMapData;

    /// <summary>
    /// 随机任务数据列表
    /// </summary>
    /// <returns></returns>
    private List<RandomEventData> randomTaskList;

    /// <summary>
    /// 固定点位事件数据列表
    /// </summary>
    /// <returns></returns>
    private List<RandomEventData> stageEventList;

    /// <summary>
    /// 委托任务数据
    /// </summary>
    /// <returns></returns>
    private NpcTaskData npcTaskData;

    private ClaimBuff claimBuff; //遗迹事件获得的buff信息

    private bool isChangeMap = false; //是否切换地图

    private ConfigChapterUnit chapterUnit;  // 章节数据

    private bool isLockMapFog = false;

    /// <summary>
    /// 引导的气泡
    /// </summary>
    private GComponent QiPao = null;

    #endregion

    #region 钻石矿和金币堆相关变量
    /// <summary>
    /// 用于存储钻石和金币堆的位置
    /// </summary>
    private Dictionary<Vector2, StorehouseData> _resStorehouse = new Dictionary<Vector2, StorehouseData>();
    /// <summary>
    /// X轴的随机位置的间隔
    /// </summary>
    private int mapIntervalX = 0;
    /// <summary>
    /// Y轴的随机位置的间隔
    /// </summary>
    private int mapIntervalY = 0;
    /// <summary>
    /// 随机生成地图的间隔数量
    /// </summary>
    private const int mapInterval = 20;
    /// <summary>
    /// 生成的像素间隔
    /// </summary>
    private const int mapPXInterval = 150;
    /// <summary>
    /// 自动拾取方位
    /// </summary>
    private const float pickupRange = 200;
    /// <summary>
    /// 可以行走的坐标区间坐标（分割后的坐标）
    /// </summary>
    private List<Vector2> walkableList = new List<Vector2>();
    /// <summary>
    /// 用于快速检测是否已经被使用
    /// </summary>
    private Dictionary<Vector2, bool> randomResPosList = new Dictionary<Vector2, bool>();
    /// <summary>
    /// 点是存在A队列还是B队列
    /// </summary>
    private Dictionary<Vector2, bool> randomResTagsAB = new Dictionary<Vector2, bool>();
    /// <summary>
    /// 奇数的表
    /// </summary>
    private List<Vector2> randomResPosListA = new List<Vector2>();
    /// <summary>
    /// 偶数的表，等奇数的表使用完了再使用偶数的表
    /// </summary>
    private List<Vector2> randomResPosListB = new List<Vector2>();
    /// <summary>
    /// 引导所使用的点
    /// </summary>
    private List<Vector2> GuidePosList = new List<Vector2>();
    /// <summary>
    /// 将要飞入到玩家身体的金币的动画
    /// </summary>
    private List<UICoinEffect> _storehouseDataList = new List<UICoinEffect>();
    //需要避开的节点区域
    private List<GObject> stageList = new List<GObject>();
    private List<MapStageData> eventStageList = new List<MapStageData>();
    /// <summary>
    /// 金币位置与事件缓存的key
    /// </summary>
    public const string goldStorehouseJsonKey = "GoldStorehouseJson";
    /// <summary>
    /// 钻石位置与事件缓存的key
    /// </summary>
    public const string diamondStorehouseJsonKey = "DiamondStorehouseJson";

    /// <summary>
    /// 当前金币堆事件的数据
    /// </summary>
    private RandomEventData rdGold_EventData = null;
    /// <summary>
    /// 当前钻石矿事件的数据
    /// </summary>
    private RandomEventData rdDiamond_EventData = null;

    /// <summary>
    /// 是否下金币雨中(0=没有操作，1=移除中，2=播放金币雨，3=等待播放金币雨)
    /// </summary>
    private int playGoldRaining = 0;
    /// <summary>
    /// 是否下钻石雨中
    /// </summary>
    private int playDiamondRaining = 0;
    /// <summary>
    /// 金币雨列表
    /// </summary>
    private List<GObject> goldRainList = new List<GObject>();
    /// <summary>
    /// 加多少的数值
    /// </summary>
    private List<GObject> addCurrencyList = new List<GObject>();
    /// <summary>
    /// 动作播放中
    /// </summary>
    public bool actionPlaying = false;
    /// <summary>
    /// 点击事件点进行移动的点
    /// </summary>
    private Vector2 touchEventPos = new Vector2();
    /// <summary>
    /// 移动到达的目的地数据
    /// </summary>
    private StorehouseData touchEventData = null;
    /// <summary>
    /// 所有随机事件的节点
    /// </summary>
    private List<GObject> allStorehouseObj = new List<GObject>();
    /// <summary>
    /// 点击选中的效果
    /// </summary>
    private UI_MineSelect mineSelect = null;
    /// <summary>
    /// 刷新检测行走
    /// </summary>
    private bool runChkTag = false;
    /// <summary>
    /// 引导中
    /// </summary>
    private bool isGuide = false;

    private JumpTypeEnum _jumpTypeEnum; //任务跳转
    #endregion

    #region 导航线相关变量
    private List<UI_pathSpineItem> _pathArrows = new List<UI_pathSpineItem>(); // 存储路径箭头
    private const float ArrowSpacing = 60f; // 箭头间距
    private const float ArrowFollowThreshold = 30f; // 箭头跟随阈值
    private GComponent _arrowContainer; // 箭头容器
    #endregion

    #region 小游戏地图相关（推箱子、奇遇商人）
    private eRandomEventType openMiniMapType = 0;
    #endregion

    private string saveFilePath;
    
    public ChapterMapView()
    {
        this.type = UIType.Normal;
        this.package = "BigMap";
        this.name = "ChapterMap";
        this.component = "ChapterMap";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    public int GetScene(int sid)
    {
        if (sid > 3) { return 1; }
        return sid;
    }

    /// <summary>
    /// 新地图需要重新加载
    /// </summary>
    private void loadMapPanel()
    {
        chapterMapData = MapChapterManager.Instance.GetChapterMapInfoData();
        chapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId).Clone();

        if (_mapPanel == null)
        {
            chapterMap.Loading.visible = true;
            GTween.To(0, 100, 2.0f).SetEase(EaseType.Linear)
                .OnUpdate((GTweener tweener) =>
                {
                    chapterMap.LoadTitle.text = Math.Round(tweener.value.x, 2).ToString() + "%";
                })
                .OnComplete(() =>
                {
                    //chapterMap.Loading.visible = false;
                    MapChapterManager.Instance.isLoadComplete = true;
                });

            _mapPanel = UIPackage.CreateObject("BigMap", "ChapterPanel" + GetScene(chapterUnit.Scenes)).asCom;
            chapterMap.panelClip.panel.AddChildAt(_mapPanel, 0);
            _mapPanel.xy = Vector2.zero;
            //_mapPanel.sortingOrder = 0;
            chapterMap.panelClip.panel.SetChildIndex(_mapPanel, 0);

            walkMap = _mapPanel.GetChild("walkMap").asLoader;
            mapbg = _mapPanel.GetChild("mapbg").asCom;
            btnLastChapter = _mapPanel.GetChild("btnLastChapter").asCom;
            spineLastChapter = _mapPanel.GetChild("spineLastChapter").asLoader3D;
            spineStageNext = _mapPanel.GetChild("spineStageNext").asLoader3D;
            stageNext = _mapPanel.GetChild("stageNext").asCom;
            posNext = _mapPanel.GetChild("posNext").asLoader;
            posLast = _mapPanel.GetChild("posLast").asLoader;
        }

        // 创建箭头容器
        if (_arrowContainer == null)
        {
            _arrowContainer = new GComponent();
            int mapBgIndex = this.chapterMap.panelClip.panel.GetChildIndex(_mapPanel);
            this.chapterMap.panelClip.panel.AddChildAt(_arrowContainer, mapBgIndex + 1);
            _arrowContainer.touchable = false;
        }

        this.walkMap.onClick.Add(this.OnClickWalkMap);  // 点击可行走区域
    }

    protected override void OnInit()
    {
        base.OnInit();

        this.chapterMap.mapEX.visible = false;
        commonUnit2006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2006);
        this.chapterMap.exitBtn.onClick.Add(OnClickExitBtn);

        mineSelect = chapterMap.panelClip.panel.MineSelect as UI_MineSelect;
        mineSelect.sortingOrder = 0;
        mineSelect.visible = false;
        mineSelect.touchable = false;

        FairyGUI.Stage.inst.onTouchEnd.AddCapture(__stageTouchEnd);

        // 钓鱼
        _common2011 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2011);
        _common2014 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2014);
        _bubble1011 = ConfigDataGroup.GetInstance<ConfigBubble>().Get(1011);

        // 开箱子砍树
        commonUnit201 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(201);
        TREASURE_DETECT_RANGE = float.Parse(commonUnit201.Param1 != null ? commonUnit201.Param1 : "0");
        MakeTreasureTime = int.Parse(commonUnit201.Param2 != null ? commonUnit201.Param2 : "0");

        ((UI_ComMessage)this.chapterMap.comMessage).InitChatInfo();

        _resStorehouse.Clear();

        cursorSpine = chapterMap.panelClip.panel.cursorSpine;
        cursorSpine.visible = false;

        // 装备气泡组件
        _equipQiPao = chapterMap.panelClip.panel.equipQiPao;
        _equipQiPao.visible = false;

        this.chapterMap.comPandaInfo.onClick.Add(this.OnClickToGetTaskReward);

        this.chapterMap.comBuff.buffList.itemRenderer = BuffListRender;

        this.chapterMap.mainComPandaInfo.onClick.Add(this.OnClickToGetMainTaskReward);//主线任务

        this.chapterMap.fightCurStageBtn.onClick.Add(() => { this.OnClickFightCurStageBtn();});//挑战当前关卡

        this.chapterMap.functionList.itemRenderer = FunctionListRender;//功能栏位

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ROLE_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_EVENTS_UPDATE, this.UpdateRandomEvents);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_Dorp_NPC_TASK, OnDropTask);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateClaimBuffUI);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.EVENT_GAIN_BUFF_ANIMATION, ShowGainBuffAni);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_NEXT_CHAPTER_UPDATE, UpdataNextChapterMap);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_GOLD_EVENTS_UPDATE, PlayGoldRain);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_GOLD_EVENTS_END, GoldEventOver);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHAPTER_MAP_MOVE_STOP, StopRoleMove);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE, LaodRandomBossAndPetEvent);
        EventDispatcher.GameWorld.Regist<BigMapObject, bool>(EventDefine.EVENT_PLAY_CATCH_PET_EVENTS, CatchPetEvent);  //抓捕宠物
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_TREASURE_UPDATE, UpdataTreasureInfo);//开箱子砍树更新
        EventDispatcher.GameWorld.Regist<ulong>(EventDefine.EVENT_RANDOM_TREASURE_END, ClearTreasureDataByEventGuid);//开箱子砍树

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_UPDATE, PlayDiamondRain);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_END, DiamondEventOver);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RANDOM_EVENTS_DATA_UPDATE, UpdateRandomEventData);
        EventDispatcher.GameWorld.Regist<Vector2, BigMapObject>(EventDefine.EVENT_BOSS_PET_AUTO_FIND_EVENT, AutoFindBossObject);

        //EventDispatcher.GameWorld.Regist(EventDefine.EVENT_OPEN_ADVENTURE_CAVEMAP, ShowAdventureCaveMap);
        //EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLOSE_ADVENTURE_CAVEMAP, CloseAdventureCaveMap);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ENTER_AND_EXIT_RUIN, ExitAndEnterRuinUpdate);

        // 注册更新主线任务事件
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_TASK_UPDATE, this.UpdateMainTaskInfo);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFuncInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateFuncRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateFuncRedDot);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpdateFuncRedDot);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        if (values.Length > 0)
        {
            _jumpTypeEnum = JumpTypeEnum.Normal;
            if (values[0] is bool)
            {
                isChangeMap = values[0] is bool;
            }
            else if (values[0] is int)
            {
                nextStageId = (int)values[0];
            }
            else if (values[0] is JumpTypeEnum)
            {
                _jumpTypeEnum = (JumpTypeEnum)values[0];
            }
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        UpdateClaimBuffUI();

        if (clickIntervalTime <= 1)
        {
            clickIntervalTime += Time.deltaTime;
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        //重置引导
        //var l = new List<GuideID>();
        //l.Add(GuideID.guideId_3700);
        //l.Add(GuideID.guideId_3701);
        //l.Add(GuideID.guideId_3702);
        //l.Add(GuideID.guideId_3800);
        //l.Add(GuideID.guideId_3801);
        //l.Add(GuideID.guideId_3802);
        //l.Add(GuideID.guideId_3803);
        //l.Add(GuideID.guideId_3900);
        //l.Add(GuideID.guideId_3901);
        //l.Add(GuideID.guideId_3902);
        //l.Add(GuideID.guideId_4000);
        //l.Add(GuideID.guideId_4001);
        //l.Add(GuideID.guideId_4100);
        //l.Add(GuideID.guideId_4101);
        //l.Add(GuideID.guideId_4102);
        //GuideManager.Instance.SendToLostGuide((int)GuideID.guideId_3703, l);

        loadMapPanel();

        if (!this.chapterMap.panelClip.visible)
        {
            CloseAdventureCaveMap();
        }

        MapObjectManager.Instance.ClearBattleScreen(); // 退出战斗关卡 清除场景

        UpdateRoleInfo();

        InitMapStageData();

        //委托任务面板
        UpdateTaskInfo();

        //主线任务面板
        UpdateMainTaskInfo();

        UpdateFuncInfo();//功能栏位

        resourceLoadNum = 0;
        LoadMapResource();

        UpdateClaimBuffUI();

        //迷雾
        InitFogMask();

        UpdateFightCurStageBtnInfo();

        _equipQiPao.visible = false;
        GameManager.Instance.TimerManager.SetTimer(1f, () =>
        {
            // 是否存在未分解或未装备的装备
            if (EquipManager.Instance.curNoEquipGuid != 0)
            {
                UIManager.Instance.ShowUIPanel("Equip", EquipManager.Instance.curNoEquipGuid, EquipManager.Instance.isNewEquip, 1);
            }
        });

        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_None ||
            DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Normal ||
            DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Circle)
        {   // 退出关卡战斗  玩家选择去下一关，或挂机回到大地图等所有离开关卡的操作，客户端发送
            var builder = StageExit_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_Stage_Exit_CS, builder.Build());
        }
        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_NoneInCamp)
        {   // 离开营地
            var builder = CampExit_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_Camp_Exit_CS, builder.Build());
        }

        if (playGoldRaining == 3)
        {//播放金币雨
            PlayGoldRain();
        }

        if (playDiamondRaining == 3)
        {//播放钻石雨
            PlayDiamondRain();
        }

        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        view?.HideMaskBG();

        mapPlayEffct();

        GetFogSaveFilePath();
    }

    protected override void OnHide()
    {
        base.OnHide();
        runChkTag = false;
        if (WKTweener != null)
        {
            WKTweener.Kill();
            WKTweener = null;
        }
        //mapObjDic = MapChapterManager.Instance.GetMapObjects();
        //foreach(var item in mapObjDic)
        //{
        //    item.Value.bigMapObject.Destroy();
        //}

        mineSelect.visible = false;
        cursorSpine.visible = false;
        actionPlaying = false;

        playGoldRaining = 0;
        playDiamondRaining = 0;
        //金币堆
        foreach (var item in _storehouseDataList)
        {
            item.Destroy();
        }
        _storehouseDataList.Clear();
        foreach (var item in goldRainList)
        {
            item.Dispose();
        }
        goldRainList.Clear();
        foreach (var item in addCurrencyList)
        {
            item.Dispose();
        }
        addCurrencyList.Clear();

        ClearPathArrows(); // 界面隐藏时清除导航线   暂时注释
        Utils.ClearSpineModelOnFGUI(this.chapterMap.panelClip.panel.hero);
        if (petEventGraph != null)
        {
            Utils.ClearSpineModelOnFGUI(petEventGraph);
        }

        if (moveMapToBossStageTween != null)
        {
            moveMapToBossStageTween.Kill();
            moveMapToBossStageTween = null;
        }
        ClearMoveTweener();

        // 保存英雄位置（基于当前用户UID）
        if (this.chapterMap.panelClip.panel.hero != null)
        {
            Vector2 heroPosition = new Vector2(
                this.chapterMap.panelClip.panel.hero.x,
                this.chapterMap.panelClip.panel.hero.y
            );
            GameManager.Instance.SaveHeroPosition(heroPosition);
            // 保存探索数据
            SaveExploredData();
        }

        MapChapterManager.Instance.HideMapObject();

        if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_None)
        {   // 进入营地   玩家选择去下一关，或挂机回到大地图等所有离开关卡的操作，客户端发送
            var builder = CampEnter_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_Camp_Enter_CS, builder.Build());
        }

        StopFishingCoroutine();

        MakingEquipInExitBigMap();
        
        _jumpTypeEnum = JumpTypeEnum.Normal;
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        ClearResStorehouseData();

        ClearPathArrows(); // 销毁时清除导航线  暂时注释
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ROLE_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, UpdateRoleInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_EVENTS_UPDATE, UpdateRandomEvents);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_Dorp_NPC_TASK, OnDropTask);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateClaimBuffUI);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.EVENT_GAIN_BUFF_ANIMATION, ShowGainBuffAni);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_NEXT_CHAPTER_UPDATE, UpdataNextChapterMap);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_GOLD_EVENTS_UPDATE, PlayGoldRain);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_GOLD_EVENTS_END, GoldEventOver);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHAPTER_MAP_MOVE_STOP, StopRoleMove);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE, LaodRandomBossAndPetEvent);
        EventDispatcher.GameWorld.UnRegist<BigMapObject, bool>(EventDefine.EVENT_PLAY_CATCH_PET_EVENTS, CatchPetEvent);  //抓捕宠物
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_TREASURE_UPDATE, UpdataTreasureInfo);//开箱子砍树更新
        EventDispatcher.GameWorld.UnRegist<ulong>(EventDefine.EVENT_RANDOM_TREASURE_END, ClearTreasureDataByEventGuid);//开箱子砍树

        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_UPDATE, PlayDiamondRain);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_DIAMOND_EVENTS_END, DiamondEventOver);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RANDOM_EVENTS_DATA_UPDATE, UpdateRandomEventData);
        EventDispatcher.GameWorld.UnRegist<Vector2, BigMapObject>(EventDefine.EVENT_BOSS_PET_AUTO_FIND_EVENT, AutoFindBossObject);

        //EventDispatcher.GameWorld.Regist(EventDefine.EVENT_OPEN_ADVENTURE_CAVEMAP, ShowAdventureCaveMap);
        //EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLOSE_ADVENTURE_CAVEMAP, CloseAdventureCaveMap);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ENTER_AND_EXIT_RUIN, ExitAndEnterRuinUpdate);

        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_TASK_UPDATE, this.UpdateMainTaskInfo);
        
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FUN_PREVIEW_UPDATE, this.UpdateFuncInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.UpdateFuncRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_VILLAGE_PRODUCT_FULL, this.UpdateFuncRedDot);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_VILLAGE_PET_SC_SUCC, this.UpdateFuncRedDot);

        FairyGUI.Stage.inst.onTouchEnd.RemoveCapture(__stageTouchEnd);

        MakingEquipInExitBigMap();
    }

    private void __stageTouchEnd(EventContext context)
    {
        if (_endFishingCoroutine != null && !GuideManager.Instance.IsShowGuiding || _jumpTypeEnum == JumpTypeEnum.MapFish)
        {
            StopFishingCoroutine();
            _fishingBarTween?.Kill();
            this.chapterMap.panelClip.panel.fishingWorkBar.visible = false;
            CheckHeroNearFishingPos(true);
        }

        if (_jumpTypeEnum != JumpTypeEnum.Normal)
        {
            _jumpTypeEnum = JumpTypeEnum.Normal; 
            JumpManager.Instance.HideFinger();
        }

        if (MapChapterManager.Instance.autoSkipFindCurStage)
        {
            MapChapterManager.Instance.autoSkipFindCurStage = false;
        }
    }

    private void UpdataNextChapterMap()
    {
        Show();
    }

    /// <summary>
    /// 加载地图资源
    /// </summary>
    private void LoadMapResource()
    {
        if (this.state != UIState.Show) return;

        // 大地图资源加载
        for (int i = 1; i < 7; i++)
        {
            var loader = (this.mapbg.GetChild("bg" + i) as GLoader);
            var url = UIResource.GetChapterMapBgByMapId(GetScene(chapterUnit.Scenes), i);
            loader.url = url;
        }

        this.walkMap.url = UIResource.GetChapterMapPathBgByMapId(GetScene(chapterUnit.Scenes));
        walkMapTexture = null;
        GameManager.Instance.StartCoroutine(CheckTextureReady());

        InitHero();

        UpdatePassTip();

        if (isChangeMap)  //切换地图 弹新地图的名称文字提示
        {
            if (chapterMapData != null)
            {
                chapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId).Clone();
                UIManager.Instance.Toast(ConfigUtils.GetTextById(chapterUnit.Name, chapterUnit.NameParam));
            }
            isChangeMap = false;
        }

        //上一章按钮
        this.spineLastChapter.visible = this.btnLastChapter.visible = chapterMapData.chapterId != 1 ? true : false;
    }

    private void UpdateRandomEvents()
    {
        if (this.state != UIState.Show) return;

        InitMapStageData();

        //委托任务面板
        // UpdateTaskInfo();
        UpdateRandomEventData();
    }

    //角色动画spine
    private SkeletonAnimation petEventSpine;
    private GGraph petEventGraph;
    private List<ConfigStageUnit> stageCfgList = new List<ConfigStageUnit>();
    private void InitMapStageData()
    {
        if (this._mapPanel == null) { return; }

        MapChapterManager.Instance.PlayBigMapBGM(chapterMapData.chapterId); stageCfgList.Clear();
        List<ConfigStageUnit> stageUnitsList = ConfigUtils.GetStageUnitByChapterId(this.chapterMapData.chapterId);
        for (int i = 0; i < stageUnitsList.Count; i++)
        {
            if (stageUnitsList[i].Node == 5)
            {
                stageCfgList.Add(stageUnitsList[i]);
            }
        }

        InitFishingPos();//钓鱼

        CreateTreasureItemAndSetData();

        randomTaskList = MapChapterManager.Instance.GetRandomEventList();
        stageEventList = MapChapterManager.Instance.GetMapStageEventList();

        UpdateRandomEventData();
        stageDataList.Clear();
        eventStageList.Clear();

        int stageDataIndex = 0; //关卡数据索引
        int randomEventIndex = 0; //随机事件数据索引
        stageList.Clear();
        string[] stageIdArr = chapterUnit.MapPos.Split(',');
        for (int i = 0; i < stageIdArr.Length; i++)
        {
            MapStageData mapStageData = new MapStageData();
            mapStageData.stageTaskData = null;

            int dataStageId = int.Parse(stageIdArr[i]);
            if (dataStageId == 1 || dataStageId == 2)
            {
                GLoader3D yc = this._mapPanel.GetChild("YC" + (stageDataIndex + 1)) as GLoader3D;
                
                UI_stageItem stageItem = this._mapPanel.GetChild("stage" + (stageDataIndex + 1)) as UI_stageItem;
                stageList.Add(stageItem);
                GLoader stageLoader = stageItem.GetChild("icon") as GLoader;
                stageItem.bg.visible = false;
                stageItem.model.visible = false;
                stageItem.visible = true;
                stageItem.touchable = true;
                if (yc != null)
                {
                    stageLoader.alpha = 0f;
                }
                else
                {
                    stageLoader.alpha = 1f;
                }

                GLoader moveLoader = this._mapPanel.GetChild("pos" + (stageDataIndex + 1)) as GLoader;
                if (stageDataIndex < stageCfgList.Count && stageLoader != null && moveLoader != null)
                {
                    mapStageData.stageLoader = stageLoader;
                    mapStageData.moveLoader = moveLoader;
                    stageLoader.data = i;
                    stageLoader.visible = true;
                    stageLoader.touchable = true;
                    stageLoader.onClick.Add(OnClickStageLoader);
                    stageLoader.sortingOrder = 1;

                    mapStageData.posIndex = stageDataIndex;
                    mapStageData.posType = MapPosType.Stage;
                    mapStageData.mapStageId = stageCfgList[stageDataIndex].LevelId;
                    if (stageCfgList[stageDataIndex].LevelId % 5 == 0)
                    {
                        mapStageData.posType = MapPosType.Boss;
                        stageItem.visible = true;
                        stageItem.touchable = true;
                    }
                    stageDataIndex++;

                    ConfigStageUnit stageUnit = ConfigUtils.GetStageUnitByIdAndNode(mapStageData.mapStageId, 4);
                    stageItem.title3.text = stageItem.title2.text = stageItem.title1.text = stageItem.title = string.Format(ConfigUtils.GetTextById(stageUnit.Name), stageUnit.Chapter, stageUnit.LevelId % 100);
                }
            }
            else if (dataStageId == 3 || dataStageId == 0)  // 3 事件点 0 事件点隐藏
            {
                UI_stageItem stageItem = this._mapPanel.GetChild("stageEvent" + (randomEventIndex + 1)) as UI_stageItem;
                if (stageItem == null) { continue; }
                stageList.Add(stageItem);
                stageItem.visible = false;
                stageItem.touchable = false;
                GLoader stageLoader = stageItem.GetChild("icon") as GLoader;
                stageItem.titleBg.visible = false;
                stageItem.title3.text = stageItem.title2.text = stageItem.title1.text = stageItem.title = "";
                stageItem.bg.visible = false;
                stageItem.model.visible = false;
                stageItem.huntingTaskFlagIcon.visible = false;
                stageLoader.sortingOrder = 1;

                GLoader moveLoader = this._mapPanel.GetChild("posEvent" + (randomEventIndex + 1)) as GLoader;
                mapStageData.stageLoader = stageLoader;
                mapStageData.moveLoader = moveLoader;

                eventStageList.Add(mapStageData);

                if (dataStageId != 0 && randomEventIndex < stageEventList.Count && stageLoader != null && moveLoader != null)
                {
                    stageItem.visible = true;
                    stageItem.touchable = true;
                    RandomEventData taskData = stageEventList[randomEventIndex];
                    if (taskData.typeGroup == 2 || (taskData.eventType == (int)eRandomEventType.eRandomEventType_StageDropPet && taskData.batchStuffList.Count <= 0))
                    {
                        continue;
                    }
                    stageLoader.data = i;
                    stageLoader.touchable = true;
                    stageLoader.onClick.Add(OnClickStageLoader);

                    mapStageData.stageTaskData = taskData;
                    mapStageData.mapStageId = dataStageId;
                    mapStageData.posType = MapPosType.Event;

                    //stageLoader.grayed = false;
                    stageLoader.visible = true;
                    stageLoader.touchable = true;

                    if (taskData.eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Finished || taskData.batchStuffList.Count > 0 && taskData.batchStuffList[0].eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Finished) //完成灰态不能点击
                    {
                        //stageLoader.grayed = true;
                        if (taskData.eventType != (int)eRandomEventType.eRandomEventType_NpcTask)//狩猎任务
                        {
                            stageLoader.visible = false;
                            stageLoader.touchable = false;
                            stageItem.visible = false;
                            stageItem.touchable = false;
                        }
                    }

                    ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
                    //是委托任务
                    if (eventData == null)
                    {
                        Debug.LogError("事件数据是空的：" + taskData.eventId);
                        return;
                    }
                    if (stageLoader.visible)
                    {
                        stageItem.title3.text = stageItem.title2.text = stageItem.title1.text = stageItem.title = GameManager.Instance.GetTextNameByIdWithIndex(eventData.Name);
                        stageItem.titleBg.visible = true;
                    }

                    if (eventData.Type == (int)StageEventType.Task)
                    {
                        // stageItem.huntingTaskFlagIcon.visible = true;
                    }

                    if ((eventData.Type == (int)StageEventType.Pet || eventData.Type == (int)StageEventType.Task) && stageItem.visible)
                    {
                        stageLoader.url = UIResource.GetMapEventIcon(GetScene(chapterUnit.Scenes), eventData.Resource);
                        stageItem.InvalidateBatchingState();
                        stageLoader.alpha = 0;
                        
                        //stageLoader.visible = false;
                        stageItem.model.visible = true;
                        petEventGraph = stageItem.model;
                        stageItem.visible = false;
                        stageItem.touchable = false;
                        if (taskData.batchStuffList.Count > 0)
                        {
                            stageItem.visible = true;
                            stageItem.touchable = true;
                            // ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(taskData.batchStuffList[0].cfgId);
                            // int groupId = eventStageUnit.MonsterData;
                            // var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
                            // ConfigMonsterGroupUnit data = monsterGroupArr[0];
                            // ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);

                            string n = eventData.Type == (int)StageEventType.Task ? "Hero_30002" : "Hero_30001";
                            //会动的NPC
                            Utils.SetSpineModelOnFGUI(stageItem.model, n, 14f, HeroState.idle.ToString(), (o) =>
                            {
                                if (o is SkeletonAnimation animation)
                                    petEventSpine = animation;
                                
                                if (taskData.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
                                {
                                    Utils.SetSpineColor(petEventSpine); // "#24150E");
                                }
                                else
                                {
                                    Utils.SetSpineColor(petEventSpine, "#ffffff");
                                }
                            });
                        }

                        if (petEventSpine != null)
                        {
                            if (taskData.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
                            {
                                Utils.SetSpineColor(petEventSpine); // "#24150E");
                            }
                            else
                            {
                                Utils.SetSpineColor(petEventSpine, "#ffffff");
                            }
                        }
                    }
                    else
                    {
                        //stageLoader.url = UIResource.GetMapEventBgByEventId(eventData.Resource);
                        stageLoader.url = UIResource.GetMapEventIcon(GetScene(chapterUnit.Scenes), eventData.Resource);
                        stageLoader.alpha = 1f;

                        // 刷新当前容器及其子组件的布局
                        // 或触发全局布局更新
                        //GRoot.inst.UpdateSelf();
                    }
                }
                else
                {
                    mapStageData.posType = MapPosType.Normal;
                    mapStageData.stageLoader = null;
                }
                randomEventIndex++;
            }
            stageDataList.Add(mapStageData);
        }

        // 下一章节 数据
        MapStageData mapStageData1 = new MapStageData();
        mapStageData1.posType = MapPosType.NextChapter;
        mapStageData1.stageLoader = ((UI_stageItem)this.stageNext).GetChild("icon") as GLoader; ;
        mapStageData1.moveLoader = this.posNext;
        stageDataList.Add(mapStageData1);
        mapStageData1.stageLoader.data = stageDataList.Count - 1;
        mapStageData1.stageLoader.touchable = true;
        mapStageData1.stageLoader.onClick.Add(OnClickStageLoader);
        stageList.Add(mapStageData1.stageLoader);
        stageList.Add(this.posNext);

        var stage2 = ((UI_stageItem)this.stageNext);
        if (stage2 != null)
        {
            stage2.titleBg.visible = false;
            stage2.title = "";
            stage2.bg.visible = false;
            stage2.model.visible = false;
        }
        // 上一章节 数据
        MapStageData mapStageData2 = new MapStageData();
        mapStageData2.posType = MapPosType.LastChapter;
        mapStageData2.stageLoader = this.btnLastChapter.GetChild("icon") as GLoader;
        mapStageData2.moveLoader = this.posLast;
        stageDataList.Add(mapStageData2);
        mapStageData2.stageLoader.data = stageDataList.Count - 1;
        mapStageData2.stageLoader.touchable = true;
        mapStageData2.stageLoader.onClick.Add(OnClickStageLoader);
        stageList.Add(mapStageData1.stageLoader);
        stageList.Add(this.posLast);

        var stage1 = (UI_stageItem)this.btnLastChapter;
        if (stage1 != null)
        {
            stage1.titleBg.visible = false;
            stage1.title = "";
            stage1.bg.visible = false;
            stage1.model.visible = false;
        }

        //SetBossStageShow();
    }

    private void InitHero(string aniName = "idle")
    {
        try
        {
            Vector2 spawnPosition = Vector2.zero;
            string curPlayerUID = DataManager.Instance.GetRoleData().userID;
            string posXKey = GameManager.Instance.DownLineHeroPosX + curPlayerUID;
            string posYKey = GameManager.Instance.DownLineHeroPosY + curPlayerUID;
            // 优先使用保存的位置
            if (UnityEngine.PlayerPrefs.HasKey(posXKey))
            {
                float posX = UnityEngine.PlayerPrefs.GetFloat(posXKey);
                float posY = UnityEngine.PlayerPrefs.GetFloat(posYKey);
                spawnPosition = new Vector2(posX, posY);
            }
            else
            {
                if (chapterPosType == MapPosType.NextChapter)
                {
                    spawnPosition = new Vector2(this.posLast.x, this.posLast.y);
                }
                else if (chapterPosType == MapPosType.LastChapter)
                {
                    spawnPosition = new Vector2(this.posNext.x, this.posNext.y);
                }
                else
                {
                    spawnPosition = new Vector2(this.posLast.x, this.posLast.y);
                }
            }

            //关卡战斗表现spine
            this.chapterMap.panelClip.panel.battleSpine.playing = false;
            this.chapterMap.panelClip.panel.battleSpine.frame = 0;
            this.chapterMap.panelClip.panel.battleSpine.visible = false;

            //初始化角色位置
            this.chapterMap.panelClip.panel.hero.SetXY(spawnPosition.x, spawnPosition.y);

            HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
            //if (_heroSpine != null)
            //{
            //    _heroSpine.state.SetAnimation(0, "idle", true);
            //    InitRoleData(spawnPosition);
            //    return;
            //}
            Utils.SetSpineModelOnFGUI(this.chapterMap.panelClip.panel.hero, myHero.HeroUnit.Model, 65f, aniName, (o) =>
            {
                if (o is SkeletonAnimation animation)
                {
                    _heroSpine = animation;
                }

                InitRoleData(spawnPosition);
            });
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("InitHero 英雄初始化出错");
            EngineBase.EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_STAGE_COMPLETE_RECV_SUCCESS);
        }
    }

    private void InitRoleData(Vector2 spawnPosition)
    {
        /*   隐藏关卡开启时，移动地图到隐藏关卡
        //是否显示隐藏关卡云 设置隐藏关卡
        SetBossStageShow();

        if (mapBossStageMoveIndex == -1)
        {
            // 使用实际生成位置初始化地图
            InitMapPosition(spawnPosition);
        }
        else
        {
            if (stageDataList[mapBossStageMoveIndex] != null && stageDataList[mapBossStageMoveIndex].stageLoader != null)
            {
                SaveMapBossStageMovePlayerPrefsData();
                moveToBossStage(spawnPosition, stageDataList[mapBossStageMoveIndex].stageLoader.parent.xy, stageDataList[mapBossStageMoveIndex].stageLoader.parent);
            }
        }
        */
        
        // 使用实际生成位置初始化地图
        InitMapPosition(spawnPosition);

        if (nextStageId != 0 || _jumpTypeEnum != JumpTypeEnum.Normal)
        {
            resourceLoadNum++;
            AutoFindObject();
        }

        LoadExploredData();  //加载探索迷雾数据

        ChkHeroRunJam();
    }

    // 初始化地图位置
    private void InitMapPosition(Vector2 heroPosition)
    {
        this.chapterMap.panelClip.panel.position = Vector2.zero;
        this.chapterMap.panelClip.panel.position -= new Vector3(heroPosition.x, heroPosition.y);
        this.chapterMap.panelClip.panel.position += new Vector3(GRoot.inst.width / 2, GRoot.inst.height / 2);
        this.chapterMap.panelClip.panel.xy = new Vector2(
            Mathf.Clamp(this.chapterMap.panelClip.panel.position.x, -this.walkMap.width + this.chapterMap.panelClip.width, 0),
            Mathf.Clamp(this.chapterMap.panelClip.panel.position.y,
                GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height,
                this.walkMap.height - this.chapterMap.panelClip.panel.height)
        );

        //范围限制
        if (this.chapterMap.panelClip.panel.position.x >= 0)
        {
            this.chapterMap.panelClip.panel.x = 0;
        }
        else if (this.chapterMap.panelClip.panel.position.x <= -this.walkMap.width + this.chapterMap.panelClip.panel.width)
        {
            this.chapterMap.panelClip.panel.x = -this.walkMap.width + this.chapterMap.panelClip.panel.width;
        }
        if (this.chapterMap.panelClip.panel.position.y <= (GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height))
        {
            this.chapterMap.panelClip.panel.y = GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height;
        }
        else if (this.chapterMap.panelClip.panel.position.y >= this.walkMap.height - this.chapterMap.panelClip.panel.height)
        {
            this.chapterMap.panelClip.panel.y = this.walkMap.height - this.chapterMap.panelClip.panel.height;
        }
    }

    /// <summary>
    /// 下一关卡要战斗的提示
    /// </summary>
    private void UpdatePassTip()
    {
        int guideIndex = -1;
        bool isPass = false;
        for (int i = 0; i < stageDataList.Count; i++)
        {
            if (stageDataList[i].posType == MapPosType.Stage || stageDataList[i].posType == MapPosType.Boss) //|| stageDataList[i].posType == MapPosType.Boss)
            {
                isPass = false;
                for (int j = 0; j < chapterMapData.stageIdslist.Count; j++)
                {
                    if (stageDataList[i].mapStageId == chapterMapData.stageIdslist[j])
                    {
                        isPass = true;
                        break;
                    }
                }
                if (!isPass)
                {
                    guideIndex = i;
                    break;
                }
            }
        }

        // 提示动效
        this.chapterMap.panelClip.panel.tipSpine.playing = false;
        this.chapterMap.panelClip.panel.tipSpine.frame = 0;
        this.chapterMap.panelClip.panel.tipSpine.visible = false;
        if ((stageDataList.Count - 2 - MapChapterManager.Instance.GetRandomEventList().Count) > chapterMapData.stageIdslist.Count)
        {
            if (guideIndex != -1)
            {
                this.chapterMap.panelClip.panel.tipSpine.visible = true;
                this.chapterMap.panelClip.panel.tipSpine.playing = true;
                this.chapterMap.panelClip.panel.tipSpine.xy = new Vector2(
                    stageDataList[guideIndex].stageLoader.parent.x - this.chapterMap.panelClip.panel.tipSpine.width * 0.5f + stageDataList[guideIndex].stageLoader.width * 0.5f,
                    stageDataList[guideIndex].stageLoader.parent.y - this.chapterMap.panelClip.panel.tipSpine.height
                    );
            }
        }

        bool isShowNextBtn = false;
        if (isPass) //关卡全部通过
        {
            //显示下一章节按钮
            isShowNextBtn = true;
            ConfigChapterUnit nextChapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId + 1);
            if (nextChapterUnit == null) //没有下一章节的数据那么不显示章节按钮
                isShowNextBtn = false;
        }
        this.stageNext.visible = isShowNextBtn;
        this.spineStageNext.visible = isShowNextBtn;
    }

    // 图集纹理加载完成
    IEnumerator CheckTextureReady()
    {
        if (walkMapTexture == null)
        {
            while (this.chapterMap == null || this.chapterMap.panelClip == null || this.chapterMap.panelClip.panel == null || this.walkMap == null ||
                   this.walkMap.texture == null || this.walkMap.texture.nativeTexture == null)
            {
                yield return null;
            }
            walkMapTexture = this.walkMap.texture.nativeTexture as Texture2D;//GetWalkMapTexture();
            aStarPathFinder = new AStarPathFinder(walkMapTexture.width, walkMapTexture.height, walkMapTexture, gridSizeTextture);
            //Debug.Log("重新加载地图后，清理金币和钻石");
            ClearResStorehouse();
            InitWalkPosList();
            InitNewWalkPosList();

            InitPileGoldEvent();//金币堆事件
            InitDiamondMineEvent();//钻石矿事件

            //开箱子砍树
            AllotTreasureItem();

            CheckHeroNearFishingPos(true);//钓鱼

            if (MapChapterManager.Instance.autoSkipFindCurStage)
            {
                MapChapterManager.Instance.autoSkipFindCurStage = false;
                OnClickFightCurStageBtn();
                UpdateFightCurStageBtnInfo();
            }

            chapterMap.Loading.visible = false;

            ChkGuideToTouch();//引导
        }

        LaodRandomBossAndPetEvent();

        if (nextStageId != 0 || _jumpTypeEnum != JumpTypeEnum.Normal)
        {
            resourceLoadNum++;
            AutoFindObject();
        }

        if (MapChapterManager.Instance.autoSkipFindCurStage)
        {
            MapChapterManager.Instance.autoSkipFindCurStage = false;
            OnClickFightCurStageBtn();
            UpdateFightCurStageBtnInfo();
        }

        
        ChkHeroRunJam();
    }
    /// <summary>
    /// 地图定位玩家
    /// </summary>
    private void HeroPositioning()
    {
        if (walkMap != null && !walkMap.isDisposed && chapterMap.panelClip.panel.hero != null && !chapterMap.panelClip.panel.hero.isDisposed && chapterMap.panelClip.panel.hero.displayObject != null)
        {
                
            var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
            Vector2 heroPos = walkMap.parent.GlobalToLocal(globalHeroPos);
            UpdateMapPosition(heroPos);//移动地图
        }
    }

    /// <summary>
    /// 检测卡住
    /// </summary>
    private void ChkHeroRunJam()
    {
        HeroPositioning();
        
        GameManager.Instance.TimerManager.ClearTimer(DelayHeroRunJam);
        GameManager.Instance.TimerManager.SetTimer(3.0f, DelayHeroRunJam);
    }

    /// <summary>
    /// 检测位置是否是可行走区域，如果不是，飞到传送门位置
    /// </summary>
    private void DelayHeroRunJam()
    {
        if (aStarPathFinder != null && walkMap != null && walkMap.displayObject != null && chapterMap.panelClip.panel.hero != null && chapterMap.panelClip.panel.hero.displayObject != null)
        {
            var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
            Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);
            if (!aStarPathFinder.isCanWalk(heroPos))
            //Vector2 pos = this.chapterMap.panelClip.panel.hero.xy;
            //if (!aStarPathFinder.isCanWalk(new Vector2(pos.x, pos.y - this.mapbg.position.y)))
            {
                //Debug.Log("坐标在非可行走区域");
                chapterMap.panelClip.panel.hero.xy = this.posLast.xy;
                HeroPositioning();
            }
            //else
            //{
            //    Debug.Log("坐标在可行走区域 1111111111111111111111");
            //}
        }
    }

    //预先计算出可以使用可以行走的生成随机坐标点
    private void InitWalkPosList()
    {
        walkableList.Clear();

        //需要间隔随机函数（X轴拆分N个，Y轴拆分N个）
        mapIntervalX = walkMapTexture.width / mapInterval;
        mapIntervalY = walkMapTexture.height / mapInterval;

        Vector2 pos;
        for (int x = 2; x < mapInterval - 2; x++)
        {
            for (int y = 2; y < mapInterval - 2; y++)
            {
                pos.x = x * mapIntervalX;
                pos.y = walkMapTexture.height - y * mapIntervalY;

                if (aStarPathFinder.isCanWalk(pos))
                {//可行走坐标
                    walkableList.Add(new Vector2(pos.x, pos.y));
                }
            }
        }

        //Debug.Log(mapInterval + "===可使用的点位数量：" + walkableList.Count + " | " + randomResPosList.Count);
    }

    //预先计算出可以使用可以行走的生成随机坐标点
    private void InitNewWalkPosList()
    {
        randomResPosListA.Clear();
        randomResPosListB.Clear();
        randomResPosList.Clear();

        if (!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickEquip) && chapterUnit.Id == 1 && !FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroClassic).Item1)
        {
            for (int i = 1; i <= 3; i++)
            {
                var obj = this.chapterMap.panelClip.panel.GetChild("FixedPos" + i);
                if (obj != null)
                {
                    randomResPosList.Add(obj.xy, true);
                    GuidePosList.Add(obj.xy);
                    obj.RemoveFromParent();
                    //引导的位置初始测试
                    //var node = UI_DiamondChip.CreateInstance();
                    //this.chapterMap.panelClip.panel.AddChild(node);
                    //node.xy = obj.xy;
                }
            }
        }

        int px, py, dis = 44, ix = 0, iy = 0;
        Vector2 pos, pos1, pos2, pos3, pos4;
        int exX = (int)(mapPXInterval * 0.5f);
        for (int x = mapPXInterval; x < walkMapTexture.width - mapPXInterval; x += mapPXInterval)
        {
            for (int y = mapPXInterval; y < walkMapTexture.height - mapPXInterval; y += mapPXInterval)
            {
                px = x + (iy % 2 == 0 ? exX : 0);
                py = walkMapTexture.height - y;

                pos.x = px;
                pos.y = py;
                if (aStarPathFinder.isCanWalk(pos))
                {//可行走坐标
                    Vector2 p = ConvertWalkMapLocalToParent(pos);
                    //可生成坐标
                    pos1.x = px - dis; pos1.y = py;
                    pos2.x = px + dis; pos2.y = py;
                    pos3.x = px; pos3.y = py - dis;
                    pos4.x = px; pos4.y = py + dis;
                    if (aStarPathFinder.isCanWalk(pos1)
                        && aStarPathFinder.isCanWalk(pos2)
                        && aStarPathFinder.isCanWalk(pos3)
                        && aStarPathFinder.isCanWalk(pos4)
                        && ChkAreEventPointsAround(p)
                        )
                    {
                        if (!randomResPosList.ContainsKey(p))
                        {
                            //测试用的节点（将所有可以生成的都生成出来）
                            //var node = UI_DiamondChip.CreateInstance();
                            //this.chapterMap.panelClip.panel.AddChild(node);
                            //node.xy = p;

                            randomResPosList.Add(p, true);
                            if (ix % 2 == 0 || iy % 2 == 0)
                            {//偶数
                                randomResPosListB.Add(p);
                                randomResTagsAB[p] = true;
                            }
                            else
                            {//奇数
                                randomResPosListA.Add(p);
                                randomResTagsAB[p] = false;
                            }
                        }
                    }
                }
                iy++;
            }
            ix++;
        }

        LogUtils.Log(mapInterval + "===可使用的点位数量：" + randomResPosListA.Count + " | " + randomResPosListB.Count);
    }
    /// <summary>
    /// 添加到坐标池子
    /// </summary>
    /// <param name="pos"></param>
    private void AddRandomResPosList(Vector2 pos)
    {
        //Debug.Log($"Add随机点 {randomResPosListA.Count} | {randomResPosListB.Count}");
        if (!randomResPosList.ContainsKey(pos)) { return; }
        randomResPosList[pos] = true;

        bool isB = false;
        if (randomResTagsAB.ContainsKey(pos))
        {//A、B表哪个
            isB = randomResTagsAB[pos];
        }
        else
        {//两个队列都不存在，也可能是引导的
            return;
        }
        if (isB)
        {//B表中
            if (!randomResPosListB.Contains(pos))
            {
                randomResPosListB.Add(pos);
            }
        }
        else
        {//A表中
            if (!randomResPosListA.Contains(pos))
            {
                randomResPosListA.Add(pos);
            }
        }

        LogUtils.Log($"ADD随机点位个数剩余:{randomResPosListA.Count},{randomResPosListB.Count}");
    }
    /// <summary>
    /// 移除可以使用的表中
    /// </summary>
    /// <param name="pos"></param>
    private void RemoveRandomResPosList(Vector2 pos)
    {
        //不在可以使用的范围内
        if (randomResPosList.ContainsKey(pos) && randomResPosList[pos] == false || !randomResTagsAB.ContainsKey(pos)) { return; }
        randomResPosList[pos] = false;

        if (!randomResTagsAB[pos] && randomResPosListA.Contains(pos))
        {
            randomResPosListA.Remove(pos);
            LogUtils.Log($"删除随机点 {randomResPosListA.Count} | {randomResPosListB.Count}");
            return;
        }
        else if (randomResTagsAB[pos] && randomResPosListB.Contains(pos))
        {
            randomResPosListB.Remove(pos);
            LogUtils.Log($"删除随机点 {randomResPosListA.Count} | {randomResPosListB.Count}");
            return;
        }
    }
    
    /// <summary>
    /// 跳转自动寻路到下个关卡
    /// </summary>
    private void AutoFindObject()
    {
        if (resourceLoadNum < 2) return;
        
        if (nextStageId != 0) //自动寻路到下个关卡
        {
            for (int i = 0; i < stageDataList.Count; i++)
            {
                if (stageDataList[i].mapStageId == nextStageId)
                {
                    nextStageId = 0;
                    EventContext context = new EventContext();
                    context.data = i;
                    stageDataList[i].stageLoader.onClick.Call(context);
                    break;
                }
            }
            if (nextStageId != 0)  //都找不到对应的下一关id，那么寻路到下一章按钮处
            {
                var globalPos = stageDataList[stageDataList.Count - 2].moveLoader.LocalToGlobal(Vector2.zero);
                Vector2 targetPos = walkMap.GlobalToLocal(globalPos);
                SetMoveData(targetPos, true);
            }
            nextStageId = 0;
            return;
        }

        JumpToTarget(_jumpTypeEnum);
    }
    
    private void UpdateFightCurStageBtnInfo()
    {
        int firstUnpassedStageId = -1;
        foreach (var stageData in stageDataList)
        {
            // 只处理普通关卡和Boss
            if (stageData.posType != MapPosType.Stage && stageData.posType != MapPosType.Boss)
                continue;

            // 检查是否已通过
            bool isPassed = chapterMapData.stageIdslist.Contains(stageData.mapStageId);

            if (!isPassed)
            {
                // 找到第一个未通关的关卡
                firstUnpassedStageId = stageData.mapStageId;
                break;
            }
        }

        if (firstUnpassedStageId == -1)
        {
            // 所有关卡已通过
            this.chapterMap.fightCurStageBtn.typeCtrl.selectedIndex = 1;// 下一章节
        }
        else
        {
            // 有未通过关卡
            this.chapterMap.fightCurStageBtn.typeCtrl.selectedIndex = 0;// 当前章节
            string chapter = chapterMapData.chapterId.ToString();
            string stage = (firstUnpassedStageId % 100).ToString();
            this.chapterMap.fightCurStageBtn.stageInfo.SetVar("chapter", chapter).SetVar("stage", stage).FlushVars();
        }

    }

    /// <summary>
    /// 自动寻路挑战第一个未通过的普通关卡
    /// </summary>
    public void OnClickFightCurStageBtn(int taskToBattleStageId = 0)
    {
        // 查找第一个未通过的普通关卡
        for (int i = 0; i < stageDataList.Count(); i++)
        {
            var stageData = stageDataList[i];

            // 只处理普通关卡
            if (stageData.posType != MapPosType.Stage && stageData.posType != MapPosType.Boss)
                continue;

            bool isTaskBattle = true;
            if (taskToBattleStageId != 0)
            {
                isTaskBattle = false;
                if (taskToBattleStageId == stageData.mapStageId)
                {
                    isTaskBattle = true;
                }
            }
            
            // 检查是否已通过
            bool isPassed = chapterMapData.stageIdslist.Contains(stageData.mapStageId);

            if (!isPassed && isTaskBattle)
            {
                // 找到第一个未通过的普通关卡，触发点击事件
                EventContext context = new EventContext();
                context.data = i; // 传递关卡索引
                stageData.stageLoader.onClick.Call(context);
                MapChapterManager.Instance.autoSkipFindCurStage = false;
                return;
            }

        }

        // 所有普通关卡都已通过时，寻路到下一章按钮
        ConfigChapterUnit nextChapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId + 1);
        if (nextChapterUnit != null)
        {
            // var globalPos = stageDataList[stageDataList.Count - 2].moveLoader.LocalToGlobal(Vector2.zero);
            // Vector2 targetPos = walkMap.GlobalToLocal(globalPos);
            // SetMoveData(targetPos, true);


            EventContext context = new EventContext();
            context.data = stageDataList.Count - 2; // 传递关卡索引
            stageDataList[stageDataList.Count - 2].stageLoader.onClick.Call(context);
            selectStageIndex = (int)stageDataList.Count - 2;
            MapChapterManager.Instance.autoSkipFindCurStage = true;
        }
        else
        {
            Debug.Log("已是最后一章");
        }
    }

    private void OnClickStageLoader(EventContext context)
    {
        // 如果正在砍树，则忽略点击
        if (_isCuttingTree || actionPlaying || walkMap == null) return;

        if (clickIntervalTime <= 0.6) return;
        clickIntervalTime = 0;
        
        GLoader clickedLoader = context.sender as GLoader;
        if (clickedLoader == null) { return; }
        // LogUtils.Log("=================index=" + clickedLoader.data);

        //int tag = (int)clickedLoader.data;
        for (int i = 0; i < stageDataList.Count; i++)
        {
            if (stageDataList[i].stageLoader != null && stageDataList[i].stageLoader.data == clickedLoader.data)
            {
                MapStageData mapStageData = stageDataList[i];
                // if (mapStageData.mapStageId == MapObjectManager.Instance.GuanKaStageId && DataManager.Instance.GetRoleData().battleStatus != (int)eBattleStatus.eBattleStatus_None)
                // {
                //     UIManager.Instance.ToastByKey(8034);  //点击已经在战斗的关卡，文字提示
                //     return;
                // }
                if (mapStageData != null && mapStageData.stageTaskData != null)
                {
                    if (mapStageData.stageTaskData.eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Finished) //完成灰态不能点击
                    {
                        UIManager.Instance.ToastByKey(5118); //随机任务已完成
                        return;
                    }
                }

                ClearMoveTweener();

                if (!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickGate))
                {
                    //GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickGate);
                    GuideManager.Instance.GuideNotTouch();
                }
                if (this.QiPao != null)
                {
                    this.QiPao.Dispose();
                    this.QiPao = null;
                }

                var item = ((UI_stageItem)clickedLoader.parent);
                item.bg.visible = true;
                //item.bg.width = item.width + 20f;
                //item.bg.height = item.height + 20f;
                item.bgEffect.Play(-1, 0, null);

                selectStageIndex = i;
                var globalPos = mapStageData.moveLoader.LocalToGlobal(Vector2.zero);
                globalPos.x += mapStageData.moveLoader.width * 0.5f;
                globalPos.y += mapStageData.moveLoader.height * 0.5f;
                Vector2 targetPos = walkMap.GlobalToLocal(globalPos);
                if(GuideManager.Instance.IsShowGuiding)
                {//隐藏手指
                    GuideManager.Instance.HideHandle();
                }
                SetMoveData(targetPos, true);
                if (walkMap != null)
                {
                    //光标
                    Vector2 parentLocalCurs = walkMap.parent.GlobalToLocal(globalPos);
                    // 设置光标位置
                    cursorSpine.SetXY(parentLocalCurs.x - cursorSpine.width, parentLocalCurs.y - cursorSpine.width);
                    cursorSpine.visible = true;
                    cursorSpine.animationName = "BigMap_djlj";
                    cursorSpine.loop = true;
                    cursorSpine.playing = true;
                }

                break;
            }
        }
    }

    #region 点击路径移动  //todo 点击路径移动
    private void OnClickWalkMap(EventContext context)
    {
        if (walkMapTexture == null || actionPlaying) return;

        if (clickIntervalTime <= 0.6) return;
        clickIntervalTime = 0;

        // 如果正在砍树，则忽略点击
        if (_isCuttingTree) return;

        if (context != null)
        {
            //Debug.Log("点击范围：" + context.inputEvent.position + " " + aStarPathFinder.isCanWalk(context.inputEvent.position));
            touchEventData = null;
        }
        ClearMoveTweener();
        
        // 获取点击位置（相对于屏幕的坐标）
        Vector2 screenPos = context != null ? context.inputEvent.position : touchEventPos;
        // 将点击位置转换为相对于 walkMap 的局部坐标
        Vector2 localPos = this.walkMap.GlobalToLocal(screenPos);


        // todo 点击透明区域
        int pixelX = (int)(localPos.x);
        int pixelY = walkMapTexture.height - (int)(localPos.y);

        Vector2 arrowPos = localPos;
        Vector2 testPos = Vector2.zero;
        if (!ChkMapPassable(pixelX, pixelY))//非行走区域
        {//试着找最靠近边界的点
            var hPos = this.chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
            float idx = gridSizeTextture;
            // float t;
            for (int i = 0; i < 300; i++)
            {
                arrowPos = GetPointAlongLine2D(screenPos, hPos, idx);
                arrowPos = this.walkMap.GlobalToLocal(arrowPos);
                testPos.x = arrowPos.x;
                testPos.y = walkMapTexture.height - (arrowPos.y);
                if (ChkMapPassable((int)testPos.x, (int)testPos.y))
                {
                    localPos = arrowPos;
                    pixelX = (int)localPos.x;
                    pixelY = walkMapTexture.height - (int)(localPos.y);
                    break;
                }
                idx += gridSizeTextture * 3;
            }
        }

        //Debug.Log("换算后 = " + testPos  + arrowPos + "    x =" + pixelX + " y = " + pixelY);
        //Vector2 g1 = walkMap.LocalToGlobal(arrowPos);
        //Vector2 p1 = walkMap.parent.GlobalToLocal(g1);
        //cursorSpine.SetXY(p1.x - cursorSpine.width, p1.y - cursorSpine.height);
        //cursorSpine.visible = true;
        ////播放cursorSpine
        //cursorSpine.animationName = "BigMap_djlj";
        //cursorSpine.loop = true;
        //cursorSpine.playing = true;

        if (!ChkMapPassable(pixelX, pixelY))//非行走区域
        {
            StopStageSelectAnimation();
            if (_heroSpine != null && _heroSpine.AnimationName != "idle")
            {
                _heroSpine.state.SetAnimation(0, "idle", true);
            }

            // 点击非行走区域提示
            UIManager.Instance.ToastByKey(8071);

            //隐藏光标和清理导航线
            cursorSpine.visible = false;
            cursorSpine.playing = false;
            ClearPathArrows();// 暂时注释

            return;
        }

        //光标
        Vector2 globalPosCursor = walkMap.LocalToGlobal(localPos);
        Vector2 parentLocalCursor = walkMap.parent.GlobalToLocal(globalPosCursor);
        //设置cursorSpine的位置
        cursorSpine.SetXY(parentLocalCursor.x - cursorSpine.width, parentLocalCursor.y - cursorSpine.height);
        cursorSpine.visible = true;
        //播放cursorSpine
        cursorSpine.animationName = "BigMap_djlj";
        cursorSpine.loop = true;
        cursorSpine.playing = true;
        cursorLocalPos = localPos;

        mineSelect.visible = false;

        selectStageIndex = -1;
        //touchEventPos = localPos;
        SetMoveData(localPos);
    }
    
    public static Vector2 GetPointAlongLine2D(Vector2 pointA, Vector2 pointB, float distanceFromA)
    {
        Vector2 direction = pointB - pointA;
        float totalDistance = direction.magnitude;

        if (totalDistance <= 0)
            return pointA;

        if (distanceFromA >= totalDistance)
            return pointB;

        Vector2 unitDirection = direction.normalized;
        return pointA + unitDirection * distanceFromA;
    }

    /// <summary>
    /// 检测坐标是否可以同行
    /// </summary>
    /// <returns></returns>
    private bool ChkMapPassable(int x, int y)
    {
        if (walkMapTexture)
        {
            Color pixel = walkMapTexture.GetPixel(x, y);
            return pixel.a > 0.5f;//像素点如果是非透明的说明可以行走
        }
        return false;
    }

    private void SetMoveData(Vector2 targetPos, bool isClickStage = false, bool isFromCutTree = false)
    {
        ClearPathArrows(); // 清除旧导航线  暂时注释

        // 如果正在砍树，则忽略移动请求
        if (_isCuttingTree || actionPlaying || aStarPathFinder == null) return;
        //若正在前往砍树点的途中，点击其他目标应中断砍树任务
        if (!isFromCutTree && currentCuttingData != null)
        {
            Debug.Log("取消原本的砍树任务，改为前往新地点");
            currentCuttingData = null;
        }

        SortResStorehouse();
        //chapterMap.panelClip.panel.hero.sortingOrder = chapterMap.panelClip.panel.numChildren;

        //角色位置点转为地图上的点
        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);

        currentMovePosIndex = 0;
        GTween.Kill(chapterMap.panelClip.panel.hero);

        posList = aStarPathFinder.FindPath(heroPos, targetPos);
        LogUtils.Log($"路径数量 = {posList}");
        if (posList != null && posList.Count > 0)
        {
            GeneratePathArrows(posList); // 生成新导航线   暂时注释

            StartMove(posList[currentMovePosIndex]);
            // 设置英雄动作状态
            // if (_heroSpine != null && _heroSpine.AnimationName != "run")
            //     _heroSpine.state.SetAnimation(0, "run", true);
        }
        else
        {
            cursorSpine.visible = false;
            cursorSpine.playing = false;
            if (isClickStage)  //点击同一个关卡
            {
                EndMove();
            }
            else
            {
                if (touchEventData != null)
                {
                    ChkDisToDiamond();
                }

                // 检查是否有待处理的砍树操作
                if (currentCuttingData != null && !_isCuttingTree)
                {
                    StartCuttingAnimation(currentCuttingData);
                }

                bool skipIdle = false;
                if (curFishingPosData != null)
                {
                    StartFishingAnimation();
                    skipIdle = true;
                }

                if (!skipIdle && _heroSpine != null && _heroSpine.AnimationName != "idle")
                {
                    _heroSpine.state.SetAnimation(0, "idle", true);
                }
            }
            // if (_heroSpine != null && _heroSpine.AnimationName != "idle")
            // {
            //     _heroSpine.state.SetAnimation(0, "idle", true);
            // }

        }

        //解锁迷雾  按路径的点顺序，自动解锁，速度
        if (!isLockMapFog)
        {
            GameManager.Instance.StopCoroutine("AwaitShowCloud");
            GameManager.Instance.StartCoroutine(AwaitShowCloud());
        }
    }

    private void StartMove(Vector2 point)
    {
        point *= gridSizeTextture; //乘上网格大小
        // 移动点转换
        var global = walkMap.LocalToGlobal(point);
        var local = this.chapterMap.panelClip.panel.hero.parent.GlobalToLocal(global);

        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.parent.GlobalToLocal(globalHeroPos);

        // 设置英雄动作状态和朝向
        if (_heroSpine != null && _heroSpine.AnimationName != "run")
        {
            _heroSpine.state.SetAnimation(0, "run", true);
            // Vector3 forward = local - (Vector2)heroPos;
            // _heroSpine.skeleton.ScaleX = forward.x >= 0 ? 1 : -1;
        }
        Vector3 forward = local - (Vector2)heroPos;
        _heroSpine.skeleton.ScaleX = forward.x >= 0 ? 1 : -1;

        //Debug.Log(_heroSpine.skeleton.ScaleX == 1 ? "面向右侧" : "面向右侧");

        tweener = this.chapterMap.panelClip.panel.hero.TweenMove(local, moveSpeed);
        tweener.SetEase(EaseType.Linear);


        //tweener.OnUpdate(() => UpdateMapPosition());  // 添加地图滚动更新
        tweener.OnComplete(() =>
        {
            UpdateMapPosition(heroPos); //移动地图
            // 移动到路径点后更新导航线
            Vector2 currentHeroPos = new Vector2(
                this.chapterMap.panelClip.panel.hero.x,
                this.chapterMap.panelClip.panel.hero.y
            );
            UpdatePathArrows(currentHeroPos);//暂时注释

            if(!runChkTag)
            {
                runChkTag = true;
                //检测是否周边有可以拾取的资源
                ChkPickupRangePileGold();
                //UpdateExploredArea(point/gridSizeTextture);  // 不延迟，直接更新

                //检测是否靠近传承BOSS或宠物
                CheckBigMaoObjectDist();

                //检测是否靠近钓位点
                CheckHeroNearFishingPos();

                //检测是否靠近箱子
                CheckNearTreasureItem();

                //检测引导范围
                ChkEventPosDis();

                GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
                {
                    runChkTag = false;
                });
            }
            currentMovePosIndex++;
            if (currentMovePosIndex < posList.Count)
            {
                // 检查是否在事件点附近
                OnNearStageEvent(local);

                if (tweener != null)
                {
                    StartMove(posList[currentMovePosIndex]);
                }
            }
            else
            {
                currentMovePosIndex = 0;
                EndMove();//(local);
            }
        });
    }

    private void EndMove()
    {
        // 完成后切换动画状态
        if (_heroSpine != null)
            _heroSpine.state.SetAnimation(0, "idle", true);

        // Debug.Log($"移动到节点 完成 x:{endPos.x}  y:{endPos.y}");
        // Debug.Log($"移动到节点 完成 x:{chapterMap.panelClip.panel.hero.x}  y:{chapterMap.panelClip.panel.hero.y}");

        // 保存英雄位置
        Vector2 heroPosition = new Vector2(
            this.chapterMap.panelClip.panel.hero.x,
            this.chapterMap.panelClip.panel.hero.y
        );
        GameManager.Instance.SaveHeroPosition(heroPosition);

        if (touchEventData != null)
        {
            ChkDisToDiamond();
        }
        //TODO 终点弹出据点弹窗
        MapStageData stageData = null;
        if (selectStageIndex != -1)
        {
            for (int i = 0; i < stageDataList.Count; i++)
            {
                if (selectStageIndex == i)
                {
                    stageData = stageDataList[i];
                    break;
                }
            }
        }

        if (stageData != null)
        {
            if (stageData.posType == MapPosType.Stage || stageData.posType == MapPosType.Boss)
            {
                List<ConfigStageUnit> stageUnits = ConfigUtils.GetStageUnitById(stageData.mapStageId);
                UIManager.Instance.ShowUIPanel("ChapterStageDetail", stageUnits[stageUnits.Count - 1], _jumpTypeEnum);
            }
            else if (stageData.posType == MapPosType.Event && _jumpTypeEnum == JumpTypeEnum.Normal)
            {
                // Debug.Log("======到达事件点=====");
                ShowEventStage(stageData);
            }
            else if (stageData.posType == MapPosType.NextChapter)
            {
                ChangeBigMap(MapPosType.NextChapter);
            }
            else if (stageData.posType == MapPosType.LastChapter)
            {
                ChangeBigMap(MapPosType.LastChapter);
            }
        }

        ClearMoveTweener();
        StopStageSelectAnimation();
        ClearPathArrows(); // 到达终点清除导航线   暂时注释

        // 隐藏光标
        if (cursorSpine != null)
        {
            cursorSpine.visible = false;
            cursorSpine.playing = false;
        }

        //检查是否远离钓位点附近
        CheckHeroNearFishingPos();

        //检查是否远离箱子附近
        CheckNearTreasureItem();

        // 检查是否有待处理的砍树操作
        if (currentCuttingData != null && !_isCuttingTree)
        {
            StartCuttingAnimation(currentCuttingData);
        }

        // 钓鱼
        if (curFishingPosData != null)
        {
            StartFishingAnimation();
        }

        //跳转 指引
        if (_jumpTypeEnum != JumpTypeEnum.Normal)
        {
            if (_jumpTypeEnum == JumpTypeEnum.MapRandomEvent)
            {
                MapStageData stageEvent = GetNearObjectByType(JumpTypeEnum.MapRandomEvent) as MapStageData;
                if (stageEvent != null)
                {
                    var globalPos = stageEvent.stageLoader.LocalToGlobal(Vector2.zero);
                    Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(globalPos);
                    
                    JumpManager.Instance.ShowFinger(_jumpTypeEnum, stageEvent.stageLoader, (int)logicScreenPos.x, (int)logicScreenPos.y);
                }
            }
        }
        
        // 保存探索数据
        SaveExploredData();
    }

    private void ClearMoveTweener()
    {
        selectStageIndex = -1;
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
        }

        if (clickBigMapObject != null)
        {
            clickBigMapObject = null;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_CLICK_EVENTS);
        }
    }

    //地图移动逻辑
    private void UpdateMapPosition(Vector3 startPos)
    {
        Vector2 targetPos = this.chapterMap.panelClip.panel.hero.position;
        float x = -targetPos.x + this.chapterMap.panelClip.width / 2.0f;

        float y = 0;
        if (targetPos.y <= 0) //超出panel
        {
            y = Math.Abs(targetPos.y) + this.chapterMap.panelClip.panel.height / 2.0f + this.chapterMap.panelClip.panel.hero.height / 2.0f + (GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height);
        }
        else
        {
            if (targetPos.y > this.chapterMap.panelClip.panel.height / 2.0f) //下半屏幕
            {
                y = GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height;
            }
            else //上半屏幕
            {
                y = this.chapterMap.panelClip.panel.height / 2.0f - Math.Abs(targetPos.y) + this.chapterMap.panelClip.panel.hero.height / 2.0f + (GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height);
            }
        }
        Vector2 clampedPos = new Vector2(
            Mathf.Clamp(x, -this.walkMap.width + this.chapterMap.panelClip.width, 0),
            Mathf.Clamp(y,
                GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height,
                this.walkMap.height - this.chapterMap.panelClip.panel.height)
        );

        // 6. 更新大地图位置
        this.chapterMap.panelClip.panel.TweenMove(new Vector2(clampedPos.x, clampedPos.y), moveSpeed)
            .SetEase(EaseType.Linear) // 设置为线性移动
            .OnComplete(() =>
            {
                // 移动完成回调
            });
    }

    /// <summary>
    /// 显示事件弹窗
    /// </summary>
    public void ShowEventStage(MapStageData stageData)
    {
        //测试事件点强制为某个事件
        //List<RandomEventData> ed = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_RandomBox);
        //List<RandomEventData> ed = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_AdventureBusinessMan);
        //List<RandomEventData> ed = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_StageDropPet);
        //stageData.stageTaskData = ed[0];
        RandomEventData taskData = stageData.stageTaskData;
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
        if (eventData == null)
        {
            Debug.Log(" eventData is null, Event is nul Id=" + taskData.eventId);
            return;
        }

        if (eventData.Type == (int)StageEventType.Task) //委托任务
        {
            // 是否达到完成次数上限
            int finishCount = MapChapterManager.Instance.GetFinishEventCount((int)StageEventType.Task);
            int upLimit = ConfigUtils.GetEventUnitByType((int)StageEventType.Task).Number;
            if (finishCount >= upLimit)
            {
                UIManager.Instance.ToastByKey(8003);
                return;
            }

            // 是否有正在进行中的任务
            if (TaskInfoManager.Instance.GetNpcTask() == null)
            {
                //没有
                UIManager.Instance.ShowUIPanel("ChapterTaskStageDetail", stageData.stageTaskData);
            }
            else
            {
                //有
                // UIManager.Instance.ShowUIPanel("HuntingTaskMain", stageData.stageTaskData.guid, TaskInfoManager.Instance.GetNpcTask().taskId);
                UIManager.Instance.ShowUIPanel("HuntingTaskMain", TaskInfoManager.Instance.GetNpcTask().taskId, true);
            }

            // UIManager.Instance.ShowUIPanel("ChapterTaskStageDetail", stageData);
        }
        // else if (eventData.Type == (int)StageEventType.Boss)// || eventData.Type == (int)StageEventType.Pet)
        // {
        //     // 游荡的BOSS   跑的宠物
        //     UIManager.Instance.ShowUIPanel("ChapterEventBossStageDetail", stageData, stuffId, isRandomBoss);
        // }
        else if (eventData.Type == (int)eRandomEventType.eRandomEventType_RandomBox //随机宝箱
                 || eventData.Type == (int)eRandomEventType.eRandomEventType_RuinsBuff // 遗迹建筑
                 || eventData.Type == (int)eRandomEventType.eRandomEventType_AdventureBusinessMan // 奇遇商人
                 || eventData.Type == (int)eRandomEventType.eRandomEventType_StageDropPet) // 搜寻宠物
        {
            if (eventData.Type == (int)eRandomEventType.eRandomEventType_StageDropPet)
            {
                if (taskData.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
                {

                    if (taskData.batchStuffList[0].eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Taking)
                    {
                        UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8066));
                        return;
                    } else if (taskData.batchStuffList[0].eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Finished)
                    {
                        UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8067));
                        return;
                    }
                }
            }
            UIManager.Instance.ShowUIPanel("ChapterEventStageDetail", stageData);
        }
    }

    /// <summary>
    /// 切换地图
    /// </summary>
    private void ChangeBigMap(MapPosType mapPosType)
    {
        chapterPosType = mapPosType;
        isChangeMap = true;
        if (_mapPanel != null)
        {
            _mapPanel.Dispose();
            _mapPanel = null;
            walkMap = null;
            mapbg = null;
            btnLastChapter = null;
            spineLastChapter = null;
            spineStageNext = null;
            stageNext = null;
            posNext = null;
            posLast = null;
        }
        //Debug.Log("切换章节开始");
        ClearResStorehouse();//清理钻石和金币
        ClearTreasureDataByChangeChapter();//清理宝箱
        OnHide();

        if (mapPosType == MapPosType.NextChapter)
        {
            // Debug.Log("======到达下一章节点=====");
            ConfigChapterUnit nextChapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId + 1);
            if (nextChapterUnit != null) //下一章节的数据不为空，那么进入下一章节
            {
                GameManager.Instance.OnNewChapterStart();//进入新章节清空位置数据
                MapChapterManager.Instance.SendEnterChapterMap(nextChapterUnit.Id);
                GameManager.Instance.StopCoroutine("AwaitShowCloud");
                ClearCurrentFog();
            }
        }
        else if (mapPosType == MapPosType.LastChapter)
        {
            // Debug.Log("======到达上一章节点=====");
            if (chapterMapData != null && chapterMapData.chapterId != 1) // 不是第一个章节，那么进入上一个章节
            {
                ConfigChapterUnit lastChapterUnit = ConfigUtils.GetChapterUnitById(chapterMapData.chapterId - 1);
                GameManager.Instance.OnNewChapterStart();//进入新章节清空位置数据
                MapChapterManager.Instance.SendEnterLastChapterMap(lastChapterUnit.Id);
                GameManager.Instance.StopCoroutine("AwaitShowCloud");
            }
        }

    }

    private void StopRoleMove()
    {
        selectStageIndex = -1;
        EndMove();
    }

    /// <summary>
    /// 暂停选中动画
    /// </summary>
    private void StopStageSelectAnimation()
    {
        foreach (var stageData in stageDataList)
        {
            if (stageData.stageLoader != null && stageData.stageLoader.parent is UI_stageItem)
            {
                ((UI_stageItem)stageData.stageLoader.parent).bg.visible = false;
                ((UI_stageItem)stageData.stageLoader.parent).bgEffect.Stop();
            }
        }
    }
    #endregion

    private void UpdateRandomEventData()
    {//和策划确认过，1种类型只会存在一个
        rdGold_EventData = null;
        rdDiamond_EventData = null;
        randomTaskList = MapChapterManager.Instance.GetRandomEventList();
        eRandomEventType type = eRandomEventType.eRandomEventType_RandomGold;
        //获取对应的随机事件数据（方便下次使用）
        foreach (var item in randomTaskList)
        {
            type = (eRandomEventType)item.eventType;
            switch (type)
            {
                case eRandomEventType.eRandomEventType_RandomGold:
                    rdGold_EventData = item;
                    break;
                case eRandomEventType.eRandomEventType_RandomDiamond:
                    rdDiamond_EventData = item;
                    break;
            }
        }

        mineSelect.visible = false;
    }

    /// <summary>
    /// 玩家到达金币事件
    /// </summary>
    private void HeroToPileGoldEvent(StorehouseData data)
    {
        if(data.com == null || data.com.isDisposed) { return; }
        int eventType = MapChapterManager.Instance.GetEventTypeByGuid(data.guId);
        if (eventType == -1)
        {//过期渐变消失
            RandomEventOver(data.eventType);
            GameManager.Instance.TimerManager.SetTimer(1.0f, () =>
            {
                RemoveResStorehouseByType(eRandomEventType.eRandomEventType_RandomGold);
                actionPlaying = false;
            });
            mineSelect.visible = false;
            return;
        }
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(data.eventId);
        //到达每日上限
        if (eventData != null && MapChapterManager.Instance.GetFinishEventCount((int)eRandomEventType.eRandomEventType_RandomGold) >= eventData.Number)
        {
            UIManager.Instance.ToastByKey(8003);
            return;
        }
        //发包给服务端，通知领取
        var msg = ClaimRandomFinance_CS.CreateBuilder();
        msg.EventGuid = data.guId;//事件guid
        msg.BatchStuffId = (uint)data.batchStuffId;//金币堆或钻石堆id
        BatchStuff bs = GetRdmGoldBatchStuffById(data.batchStuffId);
        int count = bs != null ? bs.amount : Random.Range(2, 9);
        Debug.Log($"发送领取金币协议==事件guid = {data.guId} 金币堆id = {data.batchStuffId} 数量={count} 类型={data.eventType}");
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimRandomFinance_CS, msg.Build());

        mineSelect.visible = false;
        //动画
        RewardResGetShow(0, data, data.com.position);

        ClearGoldAndDiaData(data);
    }

    /// <summary>
    /// 获取单个金币堆的数据
    /// </summary>
    /// <param name="batchStuffId"></param>
    /// <returns></returns>
    private BatchStuff GetRdmGoldBatchStuffById(int batchStuffId)
    {
        if (rdGold_EventData == null) { return null; }
        foreach (var item in rdGold_EventData.batchStuffList)
        {
            if (item.id == batchStuffId)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取单个钻石矿的数据
    /// </summary>
    /// <param name="batchStuffId"></param>
    /// <returns></returns>
    private BatchStuff GetRdmDimBatchStuffById(int batchStuffId)
    {
        if (rdDiamond_EventData == null) { return null; }
        foreach (var item in rdDiamond_EventData.batchStuffList)
        {
            if (item.id == batchStuffId)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 进行播放采集进度条播放//读条时间5秒客户端写死，读条完成后播放爆炸特效，钻石飞入货币栏（客户端制作，参考金币飞入），获得钻石奖励
    /// </summary>
    /// <param name="stageData"></param>
    private void PlayDiamondGather(StorehouseData data)
    {
        if(data.com == null || data.com.isDisposed) { return; }
        ClearMoveTweener();
        // 隐藏光标
        if (cursorSpine != null)
        {
            cursorSpine.visible = false;
            cursorSpine.playing = false;
        }

        int eventType = MapChapterManager.Instance.GetEventTypeByGuid(data.guId);
        if (eventType == -1)
        {//过期渐变消失
            RandomEventOver(data.eventType);
            GameManager.Instance.TimerManager.SetTimer(1.0f, () =>
            {
                RemoveResStorehouseByType(eRandomEventType.eRandomEventType_RandomGold);
                actionPlaying = false;
            });
            mineSelect.visible = false;
            return;
        }

        //发包给服务端，通知领取
        var msg = ClaimRandomFinance_CS.CreateBuilder();
        msg.EventGuid = data.guId;//事件guid
        msg.BatchStuffId = (uint)data.batchStuffId;//金币堆或钻石堆id
        BatchStuff bs = GetRdmGoldBatchStuffById(data.batchStuffId);
        int count = bs != null ? bs.amount : Random.Range(2, 9);
        Debug.Log($"发送采矿钻石协议==事件guid = {data.guId} 钻石堆id = {data.batchStuffId} 数量={count} 类型={data.eventType}");
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimRandomFinance_CS, msg.Build());

        //MapChapterManager.Instance.PauseMapObject();//暂停怪物移动

        UI_DiamondMine com = data.com as UI_DiamondMine;
        mineSelect.visible = false;
        var roundGather = com.bar;
        roundGather.visible = true;
        roundGather.SetPivot(0.5f, 0.5f, true);
        roundGather.max = 100;
        roundGather.value = 0;
        roundGather.text = "0%";
        roundGather.touchable = true;
        com.touch.alpha = 0f;
        com.touch.visible = false;
        data.com.asCom.AddChild(roundGather);
        roundGather.SetPosition(data.com.width * 0.5f, -data.com.height * 0.5f, 0);
        // 播放英雄砍树的Spine动画
        PlayHeroState("wakuang", data.com);
        float interval = 0.733f;
        float time = interval * 0.35f;
        float s = 100f / 5f;
        time *= s;
        interval *= s;
        
        WKTweener = GTween.To(0, 100, 5f).SetEase(EaseType.Linear)
        .OnUpdate((GTweener tweener) =>
        {
            if (tweener.value.x >= time)
            {
                time += interval;
                // 播放采矿音效（0.733s播放一次）
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralScoopDiamondSE);
                //Debug.Log($"音效 = {time}");
            }

            if (_heroSpine.AnimationName != "wakuang")
            {
                PlayHeroState("wakuang", data.com);
            }
            com.touch.alpha = 0f;
            com.touch.visible = false;
            roundGather.value = tweener.value.d;
            roundGather.text = Math.Floor(tweener.value.d).ToString() + "%";
        })
        .OnComplete(() =>
        {
            roundGather.Dispose();
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
            if (data.com != null)
            {
                //给钻石飞入
                RewardResGetShow(1, data, data.com.position);
            }
            actionPlaying = false;
            _heroSpine.state.SetAnimation(0, "idle", true);
        });

        ClearGoldAndDiaData(data);
    }

    //金币和钻石发送后数据就要清理
    private void ClearGoldAndDiaData(StorehouseData data)
    {
        if(data.com != null && data.com.displayObject != null)
        {
            allStorehouseObj.Remove(data.com);//移除排序
        }
        AddRandomResPosList(data.postion);//坐标还原到坐标池
        //移除数据
        foreach (var item in _resStorehouse)
        {
            if (item.Value == data)
            {
                _resStorehouse.Remove(item.Key);
                break;
            }
        }
        SaveResStorehousePos(data.eventType);//写入到缓存
        ChkAllResStorehouse();
    }
    /// <summary>
    /// 更新头像金币等
    /// </summary>
    private void UpdateRoleInfo()
    {
        if (IsShow() && IsOnStage())
        {
            ((UI_ComUserInfo)this.chapterMap.userInfo).UpdateUserInfo();
            this.chapterMap.zhuZaoChuiCur.icon = UIResource.GetItemUrl(zhuZaoChuiId.ToString());
            ((UI_BtnCurrency)this.chapterMap.zhuZaoChuiCur).txtValue.text = StringUtils.FormatCurrency(ItemInfoManager.Instance.GetItemCount(zhuZaoChuiId));
        }
    }


    #region 在事件关卡点附近 弹窗冒泡

    private bool isShowTalk = false;

    private void OnNearStageEvent(Vector2 nearPos)
    {
        if(UIContainerPaopao != null && !UIContainerPaopao.isDisposed && UIContainerPaopao.visible) { return; }
        GObject stageObjTalk = null;
        for (int i = 0; i < stageDataList.Count; i++)
        {   //改成只有宠物事件才冒泡
            if (stageDataList[i].moveLoader != null && stageDataList[i].moveLoader.parent.visible && stageDataList[i].stageTaskData != null && stageDataList[i].stageTaskData.eventType == (int)StageEventType.Pet)
            {
                var pos1 = walkMap.parent.LocalToGlobal(stageDataList[i].moveLoader.position);
                var pos2 = walkMap.parent.LocalToGlobal(nearPos);

                //TODO 测试
                if (Math.Abs((pos1 - pos2).magnitude) < 500)
                {
                    isShowTalk = true;
                    stageObjTalk = stageDataList[i].stageLoader;
                    break;
                }
            }
        }

        if (isShowTalk)
        {
            if (UIContainerPaopao == null)
            {
                UIContainerPaopao = UIPackage.CreateObject("CommonEx", "VillagePetTalk") as UI_VillagePetTalk;
                this.chapterMap.panelClip.panel.AddChild(UIContainerPaopao);
            }

            if (UIContainerPaopao != null)
            {
                UIContainerPaopao.xy = new Vector2(stageObjTalk.parent.x + stageObjTalk.width / 2, stageObjTalk.parent.y - 70f); // - stageObjTalk.height
                UIContainerPaopao.visible = false;

                MapStageData mapStageData = null;
                for (int i = 0; i < stageDataList.Count; i++)
                {
                    if (stageDataList[i].stageLoader == stageObjTalk)
                    {
                        mapStageData = stageDataList[i];
                        break;
                    }
                }

                if (mapStageData != null && mapStageData.stageTaskData != null)
                {
                    if (mapStageData.stageTaskData.eventType == (int)StageEventType.Pet && mapStageData.stageTaskData.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished)
                    {
                        if (mapStageData.stageTaskData.batchStuffList.Count > 0 && mapStageData.stageTaskData.batchStuffList[0].eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished)
                        {
                            ConfigBubbleUnit bubble = ConfigUtils.GetBubbleById(1001);
                            ShowTalk(ConfigUtils.GetTextById(bubble.Doc));
                        }
                    }
                }
            }

            stageObjTalk = null;
            isShowTalk = false;
        }
    }

    private void ShowTalk(string talk)
    {
        if (UIContainerPaopao != null && !UIContainerPaopao.visible)
        {
            UIContainerPaopao.visible = true;
            UIContainerPaopao.talkDes.text = talk;
            GameManager.Instance.TimerManager.ClearTimer(HideTalk);
            GameManager.Instance.TimerManager.SetTimer(3f, HideTalk);
        }
    }

    private void HideTalk()
    {
        UIContainerPaopao.visible = false;
    }
    #endregion

    #region 主线任务

    private void UpdateMainTaskInfo()
    {
        // 获取当前任务信息
        ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());

        // 检查任务信息是否有效
        if (taskUnit != null)
        {
            // 设置任务名字：示例：任务1
            this.chapterMap.mainComPandaInfo.taskName.text = ConfigUtils.GetTextById(taskUnit.Name, taskUnit.NameParam);
            // 设置奖励信息
            this.chapterMap.mainComPandaInfo.reward.text = StringUtils.FormatCurrency(taskUnit.Reward);

            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(1000);
            this.chapterMap.mainComPandaInfo.rewardItem.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
            ((UI_ItemCom)this.chapterMap.mainComPandaInfo.rewardItem).hasCount.selectedIndex = 0;
            ((UI_ItemCom)this.chapterMap.mainComPandaInfo.rewardItem).txtLv.text = taskUnit.Reward.ToString();
            ((UI_ItemCom)this.chapterMap.mainComPandaInfo.rewardItem).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;

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
                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.min = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.max = int.Parse(taskUnit.Param1);
                    this.chapterMap.mainComPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), int.Parse(taskUnit.Param1));
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 1;
                        // this.chapterMap.mainComPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                        // this.chapterMap.mainComPandaInfo.txtProgress2.SetVar("min", Math.Min(int.Parse(taskUnit.Param1), TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", taskUnit.Param1).FlushVars();
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                    }
                    else
                    {
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 0;
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), taskUnit.Param1);
                        // this.chapterMap.mainComPandaInfo.txtProgress.SetVar("min", Math.Min(int.Parse(taskUnit.Param1), TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", taskUnit.Param1).FlushVars();
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_PassStage:
                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 1;
                    string[] nameParam = taskUnit.TypeNameParam.Split('|');

                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.min = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.max = 1;
                    int num = 0;
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        num = 1;
                    }
                    this.chapterMap.mainComPandaInfo.taskBar.value = Mathf.Min(num, 1);

                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(taskUnit.Param1))
                    {
                        // this.chapterMap.mainComPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName),nameParam[0], nameParam[1]);
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), nameParam[0], nameParam[1]);
                    }
                    else
                    {
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), nameParam[0], nameParam[1]);
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_GenQualityEquip:
                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 1;
                    int quality = int.Parse(taskUnit.Param1);
                    string qualityName = EquipManager.Instance.GetQualityName((QualityType)quality);
                    this.chapterMap.mainComPandaInfo.taskBar.min = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.max = 1;
                    this.chapterMap.mainComPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), 1);
                    if (TaskInfoManager.Instance.GetCurTaskNum() >= 1)
                    {
                        // this.chapterMap.mainComPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                    }
                    else
                    {
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName);
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_CollectQualityEquips:
                case (int)eMainTaskType.eMainTaskType_CollectQualityPets:
                case (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips:
                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 0;
                    string[] param1 = taskUnit.Param1.Split(',');
                    int num1 = int.Parse(param1[0]);
                    int quality1 = int.Parse(param1[1]);
                    string qualityName1 = EquipManager.Instance.GetQualityName((QualityType)quality1);

                    this.chapterMap.mainComPandaInfo.taskBar.min = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.max = num1;
                    this.chapterMap.mainComPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), num1);

                    if (TaskInfoManager.Instance.GetCurTaskNum() >= int.Parse(param1[0]))
                    {
                        // this.chapterMap.mainComPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                        // this.chapterMap.mainComPandaInfo.txtProgress2.SetVar("min", Math.Min(num1, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num1.ToString()).FlushVars();
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                    }
                    else
                    {
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), num1, qualityName1);
                        // this.chapterMap.mainComPandaInfo.txtProgress.SetVar("min", Math.Min(num1, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num1.ToString()).FlushVars();
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
                case (int)eMainTaskType.eMainTaskType_QualitySkillReachLevels:
                    this.chapterMap.mainComPandaInfo.taskType.selectedIndex = 0;
                    string[] param = taskUnit.Param1.Split(',');
                    int quality2 = int.Parse(param[0]);
                    int num2 = int.Parse(param[1]);
                    string qualityName2 = EquipManager.Instance.GetQualityName((QualityType)quality2);

                    this.chapterMap.mainComPandaInfo.taskBar.min = 0;
                    this.chapterMap.mainComPandaInfo.taskBar.max = num2;
                    this.chapterMap.mainComPandaInfo.taskBar.value = Mathf.Min(TaskInfoManager.Instance.GetCurTaskNum(), num2);

                    if (TaskInfoManager.Instance.GetCurTaskNum() >= num2)
                    {
                        // this.chapterMap.mainComPandaInfo.typeName2.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                        // this.chapterMap.mainComPandaInfo.txtProgress2.SetVar("min", Math.Min(num2, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num2.ToString()).FlushVars();
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 1;
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                    }
                    else
                    {
                        this.chapterMap.mainComPandaInfo.typeName.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.TypeName), qualityName2, num2);
                        // this.chapterMap.mainComPandaInfo.txtProgress.SetVar("min", Math.Min(num2, TaskInfoManager.Instance.GetCurTaskNum()).ToString()).SetVar("max", num2.ToString()).FlushVars();
                        this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex = 0;
                    }
                    break;
            }

            if (this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex == 0)
            {
                this.chapterMap.mainComPandaInfo.showHandle.selectedIndex = 0;
            }

            //if (taskUnit.TaskStartGuideId > 0 || taskUnit.TaskCompleteGuideId > 0)
            //{
                //EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_SHOW_FORCE_GUIDE, this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex == 1 ? taskUnit.TaskCompleteGuideId : taskUnit.TaskStartGuideId, this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex == 1 ? 2 : 1);
            //}
        }
    }

    private void OnClickToGetMainTaskReward()
    {
        if (this.chapterMap.mainComPandaInfo.rewardCtrl.selectedIndex == 1)
        {
            var builder = ClaimTaskAward_CS.CreateBuilder();
            ClaimTaskAward_CS taskAwardCs = builder.Build();
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimTaskAward_CS, taskAwardCs);

            int diamond = Random.Range(2, 9);
            TaskRewardGetShow(diamond, 1);
        }
        else
        {
            // UIManager.Instance.ToastByKey(10173);
            JumpManager.Instance.ClickMainTaskJump();
        }
    }

    #endregion

    #region 委托任务信息
    //更新任务信息
    private void UpdateTaskInfo()
    {
        this.chapterMap.comPandaInfo.visible = false;
        npcTaskData = TaskInfoManager.Instance.GetNpcTask();
        if (npcTaskData == null)
            return;

        ConfigEventTaskUnit taskUnit = ConfigUtils.GetEventTaskUnitById(npcTaskData.taskId);
        if (taskUnit != null)
        {
            this.chapterMap.comPandaInfo.visible = true;
            // this.chapterMap.comPandaInfo.taskName.text = ConfigUtils.GetTextById(taskUnit.Name, taskUnit.Param);

            // 设置任务名字：示例：任务1
            // this.chapterMap.comPandaInfo.taskName.text = ConfigUtils.GetStringByKey(8010);
            // 设置任务进度：设置任务进度的最小值和最大值
            // this.chapterMap.comPandaInfo.txtProgress.visible = false;
            this.chapterMap.comPandaInfo.taskType.selectedIndex = 0;
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2001);
            this.chapterMap.mainComPandaInfo.rewardItem.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
            this.chapterMap.comPandaInfo.rewardItem.icon = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
            ((UI_ItemCom)this.chapterMap.comPandaInfo.rewardItem).hasCount.selectedIndex = 0;
            ((UI_ItemCom)this.chapterMap.comPandaInfo.rewardItem).txtLv.text = taskUnit.Reward;
            ((UI_ItemCom)this.chapterMap.comPandaInfo.rewardItem).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;

            if (taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets)
            {
                int num = int.Parse(taskUnit.Param.Split(',')[0]);
                int quality = int.Parse(taskUnit.Param.Split(',')[1]);
                string qualityName = EquipManager.Instance.GetQualityName((QualityType)quality);
                this.chapterMap.comPandaInfo.typeName.text = String.Format(ConfigUtils.GetTextById(taskUnit.Name), num, qualityName);
                // this.chapterMap.comPandaInfo.typeName2.text = String.Format(ConfigUtils.GetTextById(taskUnit.Name), num, qualityName);
                // this.chapterMap.comPandaInfo.txtProgress.SetVar("min", Mathf.Min(npcTaskData.progress, num).ToString()).SetVar("max", num.ToString()).FlushVars();
                // this.chapterMap.comPandaInfo.txtProgress.SetVar("min", Mathf.Min(npcTaskData.progress, num).ToString()).SetVar("max", num.ToString()).FlushVars();
                this.chapterMap.comPandaInfo.taskBar.barType.selectedIndex = 0;
                int progress = Mathf.Min(num, npcTaskData.progress);
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).min = 0;
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).max = num;
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).value = progress;


                // 判断任务是否完成
                if (npcTaskData.progress >= num)
                {
                    // 已完成将rewardCtrl设为 1
                    this.chapterMap.comPandaInfo.rewardCtrl.selectedIndex = 1;
                }
                else
                {
                    // 未完成将rewardCtrl设为 0
                    this.chapterMap.comPandaInfo.rewardCtrl.selectedIndex = 0;
                }
            }
            else
            {
                this.chapterMap.comPandaInfo.typeName.text = String.Format(ConfigUtils.GetTextById(taskUnit.Name), taskUnit.Param);
                // this.chapterMap.comPandaInfo.typeName2.text = String.Format(ConfigUtils.GetTextById(taskUnit.Name), taskUnit.Param);
                // this.chapterMap.comPandaInfo.txtProgress.SetVar("min", Mathf.Min(npcTaskData.progress, int.Parse(taskUnit.Param)).ToString()).SetVar("max", taskUnit.Param).FlushVars();
                // this.chapterMap.comPandaInfo.txtProgress.SetVar("min", Mathf.Min(npcTaskData.progress, int.Parse(taskUnit.Param)).ToString()).SetVar("max", taskUnit.Param).FlushVars();
                this.chapterMap.comPandaInfo.taskBar.barType.selectedIndex = 0;
                int progress = Mathf.Min(int.Parse(taskUnit.Param), npcTaskData.progress);
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).min = 0;
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).max = int.Parse(taskUnit.Param);
                ((UI_TaskBarExp)this.chapterMap.comPandaInfo.taskBar).value = progress;

                // 判断任务是否完成
                if (npcTaskData.progress >= int.Parse(taskUnit.Param))
                {
                    // 已完成将rewardCtrl设为 1
                    this.chapterMap.comPandaInfo.rewardCtrl.selectedIndex = 1;
                }
                else
                {
                    // 未完成将rewardCtrl设为 0
                    this.chapterMap.comPandaInfo.rewardCtrl.selectedIndex = 0;
                }
            }

        }
    }

    // 领取任务奖励
    private void OnClickToGetTaskReward()
    {
        if (this.chapterMap.comPandaInfo.rewardCtrl.selectedIndex == 1)
        {
            var builder = ClaimNPCTaskAward_CS.CreateBuilder();
            builder.TaskId = (uint)npcTaskData.taskId;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimNPCTaskAward_CS, builder.Build());

            // 暂时不需要
            int diamond = Random.Range(2, 9);
            TaskRewardGetShow(diamond, 2);//1为主线任务，2为狩猎任务
        }
        else
        {
            //已接未完成 点击弹窗提示：String.id=8004（是否放弃任务？
            // TipsManger.Instance.ShowMessagePopup(ConfigUtils.GetStringByKey(8004), "否", "是");
            // UIManager.Instance.ShowUIPanel("HuntingTaskMain", stageData.stageTaskData.guid, TaskInfoManager.Instance.GetNpcTask().taskId);
            UIManager.Instance.ShowUIPanel("HuntingTaskMain", TaskInfoManager.Instance.GetNpcTask().taskId, false);
        }

        if (GuideManager.Instance.IsShowGuiding)
            GuideManager.Instance.HideGuide();
    }

    private Vector2 _pos = Vector2.zero;
    // 任务奖励获得表现效果：需要进行有无金币或钻石的判断，有的话才表现出飘的效果---主线任务飘砖石，狩猎任务飘积分
    public void TaskRewardGetShow(double diamond, int type)
    {
        if (_pos == Vector2.zero)
        {
            // 主线
            if (type == 1)
            {
                GComponent mainComPanda = this.chapterMap.mainComPandaInfo;
                Vector2 centerLocal = new Vector2(mainComPanda.width / 2, mainComPanda.height / 2);
                _pos = this.chapterMap.mainComPandaInfo.LocalToGlobal(centerLocal);
            }

            // 狩猎
            if (type == 2)
            {
                GComponent mainComPanda = this.chapterMap.comPandaInfo;
                Vector2 centerLocal = new Vector2(mainComPanda.width / 2, mainComPanda.height / 2);
                _pos = this.chapterMap.comPandaInfo.LocalToGlobal(centerLocal);
            }
        }

        // 创建金币动画对象
        // UIItemsGain itemsGain1 = new UIItemsGain();

        // 获取金币和钻石显示控件
        // UI_ComCurrency comCurrency = (UI_ComCurrency)((UI_ComUserInfo)this.chapterMap.userInfo).comCurrency;

        // 应用金币来源到目标
        // itemsGain1.ApplyItemSourceToDestination(comCurrency.btnGold.asCom, 0);

        // 开始金币飘动动画
        // itemsGain1.StartItemFly(_pos, gold);

        if (type == 1)
        {
            UI_ComCurrency comCurrency = (UI_ComCurrency)((UI_ComUserInfo)this.chapterMap.userInfo).comCurrency;
            // 钻石
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(ConstDefine.Item_Diamond);
            UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase(); //new UIItemsGain();
            itemsGain2.ApplyItemSourceToDestination(comCurrency.btnDia.asCom, itemTypeUnit.Icon);
            itemsGain2.StartItemFly(_pos, diamond);
        }

        if (type == 2)
        {
            UI_ComHeadIcon comCurrency = (UI_ComHeadIcon)((UI_ComUserInfo)this.chapterMap.userInfo).headIcon;
            UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase(); //new UIItemsGain();
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(2001);
            itemsGain2.ApplyItemSourceToDestination(comCurrency.asCom, itemTypeUnit.Icon);
            itemsGain2.StartItemFly(_pos, diamond);
        }

        // 其他物品
        // UIItemsGain itemsGain3 = new UIItemsGain();
        // itemsGain3.ApplyItemSourceToDestination(((UI_ComUserInfo)this.chapterMap.userInfo).headIcon, 3);
        // itemsGain3.StartItemFly(_pos, 1);
    }

    /// <summary>
    /// 播放资源飞入动画
    /// </summary>
    /// <param name="type">0=金币,1=钻石</param>
    /// <param name="data">数据</param>
    /// <param name="starPos"></param>
    public void RewardResGetShow(int type, StorehouseData data, Vector2 startPos)
    {
        if(data.com == null || data.com.isDisposed) { return; }
        // 获取金币和钻石显示控件
        if (type == 0)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GoldPickupSE);
        }
        else if (type == 1)
        {
            GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.DiamondPickupSE);
        }

        var action = new UICoinEffect();
        action.mp.x = 0.25f;
        action.mp.y = -0.5f;
        action.root = this.chapterMap.panelClip.panel;
        BatchStuff bs = type == 0 ? GetRdmGoldBatchStuffById(data.batchStuffId) : GetRdmDimBatchStuffById(data.batchStuffId);
        int count = bs != null ? bs.amount : Random.Range(2, 9);
        StorehouseData sd = data;
        data.com.Dispose();
        data.com = null;

        action.callback = () =>
        {
            GObject playObj = null;
            
            //给钱逻辑
            if (data.eventType == eRandomEventType.eRandomEventType_RandomGold)
            {//金币
                var g = UI_GoldAddPlay.CreateInstance();
                g.node.num.text = "+" + count;
                playObj = g;
                g.t0.Play();
            }
            else if (data.eventType == eRandomEventType.eRandomEventType_RandomDiamond)
            {//钻石
                var g = UI_DiamondAddPlay.CreateInstance();
                g.node.num.text = "+" + count;
                playObj = g;
                g.t0.Play();
            }
            if (playObj != null)
            {
                playObj.touchable = false;
                playObj.SetPivot(0.5f, 0.5f, true);
                Vector2 p = this.chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
                p.x += playObj.width * 0.25f;
                p.y -= this.chapterMap.panelClip.panel.hero.height + 80;
                GRoot.inst.AddChild(playObj);
                playObj.xy = p / GRoot.contentScaleFactor;
                addCurrencyList.Add(playObj);
                GameManager.Instance.TimerManager.SetTimer(3f, () => {
                    addCurrencyList.Remove(playObj);
                    playObj?.Dispose();
                });
            }
        };
        action.ExplodeCoins(type, startPos, this.chapterMap.panelClip.panel.hero);
        _storehouseDataList.Add(action);
    }

    /// <summary>
    /// 放弃委托的任务
    /// </summary>
    private void OnDropTask()
    {
        var builder = DropNPCTask_CS.CreateBuilder();
        builder.TaskId = (uint)npcTaskData.taskId;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_DropNPCTask_CS, builder.Build());
    }
    #endregion

    #region 增益buff相关
    //更新buffUI
    private BuffData buffData;
    private ConfigAttrEnumerationUnit attrEnum;
    private GButton btnBuff;
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

        this.chapterMap.comBuff.buffList.numItems = _showBuffInfos.Count;
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

    private Vector2 _pos2 = Vector2.zero;
    // 任务奖励获得表现效果：需要进行有无金币或钻石的判断，有的话才表现出飘的效果
    public void GainBuffGetShow(GButton btnBuff, string buffIcon, double diamond)
    {
        if (_pos2 == Vector2.zero)
            _pos2 = new Vector2(Screen.width / 2, Screen.height / 2);
        // 创建金币动画对象
        // UIItemsGain itemsGain1 = new UIItemsGain();

        // 获取金币和钻石显示控件
        // UI_ComBuff comCurrency = ((UI_ComBuff)this.chapterMap.comBuff);

        UIItemsGain itemsGain2 = UIGainBasePool.CreateUIGainBase();
        itemsGain2.ApplyItemSourceToDestinationEx(btnBuff.asCom, buffIcon);
        // itemsGain2.ApplyItemSourceToDestinationEx(comCurrency.buff1.asCom, UIResource.GetAttrIconById("attr_1"));
        itemsGain2.StartItemFly(_pos2, diamond);
    }

    private void ShowGainBuffAni(bool isShow)
    {
        if (isShow)
        {
            if (claimBuff.buffInfo == null || claimBuff.buffInfo.battleAttrList.Count <= 0)
                return;
            for (int i = 0; i < claimBuff.buffInfo.battleAttrList.Count; i++)
            {
                buffData = claimBuff.buffInfo.battleAttrList[i];
                ConfigAttrEnumerationUnit attrEnum = ConfigUtils.GetAttrEnumerationById((int)buffData.battleAttr);
                btnBuff = this.chapterMap.comBuff.GetChild("buff" + (i + 1)) as GButton;
                if (btnBuff != null)
                {
                    GainBuffGetShow(btnBuff, UIResource.GetAttrIconById(attrEnum.Icon), 1);
                }
            }
        }
    }
    #endregion

    #region 隐藏BOSS关卡
    
    private GTweener moveMapToBossStageTween;
    private bool isStopMoveMap = false;
    private float moveMapDuration = 0.5f;
    /// <summary>
    /// 地图移动到隐藏BOSS节点再返回
    /// </summary>
    private void moveToBossStage(Vector2 startPos, Vector2 targetPos, GObject stageUI = null)
    {
        this.chapterMap.panelClip.panel.moveMapMask.visible = true;
        float x = -targetPos.x + this.chapterMap.panelClip.width / 2.0f;
        if (stageUI != null)
        {
            x = -targetPos.x + this.chapterMap.panelClip.width / 2.0f - stageUI.width / 2.0f;
        }

        float y = 0;
        if (targetPos.y < 0)
        {
            y = Math.Abs(targetPos.y) + this.chapterMap.panelClip.panel.height / 2.0f + this.chapterMap.panelClip.mask.height;
            if (stageUI != null)
            {
                y -= stageUI.height;
            }
        }
        else
        {
            if (targetPos.y > this.chapterMap.panelClip.panel.height / 2.0f) //下屏幕
            {
                y = GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height;
            }
            else //上半屏幕
            {
                y = this.chapterMap.panelClip.panel.height * 0.5f - Math.Abs(targetPos.y) + this.chapterMap.panelClip.mask.height;
                y = Mathf.Clamp(y,
                    GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height,
                    this.walkMap.height - this.chapterMap.panelClip.panel.height);
            }
        }

        moveMapToBossStageTween = GTween.To(startPos, new Vector2(x, y), moveMapDuration)
        .SetTarget(this.chapterMap.panelClip.panel)
        .SetEase(EaseType.QuadOut)
        .OnUpdate((tweener) =>
        {
            Vector2 currentPos = tweener.value.vec2;
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(currentPos.x, -this.walkMap.width + this.chapterMap.panelClip.width, 0),
                Mathf.Clamp(currentPos.y,
                    GRoot.inst.height - this.chapterMap.panelClip.panel.height - this.chapterMap.panelClip.mask.height,
                    this.walkMap.height - this.chapterMap.panelClip.panel.height)
            );

            this.chapterMap.panelClip.panel.SetXY(clampedPos.x, clampedPos.y);
        })
        .OnComplete(() =>
        {
            mapBossStageMoveIndex = -1;
            if (!isStopMoveMap)
            {
                isStopMoveMap = true;
                GameManager.Instance.TimerManager.SetTimer(moveMapDuration, () =>
                {
                    moveToBossStage(this.chapterMap.panelClip.panel.xy, startPos, this.chapterMap.panelClip.panel.hero);
                });
            }
            else
            {
                isStopMoveMap = false;
                InitMapPosition(new Vector2(targetPos.x, targetPos.y));
                this.chapterMap.panelClip.panel.moveMapMask.visible = false;
                if (moveMapToBossStageTween != null)
                {
                    moveMapToBossStageTween.Kill();
                    moveMapToBossStageTween = null;
                }
            }
        });
    }
    
    /// <summary>
    /// 移动地图到隐藏关卡索引
    /// </summary>
    private int mapBossStageMoveIndex = -1;
    private List<string> saveBossStageList = new List<string>();
    /// <summary>
    /// boss关卡隐藏设置  通过的关卡id数量来判断
    /// </summary>
    /// <param name="chapterUnit"></param>
    private void SetBossStageShow()
    {
        mapBossStageMoveIndex = -1;
        //隐藏boss关卡 云是否显示
        for (int i = 0; i < stageDataList.Count; i++)
        {
            if (stageDataList[i].posType == MapPosType.Boss)
            {
                ConfigStageUnit curStageUnit = ConfigUtils.GetStageUnitByIdAndNode(stageDataList[i].mapStageId, 4);
                if (curStageUnit != null)
                {
                    GLoader3D spineStage = this._mapPanel.GetChild("spineStage" + (stageDataList[i].posIndex + 1)) as GLoader3D;
                    if (spineStage != null)
                    {
                        spineStage.visible = true;
                    }
                    stageDataList[i].stageLoader.parent.visible = true;
                    stageDataList[i].stageLoader.parent.touchable = true;

                    bool isPass = false;
                    for (int j = 1; j < curStageUnit.Node; j++)
                    {
                        isPass = false;
                        foreach (var passStageId in chapterMapData.stageIdslist)  //通过的关卡
                        {
                            if ((stageDataList[i].mapStageId - j) == passStageId)
                            {
                                isPass = true;
                            }
                        }

                        if (!isPass)
                        {
                            //if (spineStage != null)
                            //{
                            //    spineStage.visible = false;
                            //}
                            //stageDataList[i].stageLoader.parent.visible = false;
                            //stageDataList[i].stageLoader.parent.touchable = false;
                            break;
                        }
                    }
                    //if (isPass && mapBossStageMoveIndex == -1) //改隐藏关卡的前四关都通过了,地图是否移动到隐藏关卡
                    //{
                    //    string data = UnityEngine.PlayerPrefs.GetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapBossStageMove, "");
                    //    if (string.IsNullOrEmpty(data))
                    //    {
                    //        mapBossStageMoveIndex = i;
                    //    }
                    //    else
                    //    {
                    //        mapBossStageMoveIndex = i;
                    //        saveBossStageList = data.Split(',').ToList();
                    //        foreach (var stageId in saveBossStageList)
                    //        {
                    //            if (stageId != "" && int.Parse(stageId) == stageDataList[i].mapStageId)
                    //            {
                    //                mapBossStageMoveIndex = -1;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }
    }

    private void SaveMapBossStageMovePlayerPrefsData()
    {
        if (mapBossStageMoveIndex == -1 || stageDataList[mapBossStageMoveIndex] == null || stageDataList[mapBossStageMoveIndex].stageLoader == null) return;

        string data = "";
        if (UnityEngine.PlayerPrefs.HasKey(DataManager.Instance.GetRoleData().userID + SaveKey.MapBossStageMove))
        {
            data = UnityEngine.PlayerPrefs.GetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapBossStageMove);
            if (data != "")
            {
                data = data + "," + stageDataList[mapBossStageMoveIndex].mapStageId;
            }
            else
            {
                data = stageDataList[mapBossStageMoveIndex].mapStageId.ToString();
            }
        }
        else
        {
            data = stageDataList[mapBossStageMoveIndex].mapStageId.ToString();
        }
        UnityEngine.PlayerPrefs.SetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapBossStageMove, data);
        UnityEngine.PlayerPrefs.Save();
    }

    #endregion
    
    #region 迷雾相关

    private int index = 0;
    IEnumerator AwaitShowCloud()
    {
        index = 0;
        yield return new WaitForSeconds(0);//(0.3f);
        while (posList != null && posList.Count > 0 && index < posList.Count && this.state == UIState.Show)
        {
            UpdateExploredArea(posList[index]);
            yield return new WaitForSeconds(moveSpeed);//(0.01f);//(moveSpeed);
            index++;
        }
    }

    #region 探索视野

    [Header("云层设置")]
    private GGraph _fogGraph;  //迷雾UI
    private Texture2D _fogTexture;  //迷雾纹理
    private Color[] _fogPixels;  // 迷雾图片 像素 列表 颜色值
    public float exploreRadius = 200f;  // 探索半径

    private Material _cloudMaterial;  //云层材质 shader

    private byte alphaScale = 100;

    private int textureScale = 10;
    // private int textureWidth = 216;
    // private int textureHeight = 320;

    public void InitFogMask()
    {
        //todo 测试
        // PlayerPrefs.SetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData, "");
        // PlayerPrefs.SetString(DataManager.Instance.GetRoleData().userID + SaveKey.exploredMapData+chapterUnit.Scenes, "");

        IsUnlockFog();
        if (isLockMapFog)
        {
            this.chapterMap.panelClip.panel.fogMask.visible = false;
            return;
        }

        this.chapterMap.panelClip.panel.fogMask.visible = true;
        _fogGraph = this.chapterMap.panelClip.panel.fogMask;

        // 创建迷雾纹理
        _fogTexture = new Texture2D((int)_fogGraph.width / textureScale, (int)_fogGraph.height / textureScale, TextureFormat.RGBA32, false);

        _fogPixels = new Color[(int)_fogGraph.width / textureScale * (int)_fogGraph.height / textureScale];

        // _fogTexture = new Texture2D((int)textureWidth, (int)textureHeight, TextureFormat.RGBA32, false);
        // _fogPixels = new Color[(int)textureWidth * (int)textureHeight];

        // 初始全黑(未探索)
        for (int i = 0; i < _fogPixels.Length; i++)
        {
            _fogPixels[i] = new Color(0, 0, 0, 1f);
        }

        _fogTexture.SetPixels(_fogPixels);
        _fogTexture.Apply();

        // _fogGraph.DrawRect(_fogTexture.width*10, _fogTexture.height*10, 0, Color.clear, Color.white);
        _fogGraph.DrawRect(_fogTexture.width * textureScale, _fogTexture.height * textureScale, 0, Color.clear, Color.white);
        _fogGraph.displayObject.graphics.texture = new NTexture(_fogTexture);

        // 设置 云层shader、材质
        ModelManager.Instance.LoadNormalMaterial("Materials/CloudMaterial", (go) =>
        {
            _cloudMaterial = go;

            //_cloudMaterial.SetTexture("_CloudTex", _fogTexture);
            // _cloudMaterial.SetFloat("_Speed", 1);
            _cloudMaterial.SetFloat("_Alpha", 1);

            _fogGraph.displayObject.material = _cloudMaterial;
            _fogGraph.blendMode = BlendMode.Screen;
        });

        // exploredGrid = new Byte[textureWidth * textureHeight]; 
        exploredGrid = new Byte[_fogTexture.width * _fogTexture.height];
        for (int i = 0; i < exploredGrid.Length; i++)
        {
            exploredGrid[i] = (byte)(1 * alphaScale);
        }
        lastPos = Vector2.zero;
        //风
        //Utils.ShowUIPrefab(this.chapterMap.panelClip.panel.fogMask, "UI_Eff_weather3", 100);
    }

    private Vector2 lastPos = Vector2.zero;

    /**/
    private void UpdateExploredArea(Vector2 pos, bool isLoad = false)
    {
        if (isLockMapFog || walkMap == null) return;

        // 转换坐标到纹理空间
        // Vector2 texPos = WorldToTexturePos(worldPos);

        Vector2 texPos;
        // 获取英雄位置  不传位置直接用角色的位置
        if (isLoad)
        {
            var globalPos = this.chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
            texPos = chapterMap.panelClip.panel.fogMask.GlobalToLocal(globalPos);
            texPos = new Vector2(texPos.x, chapterMap.panelClip.panel.fogMask.height - texPos.y);
        }
        else
        {
            if (chapterMap == null)
            {
                return;
            }
            pos *= gridSizeTextture;
            var global = walkMap.LocalToGlobal(pos);
            texPos = this.chapterMap.panelClip.panel.GlobalToLocal(global);
            texPos.x = pos.x;
            texPos.y = chapterMap.panelClip.panel.fogMask.height - pos.y;
        }

        float outerRadius = exploreRadius; // 外顶点半径
        float transitionWidth = outerRadius * 0.9f; // 过渡区宽度

        // 计算更新区域（方形范围）
        int startX = Mathf.Max(0, (int)(texPos.x - outerRadius - transitionWidth));
        int startY = Mathf.Max(0, (int)(texPos.y - outerRadius - transitionWidth));
        int endX = Mathf.Min(_fogTexture.width * textureScale - 1, (int)(texPos.x + outerRadius + transitionWidth));
        int endY = Mathf.Min(_fogTexture.height * textureScale - 1, (int)(texPos.y + outerRadius + transitionWidth));

        bool updated = false;

        // 更新迷雾像素
        for (int y = startY; y <= endY; y += textureScale)
        {
            for (int x = startX; x <= endX; x += textureScale)
            {
                int idx = (int)(y / textureScale) * _fogTexture.width + (int)(x / textureScale);
                Vector2 pixelPos = new Vector2(x, y);

                // 计算到六芒星边缘的距离
                float edgeDist = StarEdgeDistance(texPos, pixelPos, outerRadius);

                // 核心可见区域(星型内)
                if (edgeDist <= 0)
                {
                    if (_fogPixels[idx].a > 0)
                    {
                        _fogPixels[idx].a = 0;
                        updated = true;
                    }
                }
                // 过渡区域
                else if (edgeDist <= transitionWidth)
                {
                    float edgeAlpha = Mathf.Lerp(0, 1, edgeDist / transitionWidth);

                    if (edgeAlpha < _fogPixels[idx].a)
                    {
                        _fogPixels[idx].a = edgeAlpha;
                        updated = true;
                    }
                }

                UpdateExploredGrid(x, y, _fogPixels[idx].a);
            }
        }

        if (updated)
        {
            _fogTexture.SetPixels(_fogPixels);
            _fogTexture.Apply();
            //_fogGraph.InvalidateBatchingState();
        }
    }

    // 计算点到六芒星边缘的距离
    private float StarEdgeDistance(Vector2 center, Vector2 point, float radius)
    {
        Vector2 dir = (point - center).normalized;
        float dist = Vector2.Distance(center, point);

        // 计算当前角度（0-360度）
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // 六芒星有12个顶点（内外交替）
        float spikeAngle = 360f / 12f; // 30度一个顶点

        // 找到最近的两个顶点
        int segment = Mathf.FloorToInt(angle / spikeAngle);
        float angle1 = segment * spikeAngle;
        float angle2 = (segment + 1) * spikeAngle;

        // 计算顶点位置（内外交替）
        float radius1 = (segment % 2 == 0) ? radius : radius * 0.5f;
        float radius2 = ((segment + 1) % 2 == 0) ? radius : radius * 0.5f;

        Vector2 spike1 = center + new Vector2(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad)) * radius1;

        Vector2 spike2 = center + new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad)) * radius2;

        // 计算点到线段的距离
        return DistanceToLineSegment(point, spike1, spike2);
    }

    // 计算点到线段的距离
    private float DistanceToLineSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        Vector2 ap = p - a;

        float lengthSqr = ab.sqrMagnitude;
        float dot = Vector2.Dot(ap, ab);
        float t = Mathf.Clamp01(dot / lengthSqr);

        Vector2 projection = a + t * ab;
        return Vector2.Distance(p, projection);
    }

    #endregion

    #region 探索记录
    private byte[] exploredGrid; // 记录每个网格是否被探索

    private void UpdateExploredGrid(int x, int y, float alpha)
    {
        int idx = (int)(y / textureScale) * _fogTexture.width + (int)(x / textureScale);
        byte num = (byte)(alpha * alphaScale);
        exploredGrid[idx] = num;
    }

    // 保存探索进度
    public void SaveExploredData1()
    {
        if (isLockMapFog) return;

        // UnityEngine.PlayerPrefs.SetString(DataManager.Instance.GetRoleData().userID + SaveKey.exploredMapData + chapterUnit.Scenes, Convert.ToBase64String(exploredGrid));
        // UnityEngine.PlayerPrefs.Save();
        
        // int sizeInBytes = System.Text.Encoding.UTF8.GetByteCount(Convert.ToBase64String(exploredGrid));
        // Debug.Log("====================sizeInBytes=" + sizeInBytes);
    }

    public void GetFogSaveFilePath()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, DataManager.Instance.GetRoleData().userID + SaveKey.exploredMapData + chapterUnit.Scenes +"fogData.dat");
        //Debug.Log($"====GetFogSaveFilePath={saveFilePath}");
    }
    
    public void SaveExploredData()
    {
        if (isLockMapFog) return;
        
        try
        {
            using (FileStream stream = new FileStream(saveFilePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // 先写入数据长度
                writer.Write(exploredGrid.Length);
                // 再写入数据
                writer.Write(exploredGrid);
            }
            // Debug.Log($"网格数据保存成功: {saveFilePath}");
            // Debug.Log($"文件大小: {new FileInfo(saveFilePath).Length} 字节");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
        }
    }

    public byte[] LoadFogExploredData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("文件不存在: " + saveFilePath);
            return null;
        }

        try
        {
            using (FileStream stream = new FileStream(saveFilePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // 读取数据长度
                int dataLength = reader.ReadInt32();
                // 读取数据
                return reader.ReadBytes(dataLength);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return null;
        }
    }
    
    // 加载探索进度
    public void LoadExploredData()
    {
        LogUtils.Log("===测试==LoadExploredData====");
        if (isLockMapFog) return;

        // if (UnityEngine.PlayerPrefs.HasKey(DataManager.Instance.GetRoleData().userID + SaveKey.exploredMapData + chapterUnit.Scenes))
        // {
        //     string data = UnityEngine.PlayerPrefs.GetString(DataManager.Instance.GetRoleData().userID + SaveKey.exploredMapData + chapterUnit.Scenes);
        if (LoadFogExploredData() != null)
        {
            //string data = LoadFogExploredData();

            byte[] bytes = LoadFogExploredData();//Convert.FromBase64String(data);
            if (bytes.Length > 0)
            {
                LogUtils.Log("===测试==LoadExploredData 有探索数据，更新纹理====");
                // 更新exploredGrid
                exploredGrid = bytes.Clone() as byte[];

                // 根据exploredGrid更新纹理
                UpdateTextureFromGrid();
                return;
            }
        }

        LogUtils.Log("===测试==LoadExploredData  没探索过，除了自己的位置，其它都显示迷雾====");
        UpdateExploredArea(Vector2.zero, true);
    }

    private void UpdateTextureFromGrid()
    {
        bool textureChanged = false;
        bool isUnLock = false;
        for (int i = 0; i < exploredGrid.Length; i++)
        {
            _fogPixels[i].a = (float)exploredGrid[i] / alphaScale;
            if (_fogPixels[i].a > 0)
            {
                textureChanged = true;
            }
            else
            {
                isUnLock = true;
            }
        }
        Debug.Log($"=====更新纹理==UpdateTextureFromGrid==textureChanged=={textureChanged}==");
        if (textureChanged)
        {
            _fogTexture.SetPixels(_fogPixels);
            _fogTexture.Apply();
            if (!isUnLock)
            {
                LogUtils.Log("===测试==UpdateTextureFromGrid  数据都是没有解锁的数据，只显示自己位置的迷雾====");
                UpdateExploredArea(Vector2.zero, true);
            }
        }
    }

    #endregion

    /// <summary>
    /// 清除当前地图迷雾
    /// </summary>
    private void ClearCurrentFog()
    {
        string data = "";
        bool isExist = false;
        if (UnityEngine.PlayerPrefs.HasKey(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData))
        {
            data = UnityEngine.PlayerPrefs.GetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData);
            string[] datas = data.Split(',');
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] == chapterUnit.Scenes.ToString())
                {
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                data = data + "," + chapterUnit.Scenes;
            }
        }
        else
        {
            data = chapterUnit.Scenes.ToString();
        }

        if (!isExist)
        {
            UnityEngine.PlayerPrefs.SetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData, data);
            UnityEngine.PlayerPrefs.Save(); 
        }
    }
    
    /// <summary>
    /// 是否已经探索过
    /// </summary>
    /// <returns></returns>
    private bool IsUnlockFog()
    {
        isLockMapFog = false;
        if (UnityEngine.PlayerPrefs.HasKey(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData))
        {
            var data = UnityEngine.PlayerPrefs.GetString(DataManager.Instance.GetRoleData().userID + SaveKey.MapUnlockData);
            string[] datas = data.Split(',');
            foreach (var scenesId in datas)
            {
                if (!string.IsNullOrEmpty(scenesId))
                {
                    if (int.Parse(scenesId) == chapterUnit.Scenes)
                    {
                        isLockMapFog = true;
                        break;
                    }
                }
            }
        }
        return isLockMapFog;
    }

    #endregion

    #region 金币、钻石堆

    private void AddGoldRain(Vector2 pos)
    {
        var rain = UI_GoldRain.CreateInstance();
        rain.touchable = false;
        rain.SetPivot(0.5f, 0.5f, true);
        this.chapterMap.panelClip.panel.AddChild(rain);

        float w = Screen.width * 0.5f;
        float h = Screen.height * 0.5f;

        rain.xy = pos;

        float bl = rain.LocalToGlobal(Vector2.zero).y / Screen.height + 0.2f;
        bl = Math.Max(bl, 0.3f);
        bl = Math.Min(bl, 1f);

        rain.SetScale(bl * 0.77f, bl * 0.77f);
        rain.alpha = bl;
        rain.sortingOrder = this.chapterMap.panelClip.panel.numChildren + 100;
        goldRainList.Add(rain);
        Transition t = rain.GetTransition("t0");
        bl *= 1.3f;
        bl = Math.Min(bl, 0.90f);
        t.timeScale = bl;
        t.Play();

        GameManager.Instance.TimerManager.SetTimer(3.5f, () => {
            goldRainList.Remove(rain);
            rain?.Dispose();
        });
    }

    /// <summary>
    /// 播放金币下雨
    /// </summary>
    private void PlayGoldRain()
    {
        //检测事件是否已经完成
        List<RandomEventData> eventData = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_RandomGold);
        if (eventData.Count > 0 && eventData[0] != null)
        {
            Debug.Log("金币雨 状态 = " + eventData[0].eventStatus);
        }

        if (eventData.Count <= 0 || eventData[0].eventStatus >= (int)eRandomEventStatus.eRandomEventStatus_Finished)
        {
            return;
        }

        if (!IsShow())
        {
            playGoldRaining = 3;
            return;
        }
        if (playGoldRaining == 1)
        {
            playGoldRaining = 3;
            return;
        }
        if (playGoldRaining == 2)
        {
            return;
        }
        playGoldRaining = 2;
        UpdateRandomEventData();

        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.FairySendingFlowersSE);

        //在玩家的四周进行下雨，y轴越高，透明度越低，大小随机
        GTween.To(0, 50, 14f)
            .OnUpdate((GTweener tweener) =>
            {
                if (isChangeMap)
                {
                    tweener.Kill();
                    foreach (var item in goldRainList)
                    {
                        item.Dispose();
                    }
                    goldRainList.Clear();
                    playGoldRaining = 0;
                    return;
                }

                if(chapterMap.panelClip.panel.hero == null || this.chapterMap.panelClip.panel.hero.displayObject == null || this.mapbg == null || this.mapbg.displayObject == null)
                {
                    return;
                }

                if (goldRainList.Count < tweener.value.d)
                {
                    float w = Screen.width * 0.5f;
                    float h = Screen.height * 0.5f;
                    Vector2 pos = this.chapterMap.panelClip.panel.hero.xy;

                    pos.x += Random.Range(-w, w + 1);
                    pos.y += Random.Range(-h, h + 1);
                    if (aStarPathFinder.isCanWalk(new Vector2(pos.x, pos.y - this.mapbg.position.y)))
                    {
                        AddGoldRain(pos);
                    }
                }
            })
        .OnComplete(() =>
        {
            foreach (var item in goldRainList)
            {
                item.Dispose();
            }
            goldRainList.Clear();
            playGoldRaining = 0;
        });

        ///延迟生成
        GameManager.Instance.TimerManager.SetTimer(2.0f, () =>
        {
            InitPileGoldEvent();
        });
    }

    /// <summary>
    /// 钻石雨(美术说特效暂时不做了,但刷新还是要刷的)
    /// </summary>
    private void PlayDiamondRain()
    {
        List<RandomEventData> eventData = MapChapterManager.Instance.GetRandomEventListByType((int)eRandomEventType.eRandomEventType_RandomDiamond);
        if (eventData.Count <= 0 || eventData[0].eventStatus >= (int)eRandomEventStatus.eRandomEventStatus_Finished)
        {
            return;
        }
        if (!IsShow())
        {
            playDiamondRaining = 3;
            return;
        }
        if (playDiamondRaining == 1)
        {
            playDiamondRaining = 3;
            return;
        }
        if (playDiamondRaining == 2)
        {
            return;
        }
        playDiamondRaining = 2;

        UpdateRandomEventData();
        InitDiamondMineEvent();

        playDiamondRaining = 0;
    }

    /// <summary>
    /// 检测是否范围内有事件点
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ChkAreEventPointsAround(Vector2 p)
    {
        float dis = 0;
        //关卡点
        foreach (var item in stageList)
        {
            dis = Vector2.Distance(p, item.xy);
            if (p.x > item.x - item.width * 0.5f 
                && p.x < item.x + item.width * 0.5f 
                && p.y < item.y + item.height * 0.5f
                && p.y > item.y - item.height * 0.5f
                || dis < 230f
                )
            {
                return false;
            }
        }
        return true;
    }

    //private void pingPosCount(string tag)
    //{
    //    int n = 0;
    //    foreach (var v in randomResPosList)
    //    {
    //        if (v.Value)
    //        {
    //            n++;
    //        }
    //    }
    //    //Debug.Log($"{tag}==随机点位个数剩余:{randomResPosListA.Count},{randomResPosListB.Count}, {n}");
    //}

    private Vector2 GetTypePassablePos(int type = -1)
    {//最优先使用
        if (chapterUnit.Id != 1) { return Vector2.zero; }
        int idx = 0;
        if (type == (int)eRandomEventType.eRandomEventType_RandomEuip)
        {//挖宝箱，砍树
            idx = 0;
        }
        else if (type == (int)eRandomEventType.eRandomEventType_RandomGold)
        {//金币
            idx = 2;
        }
        else if (type == (int)eRandomEventType.eRandomEventType_RandomDiamond)
        {//钻石
            idx = 1;
        }

        Vector2 pos = this.GuidePosList[idx];
        if (pos != null && pos != Vector2.zero)
        {
            if (randomResPosList.ContainsKey(pos) && randomResPosList[pos])
            {
                randomResPosList[pos] = false;
                return pos;
            }
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 随机获取一个可以行走的生成点分割后的坐标
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRdmPassablePos(int type = -1)
    {
        var pos = Vector2.zero;

        if (type != -1 && chapterUnit.Id == 1 && !GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickEquip) && !FuncPreviewManger.Instance.GetFuncOpenState(FuncOpenType.HeroClassic).Item1)
        {//引导获取
            pos = GetTypePassablePos(type);
            if (pos != Vector2.zero)
            {
                return pos;
            }
        }

        //优先使用A表中的数据
        if (randomResPosListA.Count > 0)
        {
            int rdm = Random.Range(0, randomResPosListA.Count);
            pos = randomResPosListA.ElementAt(rdm);

            if (randomResPosList.ContainsKey(pos) && randomResPosList[pos] == true)
                randomResPosListA.RemoveAt(rdm);
            randomResPosList[pos] = false;

            if (ChkOverlap(pos))
            {
                Debug.Log($"重复随机点：{pos} 剩余数量 {randomResPosListA.Count}, {randomResPosListB.Count}");
                return GetRdmPassablePos();
            }
            else
            {
                LogUtils.Log($"获取随机点：{pos} 剩余数量 {randomResPosListA.Count}, {randomResPosListB.Count}");
                return pos;
            }
        }
        else
        {
            if (randomResPosListB.Count > 0)
            {
                int rdm = Random.Range(0, randomResPosListB.Count);
                pos = randomResPosListB.ElementAt(rdm);
                if (randomResPosList.ContainsKey(pos) && randomResPosList[pos] == true)
                    randomResPosListB.RemoveAt(rdm);
                randomResPosList[pos] = false;

                if (ChkOverlap(pos))
                {
                    Debug.Log($"重复随机点：{pos} 剩余数量 {randomResPosListA.Count}, {randomResPosListB.Count}");
                    return GetRdmPassablePos();
                }
                else
                {
                    LogUtils.Log($"获取随机点：{pos} 剩余数量 {randomResPosListA.Count}, {randomResPosListB.Count}");
                    return pos;
                }
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 检测坐标点是否已经被使用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ChkOverlap(Vector2 pos)
    {
        foreach (var com in allStorehouseObj)
        {
            if (com.data != null)
            {
                var sdata = com.data as StorehouseData;
                if (sdata != null && sdata.com.position.x == pos.x && sdata.com.position.y == pos.y)
                {
                    return true;
                }
                var tdata = com.data as TreasureItemData;
                if (tdata != null && tdata.pos == pos)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 创建单个金币堆组件
    /// </summary>
    /// <returns></returns>
    private GObject CreationOnePileGold(eRandomEventType type, Vector2? pos = null)
    {
        GComponent com = null;
        switch (type)
        {
            case eRandomEventType.eRandomEventType_RandomGold:
                var gold = UI_GoldMine.CreateInstance();
                gold.icon.url = UIResource.GetMapEventIcon(GetScene(chapterUnit.Scenes), (int)eRandomEventType.eRandomEventType_RandomGold);
                com = gold;
                break;
            case eRandomEventType.eRandomEventType_RandomDiamond:
                var dia = UI_DiamondMine.CreateInstance();
                dia.icon.url = UIResource.GetMapEventIcon(GetScene(chapterUnit.Scenes), (int)eRandomEventType.eRandomEventType_RandomDiamond);
                com = dia;
                break;
        }

        com.SetScale(1.15f, 1.15f);
        com.SetSize(100, 100);//设置尺寸
        com.SetPivot(0.5f, 0.5f, true);

        int mapBgIndex = this.chapterMap.panelClip.panel.GetChildIndex(_mapPanel);
        this.chapterMap.panelClip.panel.AddChildAt(com, mapBgIndex + 1);

        if (!allStorehouseObj.Contains(com))
        {
            allStorehouseObj.Add(com);
        }
        com.touchable = false;

        Vector2 p = pos ?? Vector2.zero;
        if (p != Vector2.zero)
        {
            //获取地图坐标
            com.xy = p;
        }

        //渐变出现
        //com.alpha = 0f;
        //com.TweenFade(1f, 2f);

        return com;
    }

    /// <summary>
    /// 初始化金币事件
    /// </summary>
    private void InitPileGoldEvent()
    {
        ReadResStorehousePos(eRandomEventType.eRandomEventType_RandomGold);
    }

    /// <summary>
    /// 初始化钻石事件
    /// </summary>
    private void InitDiamondMineEvent()
    {
        ReadResStorehousePos(eRandomEventType.eRandomEventType_RandomDiamond);
    }

    /// <summary>
    /// 检测是否有按钮事件
    /// </summary>
    private void ChkAddBtnEvent(StorehouseData data)
    {
        if (data.eventType == eRandomEventType.eRandomEventType_RandomGold)
        {//金币也要能点击，走中心位置
            UI_GoldMine com = data.com as UI_GoldMine;
            com.touchable = true;
            com.onClick.Add((EventContext context) =>
            {
                var glod = (context.sender as UI_GoldMine);
                mineSelect.xy = glod.xy;
                mineSelect.x -= glod.width * 0.5f;
                mineSelect.y -= glod.height * 0.5f;
                //强制移动到矿石的左右两边进行采集，y轴要水平线上
                MoveToDiamondMining(data, context.inputEvent.position);
                mineSelect.visible = true;
            });
        }
        else if (data.eventType == eRandomEventType.eRandomEventType_RandomDiamond)
        {//按钮事件
            UI_DiamondMine com = data.com as UI_DiamondMine;
            com.touchable = true;
            com.onClick.Add(OnClickDiamondMine);

            com.touch.SetPivot(0.5f, 0.5f, true);
            com.touch.touchable = true;
            com.touch.x = com.width * 0.5f;
            com.touch.y = -com.height * 0.2f;
            com.touch.onClick.Add(() =>
            {
                Debug.Log("点击铁镐");
                MoveToDiamondMining(data, Vector2.zero);
            });
            com.touch.visible = false;
        }
    }

    private void OnClickDiamondMine(EventContext context)
    {
        var dia = (context.sender as UI_DiamondMine);
        mineSelect.xy = dia.xy;
        mineSelect.x -= dia.width * 0.5f;
        mineSelect.y -= dia.height * 0.5f;
        
        //强制移动到矿石的左右两边进行采集，y轴要水平线上
        MoveToDiamondMining(dia.data as StorehouseData, Vector2.zero);
        mineSelect.visible = true;
    }
    
    private void MoveToDiamondMining(StorehouseData data, Vector2 pos)
    {
        int eventType = MapChapterManager.Instance.GetEventTypeByGuid(data.guId);
        if (eventType == -1)
        {//过期渐变消失
            RandomEventOver(data.eventType);
            GameManager.Instance.TimerManager.SetTimer(1.0f, () =>
            {
                RemoveResStorehouseByType(data.eventType);
                actionPlaying = false;
            });

            mineSelect.visible = false;
            return;
        }
        Vector2 postion = Vector2.zero;
        if (data.eventType == eRandomEventType.eRandomEventType_RandomGold)
        {
            postion = pos;
        }
        else if (data.eventType == eRandomEventType.eRandomEventType_RandomDiamond)
        {
            UI_DiamondMine com = data.com as UI_DiamondMine;
            //引导走路到边界
            touchEventPos = com.LocalToGlobal(Vector2.zero);
            float w = com.width * 0.5f * GRoot.contentScaleFactor;
            touchEventPos.y += com.height * 0.25f;

            postion = touchEventPos;
            if (chapterMap.panelClip.panel.hero.x < com.x)
            {
                postion.x -= w;
                if (!ChkCanScreenWalk(postion))
                {
                    postion.x += w + w;
                }
            }
            else
            {
                postion.x += w;
                if (!ChkCanScreenWalk(postion))
                {
                    postion.x -= w + w;
                }
            }
        }
        touchEventData = data;
        touchEventPos = postion;
        OnClickWalkMap(null);
    }

    /// <summary>
    /// 检测点击屏幕的坐标是否可以行走
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ChkCanScreenWalk(Vector2 pos)
    {
        // 将点击位置转换为相对于 walkMap 的局部坐标
        Vector2 localPos = this.walkMap.GlobalToLocal(pos);

        // todo 点击透明区域
        int pixelX = (int)(localPos.x);
        int pixelY = walkMapTexture.height - (int)(localPos.y);
        return ChkMapPassable(pixelX, pixelY);//非行走区域
    }
    /// <summary>
    /// 随机生成资源堆
    /// </summary>
    private void CreationPileResource(eRandomEventType type)
    {
        //Debug.Log($"创建类型 = {type}");
        //如果有移除金币堆的所有数据和节点
        RemoveResStorehouseByType(type);

        RandomEventData eventData = type == eRandomEventType.eRandomEventType_RandomGold ? rdGold_EventData : rdDiamond_EventData;

        //正是使用,等待服务端联调
        if (eventData == null) { return; }
        for (int i = 0; i < eventData.batchStuffList.Count; i++)
        {
            if (eventData.batchStuffList[i].eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished)
            {
                Vector2 pos = GetRdmPassablePos((int)type);
                if (pos == Vector2.zero)
                {
                    continue;
                }
                int x = (int)pos.x, y = (int)pos.y;

                if (_resStorehouse.ContainsKey(pos))
                {
                    Debug.Log("【重复位置】" + pos + " | 可以使用的点:" + randomResPosListA.Count + " | " + randomResPosListB.Count);
                    break;
                }

                var gold = CreationOnePileGold(type, pos);
                StorehouseData d = new StorehouseData();
                d.com = gold;
                d.postion = gold.position;
                d.batchStuffId = eventData.batchStuffList[i].id;
                d.guId = eventData.guid;
                d.eventId = eventData.eventId;
                d.eventType = type;
                ChkAddBtnEvent(d);
                gold.data = d;

                if (ChkNodeDis(gold))
                {
                    pos = GetRdmPassablePos();
                    if (pos == Vector2.zero)
                    {
                        continue;
                    }
                    d.postion = pos;
                    d.com.xy = pos;
                }
                _resStorehouse[pos] = d;
            }
        }

        //排序
        SortResStorehouse();

        //保存金币堆位置数据集
        SaveResStorehousePos(type);
    }

    /// <summary>
    /// 检测是否接触金币堆
    /// </summary>
    private void ChkPickupRangePileGold()
    {
        float dist = 0;
        foreach (var item in _resStorehouse)
        {
            if (item.Value.com == null || item.Value.com.isDisposed || item.Value.com.displayObject == null)//不能使用
            {
                continue;
            }
            if (item.Value.com.alpha < 1)
            {
                continue;
            }
            //控件对于屏幕的坐标
            Vector2 pos1 = item.Value.com.xy;
            Vector2 pos2 = this.chapterMap.panelClip.panel.hero.xy;

            dist = Vector2.Distance(pos1, pos2);

            switch (item.Value.eventType)
            {
                case eRandomEventType.eRandomEventType_RandomGold:
                    //Debug.Log($"{item.Value.batchStuffId} 金币距离 = {dist} 坐标 = {pos1} 英雄坐标 = {pos2}");
                    if (dist < pickupRange)
                    {
                        //Debug.Log("金币范围内");
                        //播放飞入动画
                        HeroToPileGoldEvent(item.Value);
                        return;//有一个就可以，可以减少性能消耗
                    }
                    break;
                case eRandomEventType.eRandomEventType_RandomDiamond:
                    if (dist < pickupRange)
                    {
                        //显示采集按钮
                        UI_DiamondMine com = item.Value.com as UI_DiamondMine;
                        com.touch.visible = true;
                    }
                    else
                    {
                        //隐藏采集按钮
                        UI_DiamondMine com = item.Value.com as UI_DiamondMine;
                        com.touch.visible = false;
                    }
                    break;
            }

        }
        return;
    }

    /// <summary>
    /// 点击钻石矿采集按钮
    /// </summary>
    private void OnDiamondMining(StorehouseData data)
    {
        if (actionPlaying || data.com == null) { return; }
        //Debug.Log("采矿开始");
        //检测每日次数是否到达上限
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(data.eventId);
        //到达每日上限
        if (eventData != null && MapChapterManager.Instance.GetFinishEventCount((int)data.eventType) >= eventData.Number)
        {
            UIManager.Instance.ToastByKey(8003);
            return;
        }
        //Debug.Log("开始挖钻石矿");
        if (data.eventType == eRandomEventType.eRandomEventType_RandomDiamond)
        {
            actionPlaying = true;
            UI_DiamondMine com = data.com as UI_DiamondMine;
            mineSelect.visible = true;
            mineSelect.xy = com.xy;
            mineSelect.x -= com.width * 0.5f;
            mineSelect.y -= com.height * 0.5f;

            com.touch.alpha = 0f;
            com.touch.visible = false;

            //开始采集，播放5秒的动画
            PlayDiamondGather(data);
        }
        else if (data.eventType == eRandomEventType.eRandomEventType_RandomGold)
        {
            //var dis = Vector2.Distance(chapterMap.panelClip.panel.hero.xy, data.com.xy);
            //Debug.Log($"距离 = {dis} 坐标 = {data.com.xy}");

            //foreach (var item in _resStorehouse)
            //{
            //    if(item.Value.com == data.com)
            //    {
            //        Debug.Log(data.batchStuffId + "有在列表中......" + data.com.alpha);
            //        break;
            //    }
            //}
            HeroToPileGoldEvent(data);
        }
    }

    //检测是否到达钻石矿附近
    private void ChkDisToDiamond()
    {
        if (touchEventData == null || touchEventData.com == null)
        {
            touchEventData = null;
            return;
        }
        //如果目标点是建筑范围内,可以进行采集
        Vector2 pos1 = touchEventData.com.LocalToGlobal(Vector2.zero);
        pos1.x += touchEventData.com.width * 0.5f;
        pos1.y += touchEventData.com.height * 0.5f;
        Vector2 localPos = this.walkMap.GlobalToLocal(pos1);
        float dist = Vector2.Distance(cursorLocalPos, localPos);
        if (dist < touchEventData.com.height * 1.2f)
        {
            OnDiamondMining(touchEventData);//直接采集
        }
        touchEventData = null;
    }

    //检测所有资源点是否需要修复
    private void ChkAllResStorehouse()
    {
        List<GObject> temp = new List<GObject>();
        bool isremove = false;
        foreach (var obj in allStorehouseObj)
        {//直接过滤一边坏掉的对象数据
            if (obj != null)
            {
                if (obj.displayObject != null)
                {
                    temp.Add(obj);
                }
                else
                {//坏了
                    isremove = false;
                    foreach (var item in _resStorehouse)
                    {//金币或钻石
                        if (item.Value.com == obj)
                        {
                            _resStorehouse.Remove(item.Key);
                            isremove = true;
                            break;
                        }
                    }
                    if (!isremove)
                    {
                        foreach (var item in treasureList)
                        {//宝箱
                            if (item.com == obj)
                            {
                                treasureList.Remove(item);
                            }
                        }
                    }
                    AddRandomResPosList(obj.position);
                }
            }
        }
        allStorehouseObj = new List<GObject>(temp);

        Dictionary<Vector2, bool> copyMap = new Dictionary<Vector2, bool>(randomResPosList);

        foreach (var p in randomResPosList)
        {
            isremove = true;//默认是空点
            foreach (var obj in allStorehouseObj)
            {
                if (p.Key == obj.xy)
                {
                    isremove = false;//有对象在使用
                    break;
                }
            }
            copyMap[p.Key] = isremove;
            if (randomResTagsAB.ContainsKey(p.Key))
            {
                if(!randomResTagsAB[p.Key])
                {//A表
                    if (isremove)
                    {//空地
                        if(!randomResPosListA.Contains(p.Key))
                            randomResPosListA.Add(p.Key);
                    }
                    else
                    {
                        if (randomResPosListA.Contains(p.Key))
                            randomResPosListA.Remove(p.Key);
                    }
                }
                else
                {
                    if (isremove)
                    {//空地
                        if (!randomResPosListB.Contains(p.Key))
                            randomResPosListB.Add(p.Key);
                    }
                    else
                    {
                        if (randomResPosListB.Contains(p.Key))
                            randomResPosListB.Remove(p.Key);
                    }
                }
            }
        }
        randomResPosList = copyMap;
    }
    /// <summary>
    /// 层级排序
    /// </summary>
    private void SortResStorehouse()
    {
        // 按Y轴排序（从低到高）
        allStorehouseObj.Sort((a, b) => a.y.CompareTo(b.y));

        // 分配排序顺序
        for (int i = 0, n = 1; i < allStorehouseObj.Count; i++)
        {
            if (!allStorehouseObj[i].isDisposed)
            {
                allStorehouseObj[i].sortingOrder = n;
                if (ChkNodeDis(allStorehouseObj[i]))
                {
                    var pos = GetRdmPassablePos();
                    if (pos != Vector2.zero)
                    {
                        var g = allStorehouseObj[i] as UI_GoldMine;
                        if (g != null && !g.isDisposed)
                        {
                            g.xy = pos;
                        }
                        var d = allStorehouseObj[i] as UI_DiamondMine;
                        if (d != null && !d.isDisposed)
                        {
                            d.xy = pos;
                        }
                        var t = allStorehouseObj[i] as UI_TreasureItem;
                        if (t != null && !t.isDisposed)
                        {
                            t.xy = pos;
                        }
                    }
                }
                n++;
            }
        }

        var boosList = MapChapterManager.Instance.GetMapObjects();
        int idx = 0;
        foreach (var d in boosList)
        {
            if (d.Value.bigMapObject != null && d.Value.bigMapObject.modelObj != null && !d.Value.bigMapObject.modelObj.isDisposed)
            {
                ++idx;
                d.Value.bigMapObject.modelObj.sortingOrder = allStorehouseObj.Count + idx;
            }
        }

        if (this.chapterMap.panelClip.panel.moveMapMask != null)
        {
            this.chapterMap.panelClip.panel.moveMapMask.sortingOrder = allStorehouseObj.Count + boosList.Count + 1;
        }

        if (this.chapterMap.panelClip.panel.hero != null)
        {
            this.chapterMap.panelClip.panel.hero.sortingOrder = this.chapterMap.panelClip.panel.numChildren + 1;
        }

        if (_fogGraph != null && !_fogGraph.isDisposed)
        {
            _fogGraph.sortingOrder = this.chapterMap.panelClip.panel.numChildren + 5000;
        }

        this.chapterMap.panelClip.panel.InvalidateBatchingState();
    }


    //检测是否与其他的控件靠的比较近
    private bool ChkNodeDis(GObject obj)
    {
        if (obj.isDisposed) { return false; }

        //if(randomResPosList.ContainsKey(obj.xy) && !randomResPosList[obj.xy])
        //{
        //    Debug.Log($"点重叠了！！！{obj} , {obj.position}");
        //    return true;
        //}

        var clist = chapterMap.panelClip.panel.GetChildren();
        foreach (var item in clist)
        {
            if (!item.isDisposed && obj != item && obj.x != 0 && obj.y != 0 && item.x != 0 && item.y != 0)
            {
                if (obj.position == item.position)
                {
                    Debug.Log($"点重叠了！！！{obj} , {item}, {obj.position}");
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 根据类型移除事件资源
    /// </summary>
    /// <param name="type"></param>
    private void RemoveResStorehouseByType(eRandomEventType type)
    {
        if (_resStorehouse.Count > 0)
        {
            //pingPosCount("01");
            Dictionary<Vector2, StorehouseData> _temp = new Dictionary<Vector2, StorehouseData>();
            foreach (var item in _resStorehouse)
            {
                if (item.Value.eventType == type)
                {
                    if (item.Value.com != null)
                    {
                        allStorehouseObj.Remove(item.Value.com);
                        AddRandomResPosList(item.Value.com.position);//坐标还原到坐标池
                        item.Value.com.Dispose();
                        item.Value.com = null;
                    }
                }
                else
                {
                    _temp.Add(item.Key, item.Value);
                }
            }
            //pingPosCount("02");
            _resStorehouse = _temp;

            if (_resStorehouse.Count > 0)
            {
                SaveResStorehousePos(type);//存储数据
            }

            //Debug.Log($"移除后剩余{_resStorehouse.Count}");
            //pingPosCount("03");
            List<GObject> temp = new List<GObject>();
            //检测是否有残留
            foreach (var item in allStorehouseObj)
            {
                var d = item.data as StorehouseData;
                if (d != null && type == d.eventType && MapChapterManager.Instance.GetEventTypeByGuid(d.guId) == -1)
                {
                    AddRandomResPosList(d.com.position);//坐标还原到坐标池
                    _resStorehouse.Remove(d.com.position);
                    d.com = null;
                    item.Dispose();
                }
                else
                {
                    temp.Add(item);
                }
            }
            allStorehouseObj = new List<GObject>(temp);
            //pingPosCount("04");
        }
    }

    /// <summary>
    /// 保存随机资源的位置
    /// </summary>
    private void SaveResStorehousePos(eRandomEventType type)
    {
        string key = "";
        switch (type)
        {
            case eRandomEventType.eRandomEventType_RandomGold:
                {
                    key = goldStorehouseJsonKey;
                }
                break;
            case eRandomEventType.eRandomEventType_RandomDiamond:
                {
                    key = diamondStorehouseJsonKey;
                }
                break;
        }
        StorehousePosSave savedata = new StorehousePosSave();
        foreach (var item in _resStorehouse)
        {
            if (item.Value.eventType == type)
            {
                StorehousePosSaveData d = new StorehousePosSaveData();
                d.postion = item.Value.postion;
                d.guId = item.Value.guId;
                d.eventId = item.Value.eventId;
                d.eventType = (int)item.Value.eventType;
                d.batchStuffId = item.Value.batchStuffId;
                savedata.data.Add(d);
            }
        }
        string json = JsonUtility.ToJson(savedata);
        string name = $"{DataManager.Instance.GetRoleData().userID}{key}{chapterMapData.chapterId}";
        UnityEngine.PlayerPrefs.SetString(name, json);
        UnityEngine.PlayerPrefs.Save();
    }

    /// <summary>
    /// 读取当前地图的所有随机资源的位置
    /// </summary>
    private void ReadResStorehousePos(eRandomEventType type)
    {
        string key = "";
        RandomEventData data = null;
        switch (type)
        {
            case eRandomEventType.eRandomEventType_RandomGold:
                {
                    key = goldStorehouseJsonKey;
                    data = rdGold_EventData;
                }
                break;
            case eRandomEventType.eRandomEventType_RandomDiamond:
                {
                    key = diamondStorehouseJsonKey;
                    data = rdDiamond_EventData;
                }
                break;
        }
        if (data == null) { return; }

        ChkAllResStorehouse();

        //正常的流程是读取缓存，将数据检测
        string name = $"{DataManager.Instance.GetRoleData().userID}{key}{chapterMapData.chapterId}";
        string json = UnityEngine.PlayerPrefs.GetString(name);

        //Debug.Log($"读取缓存:{json}");
        //json = "";
        if (json != "")
        {
            StorehousePosSave d = JsonUtility.FromJson<StorehousePosSave>(json);

            if (d.data.Count > 0)
            {
                int t = (int)type;
                foreach (var item in d.data)
                {
                    if (item.eventType == t)
                    {//对应的类型
                        if (item.guId == data.guid)
                        {//对应的guid
                            if (!data.batchStuffList.Any(d => d.id == item.batchStuffId))
                            {//不是服务端下发的数据
                                //RandomEventOver(type);
                                CreationPileResource(type);//重新生成新的金币堆
                                return;
                            }
                        }
                        else
                        {//唯一ID也对不上
                            //RandomEventOver(type);
                            CreationPileResource(type);//重新生成新的金币堆
                            return;
                        }
                    }
                }
                //如果全对的上，那就使用缓存的数据进行创建
                foreach (var item in d.data)
                {
                    if (item.guId == data.guid && item.eventType == t)
                    {//单个单个创建
                        CreateStorehouseBySave(item);
                    }
                }
                SaveResStorehousePos(type);//如果数据位置有变动需要再存储一次
                SortResStorehouse();
            }
            else
            {
                CreationPileResource(type);
            }
        }
        else
        {//重新生成
            CreationPileResource(type);
        }
    }
    /// <summary>
    /// 创建一个缓存的节点数据
    /// </summary>
    private void CreateStorehouseBySave(StorehousePosSaveData item)
    {
        StorehouseData sd = new StorehouseData();

        RemoveRandomResPosList(item.postion);
        //新建金币堆
        sd.com = CreationOnePileGold((eRandomEventType)item.eventType);

        //Debug.Log($"根据缓存创建节点 = {item.eventType}, pos={item.postion}");
        sd.postion = item.postion;
        sd.com.position = item.postion;
        sd.guId = item.guId;
        sd.eventId = item.eventId;
        sd.eventType = (eRandomEventType)item.eventType;
        sd.batchStuffId = item.batchStuffId;
        _resStorehouse[item.postion] = sd;
        sd.com.data = sd;
        ChkAddBtnEvent(sd);

        if (ChkNodeDis(sd.com))
        {
            var pos = GetRdmPassablePos();
            if (pos != Vector2.zero)
            {
                sd.com.xy = pos;
            }
        }
    }

    /// <summary>
    /// 移除所有资源堆
    /// </summary>
    private void ClearResStorehouse()
    {
        //Debug.Log("清理金币和钻石的所有节点对象 = " + _resStorehouse.Count);
        foreach (var item in _resStorehouse)
        {
            //allStorehouseObj.Remove(item.Value.com);
            AddRandomResPosList(item.Value.postion);//坐标还原到坐标池
            if (item.Value.com != null)
            {
                item.Value.com.Dispose();
                item.Value.com = null;
            }
        }
        _resStorehouse.Clear();
        allStorehouseObj.Clear();

        var children = this.chapterMap.panelClip.panel.GetChildren();
        foreach (var node in children)
        {
            var g = node as UI_GoldMine;
            if (g != null)
            {
                g.Dispose();
            }
            var d = node as UI_DiamondMine;
            if (d != null)
            {
                d.Dispose();
            }
        }
    }
    private void ClearResStorehouseData()
    {
        //pingPosCount("05");
        //foreach (var item in _resStorehouse)
        //{
        //allStorehouseObj.Remove(item.Value.com);
        //AddRandomResPosList(item.Value.pos);//坐标还原到坐标池
        //if(item.Value.com != null)
        //{
        //    item.Value.com.Dispose();
        //    item.Value.com = null;
        //}
        //}
        _resStorehouse.Clear();
        allStorehouseObj.Clear();
        //pingPosCount("06");
    }
    /// <summary>
    /// 随机事件结束
    /// </summary>
    /// <param name="type"></param>
    private void RandomEventOver(eRandomEventType type)
    {
        switch (type)
        {
            case eRandomEventType.eRandomEventType_RandomGold:
                GoldEventOver();
                break;
            case eRandomEventType.eRandomEventType_RandomDiamond:
                DiamondEventOver();
                break;
        }
    }

    /// <summary>
    /// 天女散花事件结束，金币堆消散
    /// </summary>
    private void GoldEventOver()
    {
        if (_resStorehouse.Count <= 0) { return; }
        playGoldRaining = 1;
        foreach (var item in _resStorehouse)
        {
            if (item.Value.eventType == eRandomEventType.eRandomEventType_RandomGold)
            {
                if (item.Value.com != null)
                {
                    item.Value.com.alpha = 1f;
                    item.Value.com.TweenFade(0f, 1f);
                }
            }
        }
        //延迟销毁
        GameManager.Instance.TimerManager.SetTimer(1.0f, () => {
            RemoveResStorehouseByType(eRandomEventType.eRandomEventType_RandomGold);
            if (playGoldRaining == 3)
            {
                PlayGoldRain();
            }
        });

    }

    /// <summary>
    /// 钻石矿事件结束，钻石矿消散
    /// </summary>
    private void DiamondEventOver()
    {
        if (_resStorehouse.Count <= 0) { return; }
        playDiamondRaining = 1;
        //Debug.Log($"playDiamondRaining = {playDiamondRaining}");
        foreach (var item in _resStorehouse)
        {
            if (item.Value.eventType == eRandomEventType.eRandomEventType_RandomDiamond)
            {
                if (item.Value.com != null)
                {
                    item.Value.com.alpha = 1f;
                    item.Value.com.TweenFade(0f, 1f);
                }
            }
        }
        //延迟销毁
        GameManager.Instance.TimerManager.SetTimer(1.0f, () =>
        {
            RemoveResStorehouseByType(eRandomEventType.eRandomEventType_RandomDiamond);
            if (playDiamondRaining == 3)
            {
                PlayDiamondRain();
            }
        });
    }

    //根据英雄与目标播放动画
    private void PlayHeroState(string state, GObject target)
    {
        // 播放英雄砍树的Spine动画
        if (_heroSpine != null)
        {
            _heroSpine.state.SetAnimation(0, state, true);

            // 调整英雄朝向
            Vector2 treePos = target.position;
            Vector2 heroPos = this.chapterMap.panelClip.panel.hero.position;
            if (heroPos.x < treePos.x) // 英雄在目标左侧
            {
                _heroSpine.skeleton.ScaleX = 1; // 面向右侧（目标在右侧）
            }
            else // 英雄在树右侧
            {
                _heroSpine.skeleton.ScaleX = -1; // 面向左侧（目标在左侧）
            }
        }
    }

    #endregion



    #region 导航线方法
    // 生成导航线
    private void GeneratePathArrows(List<Vector2> path)
    {
        ClearPathArrows();

        if (path == null || path.Count < 2) return;

        // 计算总路径长度
        float totalLength = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalLength += Vector2.Distance(path[i] * gridSizeTextture, path[i + 1] * gridSizeTextture);
        }

        // 根据总长度计算箭头数量
        int arrowCount = Mathf.CeilToInt(totalLength / ArrowSpacing);
        if (arrowCount == 0) return; // 路径太短不需要箭头

        // 计算每个箭头的间距
        float step = totalLength / arrowCount;
        float currentDistance = 0f;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 start = path[i] * gridSizeTextture;
            Vector2 end = path[i + 1] * gridSizeTextture;
            Vector2 direction = (end - start).normalized;
            float segmentLength = Vector2.Distance(start, end);

            while (currentDistance < segmentLength && _pathArrows.Count < arrowCount)
            {
                float t = currentDistance / segmentLength;
                Vector2 arrowPos = Vector2.Lerp(start, end, t);
                CreateArrowAt(arrowPos, direction);

                currentDistance += step;
            }

            currentDistance -= segmentLength; // 减去当前段长度，剩余距离在下一段继续
        }
    }

    // 创建单个箭头
    private void CreateArrowAt(Vector2 position, Vector2 direction)
    {
        UI_pathSpineItem arrow = (UI_pathSpineItem)UIPackage.CreateObject("BigMap", "pathSpineItem");

        arrow.pathSpine.animationName = "BigMap_xinglujing_1";
        arrow.pathSpine.playing = true;
        arrow.pathSpine.loop = true;

        // 随机大小效果
        float size = Random.Range(0.5f, 1f);
        arrow.pathSpine.SetScale(size, size);

        // 设置位置
        Vector2 parentPos = ConvertWalkMapLocalToParent(position);
        arrow.SetXY(parentPos.x, parentPos.y);

        _arrowContainer.AddChild(arrow);
        _pathArrows.Add(arrow);
    }

    // 清除所有箭头
    private void ClearPathArrows()
    {
        foreach (var arrow in _pathArrows)
        {
            _arrowContainer.RemoveChild(arrow);
            arrow.Dispose();
        }
        _pathArrows.Clear();
    }

    // 更新导航线（英雄移动时调用）
    private void UpdatePathArrows(Vector2 heroPos)
    {
        if (_pathArrows.Count == 0) return;

        // 检查英雄是否接近第一个箭头
        Vector2 firstArrowPos = new Vector2(_pathArrows[0].x, _pathArrows[0].y);
        float distance = Vector2.Distance(heroPos, firstArrowPos);

        if (distance < ArrowFollowThreshold)
        {
            // 移除第一个箭头
            _arrowContainer.RemoveChild(_pathArrows[0]);
            _pathArrows[0].Dispose();
            _pathArrows.RemoveAt(0);
        }
    }

    // 坐标转换：walkMap局部坐标 -> 父节点坐标
    public Vector2 ConvertWalkMapLocalToParent(Vector2 localPos)
    {
        return new Vector2(
            localPos.x + this.walkMap.x,
            localPos.y + this.walkMap.y
        );
    }


    #endregion

    #region 传承BOSS 搜寻宠物

    private Dictionary<string, BigMapObjectData> mapObjDic = new Dictionary<string, BigMapObjectData>();
    private void LaodRandomBossAndPetEvent()
    {
        if (this.state != UIState.Show) return;

        mapObjDic = MapChapterManager.Instance.GetMapObjects();
        foreach (var item in randomTaskList)
        {
            if (item.eventType == (int)StageEventType.Pet)
            {
                if (item.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
                {
                    //宠物更新
                    foreach (var buffData in item.batchStuffList)
                    {
                        InitBossAndPetEvent(item, buffData);
                    }
                }
            }
            else if (item.eventType == (int)StageEventType.Boss)
            {   // 传承BOSS
                foreach (var buffData in item.batchStuffList)
                {
                    InitBossAndPetEvent(item, buffData);
                }
            }
        }
        SortResStorehouse();
    }

    private void InitBossAndPetEvent(RandomEventData eventData, BatchStuff stuffData)
    {
        if (mapObjDic.TryGetValue(eventData.guid.ToString() + stuffData.id, out var mapObjectData))
        {
            if (stuffData.eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Taking)
            {
                mapObjectData.bigMapObject.walkMap = this.walkMap;
                mapObjectData.bigMapObject.hero = this.chapterMap.panelClip.panel.hero;
                mapObjectData.bigMapObject.OnShow();
            }
            else
            {
                mapObjectData.bigMapObject.Destroy();
            }
        }
        else
        {
            if (stuffData.eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Taking)
            {
                BigMapObject newMapObject = new BigMapObject();
                newMapObject.walkMap = walkMap;
                newMapObject.hero = chapterMap.panelClip.panel.hero;
                newMapObject.aStarPathFinder = aStarPathFinder;
                newMapObject.gridSizeTextture = gridSizeTextture;
                newMapObject.eventData = eventData;
                newMapObject.batchStuffId = stuffData.id;
                newMapObject.SetWalkPosList(walkableList);
                newMapObject.SetStageDataList(stageDataList);
                newMapObject.objectType = BigMapObjectType.Pet;
                if (eventData.eventType == (int)StageEventType.Boss)
                {
                    newMapObject.objectType = BigMapObjectType.Boss;
                }

                if (walkableList.Count > 0 && aStarPathFinder != null)
                {
                    newMapObject.LoadObject();

                    BigMapObjectData bigMapObjectData = new BigMapObjectData();
                    bigMapObjectData.id = stuffData.id;
                    bigMapObjectData.guid = eventData.guid;
                    bigMapObjectData.bigMapObject = newMapObject;
                    ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(stuffData.cfgId);
                    if (eventStageUnit != null)
                        bigMapObjectData.monsterData = eventStageUnit.MonsterData;
                    MapChapterManager.Instance.AddMapObject(eventData.guid.ToString() + stuffData.id, bigMapObjectData);
                }
            }
        }
    }

    /// <summary>
    /// 检查boss和宠物的距离
    /// </summary>
    private void CheckBigMaoObjectDist()
    {
        foreach (var item in MapChapterManager.Instance.GetMapObjects())
        {
            item.Value.bigMapObject.SetVisibleBattleLogo(false);
            if (item.Value.bigMapObject.stateType != StateType.Attacking || (item.Value.bigMapObject.objectType == BigMapObjectType.Boss && _jumpTypeEnum == JumpTypeEnum.MapBoss))
            {
                if (item.Value.bigMapObject.GetDistance(item.Value.bigMapObject, this.chapterMap.panelClip.panel.hero))
                {
                    item.Value.bigMapObject.SetVisibleBattleLogo(true);
                    if (clickBigMapObject != null && clickBigMapObject == item.Value.bigMapObject)
                    {
                        StopRoleMove();
                        if ((int)clickPos.x == (int)item.Value.bigMapObject.modelObj.position.x && (int)clickPos.y == (int)item.Value.bigMapObject.modelObj.position.y)
                        {
                            if (item.Value.bigMapObject.objectType == BigMapObjectType.Boss)
                            {
                                item.Value.bigMapObject.ShowPopView();
                            }
                            else if (item.Value.bigMapObject.objectType == BigMapObjectType.Pet)
                            {
                                item.Value.bigMapObject.PlayCatchAction();
                            }
                        }
                    }
                }
            }
        }
    }

    private BigMapObject clickBigMapObject;  //点击的BOSS、宠物对象
    private Vector3 clickPos = Vector3.zero;  // 点击时的位置
    private void AutoFindBossObject(Vector2 targetPos, BigMapObject bigMapObject)
    {
        clickBigMapObject = bigMapObject;
        clickPos = bigMapObject.modelObj.position;
        SetMoveData(targetPos);
    }

    private void CatchPetEvent(BigMapObject obj, bool isCatch)
    {
        HideHeroAiXinSpine();
        if(actionPlaying)
        {
            actionPlaying = false;
        }
        if (isCatch)
        {
            Vector3 forward = obj.modelObj.position - chapterMap.panelClip.panel.hero.position;
            _heroSpine.skeleton.ScaleX = forward.x >= 0 ? 1 : -1;

            if (_heroSpine != null && _heroSpine.AnimationName != "zhuachong")
            {
                _heroSpine.state.SetAnimation(0, "zhuachong", true);
                AddHeroAiXinSpine();
            }
            actionPlaying = true;
        }
        else
        {
            if (_heroSpine != null && _heroSpine.AnimationName != "idle")
            {
                _heroSpine.state.SetAnimation(0, "idle", true);
            }
        }
    }

    #endregion

    #region 挖宝藏



    #endregion


    #region 开箱子砍树

    private ConfigCommonUnit commonUnit201;
    public bool _isCuttingTree = false;//是否处于砍树过程中
    private int zhuZaoChuiId = 10000001;
    private List<TreasureItemData> treasureList = new List<TreasureItemData>();
    /// <summary>
    /// 引导使用的宝箱
    /// </summary>
    private GComponent treasureGuideCom = null;
    private float TREASURE_DETECT_RANGE;// 距离箱子检测范围160像素
    private int MakeTreasureTime;//砍树进度条时间int
    private UI_EquipQiPaoItem _equipQiPao;//装备气泡组件
    private string TAGPREFIX = "TreasureData_";

    private void UpdataTreasureInfo()
    {

        CreateTreasureItemAndSetData();
        AllotTreasureItem();
    }

    /// <summary>
    /// 创建箱子（树）组件并设置数据信息
    /// </summary>
    private void CreateTreasureItemAndSetData()
    {
        // treasureList.Clear();

        List<RandomEventData> treasureRandomEventDatas = MapChapterManager.Instance.GetRandomEventListByType((int)StageEventType.OpenBox);

        if (treasureRandomEventDatas == null)
        {
            Debug.LogWarning("treasureRandomEventDatas为空!");
            return;
        }

        if (treasureRandomEventDatas.Count == 0)
            return;

        string userUid = DataManager.Instance.GetRoleData().userID;

        foreach (var item in treasureRandomEventDatas)
        {
            foreach (var stuff in item.batchStuffList)
            {
                // string uid = $"{TAGPREFIX}{userUid}_{item.guid}_{chapterUnit.Id}_{stuff.id}";
                string uid = $"{TAGPREFIX}{userUid}_{item.guid}_{stuff.id}";

                // 尝试复用已有 TreasureItemData
                var existing = treasureList.FirstOrDefault(t => t.uniqueID == uid);
                if (existing != null)
                {
                    // 更新数据
                    existing.eventGuid = item.guid;
                    existing.type = item.eventType;
                    existing.stuffId = stuff.id;
                    existing.amounts = stuff.amount;
                    existing.eventStatus = stuff.eventStatus;

                    // 同步 UI 显示
                    if (existing.com != null && !existing.com.isDisposed)
                    {
                        // existing.com.visible = existing.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished;
                        // ((UI_TreasureItem)existing.com).bar.visible = false;
                        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 10, 3, () =>
                        {
                            existing.com.visible = existing.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished;
                            ((UI_TreasureItem)existing.com).bar.visible = false;
                        });
                        ((UI_TreasureItem)existing.com).cutSpine.visible = false; // 默认隐藏
                    }
                    continue;
                }

                currentCuttingData = null;
                if (_isCuttingTree)
                {
                    // 将英雄的状态设置为idle
                    if (_heroSpine != null)
                        _heroSpine.state.SetAnimation(0, "idle", true);
                    _isCuttingTree = false;
                }
                // 新建数据（第一次才会走这里）
                TreasureItemData treasureData = new TreasureItemData();
                treasureData.uniqueID = uid;
                treasureData.eventGuid = item.guid;
                treasureData.type = item.eventType;
                treasureData.stuffId = stuff.id;
                treasureData.amounts = stuff.amount;
                treasureData.eventStatus = stuff.eventStatus;
                treasureData.pos = Vector2.zero;

                // 新建 UI
                UI_TreasureItem treasureItem = UI_TreasureItem.CreateInstance();
                treasureItem.di.url = $"Map/event/{GetScene(chapterUnit.Scenes)}_8_b.png";
                treasureItem.icon.url = UIResource.GetMapEventIcon(GetScene(chapterUnit.Scenes), (int)eRandomEventType.eRandomEventType_RandomEuip);
                treasureItem.daiji.Play();
                treasureData.com = treasureItem;
                treasureItem.visible = treasureData.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Finished;
                treasureItem.cutSpine.visible = false;
                treasureItem.cutSpine.SetScale(0.65f, 0.65f);
                // treasureItem.cutSpine.onClick.Add(() => OnClickCutTree(treasureData));
                // treasureItem.name = $"Treasure_{item.guid}_{chapterUnit.Id}_{stuff.id}";
                treasureItem.name = $"Treasure_{item.guid}_{stuff.id}";
                // treasureItem.treasureSpine.onClick.Add(() => OnTreasureClicked(treasureData));
                treasureItem.onClick.Add(() => OnClickCutTree(treasureData));
                treasureItem.bar.visible = false;
                treasureItem.data = treasureData;

                if(!allStorehouseObj.Contains(treasureItem))
                {
                    allStorehouseObj.Add(treasureItem);
                }

                treasureList.Add(treasureData);
            }
        }
        ChkAllResStorehouse();
    }

    /// <summary>
    /// 存储所有箱子数据及位置信息（内存中管理）
    /// </summary>
    private List<TreasureItemDataAndPos> _allTreasureItems = new List<TreasureItemDataAndPos>();

    /// <summary>
    /// 随机分配箱子（树）到可行走区域内并展示
    /// </summary>
    private void AllotTreasureItem()
    {
        _allTreasureItems.Clear();

        // 确保有可行走的坐标点
        //if (randomResPosList.Count == 0)
        //{
        // Debug.LogError("没有可用的可行走坐标点！");
        //return;
        //}

        // 确保有箱子需要分配
        if (treasureList.Count == 0)
        {
            Debug.LogWarning("没有需要分配的箱子！");
            return;
        }

        //测试重新将缓存清空
        //_allTreasureItems.Clear();
        //SaveTreasureItemsToLocal();

        // 存储每棵树的信息TreasureItemDataAndPos到本地，重新登录时判断本地是否存在该树的信息，存在的话让其处于原始位置，不存在的话随机分配位置
        // 尝试从本地加载之前存储的树位置
        List<TreasureItemDataAndPos> savedItems = LoadTreasureItemsFromLocal();
        //Debug.Log("=============分配所有箱子的点=============");
        // 遍历所有箱子进行分配
        foreach (TreasureItemData treasureData in treasureList)
        {
            if(treasureData.com == null || treasureData.com.isDisposed || treasureData.com.displayObject == null) { continue; }
            Vector2 position = Vector2.zero;
            bool useSavedPosition = false;

            // 检查是否有存储的位置
            if (savedItems != null)
            {
                var savedItem = savedItems.Find(item =>
                    item.uniqueID == treasureData.uniqueID);

                if (savedItem != null)
                {
                    position = new Vector2(savedItem.X, savedItem.Y);
                    useSavedPosition = true;
                }
            }

            // 没有存储的位置则获取新位置
            if (!useSavedPosition)
            {
                // 获取随机可行走位置
                position = GetRdmPassablePos((int)eRandomEventType.eRandomEventType_RandomEuip);
                // 如果没有可用位置则终止分配
                if (position == Vector2.zero)
                {
                    Debug.LogWarning("可用位置不足，无法分配箱子");
                    continue;
                }
            }

            // 设置箱子位置
            treasureData.pos = position;
            treasureData.com.SetXY(position.x, position.y);

            // 仅第一次添加时执行 AddChild（否则会重复添加）
            if (treasureData.com.parent == null)
            {
                this.chapterMap.panelClip.panel.AddChild(treasureData.com);
                RemoveRandomResPosList(position);

                if (ChkNodeDis(treasureData.com))
                {
                    position = GetRdmPassablePos();
                    treasureData.pos = position;
                    treasureData.com.SetXY(position.x, position.y);
                }
            }
            //记录引导使用的宝箱
            if (this.GuidePosList.Count > 0 && treasureData.pos == this.GuidePosList[0])
            {
                this.treasureGuideCom = treasureData.com;
            }

            // 添加到分配列表
            _allTreasureItems.Add(new TreasureItemDataAndPos()
            {
                uniqueID = treasureData.uniqueID,
                X = position.x,
                Y = position.y,
            });

            if(!_isCuttingTree)
            {
                treasureData.com.alpha = 0f;
                treasureData.com.TweenFade(1f, 1f);
            }
        }

        // 保存当前分配
        SaveTreasureItemsToLocal();

        CheckNearTreasureItem();

        SortResStorehouse();
    }

    /// <summary>
    /// 检查靠近箱子（树）附近
    /// </summary>
    private void CheckNearTreasureItem()
    {
        // 如果正在砍树，不检测附近的树
        if (_isCuttingTree) return;

        //判断铸造锤是否足够
        if (ItemInfoManager.Instance.GetItemCount(zhuZaoChuiId) <= 0)
        {
            return;
        }

        if (chapterMap.panelClip.panel.hero == null || chapterMap.panelClip.panel.hero.displayObject == null) return;

        //靠近箱子附近时，显示图标。离开箱子附近时，隐藏图标

        // 获取角色当前位置（在walkMap上的坐标）
        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);

        if (treasureList == null || treasureList.Count == 0) return;

        foreach (var item in treasureList)
        {
            if(item.com == null || item.com.isDisposed || item.com.displayObject == null) { continue; }
            // 计算箱子位置和角色位置的距离（在walkMap坐标系下）
            float distance = Vector2.Distance(heroPos, item.pos);

            // 根据距离设置图标显示
            bool shouldShow = (distance <= TREASURE_DETECT_RANGE);

            // 更新图标可见性
            if (item.com != null && !item.com.isDisposed)
            {
                ((UI_TreasureItem)item.com).cutSpine.visible = shouldShow;
            }
        }
    }

    // 当前砍树中的物品数据
    private TreasureItemData currentCuttingData;
    /// <summary>
    /// 砍树
    /// </summary>
    /// <param name="data"></param>
    private void OnClickCutTree(TreasureItemData data)
    {
        cursorSpine.visible = false;//光标隐藏

        // 处于砍树状态
        if (_isCuttingTree)
        {
            return;
        }

        // 是否存在未分解或未装备的装备,存在的话展示出之前生成装备的界面
        if (EquipManager.Instance.curNoEquipGuid != 0)
        {
            UIManager.Instance.ShowUIPanel("Equip", EquipManager.Instance.curNoEquipGuid, EquipManager.Instance.isNewEquip, 1);
            return;
        }

        // 是否达到完成次数上限
        int finishCount = MapChapterManager.Instance.GetFinishEventCount((int)StageEventType.OpenBox);
        int upLimit = ConfigUtils.GetEventUnitByType((int)StageEventType.OpenBox).Number;
        if (finishCount >= upLimit)
        {
            UIManager.Instance.Toast("今日完成数量已达上限!");
            return;
        }
        ClearMoveTweener();
        //玩家点击砍树标识后，英雄播放通用砍树动作，小树上方出现进度条
        Debug.Log("开始砍树了。。。");
        //MapChapterManager.Instance.PauseMapObject();//暂停怪物移动

        if (!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickBox))
        {
            GuideManager.Instance.SendToCompleteGuide((int)GuideID.NewAccount_ClickBox);
            GuideManager.Instance.GuideNotTouch();
        }
        if (this.QiPao != null)
        {
            this.QiPao.Dispose();
            this.QiPao = null;
        }

        UI_TreasureItem treasureUI = (UI_TreasureItem)data.com;
        mineSelect.visible = true;
        mineSelect.xy = treasureUI.xy;
        mineSelect.x -= treasureUI.width * 0.5f;
        mineSelect.y -= treasureUI.height * 0.55f;
        Vector2 treePos = treasureUI.position;
        float treeWidth = treasureUI.width;
        float treeHeight = treasureUI.height;
        // 计算树的左下角和右下角位置
        // Vector2 bottomLeft = new Vector2(treePos.x - treeWidth / 2, treePos.y + treeHeight / 2);
        // Vector2 bottomRight = new Vector2(treePos.x + treeWidth / 2, treePos.y + treeHeight / 2);
        Vector2 bottomLeft = new Vector2(treePos.x - treeWidth / 2, treePos.y);
        Vector2 bottomRight = new Vector2(treePos.x + treeWidth / 2, treePos.y);
        // 获取英雄当前位置
        Vector2 heroPos = this.chapterMap.panelClip.panel.hero.position;
        // 根据英雄位置决定目标位置
        Vector2 targetPos;
        if (heroPos.x < treePos.x) // 英雄在树左侧
        {
            targetPos = bottomLeft;
        }
        else // 英雄在树右侧
        {
            targetPos = bottomRight;
        }
        // 将目标位置转换为walkMap局部坐标
        Vector2 globalTarget = this.chapterMap.panelClip.panel.LocalToGlobal(targetPos);
        Vector2 targetInWalkMap = this.walkMap.GlobalToLocal(globalTarget);
        // 保存当前砍树数据
        currentCuttingData = data;
        // 移动英雄到目标位置
        SetMoveData(targetInWalkMap, false, true);

    }

    // 开始砍树动画
    private void StartCuttingAnimation(TreasureItemData data)
    {
        if (data == null) return;

        //铸造锤不足
        if (ItemInfoManager.Instance.GetItemCount(zhuZaoChuiId) <= 0)
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(zhuZaoChuiId).Name)));
            return;
        }

        // 设置砍树状态
        _isCuttingTree = true;

        UI_TreasureItem treasureUI = (UI_TreasureItem)data.com;
        mineSelect.visible = false;
        // 隐藏砍树标识
        treasureUI.cutSpine.visible = false;
        treasureUI.touchable = false;

        // 播放英雄砍树的Spine动画
        //if (_heroSpine != null)
        //{
        //    _heroSpine.state.SetAnimation(0, "wabao", true);

        //    // 调整英雄朝向
        //    Vector2 treePos = treasureUI.position;
        //    Vector2 heroPos = this.chapterMap.panelClip.panel.hero.position;
        //    if (heroPos.x < treePos.x) // 英雄在树左侧
        //    {
        //        _heroSpine.skeleton.ScaleX = 1; // 面向右侧（树在右侧）
        //    }
        //    else // 英雄在树右侧
        //    {
        //        _heroSpine.skeleton.ScaleX = -1; // 面向左侧（树在左侧）
        //    }
        //}
        PlayHeroState("wabao", treasureUI);

        //播放处于挖宝时宝箱的Spine状态
        //Utils.PlaySpineAnim2(treasureUI.icon, "dongxiao_2", true, 0.7f);
        treasureUI.wajue.Play();

        //弹出装备界面
        TreasureChesManager.Instance.UseTreasureChes(DataManager.Instance.GetTreasureData().id, currentCuttingData.stuffId, currentCuttingData.eventGuid);

        // 小树上方出现进度条动画
        treasureUI.bar.visible = true;
        treasureUI.bar.min = 0;
        treasureUI.bar.max = 100;
        treasureUI.bar.value = 0;
        float interval = 0.743f;
        float time = interval * 0.35f;
        float s = 100f / 1f;
        time *= s;
        interval *= s;
        GTween.To(
                0, // 起始值
                100, // 结束值
                MakeTreasureTime) // 持续时间
            .SetTarget(treasureUI.bar) // 设置目标对象
            .SetEase(EaseType.Linear) // 线性动画
            .OnUpdate((tweener) =>
            {
                if (tweener.value.x >= time)
                {
                    time += interval;
                    GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralScoopTreasureSE);
                }

                treasureUI.bar.value = tweener.value.x;
            }) // 进度条更新回调
            .OnComplete(() => OnCuttingComplete()); // 完成回调

    }

    // 砍树完成回调
    private void OnCuttingComplete()
    {
        if (currentCuttingData == null) return;

        Debug.Log("砍树完成！");

        // 获取UI组件
        UI_TreasureItem treasureUI = (UI_TreasureItem)currentCuttingData.com;

        // 隐藏进度条
        treasureUI.bar.visible = false;

        // 将英雄的状态设置为idle
        if (_heroSpine != null)
            _heroSpine.state.SetAnimation(0, "idle", true);

        // 播放处于挖宝时宝箱的Spine状态,播放完成之后需要隐藏。
        treasureUI.xiaoshi.Play();
        
        treasureUI.touchable = true;
        treasureUI.visible = false;

        // 装备气泡，飞到英雄身上：1、首先将气泡位置设置在treasureUI上方。2、获取新装备图标。3、实现飞往英雄身上的过程
        // ShowEquipBubbleFly(treasureUI);
        ShowEquipBubbleFly(treasureUI.position);


        //从点位中移除
        allStorehouseObj.Remove(currentCuttingData.com);
        AddRandomResPosList(currentCuttingData.pos);
        // 移除该currentCuttingData
        currentCuttingData = null;

        ChkAllResStorehouse();
        //});
        //Utils.PlaySpineAnim2(treasureUI.icon, "dongxiao_3", false, 0.5f, () =>
        //{
        //    treasureUI.icon.visible = false;
        //});

    }

    /// <summary>
    /// 显示装备气泡飞行动画
    /// </summary>
    /// <param name="treasureUI">宝箱UI</param>
    private void ShowEquipBubbleFly(Vector2 treasureUIPos)//UI_TreasureItem treasureUI
    {
        if (_equipQiPao == null) return;

        // 设置气泡位置在宝箱上方
        // Vector2 treasurePos = treasureUI.position;
        Vector2 treasurePos = treasureUIPos;
        _equipQiPao.SetXY(treasurePos.x, treasurePos.y - 50); // 在宝箱上方50像素处

        // 获取新装备图标
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(EquipManager.Instance.newEquipItemId);
        _equipQiPao.equipIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon.ToString());
        _equipQiPao.equipIcon.SetScale(0.7f, 0.7f);
        _equipQiPao.qiPaoSpine.SetScale(0.7f, 0.7f);

        // 播放气泡出现动画
        _equipQiPao.visible = true;
        Utils.PlaySpineAnim(_equipQiPao.qiPaoSpine, "buff", true);

        //调整气泡层级在英雄之上
        _equipQiPao.sortingOrder = this.chapterMap.panelClip.panel.hero.sortingOrder + 1;

        // 延迟后开始飞向英雄
        // GTween.DelayedCall(0f).OnComplete(() =>
        // {
        //     FlyBubbleToHero(treasurePos);
        // });

        //美术要求：先飞到宝箱下方任意位置，然后再飞到英雄身上
        GTween.DelayedCall(0f).OnComplete(() =>
        {
            FlyBubbleToRandomBelow(treasurePos);
        });

    }

    private GTweener flyTween2;
    /// <summary>
    /// 先飞到宝箱下方随机位置
    /// </summary>
    /// <param name="treasurePos"></param>
    private void FlyBubbleToRandomBelow(Vector2 treasurePos)
    {
        if (_equipQiPao == null) return;

        // 计算宝箱下方的随机位置
        float randomX = treasurePos.x + Random.Range(-50, 50); // X轴随机偏移
        float randomY = treasurePos.y + Random.Range(50, 100); // 在宝箱下方70-120像素范围

        Vector2 randomBelowPos = new Vector2(randomX, randomY);

        // 飞到宝箱下方随机位置
        flyTween2 = _equipQiPao.TweenMove(randomBelowPos, 0.5f)
            .SetEase(EaseType.QuadOut)
            .OnComplete(() =>
            {
                // 到达随机位置后，继续飞向英雄
                FlyBubbleToHero(randomBelowPos);
            });
    }

    private GTweener flyTween;
    /// <summary>
    /// 气泡飞向英雄
    /// </summary>
    /// <param name="startPos">起始位置</param>
    private void FlyBubbleToHero(Vector2 startPos)
    {
        if (_equipQiPao == null || _heroSpine == null) return;

        // 获取英雄位置
        Vector2 heroPos = this.chapterMap.panelClip.panel.hero.position;

        // 设置起始位置
        _equipQiPao.SetXY(startPos.x, startPos.y);
        _equipQiPao.alpha = 1f;

        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.EquipPickupSE);

        // 创建飞行动画
       flyTween = _equipQiPao.TweenMove(heroPos, 1.0f)
            .SetEase(EaseType.QuadOut)
            .OnUpdate((tweener) =>
            {
                float progress = tweener.normalizedTime;
                float scale = 1.0f - progress * 0.3f;// 缩小到原始大小的50%
                _equipQiPao.SetScale(scale, scale);
                // _equipQiPao.alpha = 1.0f - progress * 0.7f;// 透明度渐变（从1降到0.3）
            })
            .OnComplete(() =>
            {
                _equipQiPao.visible = false;
                _equipQiPao.SetScale(1, 1);

                // 清除砍树状态
                _isCuttingTree = false;
            });

    }

    /// <summary>
    /// 存储所有树的位置信息到本地
    /// </summary>
    private void SaveTreasureItemsToLocal()
    {
        string key = $"{TAGPREFIX}{DataManager.Instance.GetRoleData().userID}";

        // string json = JsonConvert.SerializeObject(_allTreasureItems);

        var wrapper = new TreasureItemListWrapper { treasureDatas = _allTreasureItems };
        string json = JsonUtility.ToJson(wrapper);
        UnityEngine.PlayerPrefs.SetString(key, json);
        UnityEngine.PlayerPrefs.Save();
    }

    /// <summary>
    /// 从本地加载树的位置信息
    /// </summary>
    /// <returns></returns>
    private List<TreasureItemDataAndPos> LoadTreasureItemsFromLocal()
    {
        string key = $"{TAGPREFIX}{DataManager.Instance.GetRoleData().userID}";
        if (UnityEngine.PlayerPrefs.HasKey(key))
        {
            string json = UnityEngine.PlayerPrefs.GetString(key);

            // return JsonConvert.DeserializeObject<List<TreasureItemDataAndPos>>(json);
            try
            {
                TreasureItemListWrapper wrapper = null;
                wrapper = JsonUtility.FromJson<TreasureItemListWrapper>(json);
                return wrapper?.treasureDatas;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// 当服务器下发DelRandomEvents_PC时，删除本地该事件的信息
    /// </summary>
    private void ClearTreasureDataByEventGuid(ulong eventGuid)
    {
        // 1. 移除UI和数据
        RemoveTreasureItemsByEventGuid(eventGuid);

        // 2. 从本地存储中完全移除
        string key = $"{TAGPREFIX}{DataManager.Instance.GetRoleData().userID}";
        if (UnityEngine.PlayerPrefs.HasKey(key))
        {
            List<TreasureItemDataAndPos> allItems = LoadTreasureItemsFromLocal();
            allItems.RemoveAll(item =>
                item.uniqueID.StartsWith($"{TAGPREFIX}{DataManager.Instance.GetRoleData().userID}_{eventGuid}"));

            // string json = JsonConvert.SerializeObject(allItems);

            var wrapper = new TreasureItemListWrapper { treasureDatas = allItems };
            string json = JsonUtility.ToJson(wrapper);
            UnityEngine.PlayerPrefs.SetString(key, json);
            UnityEngine.PlayerPrefs.Save();
        }
    }

    // 移除特定事件的所有树数据
    private void RemoveTreasureItemsByEventGuid(ulong eventGuid)
    {
        // 从内存中移除
        List<TreasureItemData> itemsToRemove = treasureList
            .Where(t => t.eventGuid == eventGuid)
            .ToList();
        //pingPosCount("11");
        foreach (var item in itemsToRemove)
        {
            if (item.com != null && !item.com.isDisposed)
            {
                allStorehouseObj.Remove(item.com);
                AddRandomResPosList(item.pos);
                item.com.RemoveFromParent();
                item.com.Dispose();
            }
            treasureList.Remove(item);
        }
        //pingPosCount("12");
        // 从分配列表中移除
        _allTreasureItems.RemoveAll(t =>
            treasureList.Any(data => data.uniqueID == t.uniqueID));

        // 更新本地存储
        SaveTreasureItemsToLocal();
    }

    /// <summary>
    /// 切换章节时清理宝箱数据
    /// </summary>
    private void ClearTreasureDataByChangeChapter()
    {
        if (_isCuttingTree)
        {
            if (_heroSpine != null)
                _heroSpine.state.SetAnimation(0, "idle", true);
            _isCuttingTree = false;
        }

        foreach (var treasure in treasureList)
        {
            if (treasure.com != null && !treasure.com.isDisposed)
            {
                // 停止进度条动画
                GTween.Kill(((UI_TreasureItem)treasure.com).bar);
                // 移除UI
                treasure.com.RemoveFromParent();
                treasure.com.Dispose();
            }
        }

        treasureList.Clear();
        _allTreasureItems.Clear();
        currentCuttingData = null;

        if (_equipQiPao != null)
        {
            _equipQiPao.visible = false;
        }

        var children = this.chapterMap.panelClip.panel.GetChildren();
        foreach (var node in children)
        {
            var t = node as UI_TreasureItem;
            if (t != null)
            {
                t.Dispose();
            }
        }
    }

    // 挖宝过程中退出大地图
    private void MakingEquipInExitBigMap()
    {
        if (_equipQiPao != null)
        {
            _equipQiPao.visible = false;
        }
        
        if (flyTween != null)
        {
            flyTween.Kill();
        }
        if (flyTween2 != null)
        {
            flyTween2.Kill();
        }
        _isCuttingTree = false;
    }

    #endregion


    #region 遗迹

    public void EnterRuinMapPlayEffect()
    {
        // 开始传送特效
        UIExtensions.PlayChuanSongBegin(this.chapterMap.panelClip.panel.hero, _heroSpine, null, new Vector2(-0.5f, -0.9f));
    }

    private void ExitAndEnterRuinUpdate()
    {
        UIExtensions.PlayChuanSongEnd(this.chapterMap.panelClip.panel.hero, _heroSpine, new Vector2(-0.5f, -0.9f));//传送落地特效
        this.chapterMap.exitBtn.visible = MapChapterManager.Instance.exitFlag;
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (MapChapterManager.Instance.exitFlag)
        {
            // (view?.GetBottomList().GetChildAt(2) as UI_BtnBottom).title = "大地图";
            (view?.GetBottomList().GetChildAt(2) as UI_BtnBottom).title = ConfigUtils.GetStringByKey(5171);
        }
        else
        {
            // (view?.GetBottomList().GetChildAt(2) as UI_BtnBottom).title = "营地";
            (view?.GetBottomList().GetChildAt(2) as UI_BtnBottom).title = ConfigUtils.GetStringByKey(5172);
        }
    }

    private void OnClickExitBtn()
    {
        var view = UIManager.Instance.FindByName("RuinMap") as RuinMapView;
        view?.OnClickExitRuinBtn();
    }

    #endregion

    #region 小游戏地图相关（推箱子、奇遇商人）

    private void OpenMiniMap()
    {
        MapChapterManager.Instance.HideMapObject();
        this.chapterMap.panelClip.visible = false;
        this.chapterMap.mapEX.visible = true;
    }
    private void CloseMiniMap()
    {
        this.chapterMap.panelClip.visible = true;
        this.chapterMap.mapEX.visible = false;
        this.chapterMap.mapEX.RemoveChildren();
        openMiniMapType = 0;
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_BATTLE_EVENTS_UPDATE);
        UIExtensions.PlayChuanSongEnd(this.chapterMap.panelClip.panel.hero, this._heroSpine, new Vector2(-0.5f, -0.9f));
        SortResStorehouse();
    }

    /// <summary>
    /// 打开奇遇商人
    /// </summary>
    public void ShowAdventureCaveMap(ulong eventGuid)
    {
        //播放传送，然后再回调切换逻辑
        UIExtensions.PlayChuanSongBegin(this.chapterMap.panelClip.panel.hero, this._heroSpine, () =>
        {
            OpenMiniMap();
            openMiniMapType = eRandomEventType.eRandomEventType_AdventureBusinessMan;
            UIManager.Instance.ShowUIPanel("AdventureCaveMap", this.chapterMap.mapEX, eventGuid);

            this._heroSpine.skeleton.SetColor(new Color(1, 1, 1, 1));

            var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
            view?.ChangeMapBtnTitle(true);
        }, new Vector2(-0.5f, -0.9f), false);
    }
    /// <summary>
    /// 关闭奇遇商人
    /// </summary>
    public void CloseAdventureCaveMap()
    {
        UIManager.Instance.CloseUIPanel("AdventureCaveMap");
        CloseMiniMap();
        var view = UIManager.Instance.FindByName("Lobby") as LobbyView;
        view?.ChangeMapBtnTitle(false);
    }
    /// <summary>
    /// 大地图是否显示
    /// </summary>
    /// <returns></returns>
    public bool MapIsShow()
    {
        return this.chapterMap.panelClip.visible;
    }

    /// <summary>
    /// 打开推箱子
    /// </summary>
    public void ShowSokobanMapView(ulong eventGuid)
    {
        //播放传送，然后再回调切换逻辑
        UIExtensions.PlayChuanSongBegin(this.chapterMap.panelClip.panel.hero, this._heroSpine, () =>
        {
            OpenMiniMap();
            openMiniMapType = eRandomEventType.eRandomEventType_RandomBox;
            UIManager.Instance.ShowUIPanel("SokobanMap", eventGuid);

            this._heroSpine.skeleton.SetColor(new Color(1, 1, 1, 1));
        }, new Vector2(-0.5f, -0.9f), false);
    }
    /// <summary>
    /// 关闭奇遇商人
    /// </summary>
    public void CloseSokobanMapView()
    {
        UIManager.Instance.CloseUIPanel("SokobanMap");
        CloseMiniMap();
    }

    #endregion


    #region 钓鱼

    private ConfigCommonUnit _common2011;//Param1=抛竿后闲置状态时间，Param2=垂钓状态持续时间，单位：秒
    private ConfigCommonUnit _common2014;//Param1=每日垂钓次数上限
    private ConfigBubbleUnit _bubble1011;//钓鱼气泡提示语
    public UI_Bubble _bubbleUI;//气泡提示UI标签
    private List<FishingPosData> _fishingPosDataList = new List<FishingPosData>();
    private float _fishingPosDist = 150f;//英雄与钓点的距离
    private float _fishingSpineOnHeroDist = 160f;//钓鱼Spine在英雄上方的距离
    private GLoader3D fishingSpine;//钓鱼Spine
    private GLoader fishingSpinePos;//钓鱼Spine点
    private GLoader fishingSpinePos2;//钓鱼Spine点
    public FishingPosData curFishingPosData;//当前的钓点信息
    private bool isRightForFinshing = false;//方向
    private Coroutine _endFishingCoroutine;
    private GTweener _fishingBarTween;

    /// <summary>
    /// 初始化钓鱼点位
    /// </summary>
    private void InitFishingPos()
    {
        _fishingPosDataList.Clear();

        for (int i = 1; i <= 7; i++)
        {
            GLoader fishingPos = this._mapPanel.GetChild("fishingPos" + i) as GLoader;
            GLoader fishingDirection = this._mapPanel.GetChild("fishingDirection" + i) as GLoader;

            if (fishingPos == null || fishingDirection == null) continue;

            Vector2 globalFishingPos = fishingPos.LocalToGlobal(Vector2.zero);
            Vector2 pos = walkMap.GlobalToLocal(globalFishingPos);

            Vector2 globalDirection = fishingDirection.LocalToGlobal(Vector2.zero);
            Vector2 dire = walkMap.GlobalToLocal(globalDirection);

            FishingPosData fishingPosData = new FishingPosData();
            fishingPosData.id = i;
            fishingPosData.fishingPos = pos;
            fishingPosData.fishingDirection = dire;
            fishingPosData.fishingNode = fishingPos;
            fishingPosData.fishingNode2 = fishingDirection;

            _fishingPosDataList.Add(fishingPosData);
        }

        fishingSpine = this.chapterMap.panelClip.panel.qpSpine;

    }

    /// <summary>
    /// 检查英雄是否靠近钓鱼点位
    /// </summary>
    private FishingPosData _currentNearbyFishingPos; // 记录当前显示的钓点
    public void CheckHeroNearFishingPos(bool forceRefresh = false)
    {
        if (chapterMap.panelClip.panel.hero == null || chapterMap.panelClip.panel.hero.displayObject == null) return;

        // 靠近钓鱼点附近，显示钓鱼图标。离开钓鱼点附近，隐藏图标

        // 获取角色当前位置（在walkMap上的坐标）
        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);

        // Vector2 localSpinePos = chapterMap.panelClip.panel.GlobalToLocal(globalHeroPos);

        FishingPosData nearbyPos = null;

        foreach (var item in _fishingPosDataList)
        {
            float distance = Vector2.Distance(heroPos, item.fishingPos);
            if (distance <= _fishingPosDist)
            {
                nearbyPos = item;
                break;
            }
        }

        if (nearbyPos != null)
        {
            if (forceRefresh || _currentNearbyFishingPos != nearbyPos)
            {
                SetFishingSpineInfo(nearbyPos, true);
            }
        }
        else
        {
            _currentNearbyFishingPos = null;
            fishingSpine.visible = false;
            fishingSpine.data = null;
        }

    }

    private void SetFishingSpineInfo(FishingPosData nearbyPos, bool isShowSpine = false)
    {
        _currentNearbyFishingPos = nearbyPos;
        fishingSpine.visible = isShowSpine;
        fishingSpine.SetScale(0.5f, 0.5f);
        Utils.PlaySpineAnim(fishingSpine, "BigMap_diaoyvqp", true);

        Vector2 globalDirPos = walkMap.LocalToGlobal(nearbyPos.fishingDirection);
        Vector2 localDirPos = chapterMap.panelClip.panel.GlobalToLocal(globalDirPos);
        fishingSpine.SetXY(localDirPos.x, localDirPos.y - 50f);

        fishingSpine.data = nearbyPos;
        fishingSpine.onClick.Clear();
        fishingSpine.onClick.Add(OnClickFishingSpine);

        //钓鱼引导
        if(fishingSpine != null && fishingSpine.visible)
            ChkEventGuideNext();
    }

    /// <summary>
    /// 钓鱼
    /// </summary>
    private void OnClickFishingSpine(EventContext context)
    {
        int todayFishingTimes = MapChapterManager.Instance.GetFinishEventCount((int)StageEventType.Fishing);
        if (todayFishingTimes > int.Parse(_common2014.Param1))
        {
            UIManager.Instance.ToastByKey(8003);
            return;
        }

        FishingPosData fishingPosData = (context.sender as GLoader3D)?.data as FishingPosData;

        curFishingPosData = fishingPosData;

        // 将目标位置转换为walkMap局部坐标
        Vector2 targetPos = fishingPosData.fishingPos;

        // 移动英雄到目标位置
        SetMoveData(targetPos, false);

    }

    /// <summary>
    /// 开始钓鱼，进入钓鱼休闲状态
    /// </summary>
    public void StartFishingAnimation(bool flag = true)
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.FishingRollCastSE);

        Debug.Log("进入钓鱼休闲状态");

        // 隐藏钓鱼标识Spine
        fishingSpine.visible = false;

        // 播放英雄钓鱼休闲状态Spine动画
        if (_heroSpine != null)
        {
            if (flag)
            {
                UpdateHeroFaceByFishDire();//调整方向
            }

            _heroSpine.state.SetAnimation(0, HeroState.diaoyv1.ToString(), false);
            _heroSpine.state.AddAnimation(0, HeroState.diaoyv2.ToString(), true, 0f);

            Debug.Log("当前Spine动画名: " + _heroSpine.AnimationName);
        }

        ShowProgressUI();

        float time = float.Parse(_common2011.Param1);

        // 取消上一次协程
        if (_endFishingCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_endFishingCoroutine);
        }

        _endFishingCoroutine = GameManager.Instance.StartCoroutine(EndFishingAfterSeconds(time));

        ChkEventGuideNext();
    }

    private IEnumerator EndFishingAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Debug.Log("钓鱼休闲动画播放结束");

        //进入垂钓状态
        EnterFishingStatus();
    }

    public void StopFishingCoroutine()
    {
        if (_endFishingCoroutine != null)
        {
            GameManager.Instance.StopCoroutine(_endFishingCoroutine);
            _endFishingCoroutine = null;
        }

        curFishingPosData = null;

        if (_heroSpine != null)
            _heroSpine.state.SetAnimation(0, "idle", true);
    }

    /// <summary>
    /// 进入钓鱼垂钓状态
    /// </summary>
    private void EnterFishingStatus()
    {
        Debug.Log("进入钓鱼垂钓状态");

        StopFishingCoroutine();

        // 播放英雄钓鱼垂钓状态Spine动画
        UIExtensions.PlayHeroState(_heroSpine, HeroState.diaoyv3);

        // 浮漂上方出现冒泡文字：鱼儿咬钩了，快收杆！！！
        ShowFishingBubbleTips();

        // 弹出收杆界面
        UIManager.Instance.ShowUIPanel("FishingMain");

    }

    /// <summary>
    /// 调整英雄钓鱼的朝向
    /// </summary>
    private void UpdateHeroFaceByFishDire()
    {
        if (curFishingPosData == null || _heroSpine == null) return;

        // 获取英雄当前位置（walkMap局部坐标系）
        Vector2 globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);

        Vector2 fishingDirPos = curFishingPosData.fishingDirection;

        isRightForFinshing = fishingDirPos.x >= heroPos.x;// 只判断左右

        // 设置Spine翻转方向
        if (_heroSpine.skeleton != null)
        {
            _heroSpine.skeleton.ScaleX = isRightForFinshing ? 1f : -1f;
        }

    }

    /// <summary>
    /// 展示钓鱼气泡tips
    /// </summary>
    private void ShowFishingBubbleTips()
    {
        _bubbleUI = UIPackage.CreateObject("CommonEx", "Bubble") as UI_Bubble;
        chapterMap.panelClip.panel.AddChild(_bubbleUI);
        _bubbleUI.bubbleBg.url = "ui://CommonEx/" + _bubble1011.BubbleResource;
        _bubbleUI.talkDes.text = ConfigUtils.GetTextById(_bubble1011.Doc);

        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 localSpinePos = chapterMap.panelClip.panel.GlobalToLocal(globalHeroPos);

        if (isRightForFinshing)
        {
            _bubbleUI.SetXY(localSpinePos.x + 100, localSpinePos.y - 100);
        }
        else
        {
            _bubbleUI.SetXY(localSpinePos.x - 100, localSpinePos.y - 100);
        }

        int time = _bubble1011.Duration;
        GameManager.Instance.TimerManager.SetTimer(time, () =>
        {
            _bubbleUI.RemoveFromParent();
            _bubbleUI.Dispose();
        });

    }

    /// <summary>
    /// 展示进度条-英雄头上
    /// </summary>
    private void ShowProgressUI()
    {
        this.chapterMap.panelClip.panel.fishingWorkBar.visible = true;
        this.chapterMap.panelClip.panel.fishingWorkBar.min = 0;
        this.chapterMap.panelClip.panel.fishingWorkBar.max = 100;
        this.chapterMap.panelClip.panel.fishingWorkBar.value = 0;

        this.chapterMap.panelClip.panel.fishingWorkBar.SetScale(0.8f, 0.8f);

        // 设置在角色上方
        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 localPos = this.chapterMap.panelClip.panel.fishingWorkBar.parent.GlobalToLocal(globalHeroPos);
        this.chapterMap.panelClip.panel.fishingWorkBar.SetXY(localPos.x - 30, localPos.y - 170);

        _fishingBarTween = GTween.To(
                0, // 起始值
                100, // 结束值
                int.Parse(_common2011.Param1)) // 持续时间
            .SetTarget(this.chapterMap.panelClip.panel.fishingWorkBar) // 设置目标对象
            .SetEase(EaseType.Linear) // 线性动画
            .OnUpdate((tweener) =>
            {
                this.chapterMap.panelClip.panel.fishingWorkBar.value = tweener.value.x;
            }) // 进度条更新回调
        .OnComplete(() => this.chapterMap.panelClip.panel.fishingWorkBar.visible = false); // 完成回调
    }

    public SkeletonAnimation GetHeroAni()
    {
        return _heroSpine;
    }

    #endregion

    //private AudioSource audioSource;
    //// 视频准备完成回调
    //void OnVideoPrepared(VideoPlayer vp)
    //{
    //    Debug.Log("视频准备完成，开始播放");
    //    vp.Play();
    //    audioSource.Play(); // 同步播放音频
    //}
    //引导-
    public void ChkGuideToTouch()
    {
        //第一次进游戏播放引导视频
        //if(!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_Enter))
        //{
        //    GameObject videoObj = new GameObject("DynamicVideoPlayer");
        //    // 将物体保留在场景中
        //    GameObject.DontDestroyOnLoad(videoObj);
        //    // 动态添加VideoPlayer组件
        //    VideoPlayer videoPlayer = videoObj.AddComponent<VideoPlayer>();
        //    // 动态添加AudioSource组件（处理音频）
        //    audioSource = videoObj.AddComponent<AudioSource>();
        //    if (videoPlayer != null)
        //    {
        //        // 配置VideoPlayer
        //        videoPlayer.playOnAwake = true; // 自动播放
        //        videoPlayer.isLooping = false; // 不循环播放

        //        videoPlayer.source = VideoSource.Url;
        //        videoPlayer.url = Application.streamingAssetsPath + "/998.mp4";
        //        Debug.Log($"视频路径 = {videoPlayer.url}");

        //        // 渲染设置（输出到RenderTexture）
        //        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        //        videoPlayer.targetTexture = new RenderTexture((int)GRoot.inst.width, (int)GRoot.inst.height, 0);

        //        // 使用平面的材质
        //        chapterMap.video.visible = true;
        //        chapterMap.video.texture = new NTexture(videoPlayer.targetTexture);

        //        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //        videoPlayer.SetTargetAudioSource(0, audioSource);
        //        // 准备完成后自动播放
        //        videoPlayer.prepareCompleted += OnVideoPrepared;
        //        videoPlayer.Prepare(); // 开始加载视频
        //    }
        //    return;
        //}
        //else
        //{
        //    chapterMap.video.visible = false;
        //}

        chapterMap.video.touchable = false;

        if (this.QiPao != null)
        {
            this.QiPao.Dispose();
            this.QiPao = null;
        }
        //触发新手引导-挖箱子
        if (EquipManager.Instance.GetAllEquip().Count <= 0 && this.treasureGuideCom != null && this.treasureGuideCom.displayObject != null && 700f > Vector2.Distance(this.chapterMap.panelClip.panel.hero.xy, this.treasureGuideCom.xy))
        {
            GameManager.Instance.TimerManager.SetTimer(0.01f, () =>
            {
                if(this.treasureGuideCom.visible && !this.treasureGuideCom.isDisposed)
                {
                    GuideManager.Instance.StarGuideByData(new GuideData()
                    {
                        gid = GuideID.NewAccount_ClickBox,
                        tui = this.treasureGuideCom,
                        isForce = true,
                        isSend = false,
                        isLucency = false,
                        scrollPos = new Vector2(this.treasureGuideCom.width * 0.5f, this.treasureGuideCom.height * 0.5f),
                        npcTxt = "Beginner_Doc_001",
                        npcPosType = PosType.Down,
                        //cb = () =>
                        //{
                            //头上顶一个气泡
                            //var qipao = UIPackage.CreateObject("CommonEx", "Bubble") as UI_Bubble;
                            //this.chapterMap.panelClip.panel.AddChild(qipao);
                            //qipao.xy = this.chapterMap.panelClip.panel.hero.xy;
                            //qipao.y -= this.chapterMap.panelClip.panel.hero.height + qipao.height * 0.5f;
                            //qipao.talkDes.text = ConfigUtils.GetBubbleTextById(1012);
                            //this.QiPao = qipao;
                        //}
                        touchCB = () =>
                        {
                            GuideManager.Instance.HideHandle();
                        }
                    });
                }
                else
                {//如果宝箱不存在强制完成挖掘引导
                    if(!GuideManager.Instance.GuideIsComplete(GuideID.NewAccount_ClickBox))
                    {
                        GuideManager.Instance.SendToCompleteGuide(GuideID.NewAccount_ClickBox);
                    }
                }
            });
        }
        //引导-点击第一关
        var stage = _mapPanel.GetChild("stage1").asCom;
        if (stage != null && EquipManager.Instance.GetAllEquip().Count == 1)
        {
            GuideManager.Instance.StarGuideByData(new GuideData()
            {
                bid = GuideID.NewAccount_ClickEquip,
                gid = GuideID.NewAccount_ClickGate,
                tui = stage,
                isForce = true,
                isSend = false,
                isLucency = false,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                skewing = new Vector2(stage.width * -0.25f, 0f),//偏移点击区域
                npcTxt = "Beginner_Doc_003",
                npcPosType = PosType.Down,
                //cb = () =>
                //{
                //    //头上顶一个气泡
                //    var qipao = UIPackage.CreateObject("CommonEx", "Bubble") as UI_Bubble;
                //    this.chapterMap.panelClip.panel.AddChild(qipao);
                //    qipao.sortingOrder = this.chapterMap.panelClip.panel.numChildren;
                //    qipao.xy = this.chapterMap.panelClip.panel.hero.xy;
                //    qipao.y -= this.chapterMap.panelClip.panel.hero.height + qipao.height * 0.5f;
                //    qipao.talkDes.text = ConfigUtils.GetBubbleTextById(1013);
                //    this.QiPao = qipao;
                //}
                touchCB = () =>
                {
                    GuideManager.Instance.HideHandle();
                }
            });
        }
    }
    /// <summary>
    /// 事件引导后续
    /// </summary>
    public void ChkEventGuideNext()
    {
        if(fishingSpine != null && fishingSpine.displayObject != null)
        {
            if(fishingSpine.visible)
            {
                //钓鱼引导-点击水里气泡
                GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    pid = GuideID.guideId_3603,
                    fid = FuncOpenType.Angling,
                    //giding = GuideID.guideId_3600,
                    gid = GuideID.guideId_3601,
                    tui = fishingSpine,
                    isForce = true,
                    isSend = true,
                    //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                    //scale = new Vector2(1f, 1.5f),
                    scrollPos = new Vector2(fishingSpine.width * 0.5f, fishingSpine.height * 0.5f),
                    npcTxt = "Beginner_Doc_017",
                    npcPosType = PosType.Down,
                    cb0 = () =>
                    {
                        StopRoleMove();
                        fishingSpinePos = null;
                        fishingSpinePos2 = null;
                        isGuide = false;
                    },
                    touchCB = () =>
                    {
                        //    GameManager.Instance.TimerManager.SetTimer(0.01f, () =>
                        //    {
                        //        ChkEventGuideNext();
                        //    });
                        GuideManager.Instance.HideHandle();
                    }
                });
            }
            else
            {
                //钓鱼引导-等到进度条完成 
                GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.Angling,
                    giding = GuideID.guideId_3601,
                    gid = GuideID.guideId_3602,
                    tui = null,
                    isForce = true,
                    isSend = true,
                    isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                    scale = new Vector2(1f, 1.5f),
                    npcTxt = "Beginner_Doc_018",
                    npcPosType = PosType.Down,
                    gType = global::GuideType.Wait,
                });
            }
        }
    }

    private void mapPlayEffct()
    {
        switch(chapterUnit.Scenes)
        {
            case 1:
                {
                    //流水
                    for (int i = 1; i <= 16; i++)
                    {
                        var obj = _mapPanel.GetChild("ls" + i);
                        if (obj != null)
                        {
                            obj.visible = false;
                            PlayShuiBo(obj.asLoader3D);
                        }
                    }
                    //游来游去的鱼
                    for (int i = 1; i <= 5; i++)
                    {
                        var obj = _mapPanel.GetChild("fishing" + i);
                        if (obj != null)
                        {
                            obj.visible = false;
                            PlayYuLaiHui(obj.asLoader3D);
                        }
                    }
                    //等待的鱼
                    for (int i = 1; i <= 5; i++)
                    {
                        var obj = _mapPanel.GetChild("fishingB" + i);
                        if (obj != null)
                        {
                            obj.visible = false;
                            PlayYuBLaiHui(obj.asLoader3D);
                        }
                    }
                }
                break;
            case 2:
                {
                    //一阵一阵的风
                    for (int i = 1; i <= 3; i++)
                    {
                        var obj = _mapPanel.GetChild("feng" + i);
                        if (obj != null)
                        {
                            obj.visible = false;
                            PlayShaMoFeng(obj.asLoader3D);
                        }
                    }
                }
                break;
            case 3:
                {

                }
                break;
        }
        
        
    }

    private void PlayShuiBo(GLoader3D sb)
    {
        if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
        GameManager.Instance.TimerManager.SetTimer((float)Random.Range(3, 50) / 10f, () =>
        {
            if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
            sb.visible = true;
            sb.spineAnimation.state.SetAnimation(0, "BigMap_lianyi", false);
            GameManager.Instance.TimerManager.SetTimer((float)Random.Range(3, 50) / 10f, () =>
            {
                PlayShuiBo(sb);
            });
        });
    }

    private void PlayYuLaiHui(GLoader3D sb)
    {
        if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
        GameManager.Instance.TimerManager.SetTimer((float)Random.Range(50, 80) / 10f, () =>
        {
            if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
            sb.visible = true;
            sb.scaleX = Random.Range(0, 10) < 5 ? -1 : 1;
            sb.spineAnimation.state.SetAnimation(0, "BigMap_yv", false);
            GameManager.Instance.TimerManager.SetTimer((float)Random.Range(50, 80) / 10f, () =>
            {
                PlayYuLaiHui(sb);
            });
        });
    }

    private void PlayYuBLaiHui(GLoader3D sb)
    {
        if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
        GameManager.Instance.TimerManager.SetTimer((float)Random.Range(30, 60) / 10f, () =>
        {
            if (!IsShow() || chapterUnit.Scenes != 1 || sb == null || sb.isDisposed) { return; }
            sb.visible = true;
            sb.alpha = 0f;
            sb.TweenFade(1f, 0.3f);
            sb.scaleX = Random.Range(0, 10) < 5 ? -1 : 1;
            sb.spineAnimation.state.SetAnimation(0, "BigMap_yv2", true);

            GameManager.Instance.TimerManager.SetTimer((float)Random.Range(30, 60) / 10f, () =>
            {
                sb.TweenFade(0f, 0.3f);
                PlayYuLaiHui(sb);
            });
        });
    }
    /// <summary>
    /// 风导出吹
    /// </summary>
    /// <param name="sb"></param>
    private void PlayShaMoFeng(GLoader3D sb)
    {
        if (!IsShow() || chapterUnit.Scenes != 2 || sb == null || sb.displayObject == null) { return; }
        sb.visible = false;
        GameManager.Instance.TimerManager.SetTimer((float)Random.Range(20, 50) / 10f, () =>
        {
            if (!IsShow() || chapterUnit.Scenes != 2 || sb == null || sb.displayObject == null) { return; }
            sb.visible = true;
            var pos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
            Vector2 heroPos = walkMap.parent.GlobalToLocal(pos);
            sb.x = heroPos.x + Random.Range(-GRoot.inst.width * 0.6f, GRoot.inst.width * 0.6f);
            sb.y = heroPos.y + Random.Range(-GRoot.inst.height * 0.6f, GRoot.inst.height * 0.6f);
            //随机一个地方吹
            sb.spineAnimation.state.SetAnimation(0, "BigMap_sm_feng", false);

            GameManager.Instance.TimerManager.SetTimer((float)Random.Range(20, 50) / 10f, () =>
            {
                PlayShaMoFeng(sb);
            });
        });
    }

    public bool ChkHeroIdleState()
    {
        return _heroSpine.AnimationName == HeroState.idle.ToString() || _heroSpine.AnimationName == HeroState.run.ToString();
    }

    /// <summary>
    /// 抓捕宠物头上加爱心
    /// </summary>
    public void AddHeroAiXinSpine()
    {
        var ax = chapterMap.panelClip.panel.AiXin;
        ax.visible = true;
        ax.x = this.chapterMap.panelClip.panel.hero.x - ax.width * 0.5f;
        ax.y = this.chapterMap.panelClip.panel.hero.y - this.chapterMap.panelClip.panel.hero.height * 0.9f - ax.height;
        ax.playing = true;
        ax.loop = true;
        ax.sortingOrder = chapterMap.panelClip.panel.numChildren;
    }

    /// <summary>
    /// 玩家头上的爱心隐藏
    /// </summary>
    public void HideHeroAiXinSpine()
    {
        var ax = chapterMap.panelClip.panel.AiXin;
        ax.playing = false;
        ax.loop = false;
        ax.visible = false;
    }

    /// <summary>
    /// 宠物身上加光罩
    /// </summary>
    /// <param name="pet"></param>
    public void AddPetGuangZhao(BigMapObject pet)
    {
        var petEff = chapterMap.panelClip.panel.PetLost;
        petEff.visible = true;
        petEff.x = pet.modelObj.x - petEff.width * 0.5f;
        petEff.y = pet.modelObj.y - pet.modelObj.height * 0.5f - petEff.height * 0.5f;
        petEff.spineAnimation.state.SetAnimation(0, "BigMap_zaozi", false);
        petEff.sortingOrder = chapterMap.panelClip.panel.numChildren;
        //播放完成
        GameManager.Instance.TimerManager.SetTimer(2f, () =>
        {
            HidePetGuangZhao();
        });
    }

    /// <summary>
    /// 宠物身上光罩隐藏
    /// </summary>
    public void HidePetGuangZhao()
    {
        var petEff = chapterMap.panelClip.panel.PetLost;
        petEff.visible = false;
    }

	#region 获取货币栏位

    public GButton GetBigMapUIComUserInfo()
    {
        return this.chapterMap.userInfo;
    }

    #endregion

    #region 功能栏位

    // 目前只有两个，暂时写死
    private List<ConfigSystemUnit> _funcList = new List<ConfigSystemUnit>();
    private void UpdateFuncInfo()
    {
        _funcList.Clear();
        ConfigSystemUnit dailySystemUnit = ConfigUtils.GetFunPreInfo((int)FuncOpenType.DailyTask);
        ConfigSystemUnit homeSystemUnit = ConfigUtils.GetFunPreInfo((int)FuncOpenType.Village);

        if (FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)dailySystemUnit.Id).Item1)
        {
            _funcList.Add(dailySystemUnit);
        }
        if (FuncPreviewManger.Instance.GetFuncOpenState((FuncOpenType)homeSystemUnit.Id).Item1)
        {
            _funcList.Add(homeSystemUnit);
        }

        this.chapterMap.functionList.numItems = _funcList.Count;
    }

    private void UpdateFuncRedDot()
    {
        this.chapterMap.functionList.numItems = _funcList.Count;
    }

    private void FunctionListRender(int index, GObject item)
    {
        ((UI_BtnFucntionIcon)item).icon = UIResource.GetFuncPreIcon(_funcList[index].Icon.ToString());
        ((UI_BtnFucntionIcon)item).title = ConfigUtils.GetTextById(_funcList[index].Name);
        FuncOpenType funcOpenType = (FuncOpenType)_funcList[index].Id;

        ShowFunctionRedDot(funcOpenType, ((UI_BtnFucntionIcon)item));
        
        ((UI_BtnFucntionIcon)item).data = funcOpenType;
        ((UI_BtnFucntionIcon)item).onClick.Set(this.OnClickFuncBtn);
    }

    private void OnClickFuncBtn(EventContext context)
    {
        FuncOpenType funcId = (FuncOpenType)(context.sender as UI_BtnFucntionIcon)?.data;
        ShowFunctionUI(funcId);
    }
    
    private void ShowFunctionUI(FuncOpenType funcOpenType)
    {
        switch (funcOpenType)
        {
            case FuncOpenType.DailyTask://日常任务
                UIManager.Instance.ShowUIPanel("DailyTask");
                break;
            case FuncOpenType.Village://家园
                UIManager.Instance.ShowUIPanel("VillageHome");
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
            case FuncOpenType.DailyTask://日常任务
                ((UI_BtnFucntionIcon)item).redDot.visible = TaskInfoManager.Instance.IsCanGetDailyReward();
                break;
            case FuncOpenType.Village://家园
                ((UI_BtnFucntionIcon)item).redDot.visible = VillageInfoManager.Instance.VillageRedDot();
                break;
            default:
                LogUtils.Log("错误的功能解锁Id");
                break;
        }
    }

    #endregion

    private void ChkEventPosDis()
    {
        if(touchEventData != null || currentCuttingData != null || clickBigMapObject != null || _isCuttingTree || actionPlaying) { return; }//其他操作中
        if(this.chapterMap.panelClip == null || this.chapterMap.panelClip.displayObject == null || this.chapterMap.panelClip.panel.hero == null || this.chapterMap.panelClip.panel.hero.displayObject == null) { return; }
        float dis = 180f;
        if (!GuideManager.Instance.IsShowGuiding && (
            !GuideManager.Instance.GuideIsComplete(GuideID.guideId_3701)
            || !GuideManager.Instance.GuideIsComplete(GuideID.guideId_3801)
            || !GuideManager.Instance.GuideIsComplete(GuideID.guideId_3901)
            || !GuideManager.Instance.GuideIsComplete(GuideID.guideId_4001)
            || !GuideManager.Instance.GuideIsComplete(GuideID.guideId_4101)
            ))
        {
            eRandomEventType enumType = eRandomEventType.eRandomEventType_RandomBox;
            eRandomEventStatus statusType = eRandomEventStatus.eRandomEventStatus_Dispatched;
            foreach (var item in eventStageList)
            {
                if(item.stageLoader != null 
                    && item.stageLoader.displayObject != null 
                    && item.moveLoader.displayObject != null 
                    && item.stageLoader.visible 
                    && dis > Vector2.Distance(item.moveLoader.xy, this.chapterMap.panelClip.panel.hero.xy))
                {
                    //根据类型进行引导
                    statusType = (eRandomEventStatus)item.stageTaskData.eventStatus;
                    if(statusType == eRandomEventStatus.eRandomEventStatus_Dispatched)
                    { 
                        enumType = (eRandomEventType)item.stageTaskData.eventType;
                        switch (enumType)
                        {
                            case eRandomEventType.eRandomEventType_RandomBox://深埋宝藏
                                if(GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.Sokoban,
                                    pid = GuideID.guideId_3701,
                                    gid = GuideID.guideId_3700,
                                    tui = item.stageLoader,
                                    isForce = true,
                                    isSend = false,
                                    //pType = PosType.Left,
                                    npcTxt = "Beginner_Doc_019",
                                    npcPosType = PosType.Down,
                                })) { StopRoleMove(); return; }
                                break;
                            case eRandomEventType.eRandomEventType_RuinsBuff://遗迹
                                if(GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.Relic,
                                    pid = GuideID.guideId_3801,
                                    gid = GuideID.guideId_3800,
                                    tui = item.stageLoader,
                                    isForce = true,
                                    isSend = false,
                                    //pType = PosType.Left,
                                    npcTxt = "Beginner_Doc_022",
                                    npcPosType = PosType.Down,
                                })) { StopRoleMove(); return; }
                                break;
                            case eRandomEventType.eRandomEventType_NpcTask://狩猎
                                bool isHas = false;
                                foreach(var d in item.stageTaskData.batchStuffList)
                                {
                                    if(d.eventStatus != (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
                                    {
                                        isHas = true;
                                        break;
                                    }
                                }
                                if (!isHas && GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.Hunting,
                                    pid = GuideID.guideId_4001,
                                    gid = GuideID.guideId_4000,
                                    tui = item.stageLoader,
                                    isForce = true,
                                    isSend = false,
                                    //pType = PosType.Left,
                                    npcTxt = "Beginner_Doc_027",
                                    npcPosType = PosType.Down,
                                })) { StopRoleMove(); return; }
                                break;
                            case eRandomEventType.eRandomEventType_StageDropPet://抓宠
                                if(GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.SearchPets,
                                    pid = GuideID.guideId_4102,
                                    gid = GuideID.guideId_4100,
                                    tui = item.stageLoader,
                                    isForce = true,
                                    isSend = false,
                                    //pType = PosType.Left,
                                    npcTxt = "Beginner_Doc_029",
                                    npcPosType = PosType.Down,
                                })) { StopRoleMove(); return; }
                                break;
                            case eRandomEventType.eRandomEventType_AdventureBusinessMan://奇遇
                                if(GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.AdventureBusiness,
                                    pid = GuideID.guideId_3901,
                                    gid = GuideID.guideId_3900,
                                    tui = item.stageLoader,
                                    isForce = true,
                                    isSend = false,
                                    //pType = PosType.Left,
                                    npcTxt = "Beginner_Doc_025",
                                    npcPosType = PosType.Down,
                                })) { StopRoleMove(); return; }
                                break;
                        }
                    }
                }
            }
        }

        //钓鱼点另外处理
        //if (!GuideManager.Instance.GuideIsComplete(GuideID.guideId_3601))
        //{
        //    var gHeroPos = this.chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        //    Vector2 pos = walkMap.GlobalToLocal(gHeroPos);
        //    foreach (var item in _fishingPosDataList)
        //    {
        //        if (dis > Vector2.Distance(item.fishingPos, pos))
        //        {
        //            //引导
        //            if (GuideManager.Instance.StarGuideByData(new GuideData()
        //            {
        //                fid = FuncOpenType.Angling,
        //                //giding = GuideID.guideId_4202,
        //                gid = GuideID.guideId_3600,
        //                tui = item.fishingNode,
        //                isForce = true,
        //                isSend = true,
        //                //pType = PosType.Left,
        //                scale = new Vector2(3f, 3f),
        //                npcTxt = "Beginner_Doc_016",
        //                npcPosType = PosType.Down,
        //                cb0 = () =>
        //                {
        //                    fishingSpinePos = item.fishingNode;
        //                    fishingSpinePos2 = item.fishingNode2;
        //                    isGuide = true;
        //                    StopRoleMove();
        //                }
        //            })) { return; }
        //            break;
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 推送的位置
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPushPos()
    {
        var pos = chapterMap.fightCurStageBtn.xy;
        return pos;
    }

    #region 跳转指引

    /// <summary>
    /// 跳转到目的地 
    /// </summary>
    /// <param name="jumpEnum"></param>
    public void JumpToTarget(JumpTypeEnum jumpEnum)
    {
        if (jumpEnum != JumpTypeEnum.Normal)
        {
            _jumpTypeEnum = jumpEnum;
            switch (jumpEnum)
            {
                case JumpTypeEnum.KillStageMonster: // 1001 挑战当前关卡，击杀怪
                    if (chapterMapData.stageIdslist.Count > 0)
                    {
                        int StageId = chapterMapData.stageIdslist[chapterMapData.stageIdslist.Count - 1];
                        for (int i = 0; i < stageDataList.Count; i++)
                        {
                            var stageData = stageDataList[i];
                            if (stageData.posType != MapPosType.Stage && stageData.posType != MapPosType.Boss)
                                continue;
                            if (stageData.mapStageId == StageId)
                            {
                                EventContext context = new EventContext();
                                context.data = i;
                                stageData.stageLoader.onClick.Call(context);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < stageDataList.Count; i++)
                        {
                            var stageData = stageDataList[i];
                            if (stageData.posType != MapPosType.Stage && stageData.posType != MapPosType.Boss)
                                continue;
                            
                            EventContext context = new EventContext();
                            context.data = i;
                            stageData.stageLoader.onClick.Call(context);
                            break;
                        }
                    }
                    break;
                case JumpTypeEnum.MapBaoxiang: // 1002
                    UI_TreasureItem obj = GetNearObjectByType(JumpTypeEnum.MapBaoxiang) as UI_TreasureItem;
                    if (obj != null)
                        OnClickCutTree(obj.data as TreasureItemData);
                    else
                        UIManager.Instance.ToastByKey(8079);
                    break;
                case JumpTypeEnum.MapGold:  // 1003
                    UI_GoldMine goldObj = GetNearObjectByType(JumpTypeEnum.MapGold) as UI_GoldMine;
                    if (goldObj != null)
                    {
                        var globalPos = goldObj.LocalToGlobal(Vector2.zero);
                        Vector2 gPos = walkMap.GlobalToLocal(globalPos);
                        SetMoveData(gPos);
                    }else
                        UIManager.Instance.ToastByKey(8080);
                    break;
                case JumpTypeEnum.MapDiamond:  // 1004
                    UI_DiamondMine diaObj = GetNearObjectByType(JumpTypeEnum.MapDiamond) as UI_DiamondMine;
                    if (diaObj != null)
                    {
                        EventContext context = new EventContext();
                        context.data = diaObj.data;
                        diaObj.onClick.Call(context);
                    }else
                        UIManager.Instance.ToastByKey(8081);
                    break;
                case JumpTypeEnum.MapFish:  // 1005 自动去钓鱼
                    FishingPosData fishData = GetNearObjectByType(JumpTypeEnum.MapFish) as FishingPosData;
                    if (fishData != null)
                    {
                        SetFishingSpineInfo(fishData);
                        if (fishingSpine != null && fishingSpine.displayObject != null)
                        {
                            EventContext context = new EventContext();
                            context.data = fishData;
                            fishingSpine.onClick.Call(context);
                        }
                    }
                    break;
                case JumpTypeEnum.MapRandomEvent:  // 1006 前往最近的随机事件点
                    MapStageData stageEvent = GetNearObjectByType(JumpTypeEnum.MapRandomEvent) as MapStageData;
                    if (stageEvent != null)
                    {
                        EventContext context = new EventContext();
                        context.data = stageEvent.stageLoader.data;
                        stageEvent.stageLoader.onClick.Call(context);
                        // var globalPos = stageEvent.moveLoader.LocalToGlobal(Vector2.zero);
                        // Vector2 gPos = walkMap.GlobalToLocal(globalPos);
                        // SetMoveData(gPos);
                    }else
                        UIManager.Instance.ToastByKey(8083);
                    break;
                case JumpTypeEnum.MapBoss:  // 1007 前往挑战最低关卡ID的传承BOSS
                    mapObjDic = MapChapterManager.Instance.GetMapObjects();
                    BigMapObject bigMapObject = GetNearObjectByType(JumpTypeEnum.MapBoss) as BigMapObject;
                    if (bigMapObject != null)
                    {
                        bigMapObject.Pause();
                        bigMapObject._jumpTypeEnum = JumpTypeEnum.MapBoss;
                        // bigMapObject.isGuide = true;
                        EventContext context = new EventContext();
                        context.data = JumpTypeEnum.MapBoss;
                        bigMapObject.modelObj.btnClick.onClick.Call(context);
                    }else
                        UIManager.Instance.ToastByKey(8082);
                    break;
                case JumpTypeEnum.PassStage:  // 1008 挑战关卡
                    ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());
                    OnClickFightCurStageBtn(int.Parse(taskUnit.Param1));
                    break;
            }
        }
    }

    private float distanceNum = 0f;
    private float tempDis = 0f;
    private System.Object nearObj = null;
    private System.Object GetNearObjectByType(JumpTypeEnum jumpTypeEnum)
    {
        distanceNum = 0f;
        nearObj = null;
        var globalHeroPos = chapterMap.panelClip.panel.hero.LocalToGlobal(Vector2.zero);
        Vector2 heroPos = walkMap.GlobalToLocal(globalHeroPos);
        if (jumpTypeEnum == JumpTypeEnum.MapBaoxiang)
        {
            foreach (TreasureItemData treasureData in treasureList)
            {
                if (treasureData.eventStatus == 0)
                {
                    if(treasureData.com == null || treasureData.com.isDisposed || treasureData.com.displayObject == null) { continue; }
                    var treePos = treasureData.com.LocalToGlobal(Vector2.zero);
                    Vector2 gTreePos = walkMap.GlobalToLocal(treePos);
                    tempDis = (heroPos.x - gTreePos.x) * (heroPos.x - gTreePos.x) + (heroPos.y - gTreePos.y) * (heroPos.y - gTreePos.y);
                    if (distanceNum == 0 || distanceNum > tempDis)
                    {
                        distanceNum = tempDis;
                        nearObj = treasureData.com;
                    }
                }
            }
        }
        else if (jumpTypeEnum == JumpTypeEnum.MapGold || jumpTypeEnum == JumpTypeEnum.MapDiamond)
        {
            foreach (var item in _resStorehouse)
            {
                if (item.Value.com == null || item.Value.com.isDisposed || item.Value.com.displayObject == null) continue;
                
                if (jumpTypeEnum == JumpTypeEnum.MapGold)
                {
                    if (item.Value.com is UI_GoldMine)
                    {
                        var treePos = item.Value.com.LocalToGlobal(Vector2.zero);
                        Vector2 gTreePos = walkMap.GlobalToLocal(treePos);
                        tempDis = (heroPos.x - gTreePos.x) * (heroPos.x - gTreePos.x) + (heroPos.y - gTreePos.y) * (heroPos.y - gTreePos.y);
                        if (distanceNum == 0 || distanceNum > tempDis)
                        {
                            distanceNum = tempDis;
                            nearObj = item.Value.com;
                        }
                    }
                }
                else if (jumpTypeEnum == JumpTypeEnum.MapDiamond)
                {
                    if (item.Value.com is UI_DiamondMine)
                    {
                        var treePos = item.Value.com.LocalToGlobal(Vector2.zero);
                        Vector2 gTreePos = walkMap.GlobalToLocal(treePos);
                        tempDis = (heroPos.x - gTreePos.x) * (heroPos.x - gTreePos.x) + (heroPos.y - gTreePos.y) * (heroPos.y - gTreePos.y);
                        if (distanceNum == 0 || distanceNum > tempDis)
                        {
                            distanceNum = tempDis;
                            nearObj = item.Value.com;
                        }
                    }
                }
            }
        }
        else if (jumpTypeEnum == JumpTypeEnum.MapFish)
        {
            foreach (var item in _fishingPosDataList)
            {
                tempDis = (heroPos.x - item.fishingPos.x) * (heroPos.x - item.fishingPos.x) + (heroPos.y - item.fishingPos.y) * (heroPos.y - item.fishingPos.y);
                if (distanceNum == 0 || distanceNum > tempDis)
                {
                    distanceNum = tempDis;
                    nearObj = item;
                }
            }
        }
        else if (jumpTypeEnum == JumpTypeEnum.MapRandomEvent)
        {
            foreach (var item in eventStageList)
            {
                if (item.stageLoader != null && item.stageLoader.displayObject != null && item.moveLoader.displayObject != null && item.stageLoader.visible)
                {
                    var globalPos = item.moveLoader.LocalToGlobal(Vector2.zero);
                    Vector2 moveLoaderPos = walkMap.GlobalToLocal(globalPos);
                    
                    tempDis = (heroPos.x - moveLoaderPos.x) * (heroPos.x - moveLoaderPos.x) + (heroPos.y - moveLoaderPos.y) * (heroPos.y - moveLoaderPos.y);
                    if (distanceNum == 0 || distanceNum > tempDis)
                    {
                        distanceNum = tempDis;
                        nearObj = item;
                    }
                }
            }
        }
        else if (jumpTypeEnum == JumpTypeEnum.MapBoss)
        {
            if (mapObjDic.Count <= 0)
                mapObjDic = MapChapterManager.Instance.GetMapObjects();
            foreach (var item in mapObjDic)
            {
                if (item.Value.bigMapObject.isShowObj && item.Value.monsterData != 0 && (distanceNum == 0 || distanceNum > item.Value.monsterData))
                {
                    distanceNum = item.Value.monsterData;
                    nearObj = item.Value.bigMapObject;
                }
            }
        }
        return nearObj;
    }

    #endregion
}
