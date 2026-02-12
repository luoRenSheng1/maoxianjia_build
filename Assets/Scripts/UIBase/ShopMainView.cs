
using System.Collections.Generic;
using System.Linq;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Shop;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;
using UI_TabBtn = Shop.UI_TabBtn;


public enum GiftType
{
    None,
    Daily,//每日特惠
    GiftPack,//礼包
    CrazyGuide,// 疯狂指南
}

public class CardAttrVo
{
    public int CardType;
    public int AttrId;
    public int AttrValue;
}
public class ShopMainView : UIViewBase
{
    private UI_ShopMain ShopMain => this.main as UI_ShopMain;

    private List<ConfigGiftUnit> _dailyGifts;

    private List<ConfigContinuousSaveUnit> _continuousUnits;
    private int _tthlScrollIndex = -1;

    private List<ConfigGuideRewardUnit> _guideRewardUnits;
    private List<ConfigGuideRewardUnit> _currentChapterUnits = new List<ConfigGuideRewardUnit>();
    private int _fkznScrollIndex = -1;

    private List<int> _monthlyIdList = new List<int>() {1001, 1002, 1003};
    private ConfigCommonUnit _common800001;
    private ConfigCommonUnit _common800002;
    private ConfigCommonUnit _common800003;

    private Dictionary<int, int> continuousSaveDict;
    private int waitDay;

