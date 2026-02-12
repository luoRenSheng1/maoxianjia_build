using Engine;
using FairyGUI;
using Login;
using UnityEngine;

public class PrivateAgreementView : UIViewBase
{
    private UI_PrivateAgreement PrivateAgreement => this.main as UI_PrivateAgreement;
    
    public PrivateAgreementView()
    {
        this.name = "Login";
        this.package = "Login";
        this.component = "PrivateAgreement";
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
        // this.PrivateAgreement.closeBtn.onClick.Add(this.Hide);
        this.PrivateAgreement.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.PrivateAgreement.txtLb.onClickLink.Add(this.OnClickLink);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.PrivateAgreement.txtLb.text = ConfigUtils.GetStringByKey(28);
    }

    private void OnClickLink(EventContext context)
    {
        string url = ((string) context.data);
        Application.OpenURL(url);
    }
}
