using Common;
using CommonEx;
using Engine;
using FairyGUI;
using msg;
using System;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class MessageTipsView : UIViewBase
{
    private UI_MessageTips messageTips => this.main as UI_MessageTips;
    
    private string _content;
    private string leftBtnTxt;
    private string rightBtnTxt;

    public Action<bool> callback = null;

    public MessageTipsView()
    {
        this.name = "MessageTips";
        this.package = "Common";
        this.component = "MessageTips";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.messageTips.closeBtn.onClick.Add(this.Hide);
        this.messageTips.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.messageTips.agreeBtn.onClick.Add(this.OnClickAgreeBtn);
        this.messageTips.RefusedBtn.onClick.Add(this.OnClickRefuseBtn);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _content = values[0] as string;
        leftBtnTxt = values[1] as string;
        rightBtnTxt = values[2] as string;
        callback = values[3] as Action<bool>;
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        this.messageTips.txtContent.text = _content;
        if (leftBtnTxt != "")
        {
            this.messageTips.RefusedBtn.title = leftBtnTxt;
        }

        if (rightBtnTxt != "")
        {
            this.messageTips.agreeBtn.title = rightBtnTxt;
        }
    }
    protected override void OnHide()
    {
        base.OnHide();
        this.callback = null;
    }


    private void OnClickAgreeBtn()
    {
        if(this.callback != null)
        {
            this.callback?.Invoke(true);
            this.callback = null;
        }
        else
        {
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.EVENT_Dorp_NPC_TASK);
        }
        this.SetVisible(false);
    }

    private void OnClickRefuseBtn()
    {
        this.SetVisible(false);
        if (this.callback != null)
        {
            this.callback?.Invoke(false);
        }
    }
}
