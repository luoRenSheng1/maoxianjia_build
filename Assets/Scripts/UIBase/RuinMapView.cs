
using BigMap;
using Config;
using Engine;
using FairyGUI;
using msg;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using RandomEventMonster = Engine.RandomEventMonster;

public class RuinMapView : UIViewBase
{
    private UI_RuinMap _ruinMap => this.main as UI_RuinMap;

    private ulong eventGuid;
    private List<RandomEventMonster> randomEventMonsterList = new List<RandomEventMonster>();

    private List<BuffInfo> _showBuffInfos = new List<BuffInfo>();//需要展示的buff列表，过期的不展示

    private SkeletonAnimation _heroSpine;//英雄
    private GTweener _moveTween;
    private string _currentPosition; // 记录英雄当前位置
    private bool _isFacingRight = true; // 当前朝向（默认向右）

    private SkeletonAnimation _bossSpine0;
    private SkeletonAnimation _bossSpine1;
    private SkeletonAnimation _bossSpine2;

    // 添加移动状态标志
    private bool _isMoving = false;

    private Dictionary<int, ConfigEventStageUnit> _eventStageUnitDic = new Dictionary<int, ConfigEventStageUnit>();

    // Boss与目标位置的映射
    private readonly Dictionary<int, string> _bossTargetMap = new Dictionary<int, string>
        {
            {0, "pos4"},
            {1, "pos7"},
            {2, "pos11"}
        };

    // 节点之间的连接关系（单向路径）
    private readonly Dictionary<string, List<string>> _pathConnections = new Dictionary<string, List<string>>
        {
            {"pos0", new List<string> {"pos1"}},
            {"pos1", new List<string> {"pos2"}},
            {"pos2", new List<string> {"pos3"}},
            {"pos3", new List<string> {"pos4"}},
            {"pos4", new List<string> {"pos5"}},
            {"pos5", new List<string> {"pos6"}},
            {"pos6", new List<string> {"pos7"}},
            {"pos7", new List<string> {"pos8"}},
            {"pos8", new List<string> {"pos9"}},
            {"pos9", new List<string> {"pos10"}},
            {"pos10", new List<string> {"pos11"}}
        };

