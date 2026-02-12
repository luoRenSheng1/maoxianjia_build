
using Config;
using Engine;
using EngineBase;
using Passport;

public class PassportDetailView : UIViewBase
{
    private UI_PassportDetail PassportDetail => this.main as UI_PassportDetail;
    private ConfigCommonUnit _commonUnit700;
    
    public PassportDetailView()
    {
        this.name = "PassportDetail";
        this.package = "Passport";
        this.component = "PassportDetail";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PassportBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        _commonUnit700 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(700);
        // this.PassportDetail.closeBtn.onClick.Add(this.Hide);
        this.PassportDetail.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.PassportDetail.buyBtn.onClick.Add(this.OnClickBuyBtn);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.PassportDetail.buyBtn.visible = ActivityManager.Instance.GetPassPortInfo().UnlockTier == 0;
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(int.Parse(_commonUnit700.Param3));
        if (payListUnit != null)
        {
            ((UI_AdvanccBuyBtn) this.PassportDetail.buyBtn).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_AdvanccBuyBtn) this.PassportDetail.buyBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_AdvanccBuyBtn) this.PassportDetail.buyBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }
    }

    private void OnClickBuyBtn()
    {
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(int.Parse(_commonUnit700.Param3));
        if (payListUnit != null)
        {
            UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
        }
        
    }

    private void OnUpdateUnlockAdvancePassport()
    {
        this.PassportDetail.buyBtn.visible = ActivityManager.Instance.GetPassPortInfo().UnlockTier == 0;
    }
}
