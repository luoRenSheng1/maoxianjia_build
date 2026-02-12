using Engine;
using RoleMain;

public class RoleHelpView : UIViewBase
{
    private UI_RoleHelp roleUI => this.main as UI_RoleHelp;

    public RoleHelpView()
    {
        this.name = "RoleHelp";
        this.package = "RoleMain";
        this.component = "RoleHelp";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.roleUI.closeBtn.onClick.Add(this.Hide);
        this.roleUI.closeBtn.onClick.Add(this.HideWithSoundEffect);
    }

    protected override void OnDispose()
    {
        base.OnDispose();

    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateRoleHelp();
    }

    private void UpdateRoleHelp()
    {

    }
}
