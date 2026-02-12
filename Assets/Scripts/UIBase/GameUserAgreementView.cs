using Engine;
using FairyGUI;
using Login;
using UnityEngine;

public class GameUserAgreementView : UIViewBase
{
    private UI_GameUserAgreement GameUserAgreement => this.main as UI_GameUserAgreement;
    
    public GameUserAgreementView()
    {
        this.name = "Login";
        this.package = "Login";
        this.component = "GameUserAgreement";
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
        // this.GameUserAgreement.closeBtn.onClick.Add(this.Hide);
        this.GameUserAgreement.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.GameUserAgreement.txtLb.onClickLink.Add(this.OnClickLink);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.GameUserAgreement.txtLb.text = ConfigUtils.GetStringByKey(27);
    }

    private void OnClickLink(EventContext context)
    {
        string url = ((string) context.data);
        Application.OpenURL(url);
    }
}
