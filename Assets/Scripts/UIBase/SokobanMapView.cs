using Engine;
using FairyGUI;
using msg;
using Sokoban;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 深埋宝藏推箱子地图
/// </summary>
public class SokobanMapView : UIViewBase
{
    /// <summary>
    /// 地图
    /// </summary>
    private UI_SokobanMap view => this.main as UI_SokobanMap;

    /// <summary>
    /// 英雄角色
    /// </summary>
    private GGraph hero = null;
    /// <summary>
    /// 终点光标
    /// </summary>
    private GLoader3D terminus = null;
    /// <summary>
    /// 角色动画spine
    /// </summary>
    private SkeletonAnimation heroSpine = null;
    /// <summary>
    /// 移动
    /// </summary>
    private GTweener moveTweener = null;

    /// <summary>
    /// 所有格子的位置
    /// </summary>
    private List<Vector2> cellPosList = new List<Vector2>();
    /// <summary>
    /// 推箱子左上角作为格子的起点
    /// </summary>
    private Vector2 originPos = Vector2.zero;
    /// <summary>
    /// 地板
    /// </summary>
    //private List<GComponent> lotList = new List<GComponent>();
    /// <summary>
    /// 放置点队列
    /// </summary>
    private Dictionary<Vector2, GComponent> slotList = new Dictionary<Vector2, GComponent>();
    /// <summary>
    /// 箱子队列
    /// </summary>
    private Dictionary<Vector2, GComponent> boxList = new Dictionary<Vector2, GComponent>();
    /// <summary>
    /// 障碍物队列
    /// </summary>
    private Dictionary<Vector2, GComponent> wallList = new Dictionary<Vector2, GComponent>();
    /// <summary>
    /// 专门生成的障碍物位置(障碍物用完了就给宝箱使用)
    /// </summary>
    private List<Vector2> wallWalkList = new List<Vector2>();
    /// <summary>
    /// 所有对象（用于排序）
    /// </summary>
    private List<GComponent> allObjList = new List<GComponent>();
    /// <summary>
    /// X轴数量
    /// </summary>
    private const int X_Count = 6;
    /// <summary>
    /// Y轴数量
    /// </summary>
    private const int Y_Count = 8;
    /// <summary>
    /// 空地列表(true = 空地，false = 被占用)
    /// </summary>
    private bool[,] spaceList = new bool[X_Count, Y_Count];
    /// <summary>
    /// 格子的大小
    /// </summary>
    private const int cellSize = 105;

    /// <summary>
    /// 定义四个可能的移动方向：上、下、右、左
    /// </summary>
    private readonly Vector2[] directions = {
        new Vector2(0, -1),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(-1, 0),
    };
    /// <summary>
    /// 左上，左下，右上，右下
    /// </summary>
    private readonly Vector2[] directions1 = {
        new Vector2(-1, -1),
        new Vector2(1, 1),
        new Vector2(-1, 1),
        new Vector2(1, -1)
    };

    /// <summary>
    /// 已经检测过的不检测
    /// </summary>
    private bool[,] visited = new bool[X_Count, Y_Count];
    /// <summary>
    /// 路径
    /// </summary>
    private List<Vector2> paths = new List<Vector2>();

    /// <summary>
    /// 事件ID
    /// </summary>
    public ulong eventGuid = 0;
    /// <summary>
    /// 事件数据
    /// </summary>
    private RandomEventData eventData = null;
    /// <summary>
    /// 箱子推动的tween
    /// </summary>
    //private GTweener boxTw = null;

    /// <summary>
    /// 所有箱子生成后的位置
    /// </summary>
    private Dictionary<Vector2, GComponent> boxPosRecord = new Dictionary<Vector2, GComponent>();
    /// <summary>
    /// 记录玩家初始化的位置
    /// </summary>
    private Vector2 heroPosRecord = Vector2.zero;
    /// <summary>
    /// 记录移动方向
    /// </summary>
    private Vector2 heroDir = Vector2.zero;
    /// <summary>
    /// 不可移动的箱子
    /// </summary>
    private Dictionary<Vector2, GComponent> sealBoxs = new Dictionary<Vector2, GComponent>();

    /// <summary>
    /// 移动频率
    /// </summary>
    private float moveSpeed = 0.005f;

    /// <summary>
    /// 挖掘次数
    /// </summary>
    private int excavationsCount = 0;
    /// <summary>
    /// 播放动画中
    /// </summary>
    private bool boolplaying = false;
    /// <summary>
    /// 等待回包
    /// </summary>
    private bool isWait = false;
    /// <summary>
    /// 推箱子中
    /// </summary>
    private bool TuiPlaying = false;
    /// <summary>
    /// 挖宝次数上限
    /// </summary>
    private int WaBaoLimit = 0;
    /// <summary>
    /// 挖宝读秒
    /// </summary>
    private int WaBaoTime = 0;
    /// <summary>
    /// 障碍物数量上限
    /// </summary>
    private int WallLimit = 0;
    /// <summary>
    /// 箱子物数量上限
    /// </summary>
    private int BoxLimit = 0;
    /// <summary>
    /// 孔洞插槽物数量上限
    /// </summary>
    private int SlotLimit = 0;
    /// <summary>
    /// 目标地点
    /// </summary>
    private Vector2 terminusPos = new Vector2(-1, -1);

