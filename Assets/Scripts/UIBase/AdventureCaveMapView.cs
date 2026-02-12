
using AdventureCave;
using CommonEx;
using Config;
using Engine;
using FairyGUI;
using msg;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 奇遇商人地图
/// </summary>
public class AdventureCaveMapView : UIViewBase
{
    /// <summary>
    /// 地图
    /// </summary>
    private UI_AdventureCaveView view => this.main as UI_AdventureCaveView;

    /// <summary>
    /// 移动
    /// </summary>
    private GTweener moveTweener = null;

    /// <summary>
    /// 播放提示
    /// </summary>
    private GTweener playTitleTweener = null;

    /// <summary>
    /// 英雄角色
    /// </summary>
    private GGraph hero = null;
    /// <summary>
    /// 角色动画spine
    /// </summary>
    private SkeletonAnimation heroSpine = null;
    /// <summary>
    /// 终点光标
    /// </summary>
    private GLoader3D terminus = null;

    /// <summary>
    /// 门的顺序，一共5关，第六层没有关卡顺序
    /// </summary>
    private int[] portalSequence = new int[5];

    //当前楼层
    private int level = 1;
    /// <summary>
    /// 气泡id
    /// </summary>
    private const int bubbleId = 1010;
    /// <summary>
    /// 气泡配置
    /// </summary>
    private ConfigBubbleUnit bubble = null;
    /// <summary>
    /// 传送门目标点（用于检测是否到达）
    /// </summary>
    private Vector2 destination = Vector2.zero;
    /// <summary>
    /// 传送门标记
    /// </summary>
    private int gateIdx = 0;
    /// <summary>
    /// 事件ID
    /// </summary>
    public ulong eventGuid = 0;
    /// <summary>
    /// 父节点
    /// </summary>
    //private GComponent root = null;
    /// <summary>
    /// 传送中
    /// </summary>
    private bool isTransfer = false;

