
using Common;
using Engine;
using FairyGUI;
using UnityEngine;

public class PayLoadingView : UIViewBase
{
    private UI_PayLoading PayLoading => this.main as UI_PayLoading;
    private int _type;
    public PayLoadingView()
    {
        this.name = "PayLoading";
        this.package = "Common";
        this.component = "PayLoading";
        this.removePackage = false;
        this.type = UIType.Tip;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _type = (int) values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.PayLoading.ctrl.selectedIndex = _type;
        GameManager.Instance.TimerManager.ClearTimer(this.Hide);
        GameManager.Instance.TimerManager.SetTimer(30f, this.Hide);
    }

    protected override void OnHide()
    {
        base.OnHide();

        GameManager.Instance.TimerManager.ClearTimer(this.Hide);
    }
    
}
