using BigMap;
using CommonEx;
using Config;
using EngineBase;
using FairyGUI;
using msg;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;

namespace Engine
{
    public enum BigMapObjectType
    {
        Normal = 0,
        Boss = 1,
        Pet = 2
    }
    
    public enum StateType
    {
        Normal = -1,
        Idle = 0,  //休闲状态
        RandomPos = 1,  // 移动随机点位
        BossStage = 2,  // 移动到Boss关卡点
        Attacking = 3,  // 索敌状态
        Catch = 4, //捕捉
        Win = 5,  //索敌战斗胜利
        Lose = 6, // 索敌战斗失败
    }
    
    public class BigMapObject
    {
        
        private ConfigCommonUnit common2001;
        private ConfigCommonUnit common2002;
        private ConfigCommonUnit common2003;
        private ConfigCommonUnit common2017;
        private ConfigCommonUnit common2006;
        private ConfigCommonUnit common2007;
        private ConfigCommonUnit common2018;
        
        /// <summary>
        /// UI节点
        /// </summary>
        public AStarPathFinder aStarPathFinder;
        public float gridSizeTextture;
        //关卡数据表
        public List<MapStageData> mapStageDataList = new List<MapStageData>(); 
        public GLoader walkMap;
        public GGraph hero;
        public RandomEventData eventData;
        private float modelWidth = 50; //模型大小
        /// <summary>
        /// 服务器guid下的唯一id
        /// </summary>
        public int batchStuffId;
        /// <summary>
        /// 可以行走的坐标区间坐标（分割后的坐标）
        /// </summary>
        private List<Vector2> walkableList = new List<Vector2>(); 
        
        private float idleMoveSpeed = 0; //休闲移动速度
        private int idleTime = 0; //休闲持续时长
        private float atkMoveSpeed = 0; //攻击移动速度
        private int atkTime = 0; //攻击持续时长
        private int attackRange = 2000; //索敌范围
        
        private float refreshTime = 2;
        private float refreshHeroPosTime = 0;
        private bool isAttacking = false;    //准备攻击
        
        private SkeletonAnimation bossAnimation;
        public BigMapObjectType objectType = BigMapObjectType.Normal;
        private GTweener tweenerMove;
        public UI_modelObj modelObj;
        public StateType stateType = StateType.Normal;
        
        private float catchTime = 0;
        private GTweener tweenerCatch;
        private bool isCatching = false; //捕捉中
        
        public bool isShowObj = false; 
        private float stateTime = 0;

        public JumpTypeEnum _jumpTypeEnum;
        //public bool isClicked = false;
        //是否引导中
        public bool isGuide = false;
        
        public void Tick(float deltaSeconds)
        {
            if (!isShowObj) return;
            
            stateTime -= deltaSeconds;
            if (objectType == BigMapObjectType.Boss)
            {
                if (stateTime <= 0 && !isAttacking)
                {
                    if (stateType == StateType.Idle)
                    {
                        stateType = StateType.Attacking;
                        stateTime = atkTime;
                    }
                    else if (stateType == StateType.Attacking)
                    {
                        stateType = StateType.Idle;
                        stateTime = idleTime;
                    }

                    EnterState(stateType);
                }

                if (stateType == StateType.Attacking && !isAttacking) //索敌状态 还没找到敌人
                {
                    refreshHeroPosTime += deltaSeconds;
                    if (refreshHeroPosTime > refreshTime)
                    {
                        refreshHeroPosTime = 0;
                        EnterAttackState();
                    }
                }
            }
            else
            {
                if (stateTime <= 0)
                {
                    isCatching = false;
                    if (stateType == StateType.Idle)
                    {
                        stateType = StateType.RandomPos;
                        stateTime = atkTime;
                    }
                    else if (stateType == StateType.RandomPos)
                    {
                        stateType = StateType.Idle;
                        stateTime = idleTime;
                    }
                    EnterState(stateType);
                }
            }
        }

