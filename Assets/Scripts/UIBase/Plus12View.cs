
using Engine;
using Login;

public class Plus12View : UIViewBase
{
    private UI_Plus12 Plus12 => this.main as UI_Plus12;
    
    public Plus12View()
    {
        this.name = "Login";
        this.package = "Login";
        this.component = "Plus12";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        LoginBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.Plus12.closeBtn.onClick.Add(this.Hide);
        this.Plus12.closeBtn.onClick.Add(this.HideWithSoundEffect);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.Plus12.txtLb.text = ConfigUtils.GetStringByKey(29);
    }
}
