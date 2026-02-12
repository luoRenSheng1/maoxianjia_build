
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using LoginGift;
using Passport;
using EventDispatcher = EngineBase.EventDispatcher;

public class LoginGiftView : UIViewBase
{
    private UI_LoginGift LoginGift => this.main as UI_LoginGift;
    private ConfigLoginGiftUnit _loginGiftUnit;
    private List<ItemData> _giftItemDatas = new List<ItemData>();
    public LoginGiftView()
    {
        this.name = "LoginGift";
        this.package = "LoginGift";
        this.component = "LoginGift";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        LoginGiftBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.LoginGift.closeBtn.onClick.Add(this.Hide);
        this.LoginGift.rightPane.buyBtn.onClick.Add(this.OnClickBuyBtn);
        this.LoginGift.leftPane.rewardList.itemRenderer = RewardItemRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLoginGift);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_LOGINGIFT_UPDATE, this.UpdateLoginGift);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateLoginGift();
        this.LoginGift.img.url = UIResource.GetImageUrlWithLang("xsdfs","LoginGift");
    }

    private void UpdateLoginGift()
    {
        int loginPackId = ActivityManager.Instance.LoginGiftPackID;
        if (loginPackId == 0)//已经购买了
        {
            SetVisible(false);
            return;
        }
        _loginGiftUnit = ConfigUtils.GetLoginGiftById(loginPackId);
        // this.LoginGift.leftPane.titleLb.text = _loginGiftUnit.Name;
        this.LoginGift.leftPane.titleLb.text = ConfigUtils.GetTextById(_loginGiftUnit.Name);
        _giftItemDatas.Clear();
        string[] itemArr = _loginGiftUnit.ItemId.Split('|');
        foreach (var item in itemArr)
        {
            string[] itemStrArr = item.Split(',');
            ItemData itemData = new ItemData()
            {
                id = int.Parse(itemStrArr[0]),
                count = double.Parse(itemStrArr[1])
            };
            _giftItemDatas.Add(itemData);
        }

        this.LoginGift.leftPane.rewardList.numItems = _giftItemDatas.Count;
        //itemTypeUnit.Type 1=物品，2=装备，3角色，4宠物，5技能，6符石，7=角色碎片，8=随机宝箱99=时间道具100=货币101=家园道具
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_giftItemDatas[0].id);
        if (itemTypeUnit.Type == 3)
        {
            ConfigHeroUnit heroUnit = ConfigUtils.GetHeroById(itemTypeUnit.Param);
            // this.LoginGift.rightPane.firstItemIcon.url = UIResource.GetHeroBody(heroUnit.HeroBody);
            this.LoginGift.rightPane.rewardCtrl.selectedIndex = 0;
            Utils.SetSpineModelOnFGUI(this.LoginGift.rightPane.spine, heroUnit.Model, 100f);
        }else if (itemTypeUnit.Type == 4)
        {
            ConfigPetBasisUnit petBasis = ConfigUtils.GetPetById(itemTypeUnit.Param);
            // this.LoginGift.rightPane.firstItemIcon.url = UIResource.GetPetIcon(petBasis.IconPath);
            this.LoginGift.rightPane.rewardCtrl.selectedIndex = 0;
            Utils.SetSpineModelOnFGUI(this.LoginGift.rightPane.spine, petBasis.PetModel, 100f);
        }
        else
        {
            this.LoginGift.rightPane.rewardCtrl.selectedIndex = 1;
            this.LoginGift.rightPane.firstItemIcon.url = UIResource.GetItemUrl(itemTypeUnit.Icon);
        }
        
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_loginGiftUnit.PayListId);
        if (payListUnit != null)
        {
            ((UI_EmptyRMB) (this.LoginGift.rightPane.buyBtn)).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB) (this.LoginGift.rightPane.buyBtn)).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB) (this.LoginGift.rightPane.buyBtn)).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }
    }

    private void RewardItemRender(int index, GObject item)
    {
        ItemData itemData = _giftItemDatas[index];
        ((UI_ItemCom)((UI_RewardItem)item).item).SetItemDataWithGuid(itemData, false);
        ((UI_RewardItem)item).cntLb.SetVar("cur", StringUtils.FormatCurrency(itemData.count)).FlushVars();
    }

    private void OnClickBuyBtn()
    {
        if (_loginGiftUnit.PayListId > 0)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_loginGiftUnit.PayListId);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }


    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.LoginGift.rightPane.spine);
    }
}
