using Config;
using EngineBase;
using FairyGUI;
using msg;
using UnityEngine;

namespace Engine
{
    public enum JumpTypeEnum
    {
        Normal = 0,
        //1、自动寻路类
        KillStageMonster = 1001,  //=当前关卡挑战 击杀怪物 挑战按钮关卡的上一关挑战
        MapBaoxiang = 1002,  //=索引最近的宝箱挖掘
        MapGold = 1003,  //=最近的金币堆
        MapDiamond = 1004,  //=最近的钻石矿
        MapFish = 1005,  //=最近的钓鱼点位开始钓鱼
        MapRandomEvent = 1006,  //=最近的随机事件点
        MapBoss = 1007,  //=最低关卡ID的传承BOSS
        PassStage = 1008,  //=挑战任务要求的关卡
        MapRandomBox = 1009,  //=前往深埋宝藏坐标
        
        // 2、跳转类
        CallPet = 2001,  //=跳转召唤宠物界面
        CallSkill = 2002,  //=跳转召唤技能界面
        CallHero = 2003,  //=跳转英雄召唤界面
        PetTalent = 2004,  //=跳转宠物天赋界面
        PetBook = 2005,  //=跳转宠物技能书界面
        WatchAd = 2006,  //=跳转挂机奖励界面
        // 2007,  //=跳转装备商店升级界面
        // 2008,  //=跳转角色升级界面
        // 2009,  //=跳转角色天赋界面
        // 2010,  //=跳转到神器升级界面
        // 2011,  //=跳转到圣物升级界面
        
        MiDianAtk = 3001,  //=跳转最高等级强击秘典
        MiDianLife = 3002,  //=跳转最高等级生命秘典
        MiDianDef = 3003,  //=跳转最高等级防御秘典
        MiDianPet = 3004,  //=跳转最高等级宠物秘典
        
        // 3、播放文字类
        DailyTaskTips = 4001,  //=完成{0}次日常任务
        OnlineTips = 4002,  //=在线时长{0}秒
        OnlineAward = 4003,  //=领取在线奖励{0}次
    }

    public class JumpManager : TSingleton<JumpManager>
    {
        /// <summary>
        /// 手指
        /// </summary>
        private GComponent _handle;
        
        public bool isWatchAd = false;  //是否跳转看广告

        public void OnInit()
        {
            FairyGUI.Stage.inst.onTouchEnd.AddCapture(__stageTouchEnd);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            FairyGUI.Stage.inst.onTouchEnd.RemoveCapture(__stageTouchEnd);
        }
        
        private void __stageTouchEnd(EventContext context)
        {
            HideFinger();
        }
        
        /// <summary>
        /// 是否在战斗
        /// </summary>
        private bool StageMonsterTask(JumpTypeEnum jumpType, ConfigTaskGuideUnit taskGuideUnit, int taskStageId = 0)
        {
            // 跳转指定跳转关卡
            if (DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Normal || DataManager.Instance.GetRoleData().battleStatus == (int)eBattleStatus.eBattleStatus_Circle)
            {
                if (jumpType == JumpTypeEnum.KillStageMonster)
                {
                    UIManager.Instance.ToastByKey(taskGuideUnit.Doc);
                    return true;
                }else if (jumpType == JumpTypeEnum.PassStage)
                {
                    if (DataManager.Instance.mRoleData.stageId == taskStageId)
                    {
                        UIManager.Instance.ToastByKey(taskGuideUnit.Doc);
                        return true;
                    }
                }
            }
            return false;
        }

        public int PM_TaskId = 1001;
        /// <summary>
        /// 主线任务点击
        /// </summary>
        public void ClickMainTaskJump()
        {
            // ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(PM_TaskId);
            ConfigTaskUnit taskUnit = ConfigUtils.GetTaskById(TaskInfoManager.Instance.GetCurTaskId());
            if (taskUnit.Conduct > 0)
            {
                // if (int.Parse(taskUnit.Param1) > TaskInfoManager.Instance.GetCurTaskNum())
                // { }
                ConfigTaskGuideUnit taskGuideUnit = ConfigUtils.GetTaskGuideById(taskUnit.Conduct);
                if (taskGuideUnit != null)
                {
                    if ((JumpTypeEnum)taskGuideUnit.Conduct == JumpTypeEnum.KillStageMonster || (JumpTypeEnum)taskGuideUnit.Conduct == JumpTypeEnum.PassStage)
                    {
                        bool isBattle = StageMonsterTask((JumpTypeEnum)taskGuideUnit.Conduct, taskGuideUnit, int.Parse(taskUnit.Param1));
                        if (isBattle) return;
                    }

                    JumpToView(taskGuideUnit.JumpPath, (JumpTypeEnum)taskGuideUnit.Conduct, taskGuideUnit);
                }
            }
            else
            {
                UIManager.Instance.ToastByKey(10173);
            }
        }

