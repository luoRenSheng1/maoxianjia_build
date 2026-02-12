
using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum GuideID
{
    // GetFirstReward = 100,
    // Click_AddAtk5 = 200,
    // GetReward_AddAtk5 = 300,
    // Click_AddHp5 = 400,
    // GetReward_AddHp5 = 500,
    // Click_AddAtk5_2 = 700,
    // GetReward_AddAtk5_2 = 800,
    // Click_AddHp5_2 = 900,
    // GetReward_AddHp5_2 = 1000,
    // GetReward_1007 = 1100,
    // GetReward_1011 = 1200,
    // Click_zhuzhaoBtn = 1300,
    // Click_GetEquip = 1301,
    // Click_Equiped = 1302,
    NewAccount_Enter = 1500,//强制引导部分,新号进游戏，播放漫画，下面的主UI按钮栏先暂时隐藏
    NewAccount_ClickBox = 1501,//进入大地图后，播放气泡说话,点击地上宝箱
    NewAccount_ClickSecretCanon1 = 1502,//再次点击升级秘典1
    NewAccount_ClickEquip = 1503,//点击穿戴装备按钮
    NewAccount_ClickClaimTask = 1504,//点击领取主线任务奖励
    NewAccount_ClickGate = 1505,//人物头上播放气泡说话，指引第一关
    NewAccount_ClickChallenge = 1506,//点击挑战按钮
    NewAccount_BossDeadUpLevel = 1507,//击杀BOSS后，引导点击秘典升级，顺便将下面主UI栏显示
    NewAccount_UIEquip0 = 1508,//点击主UI的装备按钮
    NewAccount_MPUpLevel = 1509,//击矿镐升级
    NewAccount_MPBuy1 = 1510,//购买矿镐升级1
    NewAccount_MPBuy2 = 1511,//购买矿镐升级2
    NewAccount_MPUpLevel1 = 1512,//点击升级按钮
    NewAccount_MPSpeed = 1513,//点击加速按钮
    NewAccount_MPSpeedDiam = 1514,//点击花费钻石加速确定
    NewAccount_MPClose = 1515,//点击关闭矿镐升级界面按钮
    NewAccount_UIEquipClose = 1516,//点击主UI的关闭装备按钮
    NewAccount_NextGate = 1517,//点击进入下一关按钮

    Click_Summon = 1600,//1002关卡完成，点击主UI的召唤按钮
    Click_SummonSkill = 1601,//点击召唤10抽
    Click_SummonSkill_Close = 1602,//点击关闭召唤确定按钮
    Click_OpenHeroSys = 1603,//点击角色按钮（技能）
    Click_SkillBtn = 1604,//点击技能标签页按钮
    Click_SkillIntensify = 1605,//点击批量强化
    Click_FirstSkillItem = 1606,//点击首个技能
    Click_EquipSkillItem = 1607,//点击装备技能
    Click_UIRoleClose = 1608,//点击关闭角色界面
    Click_UIEquipIntensifyClose = 1609,//点击关闭强化结果界面

    Click_UIEquip = 1700,//1005关卡通关，点击主UI的装备按钮
    Click_UIInheritEquip = 1701,//点击传承装备标签页
    Click_UIFirstEquip = 1702,//选中第一件装备
    Click_UIClothingEquip = 1703,//点击装备的穿戴装备按钮
    Click_UIEquipClose3 = 1704,//点击关闭装备界面

    Click_Summon2 = 2000,//1003关卡通关，点击主UI的召唤按钮
    Click_SummonPet = 2001,//点击召唤宠物10抽
    Click_SummonPet_Close = 2002,//点击关闭召唤确定按钮
    Click_OpenPet = 2003,//点击宠物按钮
    Click_PetBtn = 2004,//点击宠物标签页（上阵按钮）
    Click_FirstPetItem = 2005,//点击首个宠物
    Click_EquipPetItem = 2006,//点击装备宠物
    Click_UIPetClose2 = 2007,//点击关闭宠物界面
    // GuideCompleteGetReward = 2100,//这一步引导结束
    
    //-------------------------------------------以下为触发引导-------------------------------
    //Trigger_Click_OpenHeroSys1 = 2200,
    //Trigger_Click_PetBtn = 2201,
    //Trigger_Click_DungeonBtn = 2300,
    //Trigger_Click_Dungeon_Gold = 2301,
    //Trigger_Click_DungeonBtn2 = 2400,
    //Trigger_Click_Dungeon_Zhuzhaochui = 2401,
    //Trigger_Click_DungeonBtn3 = 2500,
    //Trigger_Click_Dungeon_Dimond = 2501,
    //Trigger_Click_DungeonBtn4 = 2600,
    //Trigger_Click_Dungeon_Exp = 2601,
    //Trigger_Click_DungeonBtn5 = 2700,
    //Trigger_Click_Dungeon_Rune = 2701,
    
    //Trigger_Click_OpenHeroSys2 = 2800,
    //Trigger_Click_OpenHeroLvBtn = 2801,
    //Trigger_Click_AddHeroLvBtn = 2802,
    
    Trigger_Click_VillageBtn = 2900,//点击家园按钮（石头矿区）
    Trigger_Click_VillageStone = 2901,//点击石头矿区建筑
    Trigger_Click_VillageStoneAddPet = 2902,//点击首个宠物，进行驻扎驻扎
    Trigger_Click_VillageStoneUploadPet = 2903,//点击宠物详情派遣按钮

    Trigger_Click_VillageBtn2 = 3000,//点击家园按钮（加工厂领奖）
    Trigger_Click_VillageFactory = 3001,//点击加工厂建筑
    Trigger_Click_VillageFactoryGetReward = 3002,//点击领取加工厂奖励
    
    Trigger_Click_VillageBtn3 = 3100,//点击家园按钮（训练场领奖）
    Trigger_Click_VillageTrain = 3101,//点击训练场建筑
    Trigger_Click_VillageTrainGetReward = 3102,//点击领取训练场奖励
    
    Trigger_Click_VillageBtn4 = 3200,//点击家园按钮（窝棚领奖）
    Trigger_Click_VillageStack = 3201,//点击窝棚建筑
    Trigger_Click_VillageStackGetReward = 3202,//点击领取窝棚奖励
    
    Trigger_Click_VillageBtn5 = 3300,//点击家园按钮（探索营地领奖）
    Trigger_Click_VillageExplore = 3301,//点击探索营地建筑
    Trigger_Click_VillageExploreGetReward = 3302,//点击领取探索营地奖励
    
    Trigger_Click_VillageBtn6 = 3400,//点击家园按钮（粮食工坊领奖）
    Trigger_Click_VillageFood = 3401,//点击粮食工坊建筑
    Trigger_Click_VillageFoodGetReward = 3402,//点击领取粮食工坊奖励

    guideId_3600 = 3600,//钓鱼引导接近后触发，点击钓鱼点
    guideId_3601 = 3601,//点击水中鱼竿气泡
    guideId_3602 = 3602,//不能点击任何地方，npc说话，等进度条完成
    guideId_3603 = 3603,//点击收杆按钮，画面动画写死暂停

    guideId_3700 = 3700,//深埋宝藏引导，接近触发，点击事件点
    guideId_3701 = 3701,//点击挖宝按钮
    guideId_3702 = 3702,//无手指，NPC说话，点击任何地方下一步
    guideId_3703 = 3703,//等玩家全部推完触发，点击宝箱

    guideId_3800 = 3800,//遗迹BUFF引导, 点击事件点
    guideId_3801 = 3801,//点击开启遗迹按钮
    guideId_3802 = 3802,//点击第一只遗迹中的怪物
    guideId_3803 = 3803,//胜利后点击奖励建筑

    guideId_3900 = 3900,//奇遇商人引导，点击事件点
    guideId_3901 = 3901,//点击传送按钮
    guideId_3902 = 3902,//点击正确的传送点

    guideId_4000 = 4000,//狩猎任务引导，点击事件点
    guideId_4001 = 4001,//点击第一个任务领取

    guideId_4100 = 4100,//抓捕宠物引导，点击事件点
    guideId_4101 = 4101,//点击抓捕按钮
    guideId_4102 = 4102,//卡住宠物不走，引导点击宠物

    guideId_4200 = 4200,//点击家园按钮（加工厂区）
    guideId_4201 = 4201,//点击加工厂区建筑
    guideId_4202 = 4202,//点击首个宠物，进行驻扎
    guideId_4203 = 4203,//点击宠物详情派遣按钮

    //Trigger_Click_OpenHeroSys3 = 3500,
    //Trigger_Click_RuneBtn = 3501,

    //Common_ClickAddAtk = 999998,
    //Common_ClickAddHp = 999997
}

