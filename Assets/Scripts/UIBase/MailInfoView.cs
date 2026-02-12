using System;
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Mail;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class MailInfoView : UIViewBase
{
    private UI_MailInfo MailInfo => this.main as UI_MailInfo;

    private MailInfo _mailInfo;
    
    public MailInfoView()
    {
        this.name = "MailInfo";
        this.package = "Mail";
        this.component = "MailInfo";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        MailBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _mailInfo = values[0] as MailInfo;
    }

    protected override void OnInit()
    {
        base.OnInit();

        this.MailInfo.rewardList.itemRenderer = RewardListItemRender;
        this.MailInfo.getBtn.onClick.Add(this.OnClickGetBtn);
        this.MailInfo.delBtn1.onClick.Add(this.OnClickDelBtn);
        this.MailInfo.delBtn2.onClick.Add(this.OnClickDelBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.MailInfo.rewardList.numItems = _mailInfo.MailAttachInfos.Count;
        this.MailInfo.contentLb.text = ConfigUtils.GetTextById(_mailInfo.Content, _mailInfo.ContentParams);
        this.MailInfo.titleLb.text = ConfigUtils.GetTextById(_mailInfo.Title, _mailInfo.TitleParams);
        this.MailInfo.endTimeLb.SetVar("value",StringUtils.FormatAfterTime((int) (_mailInfo.EndTime - ServerTimeManager.Instance.CurServerTime)).ToString()).FlushVars();
        double statusInt = Math.Pow(2, (double)eMailStatus.eMailStatus_Attach);
        bool hasAttach = (_mailInfo.Status & (int) statusInt) > 0;
        if (hasAttach && _mailInfo.MailAttachInfos.Count > 0)
        {
            this.MailInfo.stateType.selectedIndex = 2;
        }
        else if(_mailInfo.MailAttachInfos.Count > 0 && !hasAttach)
        {
            this.MailInfo.stateType.selectedIndex = 1;
        }
        else
        {
            this.MailInfo.stateType.selectedIndex = 0;
        }

      
    }

    private void RewardListItemRender(int index, GObject item)
    {
        MailAttachInfo mailAttachInfo = _mailInfo.MailAttachInfos[index];
        ItemData itemData = new ItemData();
        itemData.id = mailAttachInfo.AttachType;
        itemData.count = mailAttachInfo.AttachNum;
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
    }
    
    private void OnClickGetBtn()
    {
        var builder = MailExtract_CS.CreateBuilder();
        builder.ActionType = (int) eActionType.eActionType_List;
        builder.MailType = (eMailType) _mailInfo.MailType;
        builder.MailGuidList.Add(_mailInfo.MailId);
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailExtract_CS, builder.Build());
        
        SetVisible(false);
    }

    private void OnClickDelBtn()
    {
        var builder = MailRemove_CS.CreateBuilder();
        builder.ActionType = (int) eActionType.eActionType_List;
        builder.MailType = (eMailType) _mailInfo.MailType;
        builder.MailGuidList.Add(_mailInfo.MailId);
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailRemove_CS, builder.Build());
        
        SetVisible(false);
    }
}
