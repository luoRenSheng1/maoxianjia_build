
using CommonEx;
using Config;
using Engine;
using EngineBase;
using msg;
using Setting;

public class PlayerNameEditView : UIViewBase
{
    private UI_PlayerNameEdit PlayerNameEdit => this.main as UI_PlayerNameEdit;

    private ConfigCommonUnit _common50;
    public PlayerNameEditView()
    {
        this.name = "PlayerNameEdit";
        this.package = "Setting";
        this.component = "PlayerNameEdit";
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
        this.PlayerNameEdit.freeBtn.onClick.Add(this.OnClickFreeBtn);
        this.PlayerNameEdit.moneyBtn.onClick.Add(this.OnClickMoneyBtn);
        // this.PlayerNameEdit.closeBtn.onClick.Add(this.Hide);
        this.PlayerNameEdit.closeBtn.onClick.Add(this.HideWithSoundEffect);

        _common50 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(50);
  
        // this.PlayerNameEdit.moneyBtn.title = _common50.Param1;
        ((UI_EmptyDimandBtn) this.PlayerNameEdit.moneyBtn).diamondLb.text = _common50.Param1;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CHANGENAMECOUNTER, this.ChangePlayerNameSucc);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CHANGENAMECOUNTER, this.ChangePlayerNameSucc);
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (DataManager.Instance.GetRoleData().ChangeNameCounter > 0)
        {
            this.PlayerNameEdit.freeType.selectedIndex = 1;
        }
        else
        {
            this.PlayerNameEdit.freeType.selectedIndex = 0;
        }
    }

    private void OnClickFreeBtn()
    {
        SendToChangeName();
    }

    private void OnClickMoneyBtn()
    {
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = SendToChangeName
        };
        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.FormatStringByKey(10132, _common50.Param1), param);
    }

    private void SendToChangeName()
    {
        if (IllegalWordDetection.DetectIllegalWords(this.PlayerNameEdit.ipt.text).Count > 0)
        {
            UIManager.Instance.ToastByKey(10159);
            return;
        }
        
        if (this.PlayerNameEdit.ipt.text.Length >= 2 && this.PlayerNameEdit.ipt.text.Length <= 8)
        {
            var builder = ChangePlayerName_CS.CreateBuilder();
            builder.ChangeName = this.PlayerNameEdit.ipt.text;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ChangePlayerName_CS, builder.Build());
        }
        else
        {
            UIManager.Instance.ToastByKey(10139);
        }

    }

    private void ChangePlayerNameSucc()
    {
        if (IsShow() && IsOnStage())
        {
            this.SetVisible(false);
            UIManager.Instance.ToastByKey(10133);
        }
    }
}