    public RuinMapView()
    {
        this.name = "RuinMap";
        this.package = "BigMap";
        this.component = "RuinMap";
        this.removePackage = true;
        this.safeAreaInset = true;
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _currentPosition = "pos1";
        _ruinMap.ruinPanel.buffList.itemRenderer = BuffListRender;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_AUTOEXIT_RUIN, AutoExitRuin);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateRuinBuffInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_END_RUINBOSS_FIGHT, UpdateMapInfo);
        EventDispatcher.GameWorld.Regist<uint>(EventDefine.EVENT_RUIN_CLAIMBUFF_UPDATE, HideRewardBuild);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        if (_moveTween != null)
        {
            _moveTween.Kill();
            _moveTween = null;
        }
        _currentPosition = "pos1";
        _isMoving = false; // 确保移动状态被重置

        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_AUTOEXIT_RUIN, AutoExitRuin);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CLAIM_BUFF_UPDATE, UpdateRuinBuffInfo);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_END_RUINBOSS_FIGHT, UpdateMapInfo);
        EventDispatcher.GameWorld.UnRegist<uint>(EventDefine.EVENT_RUIN_CLAIMBUFF_UPDATE, HideRewardBuild);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        randomEventMonsterList.Clear();
        eventGuid = (ulong)values[0];
        randomEventMonsterList = values[1] as List<RandomEventMonster>;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        UpdateRuinBuffInfo();
    }

    protected override void OnShow()
    {
        base.OnShow();
        _currentPosition = "pos1";
        UpdateRuinMap();
        UpdateRuinBuffInfo();
        // UpdateMapInfo();

        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.EventMapBGM);

        if (GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Relic,
            giding = GuideID.guideId_3801,
            gid = GuideID.guideId_3802,
            tui = _ruinMap.ruinPanel.bossBtn0,
            isForce = true,
            isSend = true,
            //pType = PosType.Left,
            npcTxt = "Beginner_Doc_023",
            npcPosType = PosType.Down,
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        })) { return; }
    }

    private void UpdateRuinMap()
    {
        InitHero();
        InitRuinMap();
    }

    /// <summary>
    /// 初始化英雄
    /// </summary>
    private void InitHero(string aniName = "idle")
    {
        try
        {
            GObject startPoint = this._ruinMap.ruinPanel.GetChild("pos1");
            this._ruinMap.ruinPanel.hero.SetXY(startPoint.x, startPoint.y);

            // 设置初始朝向（向右）
            this._ruinMap.ruinPanel.hero.scaleX = 1;
            _isFacingRight = true;

            HeroInfo myHero = HeroInfoManager.Instance.GetMyHero();
            Utils.SetSpineModelOnFGUI(this._ruinMap.ruinPanel.hero, myHero.HeroUnit.Model, 50f, aniName, (o =>
            {

                if (o is SkeletonAnimation animation)
                {
                    _heroSpine = animation;
                    UIExtensions.PlayChuanSongEnd(this._ruinMap.ruinPanel.hero, _heroSpine, new Vector2(-0.25f, -0.4f));//传送落地特效
                }

            }));
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("InitHero 英雄初始化出错");
        }
    }

    /// <summary>
    /// 初始化遗迹地图信息
    /// </summary>
    private void InitRuinMap()
    {

        for (int i = 0; i < randomEventMonsterList.Count; i++)
        {
            ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(randomEventMonsterList[i].eventStageId);
            if (eventStageUnit == null)
            {
                Debug.LogError($"eventStageUnit 为 null，id = {randomEventMonsterList[i].eventStageId}");
                continue;
            }

            List<ConfigMonsterGroupUnit> monsterGroupUnits = ConfigUtils.GetMonsterGroupById(eventStageUnit.MonsterData);
            if (monsterGroupUnits.Count > 2)
            {
                Debug.Log("配置数据有误！");
                continue;
            }
            int monsterId = monsterGroupUnits[0].MonsterId;
            ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(monsterId);

            //boss
            // GGraph bossSpine = this._ruinMap.ruinPanel.GetChild("boss" + i ) as GGraph;
            GGraph bossSpine = i == 0 ? _ruinMap.ruinPanel.boss0 :
                i == 1 ? _ruinMap.ruinPanel.boss1 :
                _ruinMap.ruinPanel.boss2;
            bossSpine.visible = true;
            bossSpine.alpha = 1f;
            if (i == 1)
            {
                Utils.SetSpineModelOnFGUI(bossSpine, monsterUnit.Model, 50f, "idle", null, false);
            }
            else
            {
                Utils.SetSpineModelOnFGUI(bossSpine, monsterUnit.Model, 50f, "idle", null, true);
            }
            GButton bossBtn = this._ruinMap.ruinPanel.GetChild("bossBtn" + i) as GButton;
            bossBtn.touchable = true;
            bossBtn.data = i;
            bossBtn.onClick.Add(OnClickBossSpine);

            //奖励
            // GLoader rewardLoader = this._ruinMap.ruinPanel.GetChild("reward" + i ) as GLoader;
            // rewardLoader.visible = true;
            // rewardLoader.alpha = 1f;
            // rewardLoader.data = i;
            // rewardLoader.touchable = true;
            // rewardLoader.onClick.Add(OnClickReward);

            GComponent rewardBuild = this._ruinMap.ruinPanel.GetChild("rewardBuild" + i) as GComponent;
            rewardBuild.visible = true;
            rewardBuild.alpha = 1f;
            rewardBuild.data = i;
            rewardBuild.touchable = true;
            rewardBuild.onClick.Add(OnClickReward);

        }

        // 台阶
        this._ruinMap.ruinPanel.step0.visible = false;
        this._ruinMap.ruinPanel.step1.visible = false;

    }

    private void UpdateMapInfo()
    {

        // 战斗胜利，守卫渐变2秒消失
        int monsterIndex = MapChapterManager.Instance._defeatMonsterIndex;

        for (int i = 0; i < 3; i++)
        {
            GGraph bossSpine = i == 0 ? _ruinMap.ruinPanel.boss0 :
                i == 1 ? _ruinMap.ruinPanel.boss1 :
                _ruinMap.ruinPanel.boss2;

            GButton bossBtn = i == 0 ? _ruinMap.ruinPanel.bossBtn0 :
                i == 1 ? _ruinMap.ruinPanel.bossBtn1 :
                _ruinMap.ruinPanel.bossBtn2;

            if (bossSpine == null || bossBtn == null) continue;

            if (i < monsterIndex)
            {
                // 直接隐藏
                bossSpine.visible = false;
                bossBtn.touchable = false;
            }
            else if (i == monsterIndex)
            {
                bossBtn.touchable = false;

                // 获取SkeletonAnimation
                SkeletonAnimation spine = bossSpine.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                if (spine == null)
                {
                    bossSpine.visible = false;
                    return;
                }
                if(monsterIndex == 0)
                {
                    GuideManager.Instance.StarGuideByData(new GuideData()
                    {
                        fid = FuncOpenType.Relic,
                        giding = GuideID.guideId_3802,
                        gid = GuideID.guideId_3803,
                        tui = _ruinMap.ruinPanel.rewardBuild0,
                        isForce = true,
                        isSend = true,
                        //pType = PosType.Left,
                        npcTxt = "Beginner_Doc_024",
                        npcPosType = PosType.Down,
                        touchCB = () =>
                        {
                            GuideManager.Instance.HideGuide();
                        }
                    });
                }
                // 渐变隐藏
                GTween.To(1f, 0f, 2f)
                    .SetEase(EaseType.Linear)
                    .OnUpdate(t =>
                    {
                        float alpha = t.value.x;
                        spine.skeleton.A = alpha;
                    })
                    .OnComplete(() =>
                    {
                        bossSpine.visible = false;
                    });
            }
        }

    }

    /// <summary>
    /// 点击boss事件
    /// </summary>
    private void OnClickBossSpine(EventContext context)
    {
        // 如果正在移动中，则不响应新点击
        if (_isMoving)
        {
            return;
        }

        // 奖励是否领取，领取完buff后，才可以打下一个boss
        if (MapChapterManager.Instance._ruinBuffInfo != null)
        {
            UIManager.Instance.Toast("请先领取奖励！");
            return;
        }

        // 先移动到boss对应的pos点位，然后触发开始挑战事件，进入战斗界面
        GButton bossGraph = context.sender as GButton;
        int bossIndex = (int)bossGraph.data;

        // 当前boss未通过，不可挑战后面的boss
        if (bossIndex != MapChapterManager.Instance._canFightMonsterIndex)
        {
            UIManager.Instance.Toast("你还不能挑战此boss！");
            return;
        }

        if (!_bossTargetMap.TryGetValue(bossIndex, out string targetPosition))
            return;

        // 设置移动状态
        _isMoving = true;

        // 停止当前移动
        if (_moveTween != null)
        {
            _moveTween.Kill();
            _moveTween = null;
        }

        // 设置英雄行走动画
        if (_heroSpine != null)
            _heroSpine.state.SetAnimation(0, "run", true);

        // 计算从当前位置到目标位置的路径
        List<string> path = CalculatePath(_currentPosition, targetPosition);

        if (path.Count == 0)
        {
            // 设置英雄休闲动画
            if (_heroSpine != null)
                _heroSpine.state.SetAnimation(0, "idle", true);
            // 已在目标位置
            _isMoving = false; // 重置移动状态
            BeginRuinMosterFight(bossIndex);
            return;
        }

        // 开始移动
        MoveAlongPath(path, () =>
        {
            _currentPosition = targetPosition;
            if (_heroSpine != null)
                _heroSpine.state.SetAnimation(0, "idle", true);

            _isMoving = false; // 移动完成，重置状态
            BeginRuinMosterFight(bossIndex);
        });
    }

    // 使用BFS计算最短路径
    private List<string> CalculatePath(string start, string end)
    {
        if (start == end) return new List<string>();

        var queue = new Queue<string>();
        var visited = new HashSet<string> { start };
        var parentMap = new Dictionary<string, string>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            string current = queue.Dequeue();

            if (current == end)
            {
                // 回溯构建路径
                var path = new List<string>();
                string node = end;

                while (node != start)
                {
                    path.Add(node);
                    node = parentMap[node];
                }

                path.Reverse();
                return path;
            }

            if (_pathConnections.TryGetValue(current, out var connections))
            {
                foreach (string next in connections)
                {
                    if (visited.Contains(next)) continue;

                    visited.Add(next);
                    parentMap[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        return new List<string>(); // 没有找到路径
    }

    private void MoveAlongPath(List<string> pathPoints, Action onComplete)
    {
        if (pathPoints.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        int index = 0;
        Action moveNext = null;

        moveNext = () =>
        {
            // 检查移动是否已被取消
            if (!_isMoving)
            {
                return;
            }

            if (index >= pathPoints.Count)
            {
                onComplete?.Invoke();
                return;
            }

            string pointName = pathPoints[index];
            GObject point = this._ruinMap.ruinPanel.GetChild(pointName);

            if (point == null)
            {
                index++;
                moveNext();
                return;
            }

            // 获取当前英雄位置
            Vector2 currentPos = new Vector2(
                this._ruinMap.ruinPanel.hero.x,
                this._ruinMap.ruinPanel.hero.y
            );
            Vector2 targetPos = new Vector2(point.x, point.y);
            // 根据移动方向调整朝向
            UpdateFacingDirection(currentPos, targetPos);

            float duration = 1.0f;//移动时间
            _moveTween = this._ruinMap.ruinPanel.hero.TweenMove(
                    new Vector2(point.x, point.y), duration)
                .SetEase(EaseType.Linear)
                .OnComplete(() =>
                {
                    index++;
                    moveNext();
                });
        };

        moveNext();
    }

    // 根据移动方向更新英雄朝向
    private void UpdateFacingDirection(Vector2 currentPos, Vector2 targetPos)
    {
        // 计算移动方向（水平方向）
        bool shouldFaceRight = targetPos.x > currentPos.x;

        // 如果方向发生变化，更新朝向
        if (shouldFaceRight != _isFacingRight)
        {
            _isFacingRight = shouldFaceRight;

            // 通过翻转X轴实现朝向变化
            float scaleX = _isFacingRight ? 1f : -1f;
            this._ruinMap.ruinPanel.hero.scaleX = scaleX;
        }
    }

    //开始打怪请求
    private void BeginRuinMosterFight(int index)
    {

        DungeonMapManager.Instance.eventMonsterData = MapChapterManager.Instance.GetRandomEventMonsterByIndexId(index);
        // DungeonMapManager.Instance.eventMonsterIndexId = index;

        // RandomEventData randomEventData = MapChapterManager.Instance.GetRandomEventDataByGuid(eventGuid);
        // DungeonMapManager.Instance.mapEventData = randomEventData;
        DungeonMapManager.Instance.mapEventData = MapChapterManager.Instance._ruinEventData;
        DungeonMapManager.Instance.batchStuffId = index;

        Debug.Log($"开始挑战Boss{index}");
        var builder = BeginRuinMosterFight_CS.CreateBuilder();
        builder.EventGuid = eventGuid;//事件guid
        builder.MonsterIndex = (uint)index;//选中挑战的indexId
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_BeginRuinMosterFight_CS, builder.Build());
    }

    /// <summary>
    /// 点击奖励事件
    /// </summary>
    private void OnClickReward(EventContext context)
    {
        // 打开遗迹三选一界面
        RuinBuffInfo ruinBuffInfo = MapChapterManager.Instance._ruinBuffInfo;
        if (ruinBuffInfo == null) return;

        // int index = (int)(context.sender as GLoader)?.data;
        int index = (int)(context.sender as GComponent)?.data;
        if (index != ruinBuffInfo.monsterIndex) return;

        UIManager.Instance.ShowUIPanel("RuinSelectMain", ruinBuffInfo.guid, ruinBuffInfo.monsterIndex, ruinBuffInfo.buffIds);
    }

    /// <summary>
    /// 隐藏奖励建筑-展示台阶
    /// </summary>
    private void HideRewardBuild(uint monsterIndex)
    {

        for (int i = 0; i < 3; i++)
        {
            // GLoader reward = i == 0 ? _ruinMap.ruinPanel.reward0 :
            //     i == 1 ? _ruinMap.ruinPanel.reward1 :
            //     _ruinMap.ruinPanel.reward2;

            GComponent reward = i == 0 ? _ruinMap.ruinPanel.rewardBuild0 :
                i == 1 ? _ruinMap.ruinPanel.rewardBuild1 :
                _ruinMap.ruinPanel.rewardBuild2;

            GLoader3D step = i == 0 ? _ruinMap.ruinPanel.step0 :
                i == 1 ? _ruinMap.ruinPanel.step1 : null;

            if (reward == null) continue;

            if (i < monsterIndex)
            {
                // 之前打败过的boss的奖励，直接隐藏
                reward.visible = false;
            }
            else if (i == monsterIndex)
            {
                // 当前打败boss，对应奖励渐变隐藏
                reward.visible = true;
                reward.alpha = 1f;
                GTween.To(1f, 0f, 3f)
                    .SetEase(EaseType.Linear)
                    .OnUpdate(t =>
                    {
                        float alpha = t.value.x;
                        reward.alpha = alpha;
                    })
                    .OnComplete(() =>
                    {
                        reward.visible = false;
                    });

                if (step != null)
                {
                    step.visible = true;
                    if (i == 0)
                    {
                        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.WayGenerateSE);
                        Utils.PlaySpineAnim(step, "BigMap_yiji_taijie_2", false);
                    }

                    if (i == 1)
                    {
                        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.WayGenerateSE);
                        Utils.PlaySpineAnim(step, "BigMap_yiji_taijie_1", false);
                    }
                }
            }
            else
            {
                // 当前还没打败的 boss对应的reward，显示
                reward.visible = true;
                reward.alpha = 1f;

                if (step != null)
                {
                    step.visible = false;
                }
            }
        }

    }

    /// <summary>
    /// 遗迹buff
    /// </summary>
    private void UpdateRuinBuffInfo()
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

        _ruinMap.ruinPanel.buffList.numItems = _showBuffInfos.Count;
    }

    /// <summary>
    /// 获得的遗迹buff
    /// </summary>
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

    public void OnClickExitRuinBtn()
    {

        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam()
        {
            OkCallBack = () =>
            {
                // 播放传出遗迹地图特效
                ShowTransferEffect();
                GameManager.Instance.TimerManager.SetTimer(0.6f, () =>
                {
                    ExitRuin();
                });

                // ExitRuin();
                MapChapterManager.Instance.ResetRuinState();
            }
        };

        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(8064), param, true);
    }

    //退出遗迹地图请求
    private void ExitRuin()
    {
        var builder = ExitRuin_CS.CreateBuilder();
        builder.EventGuid = eventGuid;//事件guid
        GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ExitRuin_CS, builder.Build());
    }

    // 领取完最后一个遗迹奖励后，2秒后自动传送出副本
    private void AutoExitRuin()
    {
        if (MapChapterManager.Instance._canExitRuinMap)
        {
            // 播放传出遗迹地图特效
            // ShowTransferEffect();

            // ExitRuin();

            GameManager.Instance.TimerManager.SetTimer(2.5f, () =>
            {
                // 播放传出遗迹地图特效
                ShowTransferEffect();
                GameManager.Instance.TimerManager.SetTimer(0.6f, () =>
                {
                    ExitRuin();
                });
            });

            MapChapterManager.Instance.ResetRuinState();
        }
    }

    /// <summary>
    /// 播放传送特效
    /// </summary>
    private void ShowTransferEffect()
    {
        UIExtensions.PlayChuanSongBegin(this._ruinMap.ruinPanel.hero, _heroSpine, null, new Vector2(-0.28f, -0.6f));
    }

    public GList GetRuinBuffList()
    {
        return this._ruinMap.ruinPanel.buffList;
    }

}