    public AdventureCaveMapView()
    {
        //this.type = UIType.Normal;
        this.package = "AdventureCave";
        this.name = "AdventureCaveView";
        this.component = "AdventureCaveView";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.GuideType = FuncType.Guide;
    }
    public override void BindAll()
    {
        base.BindAll();
        AdventureCaveBinder.BindAll();
    }
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        //this.root = (GComponent)values[0];
        this.eventGuid = (ulong)values[1];
    }
    protected override void OnInit()
    {
        base.OnInit();
        ((UI_Bubble)this.view.map.panel.qipao).talkDes.text = "";

        InitMap();
        InitHero();
    }

    /// <summary>
    /// 初始化传送门的点击
    /// </summary>
    private void InitMap()
    {
        //四个门的点击响应
        for (int i = 1; i <= 4; i++)
        {
            GLoader p = this.view.map.panel.GetChild($"p{i}").asLoader;
            p?.onClick.Add(this.OnTouchPortal);
        }
        //地图点击移动事件
        this.view.map.panel.walk1?.onClick.Add(this.MoveToTouch);
        this.view.map.panel.walk2?.onClick.Add(this.MoveToTouch);
        //退出传送门
        this.view.map.panel.homeGate?.onClick.Add(this.ExitGate);
        this.view.close?.onClick.Add(this.ShowExitTips);
        //NPC
        this.view.map.panel.npcTouch?.onClick.Add(this.TouchNPC);
        this.view.map.panel.qipao?.onClick.Add(this.TouchNPC);
        this.view.map.panel.car?.onClick.Add(this.TouchNPC);
    }
    protected override void OnShow()
    {
        base.OnShow();
        this.isTransfer = false;
        this.level = 1;
        InitPortalSequence();
        ChangeMapLevel();
        
        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.EventMapBGM);

        //奇遇山洞引导-点击第一个正确的门
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.AdventureBusiness,
            giding = GuideID.guideId_3901,
            gid = GuideID.guideId_3902,
            tui = this.view.map.panel.GetChild($"p{portalSequence[0]}"),
            isForce = true,
            isSend = true,
            //pType = PosType.Left,
            npcTxt = "Beginner_Doc_026",
            npcPosType = PosType.Down,
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        });
    }
    protected override void OnHide()
    {
        base.OnHide();
    }

    /// <summary>
    /// 初始化四个门的顺序
    /// </summary>
    private void InitPortalSequence()
    {
        for (int i = 0; i < 5; i++)
        {
            portalSequence[i] = Random.Range(1, 5);
        }
        Debug.Log($"=========顺序:{portalSequence[0]}、{portalSequence[1]}、{portalSequence[2]}、{portalSequence[3]}、{portalSequence[4]}");
    }
    /// <summary>
    /// 初始化英雄角色
    /// </summary>
    private void InitHero()
    {
        this.hero = this.view.map.panel.hero;
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

        Utils.SetSpineModelOnFGUI(this.view.map.panel.npc, "Hero_30003", 16f, HeroState.idle.ToString(), (o) =>
        {
            if (o is SkeletonAnimation animation)
            {
                //animation.
                //this.heroSpine = animation;
                //等待异步创建spine
                //UIExtensions.PlayChuanSongEnd(this.hero, this.heroSpine, new Vector2(-0.25f, -0.4f));
            }
        });


        this.terminus = this.view.map.panel.terminus;
        InitHeroBirthPoint();
    }
    /// <summary>
    /// 播放英雄的状态
    /// </summary>
    /// <param name="HeroState">动画名枚举</param>
    /// <param name="isLoop">是否循环</param>
    //private void PlayHeroState(HeroState stateName, bool isLoop = true)
    //{
    //    string name = stateName.ToString();
    //    if (this.heroSpine != null && !this.heroSpine.AnimationName.Contains(name))
    //    {
    //        this.heroSpine.state?.SetAnimation(0, name, isLoop);
    //    }
    //}
    /// <summary>
    /// 初始化出生地点
    /// </summary>
    private void InitHeroBirthPoint()
    {
        if (this.hero != null && this.view.map.panel.rolePos != null)
        {
            //初始化角色位置
            this.hero?.SetXY(this.view.map.panel.rolePos.x, this.view.map.panel.rolePos.y);
            //等待异步创建spine
            if (this.heroSpine != null)
            {
                UIExtensions.PlayChuanSongEnd(this.hero, this.heroSpine, new Vector2(-0.25f, -0.4f));
            }
        }
    }

    /// <summary>
    /// 切换地图层级
    /// </summary>
    private void ChangeMapLevel()
    {
        this.view.map.panel.c1.selectedIndex = this.level <= 5 ? 0 : 1;
        //this.view.map.panel.c1.selectedIndex = 1;

        switch (this.view.map.panel.c1.selectedIndex)
        {
            case 0://小于等于5层
                RefTunnelTower();
                break;
            case 1://第六层
                RefHighestTower();
                break;
        }

        //初始化玩家的位置
        InitHeroBirthPoint();

        PlayMapLevelTips();

        GameManager.Instance.TimerManager.SetTimer(1f, () => {
            this.isTransfer = false;
        });
    }
    /// <summary>
    /// 刷新隧道的界面
    /// </summary>
    private void RefTunnelTower()
    {
    }
    /// <summary>
    /// 刷新第六层的界面
    /// </summary>
    private void RefHighestTower()
    {
        Debug.Log("NPC = " + this.view.map.panel.npc.visible);
        ShowQiPao();
    }
    /// <summary>
    /// 气泡显示
    /// </summary>
    private void ShowQiPao()
    {
        if (bubble == null)
        {
            bubble = ConfigUtils.GetBubbleById(bubbleId);
        }
        if (bubble != null)
        {
            //气泡文本
            this.view.map.panel.qipao.visible = true;
            var qp = this.view.map.panel.qipao as UI_Bubble;
            if (qp.talkDes.text.Equals(""))
            {
                qp.talkDes.text = bubble.DocNote;
            }
            //播放显示后，过几秒再显示
            GameManager.Instance.TimerManager.SetTimer(bubble.Duration, () => { HideQiPao(); });
        }
    }
    /// <summary>
    /// 隐藏气泡
    /// </summary>
    private void HideQiPao()
    {
        this.view.map.panel.qipao.visible = false;
        if (bubble != null)
        {
            GameManager.Instance.TimerManager.SetTimer(bubble.Time, () => { ShowQiPao(); });
        }
    }

    /// <summary>
    /// 播放层级提示
    /// </summary>
    private void PlayMapLevelTips()
    {
        this.playTitleTweener?.Kill();
        this.playTitleTweener = null;

        this.view.ziti.visible = true;
        this.view.ziti.text = ConfigUtils.FormatStringByKey(8069, this.level);
        this.view.t0.Play();

        this.playTitleTweener = GTween.To(0f, 1f, 3f).OnComplete(() =>
        {
            this.view.ziti.visible = false;
        });
    }

    /// <summary>
    /// 点击地块移动（矩形范围内随便动）
    /// </summary>
    /// <param name="context"></param>
    private void MoveToTouch(EventContext context)
    {
        if (ChkEventPastDue() || this.isTransfer) { return; }
        if (context == null) { return; }
        Vector2 pos = context.inputEvent.position;
        pos /= GRoot.contentScaleFactor;
        //Debug.Log(pos);
        //转换地图坐标
        //Vector2 mapPos = this.view.map.panel.map.l(pos);
        //mapPos.y = this.view.map.panel.map.height - mapPos.y;
        //移动到目的地
        MoveToPos(pos);
    }
    /// <summary>
    /// 点击传送门
    /// </summary>
    /// <param name="context"></param>
    private void OnTouchPortal(EventContext context)
    {
        if (ChkEventPastDue() || this.isTransfer) { return; }

        // 获取点击的按钮对象
        GLoader button = context.sender as GLoader;
        Vector2 targetPos = Vector2.zero;
        switch (button.name)
        {
            case "p1"://上
                {
                    //移动到目标点的下面
                    targetPos = this.view.map.panel.p1.LocalToGlobal(Vector2.zero);
                    targetPos /= GRoot.contentScaleFactor;
                    targetPos.x += this.view.map.panel.p1.width * 0.5f;
                    targetPos.y += this.view.map.panel.p1.height * 0.75f;
                    this.destination = targetPos;
                    this.gateIdx = 1;
                    MoveToPos(targetPos);
                }
                break;
            case "p2"://右
                {
                    //移动到目标点的左面
                    targetPos = this.view.map.panel.p2.LocalToGlobal(Vector2.zero);
                    targetPos /= GRoot.contentScaleFactor;
                    targetPos.x += this.view.map.panel.p2.width * 0.25f;
                    targetPos.y += this.view.map.panel.p2.height * 0.58f;
                    this.destination = targetPos;
                    this.gateIdx = 2;
                    MoveToPos(targetPos);
                }
                break;
            case "p3"://下
                {
                    //移动到目标点的上面
                    targetPos = this.view.map.panel.p3.LocalToGlobal(Vector2.zero);
                    targetPos /= GRoot.contentScaleFactor;
                    targetPos.x += this.view.map.panel.p3.width * 0.5f;
                    targetPos.y += this.view.map.panel.p3.height * 0.20f;
                    this.destination = targetPos;
                    this.gateIdx = 3;
                    MoveToPos(targetPos);
                }
                break;
            case "p4"://左
                {
                    //移动到目标点的右面
                    targetPos = this.view.map.panel.p4.LocalToGlobal(Vector2.zero);
                    targetPos /= GRoot.contentScaleFactor;
                    targetPos.x += this.view.map.panel.p4.width * 0.75f;
                    targetPos.y += this.view.map.panel.p4.height * 0.55f;
                    this.destination = targetPos;
                    this.gateIdx = 4;
                    MoveToPos(targetPos);
                }
                break;
        }
    }

    /// <summary>
    /// 移动到目标点
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    private void MoveToPos(Vector2 targetPos)
    {
        if (this.hero == null) { return; }

        //Debug.Log($"点击地图 = {targetPos}");

        moveTweener?.Kill();
        moveTweener = null;
        //播放行走
        UIExtensions.PlayHeroState(this.heroSpine, HeroState.run);

        //设置光标
        this.terminus.visible = true;
        this.terminus.xy = targetPos;
        this.terminus.x -= this.terminus.width;
        this.terminus.y -= this.terminus.height;

        Vector2 hPos = this.hero.LocalToGlobal(Vector2.zero);
        hPos /= GRoot.contentScaleFactor;

        //朝向
        this.heroSpine.skeleton.ScaleX = hPos.x < targetPos.x ? 1 : -1;

        //玩家距离目的地
        float dis = Vector2.Distance(targetPos, hPos);
        float moveSpeed = dis * 0.005f;

        Vector2 p = targetPos - hPos;
        p.x += this.hero.position.x;
        p.y += this.hero.position.y;
        //朝目标移动
        this.moveTweener = this.hero.TweenMove(p, moveSpeed).SetEase(EaseType.Linear).OnComplete(() =>
        {
            //移动完成
            OverMove();
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

        ChkMoveToGate();
    }

    /// <summary>
    /// 检测是否走到传送门位置
    /// </summary>
    private void ChkMoveToGate()
    {
        //检测是否走到指定区域
        if (this.gateIdx != 0)
        {
            Vector2 pos = this.hero.LocalToGlobal(Vector2.zero);
            pos /= GRoot.contentScaleFactor;
            float dis = Vector2.Distance(this.destination, pos);
            if (dis < 20)
            {
                if (this.gateIdx == 6)
                {
                    //打开商店界面
                    UIManager.Instance.ShowUIPanel("AdventureCaveShop", true, this.eventGuid);
                }
                else if (this.gateIdx == 5)
                {
                    //离开
                    OnExit();
                }
                else
                {
                    //是否进入下一关
                    if (this.portalSequence[this.level - 1] == this.gateIdx)
                    {//下一关
                        ++this.level;
                    }
                    else
                    {//重新开始
                        this.level = 1;
                    }
                    this.gateIdx = 0;

                    PlayChuanSong();
                }
            }
        }
    }

    //播放传送特效
    private void PlayChuanSong()
    {
        this.isTransfer = true;
        UIExtensions.PlayChuanSongBegin(this.hero, this.heroSpine, this.ChangeMapLevel, new Vector2(-0.25f, -0.4f));
    }

    /// <summary>
    /// 离开的传送门
    /// </summary>
    private void ExitGate()
    {
        var pos = this.view.map.panel.homeGate.LocalToGlobal(Vector2.zero);
        pos /= GRoot.contentScaleFactor;
        pos.x += this.view.map.panel.homeGate.width * 0.5f;
        pos.y += this.view.map.panel.homeGate.height * 0.5f;
        this.gateIdx = 5;
        this.destination = pos;
        MoveToPos(pos);
    }
    private void OnExit()
    {
        if (isTransfer) { return; }
        isTransfer = true;

        //播放离开的传送特效
        UIExtensions.PlayChuanSongBegin(this.hero, this.heroSpine, () =>
        {
            //告知服务端
            var msg = ExitCave_CS.CreateBuilder();
            msg.EventGuid = this.eventGuid;//事件guid
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ExitCave_CS, msg.Build());
            //将事件标记为完成
            MapChapterManager.Instance.AddFinishEventCount((int)eRandomEventType.eRandomEventType_AdventureBusinessMan);
            UIManager.Instance.CloseUIPanel("AdventureCaveMap");
            var mapView = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
            if (mapView != null)
                mapView.CloseAdventureCaveMap();

        }, new Vector2(-0.25f, -0.4f), false);

    }
    /// <summary>
    /// 显示中途退出
    /// </summary>
    private void ShowExitTips()
    {
        TipsManger.Instance.ShowMessagePopup(ConfigUtils.GetStringByKey(8065), ConfigUtils.GetStringByKey(32), ConfigUtils.GetStringByKey(31), (bool isAgree) =>
        {//点击同意
            if (isAgree)
            {
                OnExit();
            }
        });
    }

    /// <summary>
    /// 点击NPC头上的
    /// </summary>
    private void TouchNPC()
    {
        var pos = this.view.map.panel.npc.LocalToGlobal(Vector2.zero);
        pos /= GRoot.contentScaleFactor;
        pos.x += this.view.map.panel.npc.width * 0.5f;
        pos.y += this.view.map.panel.npc.height + 50f;
        this.gateIdx = 6;
        this.destination = pos;
        MoveToPos(pos);
    }

    /// <summary>
    /// 检测事件是否到期
    /// </summary>
    /// <returns></returns>
    private bool ChkEventPastDue()
    {
        //int eventType = MapChapterManager.Instance.GetEventTypeByGuid(this.eventGuid);
        //if (eventType == -1)
        //{//过期
        //    MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        //    {
        //        OkCallBack = () =>
        //        {
        //            //退出
        //            OnExit();
        //        }
        //    };
        //    UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(5119), param, false);

        //    return true;
        //}
        return false;
    }
}