public enum GuideType
{
    None,
    /// <summary>
    /// 普通引导（必须点击某个功能）
    /// </summary>
    Normal,
    /// <summary>
    /// 等待某个事件完成的引导（没有手指，不可点击）
    /// </summary>
    Wait,
    /// <summary>
    /// 点击任意地方继续的引导
    /// </summary>
    ClickFreely,
}
public enum PosType
{
    /// <summary>
    /// 左
    /// </summary>
    Left,
    /// <summary>
    /// 右
    /// </summary>
    Right,
    /// <summary>
    /// 下
    /// </summary>
    Down,
    /// <summary>
    /// 上
    /// </summary>
    Up,
    /// <summary>
    /// 中间
    /// </summary>
    Middle,
}
/// <summary>
/// 传入的引导数据
/// </summary>
public class GuideData
{
    /// <summary>
    /// 关卡解锁需求
    /// </summary>
    public FuncOpenType fid = 0;
    /// <summary>
    /// 需要完成的前置引导
    /// </summary>
    public GuideID bid = 0;
    /// <summary>
    /// 之前进行中的引导ID，如果是相同的才能继续，0=无效
    /// </summary>
    public GuideID giding = 0;
    /// <summary>
    /// 引导id枚举
    /// </summary>
    public GuideID gid = 0;
    /// <summary>
    /// 执行完成就不能继续
    /// </summary>
    public GuideID pid = 0;
    //public GuideID pid1 = 0;
    /// <summary>
    /// UI目标对象
    /// </summary>
    public GObject tui = null;
    /// <summary>
    /// 位置控制器
    /// </summary>
    public PosType pType = PosType.Left;
    /// <summary>
    /// 强制引导，挖空矩形的引导，只能点击一个地方
    /// </summary>
    public bool isForce = true;
    /// <summary>
    /// 发送给服务端记录，作为开始的记录
    /// </summary>
    public bool isSend = false;
    /// <summary>
    /// 在手指进行偏移位置
    /// </summary>
    public Vector2 skewing = Vector2.zero;
    /// <summary>
    /// 整体偏移
    /// </summary>
    public Vector2 scrollPos = Vector2.zero;
    /// <summary>
    /// 挖孔的大小缩放比 scale = new Vector2(0.6f, 0.6f),
    /// </summary>
    public Vector2 scale = Vector2.zero;
    /// <summary>
    /// 角色位置
    /// </summary>
    public int rolePos = 0;
    /// <summary>
    /// 同时触发外部回调
    /// </summary>
    public Action cb = null;
    public Action cb0 = null;
    /// <summary>
    /// 点击后也触发
    /// </summary>
    public Action touchCB = null;
    /// <summary>
    /// 强制遮罩是否透明
    /// </summary>
    public bool isLucency = false;
    /// <summary>
    /// 出现就算完成
    /// </summary>
    public bool isOver = false;
    /// <summary>
    /// 需要打开某个界面的,并且是置顶的
    /// </summary>
    public string uiName = "";
    /// <summary>
    /// NPC说话文本Id
    /// </summary>
    /// GameManager.Instance.GetTextNameByIdWithIndex("Language", index)
    public string npcTxt = "";
    /// <summary>
    /// Npc说的位置
    /// </summary>
    public PosType npcPosType = PosType.Down;
    /// <summary>
    /// 引导类型
    /// </summary>
    public GuideType gType = GuideType.Normal;
    /// <summary>
    /// 音频ID
    /// </summary>
    public SoundType sId = 0;
    /// <summary>
    /// 0=矩形挖孔，1=圆形挖孔，2=纹理挖孔(暂时不支持)
    /// </summary>
    public int mType = 0;
    /// <summary>
    /// 矩形的圆角
    /// </summary>
    //public int corner = 10;
}