        /// <summary>
        /// 每日任务 任务点击
        /// </summary>
        /// <param name="dailyTaskUnit"></param>
        public void ClickDailyTaskJump(ConfigDailyTaskUnit dailyTaskUnit)
        {
            if (dailyTaskUnit != null && dailyTaskUnit.Conduct != 0)
            {
                ConfigTaskGuideUnit taskGuideUnit = ConfigUtils.GetTaskGuideById(dailyTaskUnit.Conduct);
                if (taskGuideUnit != null)
                {
                    JumpToView(taskGuideUnit.JumpPath, (JumpTypeEnum)taskGuideUnit.Conduct, taskGuideUnit);
                }
            }
        }

        public void JumpToView(string viewName, JumpTypeEnum jumpType, ConfigTaskGuideUnit taskGuideUnit)
        {
            UIViewBase view = UIManager.Instance.FindTopController();
            switch (viewName)
            {
                case "ChapterMap":
                    if (view.name == viewName)
                    {
                        ((ChapterMapView) view).JumpToTarget(jumpType);
                    }
                    else
                    {
                        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                        lobbyView?.OpenBottomPanel(2, jumpType);//默认进入地图
                    }
                    break;
                case "Lobby":
                    if (jumpType == JumpTypeEnum.MiDianAtk || jumpType == JumpTypeEnum.MiDianDef || jumpType == JumpTypeEnum.MiDianLife || jumpType == JumpTypeEnum.MiDianPet)
                    {   // 跳转指引到秘典
                        if (view.name == viewName)
                        {
                            ((LobbyView)view).ClassicListScrollToView(jumpType);
                        }
                        else
                        {
                            LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
                            lobbyView?.OpenBottomPanel(2, jumpType);
                        }
                    }
                    break;
                case "SummonSystem": //召唤界面
                    LobbyView lobbyView4 = UIManager.Instance.FindByName("Lobby") as LobbyView;
                    lobbyView4?.OnlyOpenBottomPanel(4, jumpType);
                    break;
                case "OfflineReward":
                    if (jumpType == JumpTypeEnum.WatchAd)
                    {
                        isWatchAd = true;
                        var builder = OnlineAwardClick_CS.CreateBuilder();
                        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_OnlineAwardClick_CS, builder.Build());
                    }
                    break;
            }
        }

        #region 手指

        private void CreateFinger()
        {
            _handle = UIPackage.CreateObject("Common", "HandTips").asCom;//手指提示
            _handle.touchable = false;
            
            _handle.GetController("ctrl").selectedIndex = 0;
            GRoot.inst.AddChildAt(_handle, GRoot.inst.numChildren);
        }

        public void ShowFinger(JumpTypeEnum jumpType, GObject _targetUI, int posX = 0, int posY = 0)
        {
            if (jumpType == JumpTypeEnum.Normal) return;
            
            ConfigTaskGuideUnit taskGuideUnit = ConfigUtils.GetTaskGuideById((int)jumpType);
            if (taskGuideUnit == null || taskGuideUnit.FingerPosition <= 0) return;
            
            if(_targetUI == null || _targetUI.displayObject == null || _targetUI.isDisposed || !_targetUI.visible) { return; }
            
            if (_handle == null)
            {
                CreateFinger();
            }
            _handle.visible = true;
            
            Rect rect = _targetUI.TransformRect(new Rect(0, 0, _targetUI.width, _targetUI.height), _targetUI);
            if (posX != 0 || posY != 0)
            {
                _handle.SetXY(posX + rect.width * 0.5f, posY + rect.height * 0.5f);
            }
            else
            {
                _handle.SetXY((int)_targetUI.x + rect.width * 0.5f, (int)_targetUI.y + rect.height * 0.5f);
            }
            
            GRoot.inst.SetChildIndex(_handle, GRoot.inst.numChildren);
        }

        public void HideFinger()
        {
            if (_handle != null)
            {
                _handle.visible = false;
                isWatchAd = false;
            }
        }
        #endregion
    }
}