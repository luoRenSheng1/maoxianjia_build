using System.Collections.Generic;
using BestHTTP.Extensions;
using BuryGiftPack;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using EventDispatcher = EngineBase.EventDispatcher;

public class BuryGiftPackView : UIViewBase
{
    private UI_BuryGiftPack BuryGift => this.main as UI_BuryGiftPack;
    private ConfigGiftUnit _buryGiftUnit;
    private Gift_Bury _buryType;
    private List<ItemData> _giftItemDatas = new List<ItemData>();
    public BuryGiftPackView()
    {
        this.name = "BuryGiftPack";
        this.package = "BuryGiftPack";
        this.component = "BuryGiftPack";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        BuryGiftPackBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _buryType = (Gift_Bury) values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.BuryGift.closeBtn.onClick.Add(this.Hide);
        this.BuryGift.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.BuryGift.buyBtn.onClick.Add(this.OnClickBuyBtn);
        this.BuryGift.gotoBtn.onClick.Add(this.OnClickGotoBtn);
        this.BuryGift.rewardList.itemRenderer = RewardItemRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateBuryGift);
    }
    
    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_RECHARGE_GIFT_UPDATE, this.UpdateBuryGift);
    }

    protected override void OnShow()
    {
        base.OnShow();

        if (_buryType == Gift_Bury.Gift_zzc)
        {
            this.BuryGift.typeCtrl.selectedIndex = 0;
        }else if (_buryType == Gift_Bury.Gift_yxsj)
        {
            this.BuryGift.typeCtrl.selectedIndex = 2;
        }else if (_buryType == Gift_Bury.Gift_goldAdd)
        {
            this.BuryGift.typeCtrl.selectedIndex = 3;
        }
        else
        {
            this.BuryGift.typeCtrl.selectedIndex = 1;
        }
        
        UpdateBuryGift();
    }

    private void UpdateBuryGift()
    {
        _buryGiftUnit = ConfigUtils.GetGiftUnitsById((int) _buryType);
        // this.BuryGift.titleLb.text = _buryGiftUnit.Name;
        this.BuryGift.titleLb.text = ConfigUtils.GetTextById(_buryGiftUnit.Name);
        this.BuryGift.rebate.SetVar("value", ((_buryGiftUnit.Rebate).ToInt32()/100).ToString()).FlushVars();
        _giftItemDatas.Clear();
        string[] itemArr = _buryGiftUnit.ItemId.Split('|');
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

        this.BuryGift.rewardList.numItems = _giftItemDatas.Count;
        
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_buryGiftUnit.PayList);
        if (payListUnit != null)
        {
            ((UI_EmptyRMB) (this.BuryGift.buyBtn)).moneyType.selectedIndex = payListUnit.MoneyType-1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            ((UI_EmptyRMB) (this.BuryGift.buyBtn)).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
            ((UI_EmptyRMB) (this.BuryGift.buyBtn)).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
        }

        if (_buryGiftUnit.BuyNumber == 0)
        {
            this.BuryGift.ctrl.selectedIndex = 1;
            this.BuryGift.buyMax.selectedIndex = 0;
        }
        else
        {
            this.BuryGift.ctrl.selectedIndex = 0;
            LimitPackVo limitPackVo = ShopInfoManager.Instance.GetPackNum((int) _buryType);
            this.BuryGift.cntLb.SetVar("cur", limitPackVo.BuyCounter.ToString()).SetVar("total", _buryGiftUnit.BuyNumber.ToString()).FlushVars();
            this.BuryGift.buyMax.selectedIndex = limitPackVo.BuyCounter >= _buryGiftUnit.BuyNumber ? 1 : 0;
        }
    }

    private void RewardItemRender(int index, GObject item)
    {
        ItemData itemData = _giftItemDatas[index];
        ((UI_ItemCom)item).SetItemDataWithGuid(itemData, true);
    }

    private void OnClickBuyBtn()
    {
        if (_buryGiftUnit.PayList > 0)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_buryGiftUnit.PayList);
            if (payListUnit != null)
            {
                UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
            }
        }
    }

    private void OnClickGotoBtn()
    {
        LobbyView lobbyView = UIManager.Instance.FindByName("Lobby") as LobbyView;
        if (_buryType == Gift_Bury.Gift_zzc)
        {
            lobbyView?.OpenBottomPanel(3, (int) DungeonType.Zhuzhao);
        }
        else if (_buryType == Gift_Bury.Gift_yxsj)
        {
            lobbyView?.OpenBottomPanel(3, (int) DungeonType.Exp);
        }
        else if (_buryType == Gift_Bury.Gift_goldAdd)
        {
            lobbyView?.OpenBottomPanel(3, (int) DungeonType.Gold);
        }
    }
}