public class GuideManager : TSingleton<GuideManager>
{
    /// <summary>
    /// 强制全屏不能点击
    /// </summary>
    private bool isNotTouch = false;
    /// <summary>
    /// 手指
    /// </summary>
    private GComponent _handle;
    /// <summary>
    /// 挖空遮罩
    /// </summary>
    private GComponent _guideLayer;
    /// <summary>
    /// NPC说话框
    /// </summary>
    private GComponent _npcTips;
    /// <summary>
    /// NPC文本
    /// </summary>
    private GTextField _npcTipsText;
    /// <summary>
    /// npc说话的文字内容
    /// </summary>
    private string _npcTipsDes;
    /// <summary>
    /// NPC音频
    /// </summary>
    private SoundEffect _npcSound;
    /// <summary>
    /// 完成的列表
    /// </summary>
    private List<int> _completeGuideIds = new List<int>();
    /// <summary>
    /// 目标UI
    /// </summary>
    private GObject _targetUI;
    public int GuideId { private set; get; }
    /// <summary>
    /// 引导配置
    /// </summary>
    private List<ConfigGuideUnit> _guideUnits;
    /// <summary>
    /// 显示引导
    /// </summary>
    private bool _isShowGuiding;
    public bool IsShowGuiding => _isShowGuiding;
    /// <summary>
    /// 偏移
    /// </summary>
    private Vector2 _scrollPos = Vector2.zero;
    /// <summary>
    /// 手指偏移
    /// </summary>
    private Vector2 _skewing = Vector2.zero;
    /// <summary>
    /// 范围缩放
    /// </summary>
    private Vector2 _scale = new Vector2(1f, 1f);
    /// <summary>
    /// 引导类型
    /// </summary>
    private GuideType _type = GuideType.Normal;
    /// <summary>
    /// npc的模型
    /// </summary>
    private SkeletonAnimation _npcSpine;
    /// <summary>
    /// 音频ID
    /// </summary>
    private SoundType _soundId;
    /// <summary>
    /// 点击后同事也处理一些外部东西
    /// </summary>
    private Action _touchCB = null;
    /// <summary>
    /// 遮罩类型是什么
    /// </summary>
    private int maskType = 0;
    /// <summary>
    /// 文字效果
    /// </summary>
    private GTweener txtTweener = null;