    public SokobanMapView()
    {
        this.package = "Sokoban";
        this.name = "SokobanMap";
        this.component = "SokobanMap";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.GuideType = FuncType.Guide;
    }
    public override void BindAll()
    {
        base.BindAll();
        SokobanBinder.BindAll();
    }
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        //this.root = (GComponent)values[0];
        this.eventGuid = (ulong)values[0];
    }
    protected override void OnShow()
    {
        base.OnShow();
        boolplaying = false;
        this.cellPosList.Clear();
        this.wallWalkList.Clear();
        for (int y = 0; y < Y_Count; y++)
        {
            for (int x = 0; x < X_Count; x++)
            {
                var pos = new Vector2(x, y);
                this.cellPosList.Add(pos);
                this.wallWalkList.Add(pos);
                this.spaceList[x, y] = true;
            }
        }

        this.eventData = MapChapterManager.Instance.GetRandomEventDataByGuid(this.eventGuid);
        this.WaBaoLimit = this.eventData.batchStuffList.Count();
        CreateMapAll();

        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.EventMapBGM);

        EngineBase.EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ENTER_RANDOM_BOX_RESULT, RefBoxRewardRecv);

        //深埋宝藏引导-点击任意地方继续
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Sokoban,
            giding = GuideID.guideId_3701,
            gid = GuideID.guideId_3702,
            tui = null,
            isForce = true,
            isSend = true,
            isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
            //scale = new Vector2(1f, 1.5f),
            npcTxt = "Beginner_Doc_020",
            npcPosType = PosType.Down,
            gType = global::GuideType.ClickFreely,
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        });
    }
    protected override void OnHide()
    {
        base.OnHide();
        EngineBase.EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ENTER_RANDOM_BOX_RESULT, RefBoxRewardRecv);
        ClearAllMapCom();
    }

    protected override void OnInit()
    {
        base.OnInit();
        InitHero();
        InitMap();
        //NewLotList();
    }

    /// <summary>
    /// 初始化地图
    /// </summary>
    private void InitMap()
    {
        this.originPos.x = 0;
        this.originPos.y = 0;

        //地图点击移动事件
        this.view.map?.onClick.Add(this.MoveToTouch);
        //点击下面退出提示
        this.view.close?.onClick.Add(this.ShowExitTips);
        //重置按钮
        this.view.btnReset?.onClick.Add(this.OnResetGame);

        //宝藏箱子
        this.view.map.clip.box.onClick.Add(this.BoxTouch);
        //配置读取
        var data1 = ActivityManager.Instance.GetConfigCommonUnitById(2005);
        //this.WaBaoLimit = int.Parse(data1.Param1);

        this.WaBaoTime = int.Parse(data1.Param2);
        var data2 = ActivityManager.Instance.GetConfigCommonUnitById(2015);
        this.WallLimit = int.Parse(data2.Param1);
        this.BoxLimit = int.Parse(data2.Param2);
        this.SlotLimit = int.Parse(data2.Param3);
    }

    /// <summary>
    /// 获取一个格子位置
    /// </summary>
    /// <returns></returns>
    //private Vector2 GetRandomPos()
    //{
    //    int rdm = Random.Range(0, this.cellPosList.Count);
    //    Vector2 pos = this.cellPosList[rdm];
    //    this.cellPosList.RemoveAt(rdm);

    //    if (!this.spaceList[(int)pos.x, (int)pos.y])
    //    {
    //        Debug.Log("位置重复");
    //    }
    //    return pos;
    //}
    /// <summary>
    /// 墙体专用随机点位
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRandomWallPos()
    {
        int rdm = Random.Range(0, this.wallWalkList.Count);
        Vector2 pos = this.wallWalkList[rdm];

        this.wallWalkList.RemoveAt(rdm);

        //直接把周边八个格子消掉
        foreach (var dir in directions)
        {
            var p = pos + dir;
            this.wallWalkList.Remove(p);
        }
        //直接把周边八个格子消掉
        foreach (var dir in directions1)
        {
            var p = pos + dir;
            this.wallWalkList.Remove(p);
        }

        return pos;
    }
    /// <summary>
    /// 获取可以生成的宝箱随机点位
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRandomBoxPos()
    {
        int rdm = Random.Range(0, this.wallWalkList.Count);
        Vector2 pos = this.wallWalkList[rdm];
        this.wallWalkList.RemoveAt(rdm);
        return pos;
    }

    /// <summary>
    /// 重新将障碍物的位置排除掉，给宝箱使用
    /// </summary>
    private void ResetBoxWalkPos()
    {
        this.wallWalkList.Clear();
        this.wallWalkList = new List<Vector2>(this.cellPosList);

        List<Vector2> temp = new List<Vector2>();
        //箱子不能是四个边界
        foreach (var pos in this.wallWalkList)
        {
            if (pos.x != 0 && pos.y != 0 && pos.x != X_Count - 1 && pos.y != Y_Count - 1)
            {
                temp.Add(pos);
            }
        }
        this.wallWalkList = new List<Vector2>(temp);

        //箱子生成时上下左右四个格子不能存在障碍格
        foreach (var pos in this.wallList)
        {
            var p = pos.Key;
            this.wallWalkList.Remove(p);//先移除障碍物的
            foreach (var dir in directions)
            {//移除障碍物四个方向的
                var p1 = p + dir;
                this.wallWalkList.Remove(p1);
                //{
                //    Debug.Log("移除四个方向" + p1);
                //}
                //else
                //{
                //    Debug.Log("no移除四个方向" + p1);
                //}
            }
        }
    }
    /// <summary>
    /// 重新生成空洞可以获取的位置
    /// </summary>
    private void ResetSoltWalkPos()
    {
        List<Vector2> tempDir = new List<Vector2>() {
            new Vector2(-2, 0),
            new Vector2(2, 0),
            new Vector2(0, 2),
            new Vector2(0, -2),
        };
        foreach (var pos in this.wallList)
        {
            var p = pos.Key;
            foreach (var dir in tempDir)
            {
                var p1 = p + dir;
                this.wallWalkList.Remove(p1);
            }
        }
    }

    /// <summary>
    /// 计算格子XY的实际坐标
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCellXY(Vector2 pos)
    {
        return new Vector2(pos.x * cellSize + this.originPos.x, pos.y * cellSize + this.originPos.y);
    }

    /// <summary>
    /// 初始化英雄角色
    /// </summary>
    private void InitHero()
    {
        this.hero = this.view.map.clip.hero;
        HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
        Utils.SetSpineModelOnFGUI(this.hero, myHero.HeroUnit.Model, 65f, HeroState.idle.ToString(), (o) =>
        {
            if (o is SkeletonAnimation animation)
            {
                this.heroSpine = animation;
                //等待异步创建spine
                UIExtensions.PlayChuanSongEnd(this.hero, this.heroSpine, new Vector2(-0.25f, -0.4f));
            }
        });
        this.terminus = this.view.map.clip.terminus;
    }

    /// <summary>
    /// 角色的坐标换算
    /// </summary>
    /// <param name="pos">格子的XY</param>
    /// <returns>地图上的坐标</returns>
    private Vector2 GetRolePos(Vector2 pos)
    {
        var p = GetCellXY(pos);
        p += this.view.map.clip.panel1.xy;
        return p;
    }

    /// <summary>
    /// 初始化出生地点(最后生成后在找个空格作为位置)
    /// </summary>
    private void InitHeroBirthPoint()
    {
        var pos = GetRandomWallPos();

        var p = GetRolePos(pos);
        //初始化角色位置
        this.hero?.SetXY(p.x, p.y);

        this.heroPosRecord = p;

        //等待异步创建spine
        if (this.heroSpine != null)
        {
            UIExtensions.PlayChuanSongEnd(this.hero, this.heroSpine, new Vector2(-0.25f, -0.4f));
        }
        RefBoxJianTou();
        ChkBoxesAround();
    }

    /// <summary>
    /// 生成单个箱子放置点
    /// </summary>
    //private void NewLotList()
    //{
    //    for (int y = 0; y < Y_Count; y++)
    //    {
    //        for (int x = 0; x < X_Count; x++)
    //        {
    //            var com = UI_SokobanWalkable.CreateInstance();
    //            com.xy = GetCellXY(new Vector2(x, y));
    //            this.lotList.Add(com);
    //            this.view.map.clip.dipanel.AddChild(com);
    //        }
    //    }
    //}
    /// <summary>
    /// 生成单个箱子放置点
    /// </summary>
    private void NewSlot()
    {
        var com = UI_SokobanSlot.CreateInstance();
        //生成一个位置
        var pos = GetRandomBoxPos();
        com.data = pos;
        com.xy = GetCellXY(pos);
        this.view.map.clip.panel0.AddChild(com);
        //com.sortingOrder = 0;
        slotList.Add(pos, com);
        //sealBoxs.Add(pos, com);//测试
        allObjList.Add(com);
        this.spaceList[(int)pos.x, (int)pos.y] = false;
    }
    /// <summary>
    /// 生成单个箱子
    /// </summary>
    private void NewBox()
    {
        var com = UI_SokobanBoxMove.CreateInstance();
        //箱子生成时上下左右四个格子不能存在障碍格
        //生成一个位置
        var pos = GetRandomBoxPos();
        com.data = pos;
        com.xy = GetCellXY(pos);
        //com.touchable = true;
        //com.onClick.Clear();
        //com.onClick.Add(TouchBoxMove);

        this.view.map.clip.panel2.AddChild(com);
        com.sortingOrder = 1;
        boxList.Add(pos, com);
        allObjList.Add(com);
        this.spaceList[(int)pos.x, (int)pos.y] = false;

        com.left.visible = false;
        com.right.visible = false;
        com.up.visible = false;
        com.down.visible = false;

        this.boxPosRecord.Add(pos, com);
    }
    /// <summary>
    /// 生成单个障碍物（墙）
    /// </summary>
    private void NewWall()
    {
        var com = UI_SokobanWall.CreateInstance();
        //横放还是竖放，权重：50，50
        if (Random.Range(0, 100) < 50)
        {
            com.icon1.visible = true;
            com.icon2.visible = false;
        }
        else
        {
            com.icon1.visible = false;
            com.icon2.visible = true;
        }
        //障碍与障碍需要有1个格子的间距//生成一个位置
        var pos = GetRandomWallPos();
        com.data = pos;
        com.xy = GetCellXY(pos);
        this.view.map.clip.panel2.AddChild(com);
        //com.sortingOrder = 1;
        wallList.Add(pos, com);
        allObjList.Add(com);
        this.spaceList[(int)pos.x, (int)pos.y] = false;
    }

    private void ClearAllMapCom()
    {
        foreach (var item in allObjList)
        {
            item.Dispose();
        }

        this.allObjList.Clear();
        this.slotList.Clear();
        this.boxList.Clear();
        this.wallList.Clear();
        this.boxPosRecord.Clear();
        this.sealBoxs.Clear();
    }

    /// <summary>
    /// 生成所有地图上的东西
    /// </summary>
    private void CreateMapAll()
    {
        this.view.map.touchable = true;

        this.TuiPlaying = false;
        //播放标记
        this.boolplaying = false;
        //地图位置
        this.view.map.clip.y = -(this.view.map.clip.height - this.view.height);
        //挖掘次数
        this.excavationsCount = 0;
        //重置按钮
        this.view.btnReset.visible = true;

        this.view.map.clip.panel0.alpha = 1f;
        this.view.map.clip.panel1.alpha = 1f;
        this.view.map.clip.panel2.alpha = 1f;

        //门
        if (this.view.map.clip.TenMan != null)
        {
            this.view.map.clip.TenMan.visible = true;
            this.view.map.clip.TenMan.spineAnimation.state.SetAnimation(0, "CommonEx_tenman1", false);
        }

        //宝箱提示
        this.view.map.clip.boxTips.visible = false;
        this.view.map.clip.boxBar.visible = false;
        //宝箱显示情况
        RefBox();
        ClearAllMapCom();

        //障碍物
        if (this.WallLimit > 0)
        {
            for (int i = 0; i < this.WallLimit; i++)
            {
                NewWall();
            }
        }
        ResetBoxWalkPos();
        Debug.Log("箱子的位置有 = " + this.wallWalkList.Count);
        //箱子
        if (this.BoxLimit > 0)
        {
            for (int i = 0; i < this.BoxLimit; i++)
            {
                NewBox();
            }
        }
        //重置点的XY轴两格子四个方向不能存在障碍物
        ResetSoltWalkPos();
        //置放点
        if (this.SlotLimit > 0)
        {
            for (int i = 0; i < this.SlotLimit; i++)
            {
                NewSlot();
            }
        }
        //初始化玩家的位置
        InitHeroBirthPoint();
        //排序
        SortAllObj();

        //foreach(var item in boxList)
        //{
        //    item.Value.xy = GetCellXY(new Vector2(0, 0));
        //    break;
        //}
    }
    /// <summary>
    /// 层级排序
    /// </summary>
    private void SortAllObj()
    {
        allObjList.Sort((a, b) => a.y.CompareTo(b.y));

        // 分配排序顺序
        for (int i = 0; i < allObjList.Count; i++)
        {
            allObjList[i].sortingOrder = i;
        }

        //foreach (var item in slotList)
        //{
        //    item.Value.sortingOrder = 0;
        //}

        //this.hero.sortingOrder = allObjList.Count + 1;
    }
    /// <summary>
    /// 重置按钮事件
    /// </summary>
    /// <param name="context"></param>
    private void OnResetGame(EventContext context)
    {
        if (this.boolplaying) { return; }
        this.boolplaying = true;
        this.moveTweener?.Kill();
        this.moveTweener = null;
        this.terminus.visible = false;
        //播放离开的传送特效
        UIExtensions.PlayChuanSongBegin(this.hero, this.heroSpine, () =>
        {
            ResetAllPos();
            this.boolplaying = false;
        }, new Vector2(-0.25f, -0.4f), true);
    }

    /// <summary>
    /// 刷新重置后的位置
    /// </summary>
    private void ResetAllPos()
    {
        //foreach (var item in this.cellPosList)
        //{
        //    this.spaceList[(int)item.x, (int)item.y] = true;
        //}
        //this.boxList.Clear();
        ////将宝箱和玩家的位置全部重置
        //foreach (var item in this.boxPosRecord)
        //{
        //    this.spaceList[(int)item.Key.x, (int)item.Key.y] = false;
        //    item.Value.data = item.Key;
        //    this.boxList.Add(item.Key, item.Value);
        //    var box = ((UI_SokobanBoxMove)item.Value);
        //    box.Clip.c1.selectedIndex = 0;
        //    item.Value.xy = GetCellXY(item.Key);
        //    //item.Value.y += box.icon.icon.y;
        //    this.view.map.clip.panel2.AddChild(box);
        //}
        //foreach (var item in this.slotList)
        //{
        //    this.spaceList[(int)item.Key.x, (int)item.Key.y] = false;
        //}
        //foreach (var item in this.wallList)
        //{
        //    this.spaceList[(int)item.Key.x, (int)item.Key.y] = false;
        //}
        //this.sealBoxs.Clear();
        ////英雄的位置
        //this.hero.xy = this.heroPosRecord;
        ////排序
        //SortAllObj();

        //RefBoxJianTou();
        //ChkBoxesAround();

        //测试
        EngineBase.EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ENTER_RANDOM_BOX_RESULT, RefBoxRewardRecv);
        ClearAllMapCom();
        OnShow();
    }

    /// <summary>
    /// 点击移动
    /// </summary>
    /// <param name="context"></param>
    private void MoveToTouch(EventContext context)
    {
        if (this.boolplaying || this.TuiPlaying || context == null || this.heroSpine.AnimationName == HeroState.wabao.ToString()) { return; }

        //屏幕坐标
        Vector2 pos = context.inputEvent.position;

        //找到[目的]地的格子
        var ePos = GetCellPos(pos.x, pos.y);
        //点击的是否是箱子
        TouchBoxMove(ePos);
        if (!ChkSpaceCell(ePos)) { return; }

        if (IsOutRange(ePos)) { return; }
        if (this.terminusPos == ePos) { return; }

        var hPos = this.hero.LocalToGlobal(Vector2.zero);
        //找到[起点]地的格子
        var bPos = GetCellPos(hPos.x, hPos.y);

        //this.paths.Clear();
        //this.paths.Add(ePos);
        //MoveNextPos();

        RefBoxJianTou();

        //寻找路径
        GetPathList(bPos, ePos);

        //Debug.Log("点击地块后移动取消");

        if (this.paths.Count() <= 0 || this.TuiPlaying) { return; }
        moveTweener?.Kill();
        moveTweener = null;
        this.terminusPos = ePos;
        MoveNextPos();
        TerminusPos();

        this.view.map.touchable = false;

        GameManager.Instance.TimerManager.SetTimer(0.1f, () =>
        {
            this.view.map.touchable = true;
        });
    }
    /// <summary>
    /// 设置光标终点
    /// </summary>
    private void TerminusPos()
    {
        if (this.paths.Count() <= 0) { return; }
        this.terminus.visible = true;
        var pos = this.paths[this.paths.Count() - 1];
        var p = GetCellXY(pos);
        p += this.view.map.clip.panel1.xy;
        p.x += cellSize * 0.5f;
        p.y += cellSize * 0.5f;
        this.terminus.xy = p;
        this.terminus.x -= this.terminus.width;
        this.terminus.y -= this.terminus.height;
    }
    /// <summary>
    /// 移动到下个路径
    /// </summary>
    /// <returns></returns>
    private bool MoveNextPos()
    {
        if (this.paths.Count <= 0) { return false; }
        var pos = this.paths[0];
        var p = GetCellXY(pos);
        p += this.view.map.clip.panel1.LocalToGlobal(Vector2.zero) / GRoot.contentScaleFactor;
        p.x += cellSize * 0.5f;
        p.y += cellSize * 0.55f;
        //var p = GetRolePos(pos);
        this.paths.RemoveAt(0);
        MoveToPos(p);
        return true;
    }
    /// <summary>
    /// 获取格子的XY（0~5,0~7）
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector2 GetCellPos(float x, float y)
    {
        var mPos = this.view.map.clip.panel1.LocalToGlobal(Vector2.zero);
        mPos += this.originPos;
        float size = (cellSize * GRoot.contentScaleFactor);
        return new Vector2((float)Math.Floor((x - mPos.x) / size), (float)Math.Floor((y - mPos.y) / size));
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    private void MoveToPos(Vector2 targetPos)
    {
        if (this.hero == null) { return; }

        this.moveTweener?.Kill();
        this.moveTweener = null;
        //Debug.Log("移动到目标点取消移动");
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.run);

        Vector2 hPos = this.hero.LocalToGlobal(Vector2.zero);
        hPos /= GRoot.contentScaleFactor;

        //朝向
        this.heroSpine.skeleton.ScaleX = hPos.x < targetPos.x ? 1 : -1;

        //玩家距离目的地
        float dis = Vector2.Distance(targetPos, hPos);
        float ms = dis * this.moveSpeed;

        Vector2 p = targetPos - hPos;
        p.x += this.hero.position.x;
        p.y += this.hero.position.y;

        this.heroDir.x = p.x > this.hero.position.x ? 1 : -1;
        this.heroDir.y = p.y > this.hero.position.y ? 1 : -1;
        //Debug.Log("英雄移动");
        //朝目标移动
        this.moveTweener = this.hero.TweenMove(p, ms).SetEase(EaseType.Linear).OnComplete(() =>
        {
            if (!MoveNextPos())
            {//移动完成
                OverMove();
            }
        });
    }
    /// <summary>
    /// 结束移动
    /// </summary>
    private void OverMove()
    {
        //光标隐藏
        this.terminus.visible = false;
        //动画状态=待机
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.idle);
        //终点清空
        this.terminusPos = new Vector2(-1, -1);
        //检测周边是否有箱子处理
        ChkBoxesAround();
    }

    /// <summary>
    /// 已知起点和终点的格子XY，获取避开障碍的路径
    /// </summary>
    /// <param name="beginPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private List<Vector2> GetPathList(Vector2 beginPos, Vector2 endPos)
    {
        this.visited = new bool[X_Count, Y_Count];
        this.paths.Clear();

        if (ChkSpaceCell(beginPos) && !visited[(int)beginPos.x, (int)beginPos.y])
        {
            if (SearchPath(beginPos, endPos))
            {
                if (!PathContraction())//检测缩减路径
                {
                    PathContraction();
                }
                return this.paths;
            }
        }
        this.paths.Clear();
        return this.paths;
    }
    /// <summary>
    /// 将路径缩短
    /// </summary>
    private bool PathContraction()
    {
        //遍历路径，检测除了下一个节点，是否与其他路径有相连接的，如果有，就将中间的踢除
        for (int i = 0; i < this.paths.Count(); i++)
        {
            //检测四个方向
            foreach (var dir in directions)
            {
                var newPos = this.paths[i] + dir;
                if (ChkSpaceCell(newPos))//只要是空地
                {
                    for (int j = 0; j < this.paths.Count(); j++)
                    {
                        if (i != j && (j - i) > 1 && newPos == this.paths[j])//是否存在列表
                        {
                            //重新拼接一个路径
                            List<Vector2> temp = new List<Vector2>();
                            for (int idx = 0; idx <= i; idx++)
                            {
                                temp.Add(this.paths[idx]);
                            }
                            for (int idx = j; idx < this.paths.Count(); idx++)
                            {
                                temp.Add(this.paths[idx]);
                            }
                            Debug.Log($"原先长度={this.paths.Count()},缩短后={temp.Count()}");
                            this.paths = temp;

                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 寻路
    /// </summary>
    /// <param name="bPos"></param>
    /// <param name="ePos"></param>
    /// <returns></returns>
    private bool SearchPath(Vector2 bPos, Vector2 ePos)
    {
        this.visited[(int)bPos.x, (int)bPos.y] = true;
        this.paths.Add(bPos);
        //新的坐标
        if (ePos == bPos)
        {
            return true;
        }
        //获取距离最近的四个方向中的一个方向
        List<Vector2> tempPos = new List<Vector2>();
        foreach (var dir in directions)
        {
            var p = new Vector2(bPos.x + dir.x, bPos.y + dir.y);
            tempPos.Add(p);
        }
        //排序找最近的（这个寻路顺序还要优化，因为方向不一定是目标方向，可能是最佳路径方向）
        tempPos.Sort((a, b) => Vector2.Distance(a, ePos) < Vector2.Distance(b, ePos) ? -1 : 1);

        // 尝试四个方向，可能的移动方向优先
        foreach (var p in tempPos)
        {
            // 检查新位置是否有效且未被访问
            if (ChkSpaceCell(p) && !this.visited[(int)p.x, (int)p.y])
            {
                if (SearchPath(p, ePos))
                {//找到适合的就返回
                    return true;
                }
            }
        }

        paths.RemoveAt(paths.Count - 1);//移除最后一个重复的

        //还是找不到
        return false;
    }

    private bool IsOutRange(Vector2 pos)
    {
        if (pos.x < 0 || pos.x >= X_Count || pos.y < 0 || pos.y >= Y_Count)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 返回是否是可以行走空地
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ChkSpaceCell(Vector2 pos)
    {
        if (IsOutRange(pos))
        {
            return false;
        }
        //都不能走
        //return this.spaceList[(int)pos.x, (int)pos.y];
        //阻碍物不能走
        if (this.wallList.ContainsKey(pos))
            return false;

        //坑不能走(没有填木箱的情况)
        if (this.slotList.ContainsKey(pos) && !this.sealBoxs.ContainsKey(pos))
            return false;

        //木箱不能走
        if (this.boxList.ContainsKey(pos))
            return false;//不在地下的

        return true;
    }

    /// <summary>
    /// 显示中途退出
    /// </summary>
    private void ShowExitTips()
    {
        if (this.boolplaying) { return; }
        //1、事件完成后但玩家宝箱仍有挖掘次数，此时点击返回键，提示StingID 8073 奖励未领取，是否退出，
        //2、若推箱子未完成，此时点击返回键，提示StingID  8065 若离开山洞，本次事件将被判定为失败
        //3、如果挖掘次数为0，点击返回键直接退出深埋宝藏，不需要提示界面
        if (this.excavationsCount >= this.WaBaoLimit)
        {
            OnExit();
        }
        else
        {
            int id = this.boxList.Count > 0 ? 8065 : 8073;
            TipsManger.Instance.ShowMessagePopup(ConfigUtils.GetStringByKey(id), ConfigUtils.GetStringByKey(32), ConfigUtils.GetStringByKey(31), (bool isAgree) =>
            {//点击同意
                if (isAgree)
                {
                    OnExit();
                }
            });
        }

    }
    private void OnExit()
    {
        if (boolplaying) { return; }
        boolplaying = true;
        //播放离开的传送特效
        UIExtensions.PlayChuanSongBegin(this.hero, this.heroSpine, () =>
        {
            //告知服务端
            var msg = ExitBoxMap_CS.CreateBuilder();
            msg.EventGuid = this.eventGuid;//事件guid
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ExitBoxMap_CS, msg.Build());
            //将事件标记为完成
            MapChapterManager.Instance.AddFinishEventCount((int)eRandomEventType.eRandomEventType_RandomBox);
            UIManager.Instance.CloseUIPanel("SokobanMap");
            var mapView = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
            if (mapView != null)
                mapView.CloseSokobanMapView();
        }, new Vector2(-0.25f, -0.4f), false);
    }
    /// <summary>
    /// 点击箱子移动箱子
    /// </summary>
    /// <param name="context"></param>
    private bool TouchBoxMove(Vector2 TouchPos)
    {
        if (!this.boxList.ContainsKey(TouchPos)) { return false; }

        var box = this.boxList[TouchPos] as UI_SokobanBoxMove;
        //根据角色的位置和木箱的位置进行判断推动的方向
        var hPos = this.hero.LocalToGlobal(Vector2.zero);
        //角色站在的格子XY
        var h_p = GetCellPos(hPos.x, hPos.y);

        //寻找周边的箱子
        foreach (var dir in directions)
        {
            Vector2 n_P = h_p + dir;
            if (TouchPos == n_P)
            {
                return PushBox(box, h_p, TouchPos);
            }
        }

        return false;
    }

    private void RefBoxJianTou()
    {
        foreach (var box in this.boxList)
        {
            ((UI_SokobanBoxMove)box.Value).jiantou.visible = false;
        }
    }

    /// <summary>
    /// 检测周边是否有箱子
    /// </summary>
    private void ChkBoxesAround()
    {
        //获取角色的位置
        var hPos = this.hero.LocalToGlobal(Vector2.zero);
        //角色站在的格子XY
        var p = GetCellPos(hPos.x, hPos.y);

        RefBoxJianTou();

        //寻找周边的箱子
        foreach (var dir in directions)
        {
            var newPos = p + dir;
            if (this.boxList.ContainsKey(newPos))
            {
                //光标发光
                var box = this.boxList[newPos] as UI_SokobanBoxMove;
                box.jiantou.visible = true;
                box.t0.Play();
                //箭头旋转
                if (dir.x == 1)
                    box.jiantou.rotation = 0;//向右
                else if (dir.x == -1)
                    box.jiantou.rotation = -180;//向左
                else if (dir.y == -1)
                    box.jiantou.rotation = -90;//向上
                else if (dir.y == 1)
                    box.jiantou.rotation = 90;//向下
            }
        }
    }

    /// <summary>
    /// 检测推动的终点是否有阻碍物和边界，还有其他箱子
    /// </summary>
    /// <param name="p">点</param>
    /// <returns></returns>
    private bool ChkCanPromoted(Vector2 p)
    {
        if (IsOutRange(p)) { return false; }
        //不能推向阻碍物
        if (this.wallList.ContainsKey(p))
            return false;
        //不能推向木箱（没有在地下的）
        if (this.boxList.ContainsKey(p))
            return false;

        return true;
    }

    /// <summary>
    /// 推动箱子(角色朝向箱子进行推动)
    /// </summary>
    /// <param name="box"></param>
    private bool PushBox(GComponent box, Vector2 heroXY, Vector2 boxXY)
    {
        if (box == null) { return false; }

        if (!this.boxList.ContainsKey(boxXY))
        {//已经被封印，不能使用
            return false;
        }

        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.BoxMoveSE);

        //根据箱子的位置播放对应的动画
        if (boxXY.y < heroXY.y)
        {//把箱子往上推
            return MoveBox(box, directions[0]);
        }
        else if (boxXY.y > heroXY.y)
        {//把箱子往下推
            return MoveBox(box, directions[1]);
        }
        else if (boxXY.x > heroXY.x)
        {//把箱子往右推
            return MoveBox(box, directions[2]);
        }
        else if (boxXY.x < heroXY.x)
        {//把箱子往左推
            return MoveBox(box, directions[3]);
        }
        return false;
    }

    /// <summary>
    /// 播放箱子移动动画
    /// </summary>
    /// <param name="box"></param>
    /// <param name="dir"></param>
    private bool MoveBox(GComponent box, Vector2 dir)
    {
        var pos = (Vector2)box.data;
        //如果有墙和边界不能推
        if (!ChkCanPromoted(pos + dir))
        {
            return false;
        }
        //排序
        SortAllObj();

        spaceList[(int)pos.x, (int)pos.y] = true;
        boxList.Remove(pos);
        Vector2 p = GetRolePos(pos);//玩家的目的地
        pos += dir;
        //位置设置
        spaceList[(int)pos.x, (int)pos.y] = false;
        box.data = pos;
        boxList.Add(pos, box);
        this.boolplaying = true;
        //箱子要移动到的位置
        var mPos = GetCellXY(pos);

        this.TuiPlaying = true;
        //推动动作
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.tui);
        //角色也跟随移动
        this.moveTweener?.Kill();
        this.moveTweener = null;
        if (dir.x != 0)
            this.hero.x += dir.x * cellSize * 0.2f;
        //if(dir.y != 0)
        //this.hero.y += dir.y * cellSize * 0.1f;
        Vector2 hPos = this.hero.LocalToGlobal(Vector2.zero);
        hPos /= GRoot.contentScaleFactor;
        Vector2 boxPos = box.LocalToGlobal(Vector2.zero);
        this.heroSpine.skeleton.ScaleX = dir.x == 1 ? 1 : -1;

        //玩家距离目的地
        float dis = Vector2.Distance(p, this.hero.xy);
        float ms = dis * this.moveSpeed;
        Vector2 p1 = p;
        //播放
        var boxMove = (UI_SokobanBoxMove)box;
        if (dir.x == -1)
        {//往左
            boxMove.left.visible = true;
            p1.x += dir.x * cellSize * 0.2f;
        }
        else if (dir.x == 1)
        {//往左
            boxMove.right.visible = true;
            p1.x += dir.x * cellSize * 0.2f;
        }
        else if (dir.y == -1)
        {//往上
            boxMove.up.visible = true;
            p1.y += dir.y * cellSize * 0.1f;
        }
        else if (dir.y == 1)
        {//往下
            boxMove.down.visible = true;
            p1.y += dir.y * cellSize * 0.1f;
        }

        //朝目标移动
        this.moveTweener = this.hero.TweenMove(p1, ms).SetEase(EaseType.Linear).OnComplete(() =>
        {
            //角色回到格子中心
            this.hero?.SetXY(p.x, p.y);
            this.TuiPlaying = false;
        });

        box.TweenMove(mPos, ms)
        .SetEase(EaseType.Linear)
        .OnComplete(() =>
        {
            this.TuiPlaying = false;
            boxMove.left.visible = false;
            boxMove.right.visible = false;
            boxMove.up.visible = false;
            boxMove.down.visible = false;
            this.boolplaying = false;
            UIExtensions.PlayHeroState(this.heroSpine, HeroState.idle);
            ChkPutInCorresponding(box);
            ChkBoxesAround();
        });
        return true;
    }

    /// <summary>
    /// 检测箱子推入到放置点插槽
    /// </summary>
    private void ChkPutInCorresponding(GComponent box)
    {
        Vector2 pos = (Vector2)box.data;
        //检测是否位于封印列表
        if (this.sealBoxs.ContainsKey(pos))
        {
            return;
        }

        //检测位置是否在插槽孔内
        foreach (var item in slotList)
        {
            if (item.Key == pos)
            {
                GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.BoxInTrenchSE);

                this.spaceList[(int)pos.x, (int)pos.y] = true;
                //将宝箱设置未不可再移动的
                this.sealBoxs.Add(pos, box);
                this.boxList.Remove(pos);
                Debug.Log("嵌入成功！");
                //播放下陷动画
                var b = (UI_SokobanBoxMove)box;
                b.Clip.t0.Play();
                GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
                {
                    this.view.map.clip.panel1.AddChild(box);
                    SortAllObj();
                    //道路变成可以行走
                    this.spaceList[(int)pos.x, (int)pos.y] = true;
                    b.Clip.c1.selectedIndex = 1;
                    //检测是否已经满足打开大门的条件
                    if (this.sealBoxs.Count >= this.slotList.Count)
                    {
                        Debug.Log("全部嵌入！");
                        RefBox();
                        this.boolplaying = true;
                        //所有东西都消失
                        foreach (var com in allObjList)
                        {
                            com.TweenFade(0f, 0.3f);
                        }
                        this.view.map.clip.panel0.TweenFade(0f, 0.3f);
                        this.view.map.clip.panel1.TweenFade(0f, 0.3f);
                        this.view.map.clip.panel2.TweenFade(0f, 0.3f);

                        //隐藏重置按钮
                        this.view.btnReset.visible = false;

                        this.paths.Clear();
                        this.paths.Add(new Vector2(3, 2));
                        MoveNextPos();

                        //地图向下移动
                        this.view.map.clip.TweenMove(new Vector2(0f, 0f), 0.3f)
                            .SetEase(EaseType.Linear)
                            .OnComplete(() =>
                            {
                                //播放大门开启特效
                                if (this.view.map.clip.TenMan != null)
                                {
                                    this.view.map.clip.TenMan.spineAnimation.state.SetAnimation(0, "CommonEx_tenman2", false);
                                }
                                this.boolplaying = false;

                                //深埋宝藏引导-点击任意地方继续
                                GuideManager.Instance.StarGuideByData(new GuideData()
                                {
                                    fid = FuncOpenType.Sokoban,
                                    giding = GuideID.guideId_3702,
                                    gid = GuideID.guideId_3703,
                                    tui = this.view.map.clip.box,
                                    isForce = true,
                                    isSend = true,
                                    //isLucency = true,//透明强制引导的黑色遮罩，但只能点击手指指向区域
                                    scale = new Vector2(0.6f, 0.7f),
                                    npcTxt = "Beginner_Doc_021",
                                    npcPosType = PosType.Down,
                                    //gType = global::GuideType.ClickFreely,
                                    touchCB = () =>
                                    {
                                        GuideManager.Instance.HideGuide();
                                    }
                                });
                            });
                    }
                });
                //box.TweenMoveY(box.y + 3f, 0.1f)
                //    .SetEase(EaseType.QuadIn)
                //    .OnComplete(() =>
                //    {

                //});


                break;
            }
        }

    }
    /// <summary>
    /// 点击宝箱，千万宝箱的固定点
    /// </summary>
    private void BoxTouch()
    {
        if (this.sealBoxs.Count < this.slotList.Count)
        {
            UIManager.Instance.ToastByKey(8072);
            return;
        }
        if (this.isWait) { return; }
        if (this.boolplaying) { return; }
        if (this.hero == null) { return; }

        this.paths.Clear();
        this.moveTweener?.Kill();
        this.moveTweener = null;
        //Debug.Log("点击宝箱后移动取消");

        var pos = this.hero.xy;
        this.heroSpine.skeleton.ScaleX = this.hero.x < (this.view.map.clip.box.x + this.view.map.clip.box.width * 0.5f) ? 1 : -1;
        float dis1 = Vector2.Distance(pos, this.view.map.clip.pos1.xy);
        if (dis1 < 10)
        {//距离很近就直接进入挖宝
            BoxExcavate();
            return;
        }
        float dis2 = Vector2.Distance(pos, this.view.map.clip.pos2.xy);
        if (dis2 < 10)
        {//距离很近就直接进入挖宝
            BoxExcavate();
            return;
        }
        //播放行走
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.run);
        //光标
        this.terminus.visible = true;
        if (dis1 < dis2)
        //if (Random.Range(0,100) < 50)
        {
            this.terminus.xy = this.view.map.clip.pos1.xy;
            //朝目标移动
            this.moveTweener = this.hero.TweenMove(this.view.map.clip.pos1.xy, dis1 * this.moveSpeed).SetEase(EaseType.Linear).OnComplete(() =>
            {//移动完成
                BoxExcavate();
            });
        }
        else
        {
            this.terminus.xy = this.view.map.clip.pos2.xy;
            //朝目标移动
            this.moveTweener = this.hero.TweenMove(this.view.map.clip.pos2.xy, dis2 * this.moveSpeed).SetEase(EaseType.Linear).OnComplete(() =>
            {//移动完成
                BoxExcavate();
            });
        }
        this.terminus.x -= this.terminus.width * 0.5f;
        this.terminus.y -= this.terminus.height * 0.5f;
    }

    /// <summary>
    /// 点击宝箱开始挖掘
    /// </summary>
    private void BoxExcavate()
    {
        if (this.excavationsCount >= this.WaBaoLimit) { return; }
        if (this.view.map.clip.boxTips.visible == false) { return; }//测试
        this.boolplaying = true;
        //光标
        this.terminus.visible = false;
        //播放挖掘动作
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.wabao);
        //播放宝箱头上进度条
        this.view.map.clip.boxBar.visible = true;
        var bar = (GProgressBar)this.view.map.clip.boxBar.component;
        this.view.map.clip.boxTips.visible = false;
        //进度条动画
        bar.min = 0;
        bar.max = 100;
        bar.value = 0;
        float interval = 0.743f;
        float time = interval * 0.35f;
        float s = 100f / 3f;
        time *= s;
        interval *= s;
        float speedFactor = 0.65f;
        interval *= speedFactor;
        //朝向
        this.heroSpine.skeleton.ScaleX = this.hero.x < this.view.map.clip.box.x + this.view.map.clip.box.width * 0.5f ? 1 : -1;

        GTween.To(0, 100, this.WaBaoTime)
            .SetTarget(bar) // 设置目标对象
            .SetEase(EaseType.Linear) // 线性动画
            .OnUpdate((tweener) =>
            {// 进度条更新回调
                bar.value = tweener.value.d;

                float offset = interval * 0.1f;
                if (tweener.value.x >= time - offset)
                {
                    time += interval;
                    GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralScoopTreasureSE);
                }
            })
            .OnComplete(() =>
            {// 完成回调
                UIExtensions.PlayHeroState(this.heroSpine, HeroState.idle);
                this.boolplaying = false;
                RefBox(() =>
                {
                    if(this.eventData.batchStuffList.Count - 1 < this.excavationsCount) { return; }
                    var bs = this.eventData.batchStuffList[this.excavationsCount];
                    if (bs.cfgId == (int)eRandEventInnerResult.eRandEventInnerResult_LoreBoss)
                    {//boss
                        UIManager.Instance.ShowUIPanel("ChapterEventBossStageDetail", this.eventData, bs.id, true, true);
                    }
                    else
                    {
                        //请求服务端
                        var builder = ClaimRandomBox_CS.CreateBuilder();
                        builder.EventGuid = this.eventGuid;
                        builder.StuffId = (uint)bs.id;
                        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimRandomBox_CS, builder.Build());
                    }
                    //等待服务器回包
                    isWait = true;

                    GameManager.Instance.TimerManager.ClearTimer(this.WaitDispose);
                    GameManager.Instance.TimerManager.SetTimer(2f, this.WaitDispose);
                });
            });
    }
    private void WaitDispose()
    {
        isWait = false;
    }
    /// <summary>
    /// 刷新宝箱显示
    /// </summary>
    private void RefBox(Action cb = null)
    {
        int count = this.WaBaoLimit;
        //几次的文本
        this.view.map.clip.num.text = $"{this.excavationsCount}/{count}";

        this.view.map.clip.boxTips.visible = this.slotList.Count > 0 && this.sealBoxs.Count >= this.slotList.Count && this.excavationsCount < count;
        this.view.map.clip.boxBar.visible = false;

        if(this.slotList.Count <= 0 || this.sealBoxs.Count < this.slotList.Count)
        {//没解锁，不可挖掘，宝箱闭合
            this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_1", true);
        }
        else
        {//解锁
            if(this.excavationsCount == this.WaBaoLimit - 1 && cb != null)
            {
                //最后一次开箱，播放开宝箱特效
                this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_3", false);
                GameManager.Instance.TimerManager.SetTimer(0.4f, () =>
                {
                    this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_4", false);
                    GameManager.Instance.TimerManager.SetTimer(0.3f, () =>
                    {
                        cb?.Invoke();
                        this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_5", false);
                    });
                });
                return;
            }
            else if (this.excavationsCount >= 1 && this.excavationsCount < this.WaBaoLimit - 1)
            {//解锁，已挖掘
                this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_2", true);
            }
            else if (this.excavationsCount >= this.WaBaoLimit)
            {//解锁，挖掘完
                this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_5", true);
                GameManager.Instance.TimerManager.SetTimer(1f, () =>
                {
                    AboutLeave();
                });
            }
        }

        cb?.Invoke();
    }

    /// <summary>
    /// 即将离开副本
    /// </summary>
    private void AboutLeave()
    {
        var view = UIManager.Instance.FindByName("SokobanReward");
        if (view != null && view.IsShow())
        {
            //延迟2秒，等关闭
            GameManager.Instance.TimerManager.SetTimer(2f, () =>
            {
                AboutLeave();
            });
            return;
        }

        var reward = UIManager.Instance.FindByName("GetReward");
        if (reward != null && reward.IsShow())
        {
            //延迟2秒，等关闭
            GameManager.Instance.TimerManager.SetTimer(2f, () =>
            {
                AboutLeave();
            });
            return;
        }
        

        //延迟2秒，开始传送
        //GameManager.Instance.TimerManager.SetTimer(0.5f, () =>
        //{
            OnExit();
        //});
    }

    /// <summary>
    /// 挖宝后刷新
    /// </summary>
    public void RefBoxRewardRecv()
    {
        if (!IsShow() || !IsOnStage()) { return; }
        GameManager.Instance.TimerManager.ClearTimer(this.WaitDispose);
        this.excavationsCount++;
        this.boolplaying = false;
        this.isWait = false;
        RefBox();
        //if (this.excavationsCount == 1 && this.excavationsCount < this.WaBaoLimit)
        //{
        //    //播放开宝箱特效
        //    this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_2", false);

        //    this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_3", false);
            
        //    this.view.map.clip.boxSpine?.spineAnimation.state.SetAnimation(0, "CommonEx_bx_4", true);
        //}
    }
}