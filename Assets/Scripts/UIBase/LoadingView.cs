using Common;
using EngineBase;
using Login;

public class LoadingView : UIViewBase
{
    private UI_LoadingUI loadingMain
    {
        get { return this.main as UI_LoadingUI; }
    }

    private CTimer timerLoading = new CTimer();

    public LoadingView()
    {
        this.type = UIType.Tip;
        this.name = "Loading";
        this.package = "Common";
        this.component = "LoadingUI";
        this.type = UIType.Top;
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();

        LoginBinder.BindAll();
    }
    
    protected override void OnShow()
    {
        base.OnShow();
    
        GameManager.Instance.TimerManager.ClearTimer(OnLoadingUIDone);
        GameManager.Instance.TimerManager.SetTimer(2.0f, OnLoadingUIDone);
        timerLoading.Startup(2.0f);
    }

    private void OnLoadingUIDone()
    {
        UIManager.Instance.DoLoadingUIDone();
        GameManager.Instance.TimerManager.SetTimer(1f, () =>
        {
            UIManager.Instance.DestroyController(this);
        });
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        this.loadingMain.txtValue.text = string.Format("{0:N2}%", timerLoading.GetPassPrecent() * 100.0f);
    }
}