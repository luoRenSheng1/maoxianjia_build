
using System.Collections.Generic;
using System.Linq;
using BestHTTP.Extensions;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using FirstPay;
using msg;
using EventDispatcher = EngineBase.EventDispatcher;

public class FirstPayView : UIViewBase
{
    private UI_FirstPayMain FirstPayMain => this.main as UI_FirstPayMain;
    
    private ConfigCommonUnit firstPayUnit;
    private List<FirstPayDayOneRwInfo> _firstPayDayOneRwInfos;//首充第一天奖励
    private bool hasShownSkillEffect = false;// 技能特效展示

    private int isGet = 0;
    private (int, bool) _fMap;
    
    public FirstPayView()
    {
        this.name = "FirstPayMain";
        this.package = "FirstPay";
        this.component = "FirstPayMain";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        FirstPayBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();

        firstPayUnit = ActivityManager.Instance.GetConfigCommonUnitById(800007);
        
        // this.FirstPayMain.closeBtn.onClick.Add(this.Hide);
        this.FirstPayMain.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.FirstPayMain.FPRewardList.itemRenderer = FPRewardListRender;
        this.FirstPayMain.payBtn.onClick.Add(this.OnClickPay);
        this.FirstPayMain.getFPRewardBtn.onClick.Add(this.OnClickGetFPReward);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.OnFirstPayUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.OnGetRwStatusUpdate);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UPDATE, this.OnFirstPayUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_FIRST_PAY_UI_UPDATE, this.OnGetRwStatusUpdate);
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        // PlaySpine();

        // if (!hasShownSkillEffect)
        // {
        //     SkillIconToShowEff();
        //     hasShownSkillEffect = true;
        // }

        OnFirstPayUpdate();
        
        OnFPDayOneRwUpdate();
        OnFPDayTwoRwUpdate();
        OnFPDayThreeRwUpdate();
        
        
    }

    protected override void OnHide()
    {
        base.OnHide();
        // Utils.HideUIPrefab(this.FirstPayMain.startSpine);
        // Utils.HideUIPrefab(this.FirstPayMain.fireSpine);
        // Utils.HideUIPrefab(this.FirstPayMain.spine);
    }

    private void PlaySpine()
    {
        //UI_shouchong_liuxing
        //UI_shouchong_yanwu
        // Utils.ShowUIPrefab(this.FirstPayMain.startSpine, "UI_shouchong_liuxing", 100f, "Effect/");
        // Utils.ShowUIPrefab(this.FirstPayMain.fireSpine, "UI_shouchong_yanwu", 100f, "Effect/");
        // string strResName = ConfigUtils.GetHeroModelPathByID(20021);
        // Utils.SetSpineModelOnFGUI(this.FirstPayMain.spine,strResName,180f,"idle",null,true);
    }

    // private void SkillIconToShowEff()
    // {
    //     string[] rwItem = firstPayUnit.Param1.Split(',');
    //     int skillId = int.Parse(rwItem[0]);
    //     UIManager.Instance.ShowUIPanel("SkillEffectPre", skillId);
    // }

    //刷新领取状态
    private void OnGetRwStatusUpdate()
    {
        OnFPDayOneRwUpdate();
        OnFPDayTwoRwUpdate();
        OnFPDayThreeRwUpdate();
    }

    // 第二天奖励
    private void OnFPDayTwoRwUpdate()
    {
        if (firstPayUnit == null)
        {
            this.FirstPayMain.FPReward2.item.visible = false;
            LogUtils.LogErrorFormat("Common配置有误");
            return;
        }
        
        string[] rewardItem = firstPayUnit.Param2.Split(',');

        if (rewardItem == null)
        {
            LogUtils.LogErrorFormat("Common配置表Param2参数有误");
            return;
        }
        
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardItem[0]));
        
        UI_ItemCom item = (UI_ItemCom)this.FirstPayMain.FPReward2.item;
        
        item.ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        item.txtLv.text = rewardItem[1].ToString();
        item.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        
        this.FirstPayMain.FPReward2.item.data = int.Parse(rewardItem[0]);
        this.FirstPayMain.FPReward2.item.onClick.Set(OnItemTips);
        
        var firstPayInfo = ActivityManager.Instance.GetFirstPayInfos().FirstOrDefault(info => info.WhichDay == 2);
        if (firstPayInfo != null && firstPayInfo.Tag == 1)
        {
            this.FirstPayMain.FPReward2.isGet.selectedIndex = 1;
        }
        else
        {
            this.FirstPayMain.FPReward2.isGet.selectedIndex = 0;
        }
        
    }

    // 第三天奖励
    private void OnFPDayThreeRwUpdate()
    {
        if (firstPayUnit == null)
        {
            this.FirstPayMain.FPReward3.item.visible = false;
            LogUtils.LogErrorFormat("Common配置有误");
            return;
        }
        
        string[] rewardItem = firstPayUnit.Param3.Split(',');
        
        if (rewardItem == null)
        {
            LogUtils.LogErrorFormat("Common配置表Param3参数有误");
            return;
        }
        
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(rewardItem[0]));
        
        UI_ItemCom item = (UI_ItemCom)this.FirstPayMain.FPReward3.item;

        item.ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        item.txtLv.text = rewardItem[1].ToString();
        item.icon = UIResource.GetItemUrl(itemTypeUnit.Icon);

        this.FirstPayMain.FPReward3.item.data = int.Parse(rewardItem[0]);
        this.FirstPayMain.FPReward3.item.onClick.Set(OnItemTips);

        var firstPayInfo = ActivityManager.Instance.GetFirstPayInfos().FirstOrDefault(info => info.WhichDay == 3);
        if (firstPayInfo != null && firstPayInfo.Tag == 1)
        {
            this.FirstPayMain.FPReward3.isGet.selectedIndex = 1;
        }
        else
        {
            this.FirstPayMain.FPReward3.isGet.selectedIndex = 0;
        }
        
    }

    // 第一天奖励列表渲染
    private void FPRewardListRender(int index, GObject item)
    {
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(_firstPayDayOneRwInfos[index].itemId);
        
        ((UI_ItemCom)((UI_FirstItem) item).item).ctrlQuality.selectedIndex = itemTypeUnit.Quality - 1;
        ((UI_ItemCom)((UI_FirstItem) item).item).txtLv.text = _firstPayDayOneRwInfos[index].num.ToString();
        if (itemTypeUnit.Id == 2000)
        {
            ((UI_ItemCom)((UI_FirstItem) item).item).txtLv.text = ((_firstPayDayOneRwInfos[index].num / 10000) + "万").ToString();
        }

        ((UI_ItemCom)((UI_FirstItem) item).item).icon = UIResource.GetItemUrl(itemTypeUnit.Icon);
        ((UI_ItemCom)((UI_FirstItem) item).item).data = _firstPayDayOneRwInfos[index].itemId;
        ((UI_ItemCom)((UI_FirstItem) item).item).onClick.Set(OnItemTips);

        var firstPayInfo = ActivityManager.Instance.GetFirstPayInfos().FirstOrDefault(info => info.WhichDay == 1);
        if (firstPayInfo != null && firstPayInfo.Tag == 1)
        {
            ((UI_FirstItem)item).isGet.selectedIndex = 1;
        }
        else
        {
            ((UI_FirstItem)item).isGet.selectedIndex = 0;
        }
        
    }

    // 物品信息展示
    private void OnItemTips(EventContext context)
    {
        int itemId = (int)((UI_ItemCom)context.sender).data;
        if (itemId != 0)
        {
            TipsManger.Instance.ShowPopupTip((UI_ItemCom)context.sender, Tipstype.None, itemId);
        }
    }

    // 第一天奖励
    private void OnFPDayOneRwUpdate()
    {
        _firstPayDayOneRwInfos = ActivityManager.Instance.GetFirstPayDayOneRws();
        this.FirstPayMain.FPRewardList.numItems = _firstPayDayOneRwInfos.Count;
    }

    // 首充事件处理
    private void OnFirstPayUpdate()
    {
        if (IsShow() && IsOnStage())
        {
            if (!DataManager.Instance.GetRoleData().IsFirstCharge)
            {
                this.FirstPayMain.isPay.selectedIndex = 0;

                if (firstPayUnit.Param4.ToInt32() > 0)
                {
                    ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(firstPayUnit.Param5.ToInt32());
                    if (payListUnit != null)
                    {
                        this.FirstPayMain.payBtn.moneyType.selectedIndex = payListUnit.MoneyType - 1;
                        double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
                        this.FirstPayMain.payBtn.money.SetVar("value", price.ToString("f2")).FlushVars();
                        this.FirstPayMain.payBtn.money2.SetVar("value", price.ToString("f2")).FlushVars();
                    }
                }
            }
            else
            {
                _fMap = ActivityManager.Instance.CanGetFirstPayInfo();
                if (!_fMap.Item2)
                {
                    this.FirstPayMain.isPay.selectedIndex = 2;
                }
                else
                {
                    this.FirstPayMain.isPay.selectedIndex = 1;
                }
            }
        }
    }

    // 购买事件处理
    private void OnClickPay()
    {
        if (this.FirstPayMain.isPay.selectedIndex == 0)
        {
            //PayListId !=0 充值
            if (firstPayUnit.Param5.ToInt32() > 0)
            {
                ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(firstPayUnit.Param5.ToInt32());
                if (payListUnit != null)
                {
                    UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
                }
            }
        }      
    }

    // 领取事件处理
    private void OnClickGetFPReward()
    {
        if (this.FirstPayMain.isPay.selectedIndex == 2)
        {
            UIManager.Instance.ToastByKey(10187);
            return;
        }
        // 向服务器发送领取奖励请求
        var builder = NewPlayerRechargeClaimAward_CS.CreateBuilder();
        builder.Id = (uint) _fMap.Item1;
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_NewPlayerRechargeClaimAward_CS, builder.Build());
    }

}