        public void LoadObject()
        {
            if (bossAnimation == null)
            {
                int cfgId = 0;
                foreach (var stuffData in eventData.batchStuffList)
                {
                    if (stuffData.id == batchStuffId)
                    {
                        cfgId = stuffData.cfgId;
                        break;
                    }
                }

                ConfigEventStageUnit eventStageUnit = ConfigUtils.GetEventStageUnitById(cfgId);
                int groupId = eventStageUnit.MonsterData;
                var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
                ConfigMonsterGroupUnit data = monsterGroupArr[0];
                ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
                
                ModelManager.Instance.LoadNormalPrefab("Role/" + monsterUnit.Model, (go) =>
                {
                    var gameObject = GameObject.Instantiate(go as GameObject);
                    gameObject.transform.localScale = new Vector3(modelWidth, modelWidth, modelWidth);
                    GoWrapper wrapper = new GoWrapper(gameObject);
                    modelObj = UIPackage.CreateObject("BigMap", "modelObj") as UI_modelObj;
                    
                    modelObj.model.SetNativeObject(wrapper);
                    (hero.parent as GComponent).AddChild(modelObj);
                    
                    bossAnimation = modelObj.model.displayObject.gameObject.GetComponentInChildren<SkeletonAnimation>();
                    bossAnimation.AnimationState.SetAnimation(0, "idle", true);

                    modelObj.bubble.SetPosition(0, -220, 0);
                    modelObj.logo.visible = false;
                    modelObj.logo.SetPosition(-45, -260, 0);
                    
                    modelObj.btnClick.xy = new Vector2(-50, -220);
                    modelObj.btnClick.onClick.Add(OnClickIdleObject);

                    modelObj.logo.url = UIResource.GetMapItemBgByName("tiaozhanboss");
                    if (objectType == BigMapObjectType.Pet)
                    {
                        modelObj.bar.visible = false;
                        modelObj.logo.url = UIResource.GetMapItemBgByName("zhuabuchongwu");
                    }
                    modelObj.petRun.visible = false;
                    
                    this.modelObj.bg.visible = false;
                    this.modelObj.objBgEffect.Stop();
                    
                    Init();
                    // 可点击区域画线  和锚点有关  添加的物体在0，0 锚点的位置
                    // GGraph clickArea = new GGraph();
                    // clickArea.DrawRect(modelObj.width, modelObj.height, 5, Color.blue, Color.clear);
                    // clickArea.touchable = true;
                    // clickArea.onClick.Add(OnClickIdleObject);
                    // clickArea.SetPosition(0, 0, 0);
                    // modelObj.AddChild(clickArea);
                });
            }
        }

