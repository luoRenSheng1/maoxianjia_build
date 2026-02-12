
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;
using Summon;
using EventDispatcher = EngineBase.EventDispatcher;

public class SpecialCardView : UIViewBase
{
    private UI_SpecialCard SpecialCard => this.main as UI_SpecialCard;
    private ConfigMonthlyUnit _monthly;
    
    public SpecialCardView()
    {
        this.name = "SpecialCard";
        this.package = "Summon";
        this.component = "SpecialCard";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SummonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.SpecialCard.closeBtn.onClick.Add(this.Hide);
        this.SpecialCard.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.SpecialCard.buyBtn.onClick.Add(this.OnClickBuyBtn);
        this.SpecialCard.getBtn.onClick.Add(this.OnClickGetBtn);
        _monthly = ConfigUtils.GetMonthlyUnit(1004);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS, this.UpdateHeroMonthInfo);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS, this.UpdateHeroMonthInfo);
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (_monthly.PayListID > 0)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_monthly.PayListID);
            if (payListUnit != null)
            { 
                ((UI_EmptyRMB) (this.SpecialCard.buyBtn)).moneyType.selectedIndex = payListUnit.MoneyType-1;
                double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
                ((UI_EmptyRMB) (this.SpecialCard.buyBtn)).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
                ((UI_EmptyRMB) (this.SpecialCard.buyBtn)).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
            }

            string[] itemArr = _monthly.ItemId.Split(',');
            ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(itemArr[0]));
            ((UI_ItemCom) this.SpecialCard.item0.item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
            ((UI_ItemCom) this.SpecialCard.item0.item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
            // this.SpecialCard.item0.nameLb.SetVar("name", itemTypeUnit.Name).SetVar("value", itemArr[1]).FlushVars();
            this.SpecialCard.item0.nameLb.SetVar("name", ConfigUtils.GetTextById(itemTypeUnit.Name)).SetVar("value", itemArr[1]).FlushVars();
            
            string[] dailyItemArr = _monthly.DailyItemId.Split(',');
            ConfigItemTypeUnit itemTypeUnit2 = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(dailyItemArr[0]));
            ((UI_ItemCom) this.SpecialCard.item1.item).ctrlQuality.selectedIndex = itemTypeUnit2.Quality - 1;
            ((UI_ItemCom) this.SpecialCard.item1.item).icon = UIResource.GetItemUrl(itemTypeUnit2.Icon);
            // this.SpecialCard.item1.nameLb.SetVar("name", itemTypeUnit2.Name).SetVar("value", dailyItemArr[1]).FlushVars();
            this.SpecialCard.item1.nameLb.SetVar("name", ConfigUtils.GetTextById(itemTypeUnit2.Name)).SetVar("value", dailyItemArr[1]).FlushVars();
        }

        UpdateHeroMonthInfo();
    }

    private void OnClickBuyBtn(EventContext context)
    {
        
        //PayListId !=0 充值
        if (_monthly.PayListID > 0)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_monthly.PayListID);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }

    private void UpdateHeroMonthInfo()
    {
        this.SpecialCard.ctrl.selectedIndex = ActivityManager.Instance.hasPurchaseMonth1004 ? 1 : 0;
        this.SpecialCard.getCtrl.selectedIndex = ActivityManager.Instance.GetTodayReward1004 ? 1: 0;

        int totalSecond =  (int) (ActivityManager.Instance.Month1004_EndTime - ServerTimeManager.Instance.CurServerTime);
        if (totalSecond > 0)
        {
            this.SpecialCard.timeLb.SetVar("value", StringUtils.GetTimeString2(totalSecond)).FlushVars();
        }
        else
        {
            this.SpecialCard.timeLb.SetVar("value", "15").FlushVars();
            this.SpecialCard.ctrl.selectedIndex = 0;
        }
    }
    

    private void OnClickGetBtn()
    {
        var builder = DailyClaimHeroMonthAward_CS.CreateBuilder();
        builder.ActivityId = 1004;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DailyClaimHeroMonthAward_CS, builder.Build());
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        int totalSecond =  (int) (ActivityManager.Instance.Month1004_EndTime - ServerTimeManager.Instance.CurServerTime);
        if (totalSecond > 0)
        {
            this.SpecialCard.timeLb.SetVar("value", StringUtils.GetTimeString2(totalSecond)).FlushVars();
        }
        else
        {
            this.SpecialCard.timeLb.SetVar("value", "15").FlushVars();
            this.SpecialCard.ctrl.selectedIndex = 0;
        }
    }
}
