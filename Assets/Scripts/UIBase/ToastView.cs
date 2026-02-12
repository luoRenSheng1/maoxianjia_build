using Common;

//用例:UIManager.Instance.Toast("Toast");
//用例:UIManager.Instance.Toast(IconUrl,"Toast");
public class ToastView : UIViewBase
{
    private UI_ToastWindow toastView => this.main as UI_ToastWindow;

    public ToastView()
    {
        this.name = "Toast";
        this.package = "Common";
        this.component = "ToastWindow";
        this.removePackage = true;
        this.type = UIType.Tip;
    }
    
    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnDispose()
    {
        base.OnDispose();

        GameManager.Instance.TimerManager.ClearTimer(OnHideUI);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);

        if (values.Length  == 1)//纯文本
        {
            string firstValue = (string)values[0]; // 取出第一个参数
            Toast(firstValue);
        }else if (values.Length == 2)//带图标的
        {
            string iconUrl = (string)values[0]; // 取出第一个参数
            string strValue = (string)values[1]; // 取出第一个参数
            Toast(iconUrl, strValue);
        }
        else
        {
            LogUtils.LogWarning("No values provided.");
        }
    }

    private void Toast(string value)
    {
        //LogUtils.LogWarning($"ToastView value：{value}");
        this.toastView.ctrlMode.selectedIndex = 0;
        if (!string.IsNullOrEmpty(value))
        {
            this.toastView.t0.Play();
            this.toastView.tips.txt_title.text = value;
            GameManager.Instance.TimerManager.ClearTimer(OnHideUI);
            GameManager.Instance.TimerManager.SetTimer(2.0f, OnHideUI);
        }
        else
        {
            LogUtils.LogWarning("value does not exist");
        }
    }
    
    private void Toast(string iconUrl, string value)
    {
        //LogUtils.LogWarning($"ToastView value：{value}");
        this.toastView.ctrlMode.selectedIndex = 1;
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(iconUrl))
        {
            this.toastView.iconAni.Play();
            this.toastView.iconTips.txt_Icon_title.text = value;
            this.toastView.iconTips.loader_Icon.url = iconUrl;
            GameManager.Instance.TimerManager.ClearTimer(OnHideUI);
            GameManager.Instance.TimerManager.SetTimer(2.0f, OnHideUI);
        }
        else
        {
            LogUtils.LogWarning("value does not exist");
        }
    }

    private void OnHideUI()
    {
        this.SetVisible(false);
    }
}