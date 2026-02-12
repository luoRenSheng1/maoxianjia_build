using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using DailyTask;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using Random = UnityEngine.Random;

public class DailyTaskView : UIViewBase
{
    private UI_DailyTask DailyTask => this.main as UI_DailyTask;

    private List<DailyTaskInfo> _dailyTaskList;
    private List<DailyTaskInfo> _curDailyTaskList;
    private Coroutine _coTimeFlow;

    public DailyTaskView()
    {
        this.name = "DailyTask";
        this.package = "DailyTask";
        this.component = "DailyTask";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        DailyTaskBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _dailyTaskList = new List<DailyTaskInfo>();
        _curDailyTaskList = new List<DailyTaskInfo>();
        // this.DailyTask.closeBtn.onClick.Add(this.Hide);
        this.DailyTask.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.DailyTask.dailyList.itemRenderer = DailyTaskItemRender;

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.OnDailyTaskUpdate);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DAILY_TASK_UPDATE, this.OnDailyTaskUpdate);
    }

    protected override void OnShow()
    {
        base.OnShow();
        OnDailyTaskUpdate();
        StartTimeLeftCd();
        TaskInfoManager.Instance.SendToGetDailyTaskCS();
    }
    
    public void StartTimeLeftCd()
    {
        if (null != _coTimeFlow)
            GameManager.Instance.StopCoroutine(_coTimeFlow);

        int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
        if (totalSecond > 0)
        {
            this.DailyTask.timeLb.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            _coTimeFlow = GameManager.Instance.StartCoroutine(TimeFlow());
        }
    }

    IEnumerator TimeFlow()
    {
        while (true)
        {
            int totalSecond = ServerTimeManager.Instance.GetToZeroLeftTime();
            if (totalSecond > 0)
            {
                this.DailyTask.timeLb.SetVar("value", StringUtils.GetTimeString((int)totalSecond)).FlushVars();
            }

            if (totalSecond <= 0)
            {
                TaskInfoManager.Instance.SendToGetDailyTaskCS();
                break;
            }

            yield return GameManager.Instance.waitSec1;
        }
    }

    private void DailyTaskItemRender(int index, GObject item)
    {
        ConfigDailyTaskUnit dailyTaskUnit = _curDailyTaskList[index].DailyTaskUnit;
        ((UI_DailyTaskItem) item).rewardBtn.onClick.Clear();
        ((UI_DailyTaskItem) item).rewardBtn.data = dailyTaskUnit;
        if (dailyTaskUnit.Type == (int) TaskType.eMainTaskType_GetQualityEquip || dailyTaskUnit.Type == (int) TaskType.eMainTaskType_login)
        {
            ((UI_DailyTaskItem) item).desc.text = ConfigUtils.GetTextById(dailyTaskUnit.Name);
            ((UI_DailyTaskItem) item).taskBar.min = 0;
            ((UI_DailyTaskItem) item).taskBar.max = 1;
            ((UI_DailyTaskItem) item).taskBar.value = Math.Min(1,_curDailyTaskList[index].Progress);
            if (_curDailyTaskList[index].Progress >= 1)
            {
                if (_curDailyTaskList[index].CanGetRw == 2)
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 2;
                }
                else
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 1;
                    ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickGetDailyRw);
                }
            }
            else
            {
                ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 0;
                ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickTips);
            }
        }
        else if (dailyTaskUnit.Type == (int) TaskType.eMainTaskType_PassStage)
        {
            ((UI_DailyTaskItem) item).desc.text = ConfigUtils.GetTextById(dailyTaskUnit.Name);
            ((UI_DailyTaskItem) item).taskBar.min = 0;
            ((UI_DailyTaskItem) item).taskBar.max = 1;
            int progress = 0;
            if (_curDailyTaskList[index].Progress > 0)
            {
                progress = 1;
            }
            ((UI_DailyTaskItem) item).taskBar.value = Math.Min(1,progress);
            if (_curDailyTaskList[index].Progress >= int.Parse(dailyTaskUnit.Param))
            {
                if (_curDailyTaskList[index].CanGetRw == 2)
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 2;
                }
                else
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 1;
                    ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickGetDailyRw);
                }
            }
            else
            {
                ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 0;
                ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickTips);
            }
        }
        else if (dailyTaskUnit.Type == (int) TaskType.eMainTaskType_OnlineTime)
        {
            int onlineTime = (int)(ServerTimeManager.Instance.CurServerTime - ServerTimeManager.Instance.LoginTimeStamp);
            int progress = _curDailyTaskList[index].Progress;
            progress += onlineTime;
            progress = Mathf.Min(int.Parse(dailyTaskUnit.Param), progress);
            ((UI_DailyTaskItem) item).desc.text = StringUtils.Format(ConfigUtils.GetTextById(dailyTaskUnit.Name),dailyTaskUnit.Param);
            ((UI_DailyTaskItem) item).taskBar.min = 0;
            ((UI_DailyTaskItem) item).taskBar.max = int.Parse(dailyTaskUnit.Param);
            ((UI_DailyTaskItem) item).taskBar.value = progress;
            if (progress >= int.Parse(dailyTaskUnit.Param))
            {
                if (_curDailyTaskList[index].CanGetRw == 2)
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 2;
                }
                else
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 1;
                    ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickGetDailyRw);
                }
            }
            else
            {
                ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 0;
                ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickTips);
            }
        }
        else
        {
            int progress = Mathf.Min(int.Parse(dailyTaskUnit.Param), _curDailyTaskList[index].Progress);
            ((UI_DailyTaskItem) item).desc.text = StringUtils.Format(ConfigUtils.GetTextById(dailyTaskUnit.Name), dailyTaskUnit.Param);
            ((UI_DailyTaskItem) item).taskBar.min = 0;
            ((UI_DailyTaskItem) item).taskBar.max = int.Parse(dailyTaskUnit.Param);
            ((UI_DailyTaskItem) item).taskBar.value = progress;
            if (_curDailyTaskList[index].Progress >= int.Parse(dailyTaskUnit.Param))
            {
                if (_curDailyTaskList[index].CanGetRw == 2)
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 2;
                }
                else
                {
                    ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 1;
                    ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickGetDailyRw);
                }
            }
            else
            {
                ((UI_DailyTaskItem) item).flagCtrl.selectedIndex = 0;
                ((UI_DailyTaskItem) item).rewardBtn.onClick.Set(this.OnClickTips);
            }
        }

        if (((UI_DailyTaskItem) item).flagCtrl.selectedIndex == 0)
        {
            ((UI_DailyTaskItem)item).rewardBtn.text = ConfigUtils.GetStringByKey(52);
        }
    
        string[] rewardItemList = dailyTaskUnit.Reward.Split(',');
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardItemList[0]));
        ((UI_ItemCom)((UI_DailyTaskItem) item).item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom)((UI_DailyTaskItem) item).item).txtLv.text = rewardItemList[1].ToString();
        ((UI_ItemCom)((UI_DailyTaskItem) item).item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
    
        ((UI_ItemCom) ((UI_DailyTaskItem) item).item).data = int.Parse(rewardItemList[0]);
        ((UI_ItemCom)((UI_DailyTaskItem) item).item).onClick.Set(OnItemTips);
    }
    
    private void OnItemTips(EventContext context)
    {
        int itemId = (int)((UI_ItemCom)context.sender).data;
        if (itemId != 0)
        {
            TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
        }
    }

    private int tempNum = 0;
    private void OnClickTips(EventContext context)
    {
        ConfigDailyTaskUnit dailyTaskUnit =  ((GButton)context.sender).data as ConfigDailyTaskUnit;
        if (dailyTaskUnit.Type == (int) TaskType.eMainTaskType_DailyTaskNum)  // 32 日常任务数
        {
            tempNum = 0;
            foreach (var dailyData in _curDailyTaskList)
            {
                if (dailyData.DailyTaskUnit == dailyTaskUnit)
                {
                    tempNum = int.Parse(dailyTaskUnit.Param) - dailyData.Progress;
                    tempNum = tempNum > 0 ? tempNum : 0;
                    break;
                }
            }
            var str = string.Format(ConfigUtils.GetStringByKey(8084), tempNum);
            UIManager.Instance.Toast(str);
            return;
        }else if (dailyTaskUnit.Type == (int)TaskType.eMainTaskType_OnlineTime) // 34 在线时长
        {
            tempNum = 0;
            foreach (var dailyData in _curDailyTaskList)
            {
                if (dailyData.DailyTaskUnit == dailyTaskUnit)
                {
                    int onlineTime = (int)(ServerTimeManager.Instance.CurServerTime - ServerTimeManager.Instance.LoginTimeStamp);
                    onlineTime += dailyData.Progress;
                    tempNum = int.Parse(dailyTaskUnit.Param) - onlineTime;
                    tempNum = tempNum > 0 ? tempNum : 0;
                    break;
                }
            }
            var str = string.Format(ConfigUtils.GetStringByKey(8085), tempNum);
            UIManager.Instance.Toast(str);
            return;
        }else if (dailyTaskUnit.Type == (int)TaskType.eMainTaskType_GetOnlineRw) // 38 领取在线奖励
        {
            tempNum = 0;
            foreach (var dailyData in _curDailyTaskList)
            {
                if (dailyData.DailyTaskUnit == dailyTaskUnit)
                {
                    tempNum = int.Parse(dailyTaskUnit.Param) - dailyData.Progress;
                    tempNum = tempNum > 0 ? tempNum : 0;
                    break;
                }
            }
            var str = string.Format(ConfigUtils.GetStringByKey(8086), tempNum);
            UIManager.Instance.Toast(str);
            return;
        }

        if (dailyTaskUnit.Conduct > 0)
        {
            this.Hide();
            JumpManager.Instance.ClickDailyTaskJump(dailyTaskUnit);
        }else
            UIManager.Instance.ToastByKey(10173);
    }
    
    private void OnClickGetDailyRw(EventContext context)
    {
        ConfigDailyTaskUnit dailyTaskUnit =  ((GButton)context.sender).data as ConfigDailyTaskUnit;
        if (dailyTaskUnit != null)
        {
            // 领取每日奖励
            var builder = ClaimDailyTaskAward_CS.CreateBuilder();
            builder.TaskId = (uint) dailyTaskUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimDailyTaskAward_CS, builder.Build());
            
            string[] rewardItemList = dailyTaskUnit.Reward.Split(',');
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardItemList[0]));
            LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
            UIItemsGain itemsGain = UIGainBasePool.CreateUIGainBase();//new UIItemsGain();
            itemsGain.ApplyItemSourceToDestination(lobbyView?.GetUserHeadIcon(), itemTypeUnit.Icon); //type=3 通关拿到钥匙
            Vector2 pos = ((GButton)context.sender).LocalToGlobal(Vector2.zero);
            if (itemTypeUnit.Id == ConstDefine.Item_GoldId)
            {
                itemsGain.StartItemFly(pos, int.Parse(rewardItemList[1]));
            }
            else
            {
                itemsGain.StartItemFly(pos, Random.Range(5,8));
            }
        }

    }

    // 修改后的每日任务更新
    private void OnDailyTaskUpdate()
    {
        _curDailyTaskList = TaskInfoManager.Instance.GetDailyTaskInfos();
        // _dailyTaskList = TaskInfoManager.Instance.GetDailyTaskInfos();
        // foreach (var item in _dailyTaskList)
        // {
        //     if (item.DailyTaskUnit.SystemId == 0)
        //     {
        //         _curDailyTaskList.Add(item);
        //     }
        //     else if (item.DailyTaskUnit.Type.Equals(TaskType.eMainTaskType_GetQualityEquip) && (item.DailyTaskUnit.SystemId < DataManager.Instance.GetRoleData().equipBoxLv))
        //     {
        //         _curDailyTaskList.Add(item);
        //     }
        //     else if (item.DailyTaskUnit.SystemId > 0 && FuncPreviewManger.Instance.FunIsOpened(item.DailyTaskUnit.SystemId) && !item.DailyTaskUnit.Type.Equals(TaskType.eMainTaskType_GetQualityEquip))
        //     {
        //         _curDailyTaskList.Add(item);
        //     }
        // }

        _curDailyTaskList.Sort(((info, prevInfo) =>
        {
            int result = 0;
            result = info.CanGetRw > prevInfo.CanGetRw ? 1 : (info.CanGetRw == prevInfo.CanGetRw) ? 0 : -1;
            if (result == 0)
                result = info.DailyTaskUnit.Id < prevInfo.DailyTaskUnit.Id ? -1 : 1;
            return result;
        }));
        this.DailyTask.dailyList.numItems = _curDailyTaskList.Count;
        this.DailyTask.dailyList.ScrollToView(0);
    }

}
