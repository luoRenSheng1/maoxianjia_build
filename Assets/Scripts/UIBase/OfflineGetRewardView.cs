using Offline;

public class OfflineGetRewardView : UIViewBase
{
    private UI_OfflineGetReward OfflineGetReward => this.main as UI_OfflineGetReward;
    
    public OfflineGetRewardView()
    {
        this.name = "OfflineGetReward";
        this.package = "Offline";
        this.component = "OfflineGetReward";
    }

    public override void BindAll()
    {
        base.BindAll();
        OfflineBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
    }

    protected override void OnInit()
    {
        base.OnInit();

    }
    
    protected override void OnShow()
    {
        base.OnShow();
    }

    protected override void OnHide()
    {
        base.OnHide();

    }
    
}