        private void Init()
        {
            common2001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2001);
            common2002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2002);
            common2003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2003);
            common2017 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2017);
            common2006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2006);
            common2007 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2007);
            common2018 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2018);
            catchTime = int.Parse(common2006.Param2);
            attackRange = int.Parse(common2002.Param4);
            
            if (objectType == BigMapObjectType.Boss)
            {
                idleMoveSpeed = int.Parse(common2001.Param1) / 1000f;
                idleTime = int.Parse(common2001.Param2);
                atkMoveSpeed = int.Parse(common2002.Param1) / 1000f;
                atkTime = int.Parse(common2002.Param2);
            }
            else if (objectType == BigMapObjectType.Pet)
            {
                idleMoveSpeed = 0;
                idleTime = int.Parse(common2006.Param1);//30;//int.Parse(common2006.Param1);
                atkMoveSpeed = int.Parse(common2007.Param1) / 1000f;
                atkTime = int.Parse(common2007.Param2);//5; //int.Parse(common2007.Param2);
            }
            
            InitStage();
            
            //todo 监听战斗结束是否胜利
            EventDispatcher.GameWorld.Regist<bool, int, ulong>(EventDefine.EVENT_BOSS_PET_BATTLE_RESULT_UPDATE, BattleResultUpdate);
            EventDispatcher.GameWorld.Regist(EventDefine.EVENT_BOSS_PET_CLICK_EVENTS, ClearClickState);
        }
        
        private void InitStage()
        {
            isShowObj = true;
            
            var ChapterMap = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
            // stateType = StateType.Attacking;
            // stateTime = atkTime;
            stateType = StateType.Idle;
            stateTime = idleTime;

            if (objectType != BigMapObjectType.Pet)
            {
                var startIndex = Random.Range(0, walkableList.Count);
                var global = walkMap.LocalToGlobal(walkableList[startIndex]);
                var local = walkMap.parent.GlobalToLocal(global);
                modelObj.SetXY(local.x, local.y);

            }
            else
            {
                //统计出玩家角色一定范围内可以被使用的点
                List<Vector2> posList = new List<Vector2>();
                float sw = GRoot.inst.width * 0.5f;
                foreach (var pos in walkableList)
                {
                    if(Vector2.Distance(ChapterMap.ConvertWalkMapLocalToParent(pos), hero.xy) < sw)
                    {
                        posList.Add(pos);
                    }
                }

                if(posList.Count > 0)
                {
                    var startIndex = Random.Range(0, posList.Count);
                    var global = walkMap.LocalToGlobal(posList[startIndex]);
                    var local = walkMap.parent.GlobalToLocal(global);
                    modelObj.SetXY(local.x, local.y);
                }else
                {//没有适合的，就直接在玩家身上好了
                    modelObj.xy = hero.xy;
                }

                stateType = StateType.RandomPos;
                stateTime = atkTime;

                //抓捕宠物引导引导-点击第一个正确的门
                if (GuideManager.Instance.StarGuideByData(new GuideData()
                {
                    fid = FuncOpenType.SearchPets,
                    giding = GuideID.guideId_4101,
                    gid = GuideID.guideId_4102,
                    tui = modelObj,
                    isForce = true,
                    isSend = true,
                    scrollPos = new Vector2(modelObj.width * 0.5f, modelObj.height),
                    //pType = PosType.Left,
                    npcTxt = "Beginner_Doc_030",
                    npcPosType = PosType.Down,
                    cb0 = () =>
                    {
                        //暂停宠物移动
                        isGuide = true;
                        stateType = StateType.Idle;
                        if (bossAnimation != null)
                        {
                            bossAnimation.state.SetAnimation(0, "idle", true);
                        }
                    },
                    touchCB = () =>
                    {
                        GuideManager.Instance.HideGuide();
                        isGuide = false;
                    }
                })) { return; }
            }

            //if(objectType == BigMapObjectType.Pet)
            //    Debug.Log($"宠物的XY{modelObj.xy}");

            EnterState(stateType);
        }

        public void OnShow()
        {
            isGuide = false;
            isShowObj = true;
            this.modelObj.visible = true;
            if (stateType != StateType.Attacking)
            {
                isAttacking = false;
                EnterIdleState();
                //InitStage();
            }
            else
            {
                if (objectType == BigMapObjectType.Boss)
                {
                    isAttacking = false;
                    EnterAttackState();
                }
            }
        }
        public void Pause()
        {
            ClearTweenerMove();
            isShowObj = false;
            tweenerFade = null;
            GameManager.Instance.TimerManager.ClearTimer(BubbleFinish);
            GameManager.Instance.TimerManager.ClearTimer(EnterIdleState);
        }
        public void Hide()
        {
            this.modelObj.visible = false;
            ClearTweenerMove();
            isShowObj = false;
            tweenerFade = null;
            GameManager.Instance.TimerManager.ClearTimer(BubbleFinish);
            GameManager.Instance.TimerManager.ClearTimer(EnterIdleState);
        }
        
        public void Destroy()
        {
            if (isGuide && GuideManager.Instance.IsShowGuiding)
            {
                GuideManager.Instance.ResetGuidance4100();
            }

            // Hide();
            walkMap = null;
            ClearTweenerMove();
            isShowObj = false;
            tweenerFade = null;
            GameManager.Instance.TimerManager.ClearTimer(BubbleFinish);
            GameManager.Instance.TimerManager.ClearTimer(EnterIdleState);
            
            this.modelObj.model.TweenFade(0, 2)
                .SetEase(EaseType.Linear)
                .OnUpdate((fillnum) =>
                {
                    bossAnimation.skeleton.A = fillnum.value.x;
                    this.modelObj.alpha = fillnum.value.x;
                } )
                .OnComplete(() =>
                {
                    bossAnimation = null;
                    MapChapterManager.Instance.DestroyMapObject(this);
                    this.modelObj.Dispose();
                    hero.parent.RemoveChild(this.modelObj);
                });

            EventDispatcher.GameWorld.UnRegist<bool, int, ulong>(EventDefine.EVENT_BOSS_PET_BATTLE_RESULT_UPDATE, BattleResultUpdate);
            EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_BOSS_PET_CLICK_EVENTS, ClearClickState);
        }
        
        public void SetWalkPosList(List<Vector2> walkList)
        {
            this.walkableList = walkList;
        }
        
        public void SetStageDataList(List<MapStageData> stageDataList)
        {
            this.mapStageDataList = stageDataList;
        }
        
        private void EnterState(StateType state)
        {
            switch (state)
            {
                case StateType.Idle:
                case StateType.RandomPos:
                    //Debug.Log($"=======休闲=状态=====");
                    EnterIdleState();
                    break;
                case StateType.Attacking:
                    //Debug.Log($"=======索敌=状态=====");
                    ClearTweenerMove();
                    GameManager.Instance.TimerManager.ClearTimer(EnterIdleState);
                    ShowBubbleState((int)StateType.Attacking);
                    EnterAttackState();
                    break;
            }
        }

        #region 气泡

        /// <summary>
        /// BOSS战斗结果弹气泡
        /// </summary>
        private void BattleResultUpdate(bool isWin, int stuffId, ulong guid)
        {
            if (stuffId == batchStuffId && guid == eventData.guid)
            {
                if (isWin)
                {
                    ShowBubbleState((int)StateType.Win);
                    Destroy();
                }
                else
                {
                    ShowBubbleState((int)StateType.Lose);
                    if (objectType == BigMapObjectType.Pet)
                    {
                        moveRandomPos(true);
                    }
                }
            }
        }
        
        private ConfigBubbleUnit bubbleUnit;
        private GTweener tweenerFade;
        /// <summary>
        /// 气泡状态
        /// </summary>
        private void ShowBubbleState(int state)
        {
            switch (state)
            {
                case (int)StateType.Idle: // 在原地静止播放休闲动作
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2003.Param3));
                    if (objectType == BigMapObjectType.Pet)
                    {
                        bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2018.Param3));
                    }
                    break;
                case (int)StateType.RandomPos: // 随机选择一个目标点快速移动
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2001.Param3));
                    if (objectType == BigMapObjectType.Pet)
                    {
                        bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2018.Param4));
                    }
                    break;
                case (int)StateType.BossStage:  // 选择最近的隐藏关卡目标点移动
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2001.Param3));
                    break;
                case (int)StateType.Attacking: //索敌状态
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2002.Param3));
                    break;
                case (int)StateType.Catch:  //捕捉时候的气泡
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2006.Param3));
                    break;
                case (int)StateType.Win:  //索敌战斗胜利
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2003.Param1));
                    break;
                case (int)StateType.Lose:  //索敌战斗失败
                    bubbleUnit = ConfigUtils.GetBubbleById(int.Parse(common2003.Param2));
                    break;
            }
            (modelObj.bubble as UI_Bubble).bubbleBg.url = "ui://CommonEx/" + bubbleUnit.BubbleResource;
            (modelObj.bubble as UI_Bubble).talkDes.text = ConfigUtils.GetTextById(bubbleUnit.Doc);

            modelObj.bubble.visible = true;
            modelObj.bubble.alpha = 1;
            tweenerFade = null;
            GameManager.Instance.TimerManager.ClearTimer(BubbleFinish);
            GameManager.Instance.TimerManager.SetTimer(bubbleUnit.Duration, BubbleFinish);
        }

        private void BubbleFinish()
        {
            tweenerFade = modelObj.bubble.TweenFade(0, 1);
            tweenerFade.SetEase(EaseType.Linear);
            tweenerFade.OnComplete(() =>
            {
                modelObj.bubble.visible = false;
                tweenerFade = null;
            });
        }
        
        #endregion
        
        #region 休闲状态

        private void ChangeIdleState()
        {
            GameManager.Instance.TimerManager.ClearTimer(EnterIdleState);
            GameManager.Instance.TimerManager.SetTimer(5, EnterIdleState);
        }

        private List<int> stateIndex = new List<int>();
        private int stateParam = 0;
        private void EnterIdleState()
        {
            if (walkMap == null || walkMap.isDisposed) { return; }
            ClearTweenerMove();

            if (objectType == BigMapObjectType.Boss)
            {
                stateIndex = Utils.GetIndexByWeight(new List<int>() { int.Parse(common2017.Param1), int.Parse(common2017.Param2), int.Parse(common2017.Param3) }, 1);
                stateParam = stateIndex[0];
            }
            else
            {
                if (stateType == StateType.Idle)
                {
                    stateParam = (int)StateType.Idle;
                }else if (stateType == StateType.RandomPos)
                {
                    stateParam = (int)StateType.RandomPos;
                }
                //stateIndex = Utils.GetIndexByWeight(new List<int>() { int.Parse(common2018.Param1), int.Parse(common2018.Param2) }, 1);
                //todo 测试
                //stateIndex = new List<int>() { 0 };
            }
            
            switch (stateParam)
            {
                case (int)StateType.Idle: // 在原地静止播放休闲动作
                    //Debug.Log($"========在原地静止播放休闲动作=====");
                    bossAnimation.AnimationState.SetAnimation(0, "idle", true);
                    break;
                case (int)StateType.RandomPos:  // 随机选择一个目标点快速移动
                    //Debug.Log($"========随机选择一个目标点快速移动=====");
                    var index = Random.Range(0, walkableList.Count);
                    if (objectType == BigMapObjectType.Pet)
                    {
                        MoveToTarget(walkableList[index], atkMoveSpeed);
                    }
                    else
                    {
                        MoveToTarget(walkableList[index], idleMoveSpeed);
                    }
                    break;
                case (int)StateType.BossStage:  // 选择最近的隐藏关卡目标点移动
                    MoveToBossStage();
                    break;
            }

            ShowBubbleState(stateParam);
            if (objectType == BigMapObjectType.Boss)
            {
                ChangeIdleState();
            }
        }
        
        /// <summary>
        /// 移动到随机一个隐藏关卡
        /// </summary>
        private void MoveToBossStage()
        {
            if(walkMap == null || walkMap.isDisposed) { return; }
            int randomIndex = Random.Range(0, 4);
            int index = 0;
            GLoader moveLoader = null;
            for (int i = 0; i < this.mapStageDataList.Count; i++)
            {
                if (this.mapStageDataList[i].posType == MapPosType.Boss)
                {
                    if (randomIndex == index)
                    {
                        moveLoader = this.mapStageDataList[i].stageLoader;//mapStageData.moveLoader;
                        break;
                    }
                    index++;
                }
            }
            if (moveLoader != null)
            {
                var globalPos = moveLoader.LocalToGlobal(Vector2.zero);
                Vector2 nearPos = walkMap.GlobalToLocal(globalPos); //转地图上的点
                Vector2 targetPos = NearCanWalkPoint(nearPos);
                if (targetPos != Vector2.zero)
                {
                    MoveToTarget(targetPos, idleMoveSpeed);
                }
            }
        }
        
        private int walkIndex = 0;
        /// <summary>
        /// 附近最近可行走的点
        /// </summary>
        public Vector2 NearCanWalkPoint(Vector2 pos)
        {
            walkIndex = 0;
            while (!aStarPathFinder.isCanWalk(new Vector2((int)pos.x, (int)pos.y)))
            {
                walkIndex++;
                for (int x = -walkIndex; x <= walkIndex; x += walkIndex)
                {
                    for (int y = -walkIndex; y <= walkIndex; y += walkIndex)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        int checkX = (int)pos.x + x;
                        int checkY = (int)pos.y + y;
                        if (aStarPathFinder.isCanWalk(new Vector2(checkX, checkY)))
                        {
                            return new Vector2(checkX, checkY);
                        }
                    }
                }
                if (walkIndex > 200)
                {
                    return Vector2.zero;
                }
            }
            return pos;
        }

        /// <summary>
        /// X轴上可以行走的点
        /// </summary>
        public Vector2 PosXNearCanWalkPoint(Vector2 pos, bool isRight)
        {
            walkIndex = 0;
            while (!aStarPathFinder.isCanWalk(new Vector2((int)pos.x, (int)pos.y)))
            {
                walkIndex++;
                if (isRight)
                {
                    for (int x = 0; x <= walkIndex; x ++)
                    {
                        if (x == 0)
                            continue;
                        int checkX = (int)pos.x + x;
                        int checkY = (int)pos.y;
                        if (aStarPathFinder.isCanWalk(new Vector2(checkX, checkY)))
                        {
                            return new Vector2(checkX, checkY);
                        }
                    }
                }
                else
                {
                    for (int x = -walkIndex; x <= 0; x++)
                    {
                        if (x == 0)
                            continue;
                        int checkX = (int)pos.x + x;
                        int checkY = (int)pos.y;
                        if (aStarPathFinder.isCanWalk(new Vector2(checkX, checkY)))
                        {
                            return new Vector2(checkX, checkY);
                        }
                    }
                }

                if (walkIndex > 100)
                {
                    return Vector2.zero;
                }
            }
            return pos;
        }
        #endregion

        #region 索敌状态

        private Vector2 atkStateHeroPos  = Vector2.zero;
        private void EnterAttackState()
        {
            isAttacking = false;
            // 目标点
            if (hero != null && hero.displayObject != null && hero.displayObject.gameObject != null)
            {
                var globalBossPos = hero.LocalToGlobal(Vector2.zero);
                Vector2 targetPos = walkMap.GlobalToLocal(globalBossPos);
                
                // 起始点
                var objPos = modelObj.LocalToGlobal(Vector2.zero);
                Vector2 startPos = walkMap.GlobalToLocal(objPos);
                
                if (!IsAttackRange(this, hero))  //是否在索敌范围内
                {
                    return;
                }
            
                if (tweenerMove == null)
                {
                    atkStateHeroPos = targetPos;
                    MoveToTarget(targetPos, atkMoveSpeed);
                }
                else
                {
                    if (atkStateHeroPos != targetPos )
                    {
                        ClearTweenerMove();
                        atkStateHeroPos = targetPos;
                        MoveToTarget(targetPos, atkMoveSpeed);
                    }
                }
            }
        }

        #endregion
        
        #region 移动
        
        private int bossMoveIndex = 0;
        private List<Vector2> bossPathList;
        private void MoveToTarget(Vector2 target, float moveSpeed)
        {
            if (objectType == BigMapObjectType.Pet && isGuide) return;
            if (bossAnimation == null || walkMap == null) return;
            //Debug.Log($"====MoveToTarget===");
            ClearTweenerMove();

            // 起始点
            var globalBossPos = modelObj.LocalToGlobal(Vector2.zero);
            Vector2 startPos = walkMap.GlobalToLocal(globalBossPos);
            
            bossPathList = aStarPathFinder.FindPath(startPos, target);
            if (bossPathList != null && bossPathList.Count > 0)
            {
                if (bossAnimation != null && bossAnimation.AnimationName != "run")
                {
                    bossAnimation.state.SetAnimation(0, "run", true);
                }

                if (bossMoveIndex < bossPathList.Count)
                {
                    StartMove(bossPathList[bossMoveIndex], moveSpeed);
                }
            }
        }
        
        private void StartMove(Vector2 point, float moveSpeed)
        {
            var p = modelObj.LocalToGlobal(Vector2.zero);
            Vector2 startPos = walkMap.GlobalToLocal(p);

            point *= gridSizeTextture; //乘上网格大小
            // 移动点转换
            var global = walkMap.LocalToGlobal(point);
            var local = walkMap.parent.GlobalToLocal(global);
            
            // 设置朝向
            var globalBossPos = modelObj.LocalToGlobal(Vector2.zero);
            Vector2 bossPos = walkMap.parent.GlobalToLocal(globalBossPos);
            Vector3 forward = local - (Vector2)bossPos;
            bossAnimation.skeleton.ScaleX = forward.x >= 0 ? 1 : -1;
            //if(objectType == BigMapObjectType.Pet)
                //Debug.Log($"起点 = {startPos},目标 = {local}，距离 = {Vector2.Distance(startPos, local)}");
            tweenerMove = modelObj.TweenMove(local, moveSpeed)
                .SetEase(EaseType.Linear)
                .OnComplete(() =>
                {
                    if (stateType == StateType.Attacking)
                    {
                        if (GetDistance(this, hero))
                        {
                            EndMove();
                            return;
                        }

                        if (!IsAttackRange(this, hero))  //角色没在怪物索敌范围，怪物转闲逛
                        {
                            ClearTweenerMove();
                            stateType = StateType.Idle;
                            stateTime = idleTime;
                            EnterState(stateType);
                            return;
                        }
                    }
                    else
                    {
                        if (GetDistance(this, hero))
                        {
                            SetVisibleBattleLogo(true);
                        }
                        else
                        {
                            SetVisibleBattleLogo(false);
                        }
                    }

                    if (bossPathList != null && bossPathList.Count > 0)
                    {
                        bossMoveIndex++;
                        if (bossMoveIndex < bossPathList.Count)
                        {
                            StartMove(bossPathList[bossMoveIndex], moveSpeed);
                        }
                        else
                        {
                            EndMove();
                        }
                    }
                });
        }

        private void EndMove()
        {
            if (bossAnimation != null && bossAnimation.AnimationName != "idle")
            {
                bossAnimation.state.SetAnimation(0, "idle", true);
            }

            ClearTweenerMove();
            bossPathList = null;

            if (stateType == StateType.Attacking)
            {
                var view = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                //采矿和挖宝，钓鱼，抓捕
                if(GuideManager.Instance.IsShowGuiding || view != null && (view.actionPlaying || view._isCuttingTree || !view.ChkHeroIdleState())
                    || UIManager.Instance.IsShowByName("Equip")//穿戴装备
                    || UIManager.Instance.IsShowByName("ChapterStageDetail")//挑战关卡
                    || UIManager.Instance.IsShowByName("ChapterEventStageDetail")//挑战事件
                    || UIManager.Instance.IsShowByName("GetSingleReward")//三选一
                    || UIManager.Instance.IsShowByName("OfflineReward")//离线奖励
                    ){ return; }

                if (!GetDistance(this, hero))  //防止怪到达的时候角色已经离开
                {
                    return;
                }
                //Debug.Log("===========找到敌人开始打开挑战界面================");
                atkStateHeroPos = Vector2.zero;
                isAttacking = true;
                stateType = StateType.Idle;
                stateTime = idleTime;
                ShowPopView(true);
            }

            if (objectType == BigMapObjectType.Pet && stateType == StateType.RandomPos)
            {
                moveRandomPos();
            }
        }

        private void ClearTweenerMove()
        {
            if (tweenerMove != null)
            {
                //Debug.Log("===========ClearTweenerMove================");
                bossMoveIndex = 0;
                tweenerMove.Kill();
                tweenerMove = null;
            }
        }
        #endregion

        #region 点击boss、宠物

        private void OnClickIdleObject(EventContext context)
        {
            //if (!modelObj.logo.visible) return;
            var ChapterMap = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
            //挖掘矿石和宝箱抛掉
            if(ChapterMap != null && (ChapterMap.actionPlaying || ChapterMap._isCuttingTree)) { return; }

            if (!GetDistance(this, hero))
            {
                //isClicked = true;
                this.modelObj.bg.visible = true;
                this.modelObj.objBgEffect.Play(-1,0, null);
                
                Vector2 pos = (hero.position.x - this.modelObj.position.x) > 0 ? new Vector2(this.modelObj.position.x + modelWidth, this.modelObj.position.y) : new Vector2(this.modelObj.position.x - modelWidth, this.modelObj.position.y);
                var globalPos = walkMap.parent.LocalToGlobal(pos);
                Vector2 targetPos = walkMap.GlobalToLocal(globalPos);
                targetPos = PosXNearCanWalkPoint(targetPos,(hero.position.x - this.modelObj.position.x) > 0);
                if (targetPos == Vector2.zero)
                {
                    globalPos = this.modelObj.LocalToGlobal(Vector2.zero);
                    targetPos = walkMap.GlobalToLocal(globalPos);
                }
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_BOSS_PET_AUTO_FIND_EVENT, targetPos, this);
                
                if (objectType == BigMapObjectType.Pet) //休闲状态点击boss
                {
                    if (stateType != StateType.Idle)
                    {
                        UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8070));
                    }
                }
            }
            else
            {
                if (objectType == BigMapObjectType.Boss) //休闲状态点击boss
                {
                    ClearTweenerMove();
                    ShowPopView();
                }
                else
                {
                    if (stateType != StateType.Idle)
                    {
                        UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8070));
                        return;
                    }

                    PlayCatchAction();
                }
            }
        }

        /// <summary>
        /// 播放抓捕动画
        /// </summary>
        public void PlayCatchAction()
        {
            isCatching = true;
            modelObj.logo.visible = false;
            if (hero.parent.GetChild("moveMapMask") != null)
            {
                hero.parent.GetChild("moveMapMask").visible = true;
            }
            ShowBubbleState((int)StateType.Catch); //捕捉宠物
                    
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PLAY_CATCH_PET_EVENTS, this, true);
            //todo 读条展示 //被打断的话，要中断捕捉
            modelObj.bar.visible = true;
            modelObj.bar.min = 0;
            modelObj.bar.max = 100;
            modelObj.bar.value = 0;
            tweenerCatch = GTween.To(0, 100, catchTime) // 持续时间
                .SetTarget(modelObj.bar) // 设置目标对象
                .SetEase(EaseType.Linear) // 线性动画
                .OnUpdate((tweener) =>
                {
                    if (!isCatching)
                    {
                        modelObj.logo.visible = true;
                        CatchFailed();
                        return;
                    }
                    modelObj.bar.value = tweener.value.x;
                }) // 进度条更新回调
                .OnComplete(() => OnCuttingComplete());// 完成回调
        }
        
        /// <summary>
        /// 捕捉宠物成功回调
        /// </summary>
        private void OnCuttingComplete()
        {
            //Debug.Log("=========捕捉成功========");
            hero.parent.GetChild("moveMapMask").visible = false;
            modelObj.bar.visible = false;
            if (tweenerCatch != null)
            {
                tweenerCatch.Kill();
                tweenerCatch = null;
            }
            isShowObj = false;
            
            var builder = CatchWildPetSuccess_CS.CreateBuilder();
            builder.EventGuid = eventData.guid;
            builder.BatchStuffId = (uint)batchStuffId;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_CatchWildPetSuccess_CS, builder.Build());
            
            ClearTweenerMove();
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PLAY_CATCH_PET_EVENTS, this, false);
        }

        private List<int> battleRateList = new List<int>();
        /// <summary>
        /// 捕捉失败
        /// </summary>
        private void CatchFailed()
        {
            //Debug.Log("=========倒计时未完成捕捉失败========");
            
            hero.parent.GetChild("moveMapMask").visible = false;
            modelObj.bar.visible = false;
            
            if (tweenerCatch != null)
            {
                tweenerCatch.Kill();
                tweenerCatch = null;
            }
            battleRateList = Utils.GetIndexByWeight(new List<int>() { int.Parse(common2006.Param4), int.Parse(common2006.Param5) }, 1);
            switch (battleRateList[0])
            {
                case 0:
                    //Debug.Log("=====进战斗===");
                    ClearTweenerMove();
                    // ShowPopView();
                    
                    DungeonMapManager.Instance.mapEventData = this.eventData;
                    DungeonMapManager.Instance.batchStuffId = this.batchStuffId;
                    
                    MapChapterManager.Instance.HideMapObject();
                    
                    var builder = AttackWildPetBegin_CS.CreateBuilder();
                    builder.EventGuid = (ulong)this.eventData.guid;
                    builder.BatchStuffId = (uint)this.batchStuffId;
                    GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AttackWildPetBegin_CS, builder.Build()); 
                    break;
                case 1:
                    //Debug.Log("=====移动到随机点===");
                    moveRandomPos(true);
                    break;
            }
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_PLAY_CATCH_PET_EVENTS, this, false);
        }

        private void moveRandomPos(bool play = false)
        {
            //Debug.Log("====moveRandomPos=移动到随机点===");
            
            //modelObj.petRun.visible = true;
            //Utils.PlaySpineAnim(modelObj.petRun, "zaozi", false, () =>
            //{
            //    modelObj.petRun.visible = false;
            //});
            if(play)
            {
                var ChapterMap = UIManager.Instance.FindByName("ChapterMap") as ChapterMapView;
                ChapterMap?.AddPetGuangZhao(this);
            }

            ShowBubbleState((int)StateType.RandomPos); //捕捉宠物失败也弹这句
            
            var index = Random.Range(0, walkableList.Count);
            stateType = StateType.RandomPos;
            stateTime = atkTime;
            MoveToTarget(walkableList[index], atkMoveSpeed);
        }

        private void ClearClickState()
        {
            if (this.modelObj.bg.visible)
            {
                this.modelObj.bg.visible = false;
                this.modelObj.objBgEffect.Stop();
            }
        }
        #endregion
        
        /// <summary>
        /// 打开挑战界面
        /// </summary>
        /// <param name="isRandomBoss">是否随机boss</param>
        public void ShowPopView(bool isRandomBoss = false)
        {
            // 是否达到完成次数上限
            int finishCount = MapChapterManager.Instance.GetFinishEventCount((int)eventData.eventType);
            ConfigEventUnit eventStageUnit = ConfigUtils.GetEventDataById(eventData.eventId);
            if (finishCount >= eventStageUnit.Number) //达到每日次数上限
            {
                if (!isRandomBoss)
                {
                    UIManager.Instance.Toast(ConfigUtils.GetStringByKey(8003));
                }
                else
                {
                    isAttacking = false;
                    EnterIdleState();
                }
                return;
            }
            
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_CHAPTER_MAP_MOVE_STOP);

            if (isRandomBoss)
            {
                MapChapterManager.Instance.HideMapObject();
            }
            
            var eventBossView = UIManager.Instance.FindByName("ChapterEventBossStageDetail") as ChapterEventBossStageDetailView;
            if (eventBossView != null && !eventBossView.isRandom)
            {
                UIManager.Instance.CloseUIPanel("ChapterEventBossStageDetail");
            }
            UIManager.Instance.ShowUIPanel("ChapterEventBossStageDetail", eventData, batchStuffId, isRandomBoss,false, _jumpTypeEnum);
            if (_jumpTypeEnum != JumpTypeEnum.Normal)
            {
                _jumpTypeEnum = JumpTypeEnum.Normal;
                OnShow();
            }
        }
        
        /// <summary>
        /// 是否显示挑战logo
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisibleBattleLogo(bool visible)
        {
            if (objectType == BigMapObjectType.Pet)
            {
                modelObj.logo.visible = visible;
                if (stateType != StateType.Idle)
                {
                    modelObj.logo.visible = false;
                }
            }
            else
            {
                modelObj.logo.visible = visible;
            }
        }

        private bool IsAttackRange(BigMapObject obj1, GObject obj2)
        {
            Vector2 pos1 = obj1.modelObj.LocalToGlobal(Vector2.zero);
            Vector2 pos2 = obj2.LocalToGlobal(Vector2.zero);
            // Debug.Log($"=======两个之间的距离=={Vector2.Distance(pos1, pos2)}");
            if (Vector2.Distance(pos1, pos2) > attackRange)
            {
                return false;
            }
            return true;
        }
        
        private int duration = -1;
        public bool GetDistance(BigMapObject obj1, GObject obj2)
        {
            if (obj2 == null || obj2.displayObject == null) return false;
            
            duration = -1;
            Vector2 pos1 = obj1.modelObj.LocalToGlobal(Vector2.zero);
            Vector2 pos2 = obj2.LocalToGlobal(Vector2.zero);
            if (obj1.objectType == BigMapObjectType.Boss)
            {
                if (obj1.stateType == StateType.Attacking)
                {
                    duration = 100;
                }
                else
                {
                    duration = 150;
                }
            }else if (obj1.objectType == BigMapObjectType.Pet)
            {
                if (obj1.stateType == StateType.Idle)
                {
                    duration = 180;
                }
            }
            
            if (Vector2.Distance(pos1, pos2) < duration)
            {
                return true;
            }
            return false;
        }
    }
}
