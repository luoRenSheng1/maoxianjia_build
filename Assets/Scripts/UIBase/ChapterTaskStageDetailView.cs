using BigMap;
using Common;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using System.Collections.Generic;
using EventDispatcher = EngineBase.EventDispatcher;

public enum TaskModel
{
    None = 0,
    Normal = 1,
    Difficulty
}

public class ChapterTaskStageDetailView : UIViewBase
{
    private UI_ChapterTaskStageDetail taskStageDetail => this.main as UI_ChapterTaskStageDetail;

    private RandomEventData mapStageData;

    private List<ConfigEventTaskUnit> taskList = new List<ConfigEventTaskUnit>();
    private ConfigCommonUnit _commonUnit2009;
    
    public ChapterTaskStageDetailView()
    {
        this.package = "BigMap";
        this.name = "ChapterTaskStageDetail";
        this.component = "ChapterTaskStageDetail";
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
        _commonUnit2009 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(2009);
        // this.taskStageDetail.name.text = ConfigUtils.GetStringByKey(8011);
        // this.taskStageDetail.closeBtn.onClick.Add(this.Hide);
        this.taskStageDetail.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.taskStageDetail.huntingShopBtn.onClick.Add(this.OnClickHuntingShopBtn);
        
        this.taskStageDetail.list.itemRenderer = TaskListRender;
        
        this.taskStageDetail.resetTaskBtn.onClick.Add(this.OnClickResetTaskBtn);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateTaskInfo);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DELEGATE_TASK_UPDATE, UpdateTaskInfo);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DELEGATE_TASK_POINTS_UPDATE, UpdateTaskInfo);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        mapStageData = values[0] as RandomEventData;
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        // if (mapStageData.stageTaskData.extraIdList.Count <= 0)
        // {
        //     OnClickCloseButton();
        //     return;
        // }

        UpdateTaskInfo();

        //狩猎引导-点击第一个领取任务
        GuideManager.Instance.StarGuideByData(new GuideData()
        {
            fid = FuncOpenType.Hunting,
            giding = GuideID.guideId_4000,
            gid = GuideID.guideId_4001,
            tui = ((UI_TaskItem)this.taskStageDetail.list.GetChildAt(0)).takeBtn,
            isForce = true,
            isSend = true,
            //pType = PosType.Left,
            npcTxt = "Beginner_Doc_028",
            npcPosType = PosType.Down,
            touchCB = () =>
            {
                GuideManager.Instance.HideGuide();
            }
        });
    }

    private void UpdateTaskInfo()
    {
        taskList.Clear();
        foreach (var stuff in mapStageData.batchStuffList)
        {
            if (stuff.eventStatus == (int)eRandomEventStatus.eRandomEventStatus_Dispatched)
            {
                int taskId = stuff.cfgId;
                ConfigEventTaskUnit taskUnit = ConfigUtils.GetEventTaskUnitById(taskId);
                if (taskUnit != null)
                {
                    taskList.Add(taskUnit);
                }
            }
        }
        this.taskStageDetail.list.numItems = taskList.Count;

        ((UI_NewCurrency)this.taskStageDetail.currency).txtValue.text = DataManager.Instance.GetRoleData().npcTaskPoints.ToString();//积分

        this.taskStageDetail.costItem.icon.url = UIResource.GetItemUrl(_commonUnit2009.Param1.Split(',')[0]);//砖石图标
        this.taskStageDetail.costItem.num.SetVar("total", StringUtils.FormatCurrency(DataManager.Instance.GetRoleData().dia))
            .SetVar("cost", _commonUnit2009.Param1.Split(',')[1]).FlushVars();

        if (DataManager.Instance.GetRoleData().dia < int.Parse(_commonUnit2009.Param1.Split(',')[1]))
        {
            taskStageDetail.resetBtnStatus.selectedIndex = 1;
            taskStageDetail.costItem.stautsCtrl.selectedIndex = 1;
        }
        else
        {
            taskStageDetail.resetBtnStatus.selectedIndex = 0;
            taskStageDetail.costItem.stautsCtrl.selectedIndex = 0;
        }
        
    }

    private void TaskListRender(int index, GObject item)
    {
        ConfigEventTaskUnit taskUnit = taskList[index];
        
        // ((UI_TaskItem)item).name.text = ConfigUtils.GetTextById(taskUnit.Name);
        ((UI_TaskItem)item).item.num.text = taskUnit.Reward;
        
        // ((UI_TaskItem)item).item.onClick.Add(ClickRewardItem);//目前不需要

        if (taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityEquips || taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityLoreEquips || taskUnit.Type == (int)eMainTaskType.eMainTaskType_CollectQualityPets)
        {
            int num = int.Parse(taskUnit.Param.Split(',')[0]);
            int quality = int.Parse(taskUnit.Param.Split(',')[1]);
            string qualityName = EquipManager.Instance.GetQualityName((QualityType)quality);
            ((UI_TaskItem)item).name.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.Doc), num, qualityName);
            ((UI_TaskItem)item).bar.min = 0;
            ((UI_TaskItem)item).bar.max = num;
            ((UI_TaskItem)item).bar.value = 0;
        }
        else
        {
            ((UI_TaskItem)item).name.text = StringUtils.Format(ConfigUtils.GetTextById(taskUnit.Doc), taskUnit.Param);
            ((UI_TaskItem)item).bar.min = 0;
            ((UI_TaskItem)item).bar.max = int.Parse(taskUnit.Param);
            ((UI_TaskItem)item).bar.value = 0;
        }

        ((UI_TaskItem)item).statuCtrl.selectedIndex = 0;
        ((UI_TaskItem)item).takeBtn.data = index;
        ((UI_TaskItem)item).takeBtn.onClick.Add(OnClickTaskButton);
    }
    
    private void ClickRewardItem(EventContext context)
    {   //物品提示   后期有需要的时候再修改
        // int itemId = (int)((UI_rwItem)context.sender).data;
        // TipsManger.Instance.ShowPopupTip((UI_rwItem)context.sender, Tipstype.None, itemId);
    }
    
    private void OnClickTaskButton(EventContext context)
    {
        if (mapStageData.endTime <= ServerTimeManager.Instance.CurServerTime)
        {
            UIManager.Instance.ToastByKey(8005);
            this.Hide();
            return;
        }
        
        int index = (int)((GButton)context.sender).data;
        var builder = AcceptNPCTask_CS.CreateBuilder();
        builder.TaskId = (uint)taskList[index].Id;
        builder.EventGuid = mapStageData.guid;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AcceptNPCTask_CS, builder.Build());

        OnClickCloseButton();
        // OnClickCloseButton(mapStageData.stageTaskData.guid, taskList[index].Id);
    }
    
    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseUIPanel("ChapterTaskStageDetail");
        
        //点击后切换成已领取任务界面状态
        // UIManager.Instance.ShowUIPanel("HuntingTaskMain", guid, taskId);
    }

    /// <summary>
    /// 打开狩猎商店
    /// </summary>
    private void OnClickHuntingShopBtn()
    {
        UIManager.Instance.ShowUIPanel("HuntingShop");
    }

    /// <summary>
    /// 刷新任务
    /// </summary>
    private void OnClickResetTaskBtn()
    {
        //砖石是否充足
        if (DataManager.Instance.GetRoleData().dia < int.Parse(_commonUnit2009.Param1.Split(',')[1]))
        {
            UIManager.Instance.Toast(ConfigUtils.FormatStringByKey(8038, ConfigUtils.GetTextById(ConfigUtils.GetConfigItemTypeUnitById(int.Parse(_commonUnit2009.Param1.Split(',')[0])).Name)));
            return;
        }
        
        var builder = ShuffleNpcTasks_CS.CreateBuilder();
        builder.EventGuid = mapStageData.guid;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ShuffleNpcTasks_CS, builder.Build());
    }

}
