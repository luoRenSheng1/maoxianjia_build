
using System.Collections.Generic;
using BestHTTP.Extensions;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Shop;
using EventDispatcher = EngineBase.EventDispatcher;

public class ShopAdvancedFKView : UIViewBase
{
    private UI_FengkuangPay FengkuangPay => this.main as UI_FengkuangPay;

    private List<ItemData> _rewards = new List<ItemData>();
    private ConfigCommonUnit _common800004;
    private ConfigCommonUnit _common800005;
    private ConfigCommonUnit _common800006;

    private int _chapterType;
    private List<uint> buyIdList;
    public ShopAdvancedFKView()
    {
        this.name = "ShopAdvancedFK";
        this.package = "Shop";
        this.component = "FengkuangPay";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        ShopBinder.BindAll();
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _chapterType = (int)values[0];
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.FengkuangPay.closeBtn.onClick.Add(this.Hide);
        this.FengkuangPay.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.FengkuangPay.rwList.itemRenderer = RewardItemRender;
        this.FengkuangPay.payBtn.onClick.Add(this.OnClickPay);
        this.FengkuangPay.payBtn2.onClick.Add(this.OnClickPay2);
        this.FengkuangPay.payBtn3.onClick.Add(this.OnClickPay3);
        this.FengkuangPay.getBtn.onClick.Add(this.OnClickGet);

        _common800004 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800004);
        _common800005 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800005);
        _common800006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(800006);

        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.OnAdvanceFKUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_CRAZY_GUIDE_PAY_SUCCESS, this.VisibleBtn);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CRAZY_GUIDE_UPDATE, this.OnAdvanceFKUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_CRAZY_GUIDE_PAY_SUCCESS, this.VisibleBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        PlaySpine();
        OnAdvanceFKUpdate();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.HideUIPrefab(this.FengkuangPay.spine);
    }

    private void PlaySpine()
    {
        string strResName = ConfigUtils.GetHeroModelPathByID(20022);
        Utils.SetSpineModelOnFGUI(this.FengkuangPay.spine, strResName, 120f, "idle", null, false);
    }

    private void RewardItemRender(int index, GObject item)
    {
        ((UI_ItemCom) item).SetItemDataWithGuid(_rewards[index], true);
    }

    private void OnAdvanceFKUpdate()
    {
        OnChangeTitle();
        OnAdvanceFKRwListUpdate();
        OnChangeMoneyAndRebate();
        buyIdList = ActivityManager.Instance.BuyIdsInfo();
        VisibleBtn();
    }

    // 切换章节标题：高级疯狂指南第x章
    private void OnChangeTitle()
    {
        this.FengkuangPay.title.SetVar("value", _chapterType.ToString()).FlushVars();
    }

    private void OnChangeMoneyAndRebate()
    {
        if (_chapterType == 1)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3006);
            UI_EmptyRMB payBtn = (UI_EmptyRMB)this.FengkuangPay.payBtn;
            payBtn.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            payBtn.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            payBtn.moneyLb2.SetVar("value",price.ToString()).FlushVars();
            this.FengkuangPay.flTitle.SetVar("value", ((_common800004.Param2).ToInt32()/100).ToString()).FlushVars();
            this.FengkuangPay.btnCtrl.selectedIndex = 0;
        }
        else if (_chapterType == 2)
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3007);
            UI_EmptyRMB payBtn2 = (UI_EmptyRMB)this.FengkuangPay.payBtn2;
            payBtn2.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            payBtn2.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            payBtn2.moneyLb2.SetVar("value",price.ToString()).FlushVars();
            this.FengkuangPay.flTitle.SetVar("value", ((_common800005.Param2).ToInt32()/100).ToString()).FlushVars();
            this.FengkuangPay.btnCtrl.selectedIndex = 1;
        }
        else
        {
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(3008);
            UI_EmptyRMB payBtn3 = (UI_EmptyRMB)this.FengkuangPay.payBtn3;
            payBtn3.moneyType.selectedIndex = payListUnit.MoneyType - 1;
            double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
            payBtn3.moneyLb1.SetVar("value",price.ToString()).FlushVars();
            payBtn3.moneyLb2.SetVar("value",price.ToString()).FlushVars();
            this.FengkuangPay.flTitle.SetVar("value", ((_common800006.Param2).ToInt32()/100).ToString()).FlushVars();
            this.FengkuangPay.btnCtrl.selectedIndex = 2;
        }
    }

    private void OnAdvanceFKRwListUpdate()
    {
        // 根据不同章节展示出不同的奖励列表
        ConfigCommonUnit commonUnit = null;
        switch (_chapterType)
        {
            case 1:
                commonUnit = _common800004;
                break;
            case 2:
                commonUnit = _common800005;
                break;
            case 3:
                commonUnit = _common800006;
                break;
        }

        string[] rewardGroups = commonUnit.Param1.Split('|');

        _rewards.Clear();
        foreach (var rewardGroup in rewardGroups)
        {
            string[] rewardItemList = rewardGroup.Split(',');
            if (rewardItemList.Length == 2)
            {
                int itemId = int.Parse(rewardItemList[0]);
                int itemNum = int.Parse(rewardItemList[1]);
                
                _rewards.Add(new ItemData
                {
                    id = itemId,
                    count = itemNum
                });
            }
        }
        this.FengkuangPay.rwList.numItems = _rewards.Count;
    }

    // 购买高级疯狂指南第一章
    private void OnClickPay()
    {
        if (this.FengkuangPay.isPay.selectedIndex == 0)
        {
            //PayListId !=0 充值
            if (_common800004.Param3.ToInt32() > 0)
            {
                ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_common800004.Param3.ToInt32());
                if (payListUnit != null)
                {
                    UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
                }
            }
        }
    }

    // 购买高级疯狂指南第二章
    private void OnClickPay2()
    {
        if (this.FengkuangPay.isPay.selectedIndex == 0)
        {
            //PayListId !=0 充值
            if (_common800005.Param3.ToInt32() > 0)
            {
                ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_common800005.Param3.ToInt32());
                if (payListUnit != null)
                {
                    UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
                }
            }
        }
    }
    
    // 购买高级疯狂指南第三章
    private void OnClickPay3()
    {
        if (this.FengkuangPay.isPay.selectedIndex == 0)
        {
            //PayListId !=0 充值
            if (_common800006.Param3.ToInt32() > 0)
            {
                ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(_common800006.Param3.ToInt32());
                if (payListUnit != null)
                {
                    UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
                }
            }
        }
    }

    private void VisibleBtn()
    {
        if (buyIdList.Contains(1))
        {
            this.FengkuangPay.payBtn.visible = false;
        }
        
        if (buyIdList.Contains(2))
        {
            this.FengkuangPay.payBtn2.visible = false;
        }
        
        if (buyIdList.Contains(3))
        {
            this.FengkuangPay.payBtn3.visible = false;
        }
    }

    private void OnClickGet()
    {
    }

}