    public void OnInit()
    {
        GuideId = -1;
        this._completeGuideIds = new List<int>();
        _guideUnits = ConfigDataGroup.GetInstance<ConfigGuide>().Data.Values.ToList();
    }

    public override void Dispose()
    {
        this._completeGuideIds.Clear();
        base.Dispose();
    }
    /// <summary>
    /// 添加到完成的引导队列中
    /// </summary>
    /// <param name="guide"></param>
    public void PushCompleteGuide(int guide)
    {
        if (!_completeGuideIds.Contains(guide))
            _completeGuideIds.Add(guide);
    }
    public void RemoveCompleteGuide(int guide)
    {
        if (!_completeGuideIds.Contains(guide))
            _completeGuideIds.Remove(guide);
    }
    /// <summary>
    /// 检测引导是否完成
    /// </summary>
    /// <param name="guideId"></param>
    /// <returns></returns>
    public bool GuideIsComplete(int guideId)
    {
        //return true;  //TODO 先关闭所有引导  后面重新做
        foreach (var item in _completeGuideIds)
        {
            if (guideId == item)
                return true;
        }
        return false;
    }
    public bool GuideIsComplete(GuideID guideId)
    {
        return this.GuideIsComplete((int)guideId);
    }
    /// <summary>
    /// 大阶段的guideId是个整数 如果1000 小阶段的guideId是+i 如果1001
    /// </summary>
    /// <param name="guideId"></param>
    /// <returns></returns>
    public bool NotShowThisGuide(int guideId)
    {
        bool isComplete = GuideIsComplete(guideId);
        if (isComplete)
            return true;
        //任务触发引导
        ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());
        int newGuideId = guideId > 99999 ? guideId : guideId - guideId % 100;
        //if (taskUnit != null && taskUnit.TaskCompleteGuideId != newGuideId)
        //{
        //    if (taskUnit.TaskStartGuideId != newGuideId)
        //        return true;//换算的引导ID，小阶段
        //    else
        //    {
        //        return false;
        //    }
        //}

