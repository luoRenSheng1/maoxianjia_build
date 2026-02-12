
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using msg;
using Summon;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class SummonHeroView : UIViewBase
{
    private UI_SummonHero SummonHero => this.main as UI_SummonHero;

    private ConfigCommonUnit _common3006;
    
    private JumpTypeEnum _jumpTypeEnum;  //是否显示手指
    
    public SummonHeroView()
    {
        this.name = "SummonHero";
        this.package = "Summon";
        this.component = "SummonHero";
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
        // this.SummonHero.closeBtn.onClick.Add(this.Hide);
        this.SummonHero.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.SummonHero.summonFree.onClick.Add(this.OnClickSummonFree);
        this.SummonHero.summon1.onClick.Add(this.OnClickSummon1);
        this.SummonHero.summon10.onClick.Add(this.OnClickSummon10);
        this.SummonHero.tipsBtn.onClick.Add(this.OnClickTipsBtn);
        this.SummonHero.giftBtn.onClick.Add(this.OnClickSpecialBtn);
        this.SummonHero.ticketLb.onClick.Add(this.OnClickItemTips);

        _common3006 = ConfigDataGroup.GetInstance<ConfigCommon>().Get(3006);
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_ITEM_UPDATE, this.OnItemUpdate);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_HERO_LOTTERY_SUCCESS, this.UpdateHeroLottery);
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS, this.UpdateSpecialCardReddot); 
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_ITEM_UPDATE, this.OnItemUpdate);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_HERO_LOTTERY_SUCCESS, this.UpdateHeroLottery);
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_HERO_MONTHACTIVITY_SUCCESS, this.UpdateSpecialCardReddot); 
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        
        _jumpTypeEnum = JumpTypeEnum.Normal;
        if (values[0] != null)
        {
            if (values[0] is JumpTypeEnum)
                _jumpTypeEnum = (JumpTypeEnum)values[0];
        }
    }
    
    protected override void OnShow()
    {
        base.OnShow();
        var builder = GetHeroMonthActivityInfo_CS.CreateBuilder();
        GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_GetHeroMonthActivityInfo_CS, builder.Build());
        // this.SummonHero.bgLoader.url = UIResource.GetSummonUrl("bg1");
        OnItemUpdate();
        UpdateHeroLottery();
        UpdateSpecialCardReddot();

        Utils.PlaySpineAnim(SummonHero.girlSpine, "into", false, () =>
        {
            Utils.PlaySpineAnim(SummonHero.girlSpine, "idle", true, null,false);
        });

        this.SummonHero.yxjj.url = UIResource.GetImageUrlWithLang("yxjj","Summon");
        
        if (_jumpTypeEnum == JumpTypeEnum.CallHero)
        {
            var globalPos = this.SummonHero.summon10.LocalToGlobal(Vector2.zero);
            JumpManager.Instance.ShowFinger(_jumpTypeEnum, this.SummonHero.summon10);
        }
    }

    private void OnItemUpdate()
    {
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        this.SummonHero.ticketLb.icon = UIResource.GetItemUrl(common.Param1);
        this.SummonHero.ticketLb.txtValue.text = count.ToString();

        this.SummonHero.summon10.grayed = count < (int.Parse(common.Param2)*10);
        this.SummonHero.summon10.redCtrl.selectedIndex = count < int.Parse(_common3006.Param1) ? 0 : 1;
        this.SummonHero.summon1.grayed = count < (int.Parse(common.Param2));
        this.SummonHero.summon1.redCtrl.selectedIndex = count < int.Parse(_common3006.Param1) ? 0 : 1;
    }

    private void UpdateHeroLottery()
    {
        this.SummonHero.freeCtrl.selectedIndex = DataManager.Instance.GetRoleData().HeroFreeLottery == 0 ? 0 : 1;
        this.SummonHero.freeTimeLb.SetVar("value", StringUtils.GetTimeString(ServerTimeManager.Instance.GetToZeroLeftTime())).FlushVars();
    }

    private void OnClickSummonFree()
    {
        if (DataManager.Instance.GetRoleData().HeroFreeLottery == 0)
        {
            var builder = HeroDailyFreeLottery_CS.CreateBuilder();
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroDailyFreeLottery_CS, builder.Build());
        }
    }

    private void OnClickSummon1()
    {
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        if (count >= (int.Parse(common.Param2)))
        {
            var builder = HeroLottery_CS.CreateBuilder();
            builder.PlayerTimes = 1;
            GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroLottery_CS, builder.Build());
        }
        else
        {
            Utils.YxlNotEnough();
        }
        
    }

    private void OnClickSummon10()
    {
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        int count = ItemInfoManager.Instance.GetItemCount(int.Parse(common.Param1));
        if (count >= (int.Parse(common.Param2)*10))
        {
            this.SummonHero.@group.visible = false;
            Utils.PlaySpineAnim(SummonHero.girlSpine, "chouka", false, () =>
            {
                this.SummonHero.@group.visible = true;
                Utils.PlaySpineAnim(SummonHero.girlSpine, "idle", true);
                var builder = HeroLottery_CS.CreateBuilder();
                builder.PlayerTimes = 10;
                GameManager.Instance.Connection?.SendMessage((int) eMsgID.eMsg_HeroLottery_CS, builder.Build());
            });
        }      
        else
        {
            Utils.YxlNotEnough();
        }
    }

    private void OnClickTipsBtn()
    {
        UIManager.Instance.ShowUIPanel("SummonHeroHelp");
    }

    private void OnClickSpecialBtn()
    {
        UIManager.Instance.Toast("暂未开放！");
        // UIManager.Instance.ShowUIPanel("SpecialCard");// 关闭商业化
    }

    private void OnClickItemTips()
    {
        ConfigCommonUnit common = ShopInfoManager.Instance.GetHeroLotteryCfg();
        ConfigItemTypeUnit itemTypeUnit = ConfigUtils.GetConfigItemTypeUnitById(int.Parse(common.Param1));
        TipsManger.Instance.ShowPopupTip(this.SummonHero.ticketLb, Tipstype.Item, itemTypeUnit.Id);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(this.SummonHero.freeCtrl.selectedIndex == 1)
            this.SummonHero.freeTimeLb.SetVar("value", StringUtils.GetTimeString(ServerTimeManager.Instance.GetToZeroLeftTime())).FlushVars();
    }

    private void UpdateSpecialCardReddot()
    {
        this.SummonHero.giftBtn.redDot.visible = ActivityManager.Instance.SpecialRedDot();
    }
}
