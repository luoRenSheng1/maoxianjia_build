using msg;
using Setting;

public class ExchangeCodeView : UIViewBase
{
    private UI_ExchangeCode ExchangeCode => this.main as UI_ExchangeCode;
    
    public ExchangeCodeView()
    {
        this.name = "ExchangeCode";
        this.package = "Setting";
        this.component = "ExchangeCode";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SettingBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.ExchangeCode.closeBtn.onClick.Add(this.Hide);
        this.ExchangeCode.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.ExchangeCode.exchangeBtn.onClick.Add(this.OnClickExchangeCode);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.ExchangeCode.ipt.text = "";
    }

    private void OnClickExchangeCode()
    {
        if(string.IsNullOrEmpty(this.ExchangeCode.ipt.text))
            return;
        var builder = AwardCode_CS.CreateBuilder();
        builder.AwardCode = this.ExchangeCode.ipt.text;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_AwardCode_CS, builder.Build());
        SetVisible(false);
    }
    
}