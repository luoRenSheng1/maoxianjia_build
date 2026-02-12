using System.Collections;
using System.Collections.Generic;
using BigMap;
using Config;
using Engine;
using FairyGUI;
using msg;
using UnityEngine;

public class RoundGatherCom : UIViewBase
{
    private UI_ChapterEventStageDetail eventDetail => this.main as UI_ChapterEventStageDetail;

    // private ConfigStageUnit stageUnit;
    private MapStageData stageData;
    public RoundGatherCom()
    {
        this.package = "BigMap";
        this.name = "ChapterEventStageDetail";
        this.component = "ChapterEventStageDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        BigMapBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.eventDetail.btnFight.onClick.Add(this.OnClickFightBtn);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        stageData = values[0] as MapStageData;
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        RandomEventData taskData = stageData.stageTaskData;
        //todo 测试
        if (taskData == null)
        {
            taskData = new RandomEventData();
            taskData.eventId = 1003;
            taskData.extraIdList = new List<int>() { 1002 };
        }
        
        // RandomEventData taskData = stageData.stageTaskData;
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
        if (eventData != null)
        {
            //eventData.Resource = 10040;//todo 测试
            
            this.eventDetail.name.text = ConfigUtils.GetTextById(eventData.Name);
            this.eventDetail.icon.url = UIResource.GetMapEventPopBgByEventId(eventData.Resource);

            // string[] stageIdArr = eventData.TaskIndex.Split(",");
            
            if (eventData.Type == (int)StageEventType.Box) //随机宝箱
            {
                this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8027);
                this.eventDetail.desc.text = ConfigUtils.GetTextById(eventData.Desc.ToString());
            }else if (eventData.Type == (int)StageEventType.Relic) // 遗迹建筑
            {
                this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8028);
                ConfigRuinsBuffUnit data = ConfigUtils.GetRuinsBuffDataById(taskData.extraIdList[0]);
                this.eventDetail.desc.text = ConfigUtils.GetTextById(data.Name);
            }else if (eventData.Type == (int)StageEventType.Diamond) //钻石资源建筑
            {
                this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8026);
                this.eventDetail.desc.text = ConfigUtils.GetTextById(eventData.Desc.ToString());
            }else if (eventData.Type == (int)StageEventType.Gold) // 金币资源建筑
            {
                this.eventDetail.txtBtn.text = ConfigUtils.GetStringByKey(8026);
                this.eventDetail.desc.text = ConfigUtils.GetTextById(eventData.Desc.ToString());
            }
        }
        
            
            
        // int groupId = stageUnit.MonsterData;
        // var monsterGroupArr = ConfigUtils.GetMonsterGroupById(groupId);
        // ConfigMonsterGroupUnit data = monsterGroupArr[0];
        // ConfigMonsterUnit monsterUnit = ConfigUtils.GetMonsterById(data.MonsterId);
        
        Debug.Log("===== ChapterEventStageDetailView =====");
    }

    private void OnClickFightBtn(EventContext context)
    {
        RandomEventData taskData = stageData.stageTaskData;
        ConfigEventUnit eventData = ConfigUtils.GetEventDataById(taskData.eventId);
        if (eventData.Type == (int)StageEventType.Box) //随机宝箱
        {
            var builder = ClaimRandomBox_CS.CreateBuilder();
            //builder.BoxId = (uint)taskData.extraIdList[0];
            //builder.IsRandomEvent = true;
            //builder.RandomEventId = (uint)taskData.eventId;
            //builder.RandomEventType = (eRandomEventType)eventData.Type;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimRandomBox_CS, builder.Build());
        }else if (eventData.Type == (int)StageEventType.Relic) // 遗迹建筑
        {
            var builder = ClaimBuff_CS.CreateBuilder();
            builder.BuffId = 4; //暂时没有buff id   先写死
            // builder.IsRandomEvent = true;
            // builder.RandomEventId = (uint)taskData.eventId;
            // builder.RandomEventType = (eRandomEventType)eventData.Type;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimBuff_CS, builder.Build());
        }else if (eventData.Type == (int)StageEventType.Diamond || eventData.Type == (int)StageEventType.Gold) // 钻石资源建筑 // 金币资源建筑
        {
            var builder = ClaimRandomFinance_CS.CreateBuilder();
            // builder.RandomEventId = (uint)taskData.eventId;
            // builder.RandomEventType = (eRandomEventType)eventData.Type;
            GameManager.Instance.Connection?.SendMessage((int)eMsgID.eMsg_ClaimRandomFinance_CS, builder.Build());
        }

        UIManager.Instance.CloseUIPanel("ChapterEventStageDetail");
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.CloseUIPanel("ChapterEventStageDetail");
    }
}
