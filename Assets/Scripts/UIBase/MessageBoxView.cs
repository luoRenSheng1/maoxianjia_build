
using System;
using Common;

public class MessageBoxView : UIViewBase
{
    public struct MessageParam
    {
        public Action OkCallBack;
        public Action CancelBack;
    }
    
    private UI_MessageBoxWindow boxWindow => this.main as UI_MessageBoxWindow;
    
    private string _content;
    private MessageParam _param;
    private bool hasCancel = false;
    
    public MessageBoxView()
    {
        this.type = UIType.Tip;
        this.name = "MessageBox";
        this.package = "Common";
        this.component = "MessageBoxWindow";
        this.GuideType = FuncType.Guide;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _content = values[0] as string;
        if (values.Length > 1 && values[1] != null)
            _param = (MessageParam) values[1];
        if (values.Length > 2 && values[2] != null)
            hasCancel =(bool) values[2];
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.boxWindow.messageBox.close.visible = false;
        this.boxWindow.messageBox.close.onClick.Add(this.OnClickCancel);
        this.boxWindow.messageBox.cancleBtn.onClick.Add(this.OnClickCancel);
        this.boxWindow.messageBox.okBtn.onClick.Add(this.OnClickOkBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.boxWindow.sortingOrder = 1000000001;
        this.boxWindow.messageBox.content.text = _content;
        this.boxWindow.messageBox.typeOp.selectedIndex = hasCancel ? 1 : 0;

        boxWindow.messageBox.HandTips.visible = false;
        this.boxWindow.messageBox.cancleBtn.touchable = true;
        if (DataManager.Instance.GetTreasureData().id == 1 && GuideManager.Instance.StarGuideByData(new GuideData()
        {
            bid = GuideID.NewAccount_MPUpLevel1,
            giding = GuideID.NewAccount_MPSpeed,
            gid = GuideID.NewAccount_MPSpeedDiam,
            tui = this.boxWindow.messageBox.okBtn,
            isForce = true,
            isSend = true
        }))
        {
            //手指显示
            boxWindow.messageBox.HandTips.visible = true;
            this.boxWindow.messageBox.cancleBtn.touchable = false;
        }
    }

    private void OnClickOkBtn()
    {
        this.Hide();
        _param.OkCallBack?.Invoke();
    }

    private void OnClickCancel()
    {
        this.Hide();
        _param.CancelBack?.Invoke();
    }

    protected override void OnHide()
    {
        base.OnHide();

        //强制完成引导
        if(boxWindow.messageBox.HandTips.visible)
        {
            var gid = (int) GuideID.NewAccount_MPSpeedDiam;
            if (!GuideManager.Instance.GuideIsComplete(gid))
                GuideManager.Instance.SendToCompleteGuide(gid);
            GuideManager.Instance.HideGuide();
        }
    }
}
