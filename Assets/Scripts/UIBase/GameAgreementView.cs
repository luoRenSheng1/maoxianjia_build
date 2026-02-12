using Engine;
using FairyGUI;
using Login;
using EventDispatcher = EngineBase.EventDispatcher;

public class GameAgreementView : UIViewBase
{
    private UI_GameAgreement Agreement => this.main as UI_GameAgreement;
    
    public GameAgreementView()
    {
        this.name = "Login";
        this.package = "Login";
        this.component = "GameAgreement";
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
        // this.Agreement.closeBtn.onClick.Add(this.Hide);
        this.Agreement.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.Agreement.agreeBtn.onClick.Add(this.OnClickAgreeBtn);
        this.Agreement.RefusedBtn.onClick.Add(this.OnClickRefuseBtn);
        this.Agreement.linkLb.onClickLink.Add(this.OnClickLinkTxt);
    }

    private void OnClickAgreeBtn()
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        this.SetVisible(false);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_PLAYER_User_check, true);
    }

    private void OnClickLinkTxt(EventContext context)
    {
        GRichTextField t = context.sender as GRichTextField;
        string[] eventData = ((string) context.data).Split(":");
        if (eventData[1] == "userTips")
        {
            UIManager.Instance.ShowUIPanel("GameUserAgreement");
        }else if (eventData[1] == "PrivacyTips")
        {
            UIManager.Instance.ShowUIPanel("PrivateAgreement");
        }
    }

    private void OnClickRefuseBtn()
    {
        // GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        this.SetVisible(false);
        EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_LOGIN_PLAYER_User_check, false);
    }
}