    private List<uint> buyIdList;
    private List<uint> getIdList;
    public ShopMainView()
    {
        this.name = "ShopMain";
        this.package = "Shop";
        this.component = "ShopMain";
        this.removePackage = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ShopBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.ShopMain.tabCtrl.onChanged.Add(this.ChangeIndex);
        this.ShopMain.tabCtrl.selectedIndex = 0;
        // this.ShopMain.closeBtn.onClick.Add(this.Hide);
        this.ShopMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.ShopMain.dailyTH.dailyList.itemRenderer = DailyTHItemRender;
        _dailyGifts = ConfigUtils.GetGiftUnitsByType(GiftType.Daily);
        
        this.ShopMain.dailyTH.tipsBtn.onClick.Add(this.OnClickDailyHelpBtn);
        this.ShopMain.tthlTH.tipsBtn.onClick.Add(this.OnClickTTHLHelpBtn);
        this.ShopMain.fkTH.tipsBtn.onClick.Add(this.OnClickTQKHelpBtn);
        
        this.ShopMain.tthlTH.ttList.itemRenderer = TTHLTHItemRender;
        _continuousUnits = ConfigDataGroup.GetInstance<ConfigContinuousSave>().Data.Values.ToList();
        
        this.ShopMain.fkTH.itemList.itemRenderer = FKItemReder;
        this.ShopMain.fkTH.itemList.SetVirtual();
        _guideRewardUnits = ActivityManager.Instance.GuideRewardUnits();
        this.ShopMain.fkTH.titleCtrl.onChanged.Add(OnChapterChanged);
        this.ShopMain.fkznBtn.onClick.Add(this.OnClickFKPay);
        this.ShopMain.fkznBtn2.onClick.Add(this.OnClickFKPay2);
        this.ShopMain.fkznBtn3.onClick.Add(this.OnClickFKPay3);
        buyIdList = ActivityManager.Instance.BuyIdsInfo();
        getIdList = ActivityManager.Instance.GetIdsInfo();

        this.ShopMain.tqTH.tqList.itemRenderer = TQTHItemRender;
        _common800001 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800001);
        _common800002 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800002);
        _common800003 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800003);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS, this.OnUpdateCardActivity);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.UpdateCrazyGuideSC);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.VisibleBtn);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateRechargeGift);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_FREE_GETITEM, this.UpdateFreeItem);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_DAILY_RESET_UPDATE, this.UpdateDailyReset);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateShopReddot);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CARD_ACTIVITY_SUCCESS, this.OnUpdateCardActivity);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.UpdateCrazyGuideSC);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.VisibleBtn);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateRechargeGift);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_FREE_GETITEM, this.UpdateFreeItem);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_DAILY_RESET_UPDATE, this.UpdateDailyReset);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_SHOP_REDDOT, this.UpdateShopReddot);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.ShopMain.bg.url = UIResource.GetShopUrl("n1");
        this.ShopMain.dailyTH.bgUrl.url = UIResource.GetImageUrlWithLang("dailyTH","Shop");
        this.ShopMain.tthlTH.bgUrl.url = UIResource.GetImageUrlWithLang("tthl","Shop");
        this.ShopMain.fkTH.bgUrl.url = UIResource.GetImageUrlWithLang("fkzn","Shop");
        this.ShopMain.tqTH.bgUrl.url = UIResource.GetImageUrlWithLang("tq","Shop");
        ChangeIndex();
        UpdateShopReddot();

        ConfigHeroUnit heroUnit = ConfigUtils.GetHeroById(HeroInfoManager.Instance.GetHeroFirstID());
        Utils.SetSpineModelOnFGUI(this.ShopMain.spine, heroUnit.Model, 140f);
    }

    private void OnClickDailyHelpBtn()
    {
        UIManager.Instance.ShowUIPanel("Help", HelpType.Help_meirith);
    }

    private void OnClickTTHLHelpBtn()
    {
        UIManager.Instance.ShowUIPanel("Help", HelpType.Help_tthl);
    }
    
    private void OnClickTQKHelpBtn()
    {
        UIManager.Instance.ShowUIPanel("Help", HelpType.Help_fkzn);
    }

    private void ChangeIndex()
    {
        if (IsShow() && IsOnStage())
        {
            int index = this.ShopMain.tabCtrl.selectedIndex;
            switch (index)
            {
                case 0:
                    UpdateDailyGift();
                    break;
                case 1:
                    UpdateContinueRecharge();
                    ActivityManager.Instance.SendToGetDailyAwardInfoCS();
                    break;
                case 2:
                    UpdateCrazyGuide();
                    VisibleBtn();
                    ActivityManager.Instance.SendToGetGuideAwardInfoCS();
                    break;
                case 3:
                    UpdateTqInfo();
                    break;
            }
        }
        
    }

    private void UpdateDailyReset()
    {
        if (IsShow() && IsOnStage())
        {
            int index = this.ShopMain.tabCtrl.selectedIndex;
            switch (index)
            {
                case 0:
                    UpdateDailyGift();
                    break;
                case 1:
                    UpdateContinueRecharge();
                    break;
                case 2:
                    UpdateCrazyGuide();
                    VisibleBtn();
                    break;
                case 3:
                    UpdateTqInfo();
                    break;
            }
            UpdateShopReddot();
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.ShopMain.dailyTH.bgUrl.url = null;
        this.ShopMain.tthlTH.bgUrl.url = null;UIResource.GetImageUrlWithLang("tthl","Shop");
        this.ShopMain.fkTH.bgUrl.url = null; UIResource.GetImageUrlWithLang("fkzn","Shop");
        this.ShopMain.tqTH.bgUrl.url = null;UIResource.GetImageUrlWithLang("tq","Shop");
    }

    private (int, int) GetFreeItem(ConfigCommonUnit common)
    {
        string[] rewardArr = common.Param1.Split(',');
        return (int.Parse(rewardArr[0]), int.Parse(rewardArr[1]));
    }

    private void UpdateRechargeGift()
    {
        if (this.ShopMain.tabCtrl.selectedIndex == 0)
            UpdateDailyGift();
        UpdateDailyReset();
    }

    private void UpdateShopReddot()
    {
        VisibleBtn();
        bool dailyReddot = ActivityManager.Instance.Shop_FreeDailyPack == 0;
        if (!dailyReddot)
        {
            foreach (var item in _dailyGifts)
            {
                LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(item.Id);
                if (limitPackVo.BuyCounter == 0 || limitPackVo.EndTime < ServerTimeManager.Instance.CurServerTime)
                {
                    dailyReddot = false;
                }
                else
                {
                    bool todayGet = limitPackVo.GetAwardCounter > 0 &&
                                    limitPackVo.LastGetAwardTime < ServerTimeManager.Instance.GetNextToZeroServerTime();
                    dailyReddot = !todayGet;
                }
                if(dailyReddot) break;
            }
        }
        ((UI_TabBtn) this.ShopMain.tabList.GetChildAt(0)).redCtrl.selectedIndex = dailyReddot ? 1 : 0;

        var _continuousSaveDict = ActivityManager.Instance.GetContinuousSaveDict();
        bool tthlReddot = ActivityManager.Instance.Shop_FreeTTHLPack == 0;
        if (!tthlReddot)
        {
            if (_continuousSaveDict[1] == 0)
            {
                tthlReddot = false;
            }
            else
            {
                foreach (var item in _continuousUnits)
                {
                    int status = _continuousSaveDict[item.Day];
                    if (status == 1)
                    {
                        tthlReddot = true;
                        break;
                    }
                }

            }
        }
        ((UI_TabBtn) this.ShopMain.tabList.GetChildAt(1)).redCtrl.selectedIndex = tthlReddot ? 1 : 0;

        bool tqkReddot = ActivityManager.Instance.Shop_FreeTQKPack == 0;
        if (!tqkReddot)
        {
            foreach (var item in _monthlyIdList)
            {
                ConfigMonthlyUnit monthlyUnit = ConfigUtils.GetMonthlyUnit(item);
                if (monthlyUnit.Id == (int) CardType.Permanent)
                {
                    bool isBuy = ActivityManager.Instance.IsBuyPermanent == 1;
                    bool isGet = ActivityManager.Instance.HasGetPermanentTodayReward >= 1;
                    tqkReddot = isBuy && (!isGet ? true : false);
                }
                else
                {
                    bool isBuy = ActivityManager.Instance.GetCardPurchase((CardType) monthlyUnit.Id);
                    int totalSecond =  (int) (ActivityManager.Instance.GetCardPurchaseEndTime((CardType) monthlyUnit.Type) - ServerTimeManager.Instance.CurServerTime);
                    bool isGet = ActivityManager.Instance.GetCardTodayGet((CardType) monthlyUnit.Id);
                    tqkReddot = isBuy && (!isGet ? true : false);
                }
                if(tqkReddot) break;
            }
        }
        ((UI_TabBtn) this.ShopMain.tabList.GetChildAt(3)).redCtrl.selectedIndex = tqkReddot ? 1 : 0;
    }

    private void UpdateFreeItem()
    {
        UpdateDailyReset();
    }

    #region 每日特惠

    private void UpdateDailyGift()
    {
        (int, int) freeReward = GetFreeItem(_common800001);
        ((UI_ItemCom)((UI_fengkuangItemCom)this.ShopMain.dailyTH.freeItem).itemCom).SetItemDataWithGuid(new ItemData(freeReward.Item1, freeReward.Item2), true);
        this.ShopMain.dailyTH.freeItem.status.selectedIndex = ActivityManager.Instance.Shop_FreeDailyPack == 0 ? 1 : 2;
        this.ShopMain.dailyTH.freeItem.getBtn.onClick.Add(this.OnClickGetFreeDaily);
        
        string[] reward60Arr = _dailyGifts[0].ItemId.Split(',');
        ((UI_ItemCom)this.ShopMain.dailyTH.rw60Item.item).SetItemDataWithGuid(new ItemData(int.Parse(reward60Arr[0]), int.Parse(reward60Arr[1])), true);
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_dailyGifts[0].PayList);
        if (payListUnit != null)
        { 
            ((UI_EmptyRMB) (this.ShopMain.dailyTH.rw60Item.buyBtn)).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB) (this.ShopMain.dailyTH.rw60Item.buyBtn)).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB) (this.ShopMain.dailyTH.rw60Item.buyBtn)).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }
        (this.ShopMain.dailyTH.rw60Item.buyBtn).data = _dailyGifts[0];
        // this.ShopMain.dailyTH.rw60Item.titlelb.text = _dailyGifts[0].Name;
        this.ShopMain.dailyTH.rw60Item.titlelb.text = ConfigUtils.GetTextById(_dailyGifts[0].Name,_dailyGifts[0].NameParam);
        (this.ShopMain.dailyTH.rw60Item.buyBtn).onClick.Add(this.OnClickDailyTHBuyBtn);
        (this.ShopMain.dailyTH.rw60Item.getRwBtn).data = _dailyGifts[0];
        (this.ShopMain.dailyTH.rw60Item.getRwBtn).onClick.Add(this.OnClickDailyTHGetRewardBtn);
        LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(_dailyGifts[0].Id);
        this.ShopMain.dailyTH.rw60Item.limitLb.SetVar("cur", limitPackVo.BuyCounter.ToString()).SetVar("total", _dailyGifts[0].BuyNumber.ToString()).FlushVars();

        if (limitPackVo.BuyCounter == 0 || limitPackVo.EndTime < ServerTimeManager.Instance.CurServerTime)
        {
            this.ShopMain.dailyTH.rw60Item.rewardCtrl.selectedIndex = 0;
        }
        else
        {
            bool todayGet = limitPackVo.GetAwardCounter > 0 &&
                            limitPackVo.LastGetAwardTime < ServerTimeManager.Instance.GetNextToZeroServerTime();
            this.ShopMain.dailyTH.rw60Item.rewardCtrl.selectedIndex = todayGet ? 2 : 1;
        }
        
        this.ShopMain.dailyTH.dailyList.numItems = _dailyGifts.Count - 1;
    }
    private void DailyTHItemRender(int index, GObject item)
    {
        ConfigGiftUnit dailyUnit = _dailyGifts[index + 1];
        // ((UI_dailyTHItem) item).dailyTitle.text = dailyUnit.Name;
        ((UI_dailyTHItem) item).dailyTitle.text = ConfigUtils.GetTextById(dailyUnit.Name,dailyUnit.NameParam);
        string[] rewardArr = dailyUnit.ItemId.Split("|");
        List<ItemData> rewardItemList = new List<ItemData>();
        for (int i = 0; i < rewardArr.Length; i++)
        {
            string[] oneRewardArr = rewardArr[i].Split(",");
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(oneRewardArr[0]);
            itemData.count = double.Parse(oneRewardArr[1]);
            rewardItemList.Add(itemData);
        }

        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(dailyUnit.PayList);
        if (payListUnit != null)
        { 
            ((UI_EmptyRMB) ((UI_dailyTHItem)item).buyBtn).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB) ((UI_dailyTHItem)item).buyBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB) ((UI_dailyTHItem)item).buyBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }
        
        LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum(dailyUnit.Id);
        ((UI_dailyTHItem) item).limitLb.SetVar("cur", limitPackVo.BuyCounter.ToString()).SetVar("total", dailyUnit.BuyNumber.ToString()).FlushVars();
        
        if (limitPackVo.BuyCounter == 0 || limitPackVo.EndTime < ServerTimeManager.Instance.CurServerTime)
        {
            ((UI_dailyTHItem) item).rewardCtrl.selectedIndex = 0;
        }
        else
        {
            bool todayGet = limitPackVo.GetAwardCounter > 0 &&
                            limitPackVo.LastGetAwardTime < ServerTimeManager.Instance.GetNextToZeroServerTime();
            ((UI_dailyTHItem) item).rewardCtrl.selectedIndex = todayGet ? 2 : 1;
        }
        
        ((UI_dailyTHItem) item).rewardList.itemRenderer = this.OnRewardItemListRender;
        ((UI_dailyTHItem) item).rewardList.data = rewardItemList;
        ((UI_dailyTHItem) item).rewardList.numItems = rewardItemList.Count;
        ((UI_dailyTHItem) item).buyBtn.data = dailyUnit;
        ((UI_dailyTHItem) item).buyBtn.onClick.Add(this.OnClickDailyTHBuyBtn);
        ((UI_dailyTHItem) item).getRwBtn.data = dailyUnit;
        ((UI_dailyTHItem) item).getRwBtn.onClick.Add(this.OnClickDailyTHGetRewardBtn);
    }

    private void OnClickGetFreeDaily()
    {
        var builder = ClaimMallFreeAward_CS.CreateBuilder();
        builder.MallFunc = ePlayerAttrID.ePlayerAttrID_FreeMallDailyPack;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimMallFreeAward_CS, builder.Build());
    }

    private void OnClickDailyTHBuyBtn(EventContext context)
    {
        ConfigGiftUnit dailyUnit = (context.sender as GButton).data as ConfigGiftUnit;
        if (dailyUnit != null)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(dailyUnit.PayList);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }

    private void OnClickDailyTHGetRewardBtn(EventContext context)
    {
        ConfigGiftUnit dailyUnit = (context.sender as GButton).data as ConfigGiftUnit;
        if (dailyUnit != null)
        {
            var builder = ClaimGiftAward_CS.CreateBuilder();
            builder.GiftPackId = dailyUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimGiftAward_CS, builder.Build());
        }
    }
    
    private void OnRewardItemListRender(int index, GObject item)
    {
        List<ItemData> rewardItemList = item.parent.data as List<ItemData>;
        ((UI_ItemCom)item).SetItemDataWithGuid(rewardItemList[index], true);
    }

    #endregion

    #region 天天好礼

    private void UpdateContinueRecharge()
    {
        (int, int) freeReward = GetFreeItem(_common800002);
        ((UI_ItemCom)this.ShopMain.tthlTH.freeItem.itemCom).SetItemDataWithGuid(new ItemData(freeReward.Item1, freeReward.Item2), true);
        this.ShopMain.tthlTH.freeItem.status.selectedIndex = ActivityManager.Instance.Shop_FreeTTHLPack == 0 ? 1 : 2;
        this.ShopMain.tthlTH.freeItem.getBtn.onClick.Add(this.OnClickGetFreeTTHL);
        
        continuousSaveDict = ActivityManager.Instance.GetContinuousSaveDict();
        waitDay = ActivityManager.Instance.GetDay();
        this.ShopMain.tthlTH.ttList.numItems = _continuousUnits.Count;

        for (int i = 1; i <= continuousSaveDict.Count; i++)
        {
            if (continuousSaveDict[i] == 1) // 1 表示未领取
            {
                _tthlScrollIndex = i - 1;
                break;
            }
            else if (continuousSaveDict[i] == 0) // 0 表示未充值，且还未找到未领取的项
            {
                _tthlScrollIndex = i - 1;
                break;
            }
        }
        
        this.ShopMain.tthlTH.ttList.ScrollToView(Mathf.Max(0,_tthlScrollIndex), true, true);
    }
    
    private void TTHLTHItemRender(int index, GObject item)
    {
        ConfigContinuousSaveUnit continuousUnit = _continuousUnits[index];

        int rechargeDay = 0;//充值天数
        
        int day = continuousUnit.Day;

        if (continuousSaveDict[1] == 0)
        {
            ((UI_tthlTHItem)item).rewardCtrl.selectedIndex = 0;
            ((UI_tthlTHItem)item).reachCtrl.selectedIndex = 0; // 未达成
        }
        else
        {
            int status = continuousSaveDict[day];
            if (status == 2)
            {
                ((UI_tthlTHItem)item).rewardCtrl.selectedIndex = 2; // 已领取
                ((UI_tthlTHItem) item).reachCtrl.selectedIndex = 2;
            }
            else if (status == 1)
            {
                ((UI_tthlTHItem)item).rewardCtrl.selectedIndex = 1; // 已达成，未领取
                ((UI_tthlTHItem) item).reachCtrl.selectedIndex = 2;
            }
            else
            {
                ((UI_tthlTHItem)item).rewardCtrl.selectedIndex = 0; // 未达成
                foreach (int key in continuousSaveDict.Keys.ToList())
                {
                    if (continuousSaveDict[key] == 0)
                    {
                        rechargeDay = key;
                        break;
                    }
                }
                ((UI_tthlTHItem) item).reachCtrl.selectedIndex = 1;
                ((UI_tthlTHItem) item).dayTitle.SetVar("cur", (rechargeDay-1).ToString()).SetVar("total", day.ToString()).FlushVars();
                if (rechargeDay == 0)
                {
                    ((UI_tthlTHItem) item).reachCtrl.selectedIndex = 0;
                }

                if (waitDay != 0 && (waitDay + rechargeDay-1) > index)
                {
                    ((UI_tthlTHItem) item).rewardCtrl.selectedIndex = 3; // 待领取
                    // ((UI_tthlTHItem) item).reachCtrl.selectedIndex = 2;
                }
            }
        }

        // ((UI_tthlTHItem) item).ttTitle.text = continuousUnit.Name;
        ((UI_tthlTHItem) item).ttTitle.text = ConfigUtils.GetTextById(continuousUnit.Name,continuousUnit.NameParam);
        string[] rewardArr = continuousUnit.ItemId.Split("|");
        List<ItemData> rewardItemList = new List<ItemData>();
        for (int i = 0; i < rewardArr.Length; i++)
        {
            string[] oneRewardArr = rewardArr[i].Split(",");
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(oneRewardArr[0]);
            itemData.count = double.Parse(oneRewardArr[1]);
            rewardItemList.Add(itemData);
        }
        ((UI_tthlTHItem) item).rewardList.itemRenderer = this.OnRewardItemListRender;
        ((UI_tthlTHItem) item).rewardList.data = rewardItemList;
        ((UI_tthlTHItem) item).rewardList.numItems = rewardItemList.Count;
        
        ((UI_tthlTHItem) item).gotoBtn.onClick.Add(this.OnClickTTHLGoBtn);
        ((UI_tthlTHItem) item).getRwBtn.data = continuousUnit;
        ((UI_tthlTHItem) item).getRwBtn.onClick.Add(this.OnClickTTTHGetRewardBtn);
    }

    private void OnClickTTHLGoBtn()
    {
        this.ShopMain.tabCtrl.selectedIndex = 0;
    }
    
    private void OnClickTTTHGetRewardBtn(EventContext context)
    {
        ConfigContinuousSaveUnit saveUnit = (context.sender as GButton).data as ConfigContinuousSaveUnit;
        if (saveUnit != null)
        {
            // 领取天天好礼奖励
            // todo 客户端也要判断必须小于等于  recharge_days_of_dailyaward
            var builder = ClaimDailyAwardOfRechargeAward_CS.CreateBuilder();
            builder.DayId = saveUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimDailyAwardOfRechargeAward_CS, builder.Build());
        }
    }

    private void OnClickGetFreeTTHL()
    {
        var builder = ClaimMallFreeAward_CS.CreateBuilder();
        builder.MallFunc = ePlayerAttrID.ePlayerAttrID_FreeMallSignPack;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimMallFreeAward_CS, builder.Build());
    }
    
    #endregion

    #region 疯狂指南

    private void UpdateCrazyGuide()
    {
        buyIdList = ActivityManager.Instance.BuyIdsInfo();
        getIdList = ActivityManager.Instance.GetIdsInfo();
        
        int selectedChapterId = this.ShopMain.fkTH.titleCtrl.selectedIndex + 1;
        _currentChapterUnits = _guideRewardUnits.Where(unit => unit.ChapterId == selectedChapterId).ToList();
        this.ShopMain.fkTH.itemList.numItems = _currentChapterUnits.Count;
        this.ShopMain.fkTH.itemList.RefreshVirtualList(); // 强制刷新虚拟列表
        
        if (selectedChapterId == 1)
        {
            this.ShopMain.fkznBtnCtrl.selectedIndex = 0;
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3006);
            this.ShopMain.fkznBtn.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            this.ShopMain.fkznBtn.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            this.ShopMain.fkznBtn.moneyLb2.SetVar("value",price.ToString()).FlushVars();
        }
        else if (selectedChapterId == 2)
        {
            this.ShopMain.fkznBtnCtrl.selectedIndex = 1;
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3007);
            this.ShopMain.fkznBtn2.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            this.ShopMain.fkznBtn2.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            this.ShopMain.fkznBtn2.moneyLb2.SetVar("value",price.ToString()).FlushVars();
        }
        else
        {
            this.ShopMain.fkznBtnCtrl.selectedIndex = 2;
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3008);
            this.ShopMain.fkznBtn3.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            this.ShopMain.fkznBtn3.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            this.ShopMain.fkznBtn3.moneyLb2.SetVar("value",price.ToString()).FlushVars();
        }

        _fkznScrollIndex = _currentChapterUnits.Count - 1;
        for (int i = 0; i < _currentChapterUnits.Count; i++)
        {
            if (DataManager.Instance.GetRoleData().latestPassedStageId > _currentChapterUnits[i].Levelid)
            {
                // 有无购买高级疯狂指南，解锁条件为配置表GuideReward中的type
                // 解锁类型为自动解锁：1
                if (_currentChapterUnits[i].Type == 1)
                {
                    // 已领取
                    if (!getIdList.Contains((uint)_currentChapterUnits[i].Levelid))
                    {
                        _fkznScrollIndex = i;
                        break;
                    }
                }
                // 解锁类型为购买高级指南解锁：2
                else
                {
                    // 已购买高级指南
                    if (buyIdList.Contains((uint)_currentChapterUnits[i].ChapterId))
                    {
                        // 已领取
                        if (!getIdList.Contains((uint)_currentChapterUnits[i].Levelid))
                        {
                            _fkznScrollIndex = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                _fkznScrollIndex = i - 1;
                break;
            }
        }
        this.ShopMain.fkTH.itemList.ScrollToView(Mathf.Max(0, _fkznScrollIndex), true, true);
    }

    private void UpdateCrazyGuideSC()
    {
        UpdateCrazyGuide();
    }
    private void FKItemReder(int index, GObject item)
    {
        var guideRewardUnit = _currentChapterUnits[index];

        if (guideRewardUnit != null)
        {
            var stageUnit = ConfigUtils.GetStageUnitById(guideRewardUnit.Levelid).FirstOrDefault();
            if (stageUnit != null)
            {
                ((UI_fengkuangItem) item).num.text = (index + 1).ToString();
                // ((UI_fengkuangItem)item).title.SetVar("value", ConfigUtils.GetTextById(stageUnit.Name,stageUnit.NameParam).ToString()).FlushVars();
                ((UI_fengkuangItem)item).title.SetVar("value", string.Format(ConfigUtils.GetTextById(stageUnit.Name), stageUnit.Chapter, stageUnit.LevelId % 100)).FlushVars();
                
                string[] rewardItemList = guideRewardUnit.Reward.Split(',');
                var itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardItemList[0]));
                if (itemTypeUnit != null)
                {
                    ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
                    ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).txtLv.text = rewardItemList[1];
                    ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
                
                    // 玩家已解锁目标关卡
                    if (DataManager.Instance.GetRoleData().latestPassedStageId >= stageUnit.LevelId)
                    {
                        ((UI_fengkuangItem) item).reachCtrl.selectedIndex = 0;
                        // 有无购买高级疯狂指南，解锁条件为配置表GuideReward中的type
                        // 解锁类型为自动解锁：1
                        if (guideRewardUnit.Type == 1)
                        {
                            // 已领取
                            if (getIdList.Contains((uint)guideRewardUnit.Levelid))
                            {
                                ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 2;
                                ((UI_fengkuangItem) item).fkItemCom.itemCom.data = int.Parse(rewardItemList[0]);
                                ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).onClick.Set(OnItemTips);
                            }
                            else
                            {
                                ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 1;
                                // 点击奖励时触发领取事件，不展示tips
                                ((UI_fengkuangItem) item).fkItemCom.getBtn.data = guideRewardUnit;
                                ((UI_fengkuangItem) item).fkItemCom.getBtn.onClick.Set(this.OnClickGetFKReward);
                            }
                        }
                        // 解锁类型为购买高级指南解锁：2
                        else
                        {
                            // 已购买高级指南
                            if (buyIdList.Contains((uint)guideRewardUnit.ChapterId))
                            {
                                // 已领取
                                if (getIdList.Contains((uint)guideRewardUnit.Levelid))
                                {
                                    ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 2;
                                    ((UI_fengkuangItem) item).fkItemCom.itemCom.data = int.Parse(rewardItemList[0]);
                                    ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).onClick.Set(OnItemTips);
                                }
                                else
                                {
                                    ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 1;
                                    // 点击奖励时触发领取事件，不展示tips
                                    ((UI_fengkuangItem) item).fkItemCom.getBtn.data = guideRewardUnit;
                                    ((UI_fengkuangItem) item).fkItemCom.getBtn.onClick.Set(this.OnClickGetFKReward);
                                }
                            }
                            else
                            {
                                ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 0;

                                ((UI_fengkuangItem) item).fkItemCom.itemCom.data = guideRewardUnit;
                                ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).onClick.Set(this.GoToBuy);
                            }
                        }
                    }
                    else
                    {
                        ((UI_fengkuangItem) item).reachCtrl.selectedIndex = 1;
                        if (guideRewardUnit.Type == 2 && !buyIdList.Contains((uint)guideRewardUnit.ChapterId))
                        {
                            ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 0;
                            
                            ((UI_fengkuangItem) item).fkItemCom.itemCom.data = guideRewardUnit;
                            ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).onClick.Set(this.GoToBuy);
                        }
                        else
                        {
                            ((UI_fengkuangItemCom) ((UI_fengkuangItem) item).fkItemCom).status.selectedIndex = 0;
                            ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).data = int.Parse(rewardItemList[0]);
                            ((UI_ItemCom)((UI_fengkuangItem)item).fkItemCom.itemCom).onClick.Set(OnItemTips);
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("数据不存在:stageid = {0}", guideRewardUnit.Levelid);
            }
        }
    }

    // 领取疯狂指南奖励
    private void OnClickGetFKReward(EventContext context)
    {
        ConfigGuideRewardUnit guideUnit = (context.sender as GButton).data as ConfigGuideRewardUnit;
        if (guideUnit != null)
        {
            // 领取疯狂指南奖励
            var builder = ClaimStageGuideAward_CS.CreateBuilder();
            builder.StageId = guideUnit.Levelid;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimStageGuideAward_CS, builder.Build());
        }
    }

    private void OnItemTips(EventContext context)
    {
        int itemId = (int)((UI_ItemCom)context.sender).data;
        if (itemId != 0)
        {
            TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
        }
    }

    private void GoToBuy(EventContext context)
    {
        ConfigGuideRewardUnit guideRewardUnit = ((GButton) context.sender).data as ConfigGuideRewardUnit;
        if (guideRewardUnit != null)
        {
            UIManager.Instance.ShowUIPanel("ShopAdvancedFK", guideRewardUnit.ChapterId);
        }
    }

    private void OnChapterChanged()
    {
        UpdateCrazyGuide();
    }

    private void OnClickFKPay()
    {
        UIManager.Instance.ShowUIPanel("ShopAdvancedFK",1);
    }

    private void OnClickFKPay2()
    {
        UIManager.Instance.ShowUIPanel("ShopAdvancedFK",2);
    }
    
    private void OnClickFKPay3()
    {
        UIManager.Instance.ShowUIPanel("ShopAdvancedFK",3);
    }
    
    // 购买疯狂指南后，隐藏相应的按钮
    public void VisibleBtn()
    {
        if (buyIdList.Contains(1))
        {
            this.ShopMain.fkznBtn.visible = false;
        }
        
        if (buyIdList.Contains(2))
        {
            this.ShopMain.fkznBtn2.visible = false;
        }
        
        if (buyIdList.Contains(3))
        {
            this.ShopMain.fkznBtn3.visible = false;
        }

        UpdateFkznReddot(1);
        UpdateFkznReddot(2);
        UpdateFkznReddot(3);
        
        ((UI_TabBtn) this.ShopMain.tabList.GetChildAt(2)).redCtrl.selectedIndex =
            (UpdateFkznReddot(1) || UpdateFkznReddot(2) || UpdateFkznReddot(3)) ? 1 : 0;
    }

    private bool UpdateFkznReddot(int chapterId)
    {
        bool hasReward = ActivityManager.Instance.UpdateFkznReddot(chapterId);
        ((UI_fkTabBtn) this.ShopMain.fkTH.titleList.GetChildAt(chapterId-1)).reddot.visible = hasReward;
        return hasReward;
    }

    #endregion

    #region 特权卡

    private void UpdateTqInfo()
    {
        this.ShopMain.tqTH.tqList.numItems = _monthlyIdList.Count;
        (int, int) freeReward = GetFreeItem(_common800003);
        ((UI_ItemCom)this.ShopMain.tqTH.freeItem.itemCom).SetItemDataWithGuid(new ItemData(freeReward.Item1, freeReward.Item2), true);
        this.ShopMain.tqTH.freeItem.status.selectedIndex = ActivityManager.Instance.Shop_FreeTQKPack == 0 ? 1 : 2;
        this.ShopMain.tqTH.freeItem.getBtn.onClick.Add(this.OnClickGetFreeTQK);
    }

    private void TQTHItemRender(int index, GObject item)
    {
        ConfigMonthlyUnit monthlyUnit = ConfigUtils.GetMonthlyUnit(_monthlyIdList[index]);
        ((UI_tqTHItem) item).cardType.selectedIndex = index;
        string[] rewardArr = monthlyUnit.DailyItemId.Split("|");
        List<ItemData> rewardItemList = new List<ItemData>();
        for (int i = 0; i < rewardArr.Length; i++)
        {
            string[] oneRewardArr = rewardArr[i].Split(",");
            ItemData itemData = new ItemData();
            itemData.id = int.Parse(oneRewardArr[0]);
            itemData.count = double.Parse(oneRewardArr[1]);
            rewardItemList.Add(itemData);
        }
        ((UI_tqTHItem) item).rewardList.itemRenderer = this.OnRewardItemListRender;
        ((UI_tqTHItem) item).rewardList.data = rewardItemList;
        ((UI_tqTHItem) item).rewardList.numItems = rewardItemList.Count;

        // ((UI_tqTHItem) item).attrLb.text = monthlyUnit.Txt;
        ((UI_tqTHItem) item).attrLb.text = ConfigUtils.GetTextById(monthlyUnit.Txt);

        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(monthlyUnit.PayListID);
        if (payListUnit != null)
        { 
            ((UI_EmptyRMB) ((UI_tqTHItem)item).buyBtn).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB) ((UI_tqTHItem)item).buyBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB) ((UI_tqTHItem)item).buyBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }

        ((UI_tqTHItem) item).getRwBtn.data = monthlyUnit;
        ((UI_tqTHItem) item).getRwBtn.onClick.Add(this.OnClickTQGetRewardBtn);
        ((UI_tqTHItem) item).buyBtn.data = monthlyUnit;
        ((UI_tqTHItem) item).buyBtn.onClick.Add(this.OnClickTQBuyBtn);

        if (monthlyUnit.Id == (int) CardType.Permanent)
        {
            bool isBuy = ActivityManager.Instance.IsBuyPermanent == 1;
            bool isGet = ActivityManager.Instance.HasGetPermanentTodayReward >= 1;
            ((UI_tqTHItem) item).rewardCtrl.selectedIndex = !isBuy ? 0 : !isGet ? 1 : 2;
        }
        else
        {
            bool isBuy = ActivityManager.Instance.GetCardPurchase((CardType) monthlyUnit.Id);
            int totalSecond =  (int) (ActivityManager.Instance.GetCardPurchaseEndTime((CardType) monthlyUnit.Type) - ServerTimeManager.Instance.CurServerTime);
            bool isGet = ActivityManager.Instance.GetCardTodayGet((CardType) monthlyUnit.Id);
            ((UI_tqTHItem) item).rewardCtrl.selectedIndex = !isBuy ? 0 : !isGet ? 1 : 2;
        }

        if (monthlyUnit.Id == (int) CardType.Monthly)
        {
            ((UI_tqTHItem) item).img.url = UIResource.GetImageUrlWithLang("yk","Shop");
        }
        
        if (monthlyUnit.Id == (int) CardType.Weekly)
        {
            ((UI_tqTHItem) item).img.url = UIResource.GetImageUrlWithLang("zk","Shop");
        }
        
        if (monthlyUnit.Id == (int) CardType.Permanent)
        {
            ((UI_tqTHItem) item).img.url = UIResource.GetImageUrlWithLang("zsk","Shop");
        }

    }

    private void OnClickTQBuyBtn(EventContext context)
    {
        ConfigMonthlyUnit monthlyUnit = (context.sender as GButton).data as ConfigMonthlyUnit;
        if (monthlyUnit != null)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(monthlyUnit.PayListID);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }

    private void OnClickTQGetRewardBtn(EventContext context)
    {
        ConfigMonthlyUnit monthlyUnit = (context.sender as GButton).data as ConfigMonthlyUnit;
        if (monthlyUnit != null)
        {
            var builder = DailyClaimHeroMonthAward_CS.CreateBuilder();
            builder.ActivityId = (uint) monthlyUnit.Id;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_DailyClaimHeroMonthAward_CS, builder.Build());
        }
    }
    
    private void OnClickGetFreeTQK()
    {
        var builder = ClaimMallFreeAward_CS.CreateBuilder();
        builder.MallFunc = ePlayerAttrID.ePlayerAttrID_FreeMallHeroCardPack;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_ClaimMallFreeAward_CS, builder.Build());
    }

    private void OnUpdateCardActivity()
    {
        UpdateTqInfo();
    }
    #endregion
}
