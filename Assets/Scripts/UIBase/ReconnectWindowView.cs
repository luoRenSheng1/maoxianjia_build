using Common;
using Engine;
using EngineBase;

public class ReconnectWindowView : UIViewBase
{
    private UI_ReconnectWindow ReconnectWindow => this.main as UI_ReconnectWindow;
    public ReconnectWindowView()
    {
        this.type = UIType.Tip;
        this.name = "ReconnectWindow";
        this.package = "Common";
        this.component = "ReconnectWindow";
        this.removePackage = true;
        this.safeAreaInset = false;
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.ReconnectWindow.sortingOrder = 1000000000;

        GameManager.Instance.TimerManager.SetTimer(60.0f, this.LongReconnect);
    }

    private void LongReconnect()
    {
        GameManager.Instance.Connection.ActiveClose();
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = () =>
            {
                //断线
                ItemInfoManager.Instance.Clear();
                EquipManager.Instance.Clear();
                UIManager.Instance.CloseAllUIPanel();
                UIManager.Instance.ShowUIPanel("Login");
            },
            CancelBack = () =>
            {
                //EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
            }
        };
        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(25), param, false);
        UIManager.Instance.CloseUIPanel("ReconnectWindow");
    }

    protected override void OnHide()
    {
        base.OnHide();
        GameManager.Instance.TimerManager.ClearTimer(this.LongReconnect);
    }
}
