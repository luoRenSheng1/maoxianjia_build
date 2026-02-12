
using Common;
using Engine;
using FairyGUI;
using UnityEngine;

public class SCLoadingView : UIViewBase
{
    private UI_SCLoading ScLoading => this.main as UI_SCLoading;
     
    public SCLoadingView()
    {
        this.name = "SCLoading";
        this.package = "Common";
        this.component = "SCLoading";
        this.removePackage = false;
        this.type = UIType.Tip;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }
    

    protected override void OnShow()
    {
        base.OnShow();

        GameManager.Instance.TimerManager.ClearTimer(this.Hide);
        GameManager.Instance.TimerManager.SetTimer(1f, this.Hide);
    }

    protected override void OnHide()
    {
        base.OnHide();

        GameManager.Instance.TimerManager.ClearTimer(this.Hide);
    }
    
}
