
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

public class MailMainView : UIViewBase
{
    private UI_MailMain MailMain => this.main as UI_MailMain;

    private List<MailInfo> _mailInfos;
    public MailMainView()
    {
        this.name = "MailMain";
        this.package = "Mail";
        this.component = "MailMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        MailBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.MailMain.closeBtn.onClick.Add(this.Hide);
        this.MailMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.MailMain.mailList.itemRenderer = MailListRender;
        this.MailMain.mailList.SetVirtual();
        this.MailMain.oneKeyDel.onClick.Add(this.OnClickOneKeyDel);
        this.MailMain.oneKeyGet.onClick.Add(this.OnClickOneKeyGet);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateMailList);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_MAIL_LIST_UPDATE, this.UpdateMailList);
    }

    protected override void OnShow()
    {
        base.OnShow();
        //邮件
        MailManager.Instance.SendMailListCS();
    }

    private void MailListRender(int index, GObject item)
    {
        MailInfo mailInfo = _mailInfos[index];
        double statusRead = Math.Pow(2, (double)eMailStatus.eMailStatus_Read);
        bool isRead = (mailInfo.Status & (int)statusRead) > 0;
        double statusAttach = Math.Pow(2, (double)eMailStatus.eMailStatus_Attach);
        bool hasAttach = (mailInfo.Status & (int) statusAttach) > 0;
        if (!isRead)
        {
            ((UI_MailItem) item).mailState.selectedIndex = 0;
        }
        else
        {
            if (hasAttach && mailInfo.MailAttachInfos.Count > 0)
            {
                ((UI_MailItem) item).mailState.selectedIndex = 2;
            }
            else
            {
                if (mailInfo.MailAttachInfos.Count == 0)
                    ((UI_MailItem) item).mailState.selectedIndex = 2;
                else
                    ((UI_MailItem) item).mailState.selectedIndex = 1;
            }
        }
        
        
        ((UI_MailItem) item).titleLb.text = ConfigUtils.GetTextById(mailInfo.Title, mailInfo.TitleParams);
        ((UI_MailItem) item).endTimeLb.SetVar("value", StringUtils.FormatAfterTime((int) (mailInfo.EndTime - ServerTimeManager.Instance.CurServerTime)).ToString()).FlushVars();
        ((UI_MailItem) item).sendTimeLb.text =StringUtils.FormatTimeDifference((int)(ServerTimeManager.Instance.CurServerTime- mailInfo.SendTime));
        
        ((UI_MailItem) item).rewardList.itemRenderer = RewardListItemRender;
        ((UI_MailItem) item).rewardList.data = _mailInfos[index].MailAttachInfos;
        ((UI_MailItem) item).rewardList.numItems = _mailInfos[index].MailAttachInfos.Count;
        ((UI_MailItem) item).data = _mailInfos[index];
        ((UI_MailItem) item).onClick.Set(this.OnClickMailItem);
    }

    private void OnClickMailItem(EventContext context)
    {
        MailInfo mailInfo = (context.sender as UI_MailItem)?.data as MailInfo;
        if (mailInfo != null)
        {
            UIManager.Instance.ShowUIPanel("MailInfo", mailInfo);
            double statusInt = Math.Pow(2, (double)eMailStatus.eMailStatus_Read);
            bool isRead = (mailInfo.Status & (int) statusInt) > 0;
            if (!isRead)
            {
                var builder = MailMark_CS.CreateBuilder();
                builder.ActionType = (int) eActionType.eActionType_List;
                builder.MailType = (eMailType) mailInfo.MailType;
                builder.MailGuidList.Add(mailInfo.MailId);
                builder.Status = (int) eMailStatus.eMailStatus_Read;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailMark_CS, builder.Build());
            }

        }
    }
    
    private void RewardListItemRender(int index, GObject item)
    {
        List<MailAttachInfo> rewardItemList = item.parent.data as List<MailAttachInfo>;
        ItemData itemData = new ItemData();
        itemData.id = rewardItemList[index].AttachType;
        itemData.count = rewardItemList[index].AttachNum;
        ((UI_ItemCom) item).SetItemDataWithGuid(itemData, true);
    }

    private void UpdateMailList()
    {
        if (!IsShow() || !IsOnStage()) return;
        _mailInfos = MailManager.Instance.GetMailList();
        if (_mailInfos.Count == 0)
        {
            this.MailMain.state.selectedIndex = 1;
            return;
        }

        this.MailMain.state.selectedIndex = 0;
        this.MailMain.mailList.numItems = _mailInfos.Count;

    }

    private void OnClickOneKeyDel()
    {
        bool hasDel = false;
        foreach (var item in _mailInfos)
        {
            bool isRead = (item.Status & (int)eMailStatus.eMailStatus_Read) > 0;
            bool hasAttach = (item.Status & (int)eMailStatus.eMailStatus_Attach) > 0;
            if ((!isRead && item.MailAttachInfos.Count == 0) || (!isRead && item.MailAttachInfos.Count > 0 && hasAttach))
            {
                hasDel = true;
                break;
            }
        }

        if (!hasDel)
        {
            UIManager.Instance.ToastByKey(10130);
            return;
        }
        
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = () =>
            {
                var builder = MailRemove_CS.CreateBuilder();
                builder.ActionType = (int) eActionType.eActionType_Onekey;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailRemove_CS, builder.Build());
            }
        };
        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(10128), param);
    }

    private void OnClickOneKeyGet()
    {
        bool hasGetRw = false;
        foreach (var item in _mailInfos)
        {
            bool hasAttach = (item.Status & (int)eMailStatus.eMailStatus_Attach) > 0;
            if (item.MailAttachInfos.Count > 0 && !hasAttach)
            {
                hasGetRw = true;
                break;
            }
        }

        if (!hasGetRw)
        {
            UIManager.Instance.ToastByKey(10131);
            return;
        }
        var builder = MailExtract_CS.CreateBuilder();
        builder.ActionType = (int) eActionType.eActionType_Onekey;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_MailExtract_CS, builder.Build());
    }
}