        return false;
    }

    private static readonly Vector2 Zero = new Vector2(0, 0);

    public void StartGuide(GObject targetUI, GuideID guideId, PosType posType, bool isForce, bool isSendSC = true, int rolePosType = 0)
    {
        StartToGuide(targetUI, guideId, posType, isForce, isSendSC, Vector2.zero, rolePosType);
    }

    public void StartGuide(GObject targetUI, GuideID guideId, PosType posType, bool isForce, bool isSendSC, Vector2 scrollPos, int rolePosType = 0)
    {
        StartToGuide(targetUI, guideId, posType, isForce, isSendSC, scrollPos, rolePosType);
    }
    /// <summary>
    /// 引导开始
    /// </summary>
    /// <param name="targetUI">目标UI</param>
    /// <param name="guideId">引导ID</param>
    /// <param name="posType"></param>
    /// <param name="isForce"></param>
    /// <param name="isSendSC"></param>
    /// <param name="scrollPos">定位偏移</param>
    /// <param name="rolePosType">NPC的位置</param>
    private void StartToGuide(GObject targetUI, GuideID guideId, PosType posType, bool isForce, bool isSendSC, Vector2 scrollPos, int rolePosType = 0)
    {
        if (_isShowGuiding) return;//已经处于引导中就不进来
        Debug.Log($"当前引导 = {guideId}");
        _scrollPos = scrollPos;
        _isShowGuiding = true;
        isNotTouch = false;
        if (isForce)
        {
            _guideLayer = UIPackage.CreateObject("Common", "GuideLayer").asCom;//挖空一个矩形
            _guideLayer.SetSize(GRoot.inst.width, GRoot.inst.height);
            _guideLayer.AddRelation(GRoot.inst, RelationType.Size);
        }
        else
        {
            _guideLayer = UIPackage.CreateObject("Common", "SoftGuideLayer").asCom;//正常引导
            _guideLayer.SetSize(GRoot.inst.width, GRoot.inst.height);
            _guideLayer.AddRelation(GRoot.inst, RelationType.Size);
        }

        _handle = UIPackage.CreateObject("Common", "HandTips").asCom;//手指提示
        _handle.touchable = false;
        ConfigGuideUnit guideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get((int)guideId);
        if (posType == PosType.Down)
        {
            _guideLayer.GetController("ctrl").selectedIndex = 2;
            _handle.GetController("ctrl").selectedIndex = 2;
        }
        else
        {
            _guideLayer.GetController("ctrl").selectedIndex = posType == PosType.Left ? 0 : 1;
            _handle.GetController("ctrl").selectedIndex = posType == PosType.Left ? 0 : 1;
        }

        _guideLayer.GetController("rolePos").selectedIndex = rolePosType;

        _targetUI = targetUI;
        GuideId = (int)guideId;
        if (_type == GuideType.Normal && isSendSC && _targetUI != null)
        {
            _targetUI.onTouchEnd.Add(this.ClickToSendGuide);//注册点击放开事件

            if (!isForce)
            {
                var builder = FinishedGuideID_CS.CreateBuilder();
                if (guideUnit.RelationId.Count > 0)
                {
                    foreach (var item in guideUnit.RelationId)
                    {
                        if (item > 0)
                            builder.GuideIdList.Add((int)item);
                    }
                }
                builder.GuideIdList.Add((int)guideId);
                builder.IsHardcore = false;
                GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());

                foreach (var item in builder.GuideIdList)
                {
                    GuideManager.Instance.PushCompleteGuide((int)item);//添加到完成的队列中
                }
            }
        }

        GRoot.inst.AddChildAt(_guideLayer, GRoot.inst.numChildren);//!!Before using TransformRect(or GlobalToLocal), the object must be added first
        
        if (_type == GuideType.Normal && targetUI != null)
        {
            _targetUI.onTouchEnd.Add(this.ClickToOver);//注册点击放开事件
            
            RefreshHandlePos();//刷新手指和格子的位置

            GRoot.inst.AddChildAt(_handle, GRoot.inst.numChildren);
        }
        else
        {
            GObject window = _guideLayer.GetChild("window");
            window.size = new Vector2(0f,0f);
            window.visible = true;
            
            GObject window1 = _guideLayer.GetChild("window1");
            window1.visible = false;

            GObject window2 = _guideLayer.GetChild("window2");
            window2.visible = false;
            
            if (_type == GuideType.ClickFreely)
            {//点击任意区域
                _guideLayer.touchable = true;
                _guideLayer.onTouchEnd.Add(this.ClickToNext);
            }
        }

        var view = UIManager.Instance.FindByName("OfflineReward");
        if(view != null && view.IsShow() && view.IsOnStage())
        {
            UIManager.Instance.CloseUIPanel("OfflineReward");
        }
    }
    private void RefreshHandlePos()
    {
        if(_targetUI == null || _guideLayer == null || _targetUI.displayObject == null || _guideLayer.displayObject == null || _targetUI.isDisposed || _guideLayer.isDisposed || !_targetUI.visible) { return; }
        Rect rect = _targetUI.TransformRect(new Rect(0, 0, _targetUI.width * _scale.x, _targetUI.height * _scale.y), _guideLayer);
        GObject w = null;
        GGraph window = _guideLayer.GetChild("window").asGraph;
        window.visible = maskType == 0;
        if (window.visible) { w = window; _guideLayer.mask = window.displayObject; }
        GGraph window1 = _guideLayer.GetChild("window1").asGraph;
        window1.visible = maskType == 1;
        if (window1.visible) { w = window1; _guideLayer.mask = window1.displayObject; }
        GLoader window2 = _guideLayer.GetChild("window2").asLoader;
        window2.visible = maskType == 2;
        if(window2.visible)
        {
            w = window2;
            window2.url = _targetUI.resourceURL;
            _guideLayer.mask = window2.displayObject;
        }
        w.size = new Vector2((int)rect.size.x, (int)rect.size.y);
        Vector2 pos = new Vector2((int)rect.x, (int)rect.y) - _scrollPos;
        pos.x += (_targetUI.width - rect.width) * 0.5f;
        pos.y += (_targetUI.height - rect.height) * 0.5f;

        w.SetXY(pos.x, pos.y);

        //手指
        _handle.SetXY(pos.x + rect.width * 0.5f, pos.y + rect.height * 0.5f);

        if(_skewing != Vector2.zero)
        {
            _handle.xy += _skewing;
        }
    }
    /// <summary>
    /// 点击任意地方进行下一步
    /// </summary>
    /// <param name="context"></param>
    private void ClickToNext(EventContext context)
    {
        ClickToOver(context);
        HideGuide();

        if(_guideLayer != null && _guideLayer.displayObject != null)
        {
            _guideLayer.onTouchEnd.Remove(ClickToNext);
        }
    }
    private void ClickToOver(EventContext context)
    {
        GObject window = _guideLayer.GetChild("window");
        window.visible = false;
        GObject window1 = _guideLayer.GetChild("window1");
        window1.visible = false;
        GObject window2 = _guideLayer.GetChild("window2");
        window2.visible = false;
        //Debug.Log("引导点击后 = " + window.visible);

        GameManager.Instance.TimerManager.ClearTimer(ChkGuideJam);
        GameManager.Instance.TimerManager.SetTimer(0.6f, ChkGuideJam);

        if (_targetUI == null || _targetUI.grayed) { return; }
        _targetUI.onTouchEnd.Remove(ClickToSendGuide);
        _targetUI.onTouchEnd.Remove(ClickToOver);


        ////强制将点击禁用
        //if (_targetUI.touchable)
        //{
        //    _targetUI.touchable = false;
        //    GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
        //    {
        //        _targetUI.touchable = true;
        //    });
        //}
    }
    /// <summary>
    /// 点击后发送给服务端通知完成
    /// </summary>
    private void ClickToSendGuide(EventContext context)
    {
        ConfigGuideUnit guideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get((int)GuideId);
        if(guideUnit == null) { return; }
        var builder = FinishedGuideID_CS.CreateBuilder();

        if (guideUnit.RelationId.Count > 0)
        {
            foreach (var item in guideUnit.RelationId)
            {
                if (item > 0)
                    builder.GuideIdList.Add((int)item);
            }
        }
        builder.GuideIdList.Add((int)GuideId);
        builder.IsHardcore = true;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());

        foreach (var item in builder.GuideIdList)
        {
            GuideManager.Instance.PushCompleteGuide((int)item);
        }
    }

    public void GuideNotTouch()
    {
        if(_guideLayer == null) { return; }
        GObject window = _guideLayer.GetChild("window");
        if(window != null) window.visible = false;

        GObject window1 = _guideLayer.GetChild("window1");
        if (window1 != null) window1.visible = false;
        
        GObject window2 = _guideLayer.GetChild("window2");
        if (window2 != null) window2.visible = false;

        isNotTouch = true;
    }
    public void HideGuide()
    {
        Debug.Log("隐藏取消引导");
        _touchCB = null;
        _type = GuideType.Normal;
        _skewing = new Vector2(0f, 0f);
        _scale = new Vector2(1f, 1f);
        _isShowGuiding = false;
        _guideLayer?.Dispose();
        _handle?.Dispose();
        _npcTips?.Dispose();

        txtTweener?.Kill();
        txtTweener = null;

        if (_npcSound != null)
        {
            _npcSound.audioSource.Stop();
            _npcSound = null;
        }
    }
    public void SendToCompleteGuide(GuideID guideId, List<GuideID> idlist = null)
    {
        SendToCompleteGuide((int)guideId, idlist);
    }
    /// <summary>
    /// 将完成的发给服务端记录
    /// </summary>
    /// <param name="guideId"></param>
    public void SendToCompleteGuide(int guideId, List<GuideID> idlist = null)
    {
        var builder = FinishedGuideID_CS.CreateBuilder();
        if(idlist != null)
        {
            foreach(var item in idlist)
            {
                builder.GuideIdList.Add((int)item);
                PushCompleteGuide((int)item);
            }
        }
        else
        {
            ConfigGuideUnit guideUnit = ConfigDataGroup.GetInstance<ConfigGuide>().Get(guideId);
            if (guideUnit.RelationId.Count > 0)
            {
                foreach (var item in guideUnit.RelationId)
                {
                    if (item > 0)
                    {
                        builder.GuideIdList.Add(item);
                        PushCompleteGuide(item);
                    }
                }
            }
        }
        PushCompleteGuide(guideId);
        builder.GuideIdList.Add(guideId);
        builder.IsHardcore = true;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());
    }

    /// <summary>
    /// 将完成的发给服务端记录
    /// </summary>
    /// <param name="guideId"></param>
    public void SendToLostGuide(List<GuideID> idlist = null)
    {
        var builder = FinishedGuideID_CS.CreateBuilder();
        if (idlist != null)
        {
            foreach (var item in idlist)
            {
                builder.GuideIdList.Add((int)item);
                if (_completeGuideIds.Contains((int)item))
                    _completeGuideIds.Remove((int)item);
            }
        }
        builder.IsHardcore = false;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());
    }

    public bool IsLobbyComplete()
    {
        return true;
    }

    private void ClickToGuideNext()
    {
        if (_touchCB != null)
        {
            _touchCB.Invoke();
            _touchCB = null;
        }
        if(_targetUI != null && _targetUI.displayObject != null)
            _targetUI.onTouchEnd.Remove(ClickToGuideNext);
    }
    public void NewStarGuide(GuideID id, GObject targetUI, PosType posType = PosType.Left, bool isForce = true, bool isSendSC = true, int rolePosType = 0)
    {
        if (!GuideIsComplete((int)id))
        {
            GuideManager.Instance.HideGuide();
            GuideManager.Instance.StartGuide(targetUI, id, posType, isForce, isSendSC, rolePosType);
        }
    }
    /// <summary>
    /// 走数据模块，这样可以少写很多
    /// </summary>
    /// <param name="d"></param>
    public bool StarGuideByData(GuideData d)
    {
        //前置引导ID，如果没有配置就不检测，配置了没完成就抛弃
        if (d.bid != 0 && !GuideIsComplete(d.bid)) { return false; }

        if (d.pid != 0 && GuideIsComplete(d.pid)) { return false; }//过期的不能继续
        //if(d.pid1 != 0 && GuideIsComplete(d.pid1)) { return false; }//过期的不能继续

        if (GuideIsComplete(d.gid) || this.GuideId == (int)d.gid) { return false; }//如果是完成的，如果是相同的
        if (d.giding > 0 && this.GuideId > 0 && (int)d.giding != this.GuideId) { return false; }//之前执行中的ID

        if (d.fid > 0 && FuncPreviewManger.Instance.GetFuncOpenState(d.fid).Item1 == false) { return false; }//未解锁

        //某个界面是否显示中，如果没有显示抛弃
        if (!d.uiName.Equals("") && (!UIManager.Instance.IsShowByName(d.uiName) || !UIManager.Instance.IsTopController(d.uiName))) { return false; }
        //如果手指点击的对象是隐藏的，就不进行
        if(d.tui != null && d.tui.displayObject != null && !d.tui.visible) { return false; }

        d.cb0?.Invoke();
        HideGuide();//先清理之前的
        if (d.skewing != Vector2.zero)
        {
            _skewing = d.skewing;
        }
        if (d.scale != Vector2.zero)
        {
            _scale = d.scale;
        }
        _type = d.gType;
        maskType = d.mType;
        StartToGuide(d.tui, d.gid, d.pType, d.isForce, d.isSend, d.scrollPos, d.rolePos);//开始引导
        if(d.touchCB != null && _targetUI != null && _targetUI.displayObject != null)
        {
            _touchCB = d.touchCB;
            _targetUI.onTouchEnd.Add(this.ClickToGuideNext);
        }

        //d.npcTxt = 1012;
        //d.npcPosType = PosType.Down;
        if (d.npcTxt.CompareTo("") != 0)
        {//NPC说话
            _npcTips = UIPackage.CreateObject("Common", "NpcTips").asCom;
            GRoot.inst.AddChildAt(_npcTips, GRoot.inst.numChildren);
            
            var npc = _npcTips as UI_NpcTips;

            Utils.SetSpineModelOnFGUI(npc.npc, "Pet_10060", 90f, "idle", (o) =>
            {
                if (o is SkeletonAnimation animation)
                {
                    _npcSpine = animation;
                }
            });

            //音频ID
            int soundId = int.Parse(d.npcTxt.Substring(d.npcTxt.Length - 3));
            soundId += (int)SoundType.guide1 - 1;
            _npcSound = GameManager.Instance.SoundManager.PlayEffect(soundId, false);

            //文本
            _npcTipsDes = GameManager.Instance.GetTextNameByIdWithIndex(d.npcTxt);
            _npcTipsText = npc.txt;
            //位置
            _npcTips.x = GRoot.inst.width * 0.5f;
            float n = 10f;
            if(d.npcPosType == PosType.Up)
            {//屏幕上方
                _npcTips.y = _npcTips.height * 0.5f + n;
            }
            else if(d.npcPosType == PosType.Down)
            {//屏幕下方
                _npcTips.y = GRoot.inst.height - _npcTips.height * 0.5f - n;
            }
            else if (d.npcPosType == PosType.Middle)
            {//屏幕中心
                _npcTips.y = GRoot.inst.height * 0.5f - _npcTips.height * 0.5f;
            }
            if (d.sId != 0)
            {
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop(d.sId);
            }
            //点击跳过流程
            npc.skip.onClick.Add(this.OnSkipGuide);
            //打字机效果
            TypingEffect();
        }

        if (d.isLucency)
        {
            _guideLayer.alpha = 0.0f;
        }
        else
        {
            _guideLayer.alpha = 1.0f;
        }
        GameManager.Instance.TimerManager.SetTimer(0.05f, () =>
        {//防止手指定位错误
            RefreshHandlePos();
        });
        GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
        {//防止手指定位错误
            RefreshHandlePos();
        });

        //如果有回调
        d.cb?.Invoke();

        if (d.isOver)
        {
            SendToCompleteGuide((int)d.gid);
        }
        GameManager.Instance.TimerManager.ClearTimer(ChkGuideJam);
        GameManager.Instance.TimerManager.SetTimer(0.8f, ChkGuideJam);

        UIManager.Instance.CloseAllNotGuidePanel();

        return true;
    }

    /// <summary>
    /// 打字效果
    /// </summary>
    private void TypingEffect()
    {
        int count = _npcTipsDes.Length - 1;
        float time = count * 0.05f;
        int idx = 0, i = 0;
        txtTweener?.Kill();
        txtTweener = null;
        txtTweener = GTween.To(1, count, time).SetEase(EaseType.Linear)
            .OnUpdate((GTweener tweener) =>
            {
                if(_npcTipsText == null || _npcTipsText.displayObject == null) { return; }
                idx = (int)tweener.value.x;
                if(i != idx)
                {
                    if(idx < _npcTipsDes.Length)
                    {
                        _npcTipsText.text = _npcTipsDes.Substring(0, idx);
                    }
                    i = idx;
                }
            })
            .OnComplete(() =>
            {
                if(_npcTipsText == null || _npcTipsText.displayObject == null) { return; }
                _npcTipsText.text = _npcTipsDes;
            });
    }

    /// <summary>
    /// 关闭所有引导
    /// </summary>
    public void CloseAllGuide()
    {
        HideGuide();
        var builder = FinishedGuideID_CS.CreateBuilder();
        var guideList = ConfigDataGroup.GetInstance<ConfigGuide>();
        foreach (var item in guideList.Data)
        {
            builder.GuideIdList.Add(item.Value.Id);
            PushCompleteGuide(item.Value.Id);
        }
        builder.IsHardcore = true;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());
    }

    /// <summary>
    /// 检测是否卡住
    /// </summary>
    public void ChkGuideJam()
    {
        //手指还在，遮罩还在，但是不能点击，就是卡住
        if(_guideLayer != null && _handle != null && !isNotTouch)
        {
            //检测卡住了
            if (maskType == 0)
            {
                GObject w0 = _guideLayer.GetChild("window");
                w0.visible = true;
            }
            else if (maskType == 1)
            {
                GObject w1 = _guideLayer.GetChild("window1");
                w1.visible = true;
            }
            if (maskType == 2)
            {
                GObject w2 = _guideLayer.GetChild("window2");
                w2.visible = true;
            }
        }
    }

    /// <summary>
    /// 重新登入后检测后续引导完成
    /// </summary>
    public void ChkSubsequentGuidance()
    {
        //如果是重新登录的，引导完成度到达升级铁镐，那就完成所有强制引导
        if (GuideIsComplete(GuideID.NewAccount_MPUpLevel1) && !GuideIsComplete(GuideID.NewAccount_MPSpeedDiam))
        {
            SendToCompleteGuide((int)GuideID.NewAccount_MPSpeedDiam);
            SendToCompleteGuide((int)GuideID.NewAccount_NextGate);
        }
        //技能装备，后续重新登录全部算完成
        if (GuideIsComplete(GuideID.Click_EquipSkillItem))
        {
            SendToCompleteGuide((int)GuideID.Click_UIRoleClose);
        }
        //传承装备，后续重新登录全部算完成
        if (GuideIsComplete(GuideID.Click_UIClothingEquip))
        {
            SendToCompleteGuide((int)GuideID.Click_UIEquipClose3);
        }
        //宠物装备，后续重新登录全部算完成
        if (GuideIsComplete(GuideID.Click_EquipPetItem))
        {
            SendToCompleteGuide((int)GuideID.Click_UIPetClose2);
        }
    }
    /// <summary>
    /// 因事件刷新，抓捕宠物引导重置
    /// </summary>
    public void ResetGuidance4100()
    {
        bool isGuideing = false;
        GuideID id = (GuideID)GuideId;
        switch (id)
        {
            case GuideID.guideId_4100:
            case GuideID.guideId_4101:
            case GuideID.guideId_4102:
                isGuideing = true;
                break;
        }
        
        if(isGuideing)
        {
            GuideManager.Instance.HideGuide();
            List<GuideID> idlist = new List<GuideID>();
            idlist.Add(GuideID.guideId_4100);
            idlist.Add(GuideID.guideId_4101);
            idlist.Add(GuideID.guideId_4102);
            SendToLostGuide(idlist);
        }
    }
    /// <summary>
    /// 强制关闭多余界面，只留下地图和主界面
    /// </summary>
    public void HideHandle()
    {
        if (!IsShowGuiding) { return; }
        if (_handle != null && _handle.displayObject != null)
        {
            _handle.visible = false;
        }
        if(_guideLayer != null && _guideLayer.displayObject != null)
        {
            if (maskType == 0)
            {
                GObject window = _guideLayer.GetChild("window");
                window.size = new Vector2(0f, 0f);
            }
            else if (maskType == 1)
            {
                GObject window1 = _guideLayer.GetChild("window1");
                window1.size = new Vector2(0f, 0f);
            }
            else if (maskType == 2)
            {
                GObject window2 = _guideLayer.GetChild("window2");
                window2.size = new Vector2(0f, 0f);
            }
            _guideLayer.alpha = 0.0f;
        }
    }

    /// <summary>
    /// 跳过当前引导流程
    /// </summary>
    private void OnSkipGuide()
    {
        var builder = FinishedGuideID_CS.CreateBuilder();
        var guideList = ConfigDataGroup.GetInstance<ConfigGuide>();
        int b = (int)(Math.Floor((double)(GuideId / 100)) * 100);
        for(int i = 0; i < 99; i++)
        {
            ConfigGuideUnit data = ConfigDataGroup.GetInstance<ConfigGuide>().Get(i + b);
            if(data != null)
            {
                builder.GuideIdList.Add(data.Id);
                PushCompleteGuide(data.Id);
            }
            else
            {//没了就去发给服务器
                break;
            }
        }
        builder.IsHardcore = true;
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_FinishedGuideID_CS, builder.Build());
        HideGuide();
    }
}
