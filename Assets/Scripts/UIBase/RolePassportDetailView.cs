
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Passport;
using Spine.Unity;
using EventDispatcher = EngineBase.EventDispatcher;

public class RolePassportDetailView : UIViewBase
{
    private UI_RolePassportDetail RolePassportDetail => this.main as UI_RolePassportDetail;

    private int _heroId;
    private List<ConfigHeroAttrUnit> _heroAttrUnits;
    private ConfigHeroUnit _heroUnit;
    private PassPortInfo _passPortInfo;
    private ConfigCommonUnit _commonUnit700;
    public RolePassportDetailView()
    {
        this.name = "RolePassportDetail";
        this.package = "Passport";
        this.component = "RolePassportDetail";
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
        this.RolePassportDetail.closeBtn.onClick.Add(this.Hide);
        this.RolePassportDetail.advanceBtn.onClick.Add(this.OnClickAdvanceBtn);
        this.RolePassportDetail.levelAttrList.itemRenderer = HeroLevelAttrItemRender;
        
        EventDispatcher.GameWorld.Regist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        EventDispatcher.GameWorld.UnRegist(EventDefine.EVENT_UPDATE_PASSPORT_UNLOCK_ADVANCE, this.OnUpdateUnlockAdvancePassport);
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _heroId = (int) values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();

        _heroUnit = ConfigUtils.GetHeroById(_heroId);
        
        GameManager.Instance.TimerManager.SetTimer(0.2f, () =>
        {
            string strResName = ConfigUtils.GetHeroModelPathByID(_heroId);
            Utils.SetSpineModelOnFGUI(this.RolePassportDetail.spine, strResName, 180f, "idle");
        });
        
        this.RolePassportDetail.roleName.text = ConfigUtils.GetTextById(_heroUnit.Name);
        ((UI_qualityLabel) this.RolePassportDetail.roleName).qualityCtrl.selectedIndex = _heroUnit.HeroQuality-1;
        ((UI_roleQualityItem) this.RolePassportDetail.qIcon).quality.selectedIndex = _heroUnit.HeroQuality-1;

        ConfigSkillUnit activeSkillCfg = ConfigUtils.GetSkillById(_heroUnit.ActiveSkill);
        this.RolePassportDetail.roleSkillIcon.url = UIResource.GetItemUrl(activeSkillCfg.SkillIcon);
        this.RolePassportDetail.skillNameLb.text = ConfigUtils.GetTextById(activeSkillCfg.Name);
        double SkillDamageRate = ConfigUtils.GetHeroSkillDamage(_heroUnit.Id, 1);
        this.RolePassportDetail.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(activeSkillCfg.SkillDes), (SkillDamageRate*ConstDefine.CONFIG_PLACE).ToString("f2"));
        this.RolePassportDetail.roleSkillIcon.onClick.Set(this.OnClickSkillIconToShowEff);

        _heroAttrUnits = ConfigUtils.GetHeroAttrsByHeroId(_heroId);
        if (_heroId == HeroInfoManager.Instance.GetHeroFirstID())
        {
            this.RolePassportDetail.levelAttrList.numItems = _heroAttrUnits.Count;//初始角色没有突破属性
        }
        else
        {
            this.RolePassportDetail.levelAttrList.numItems = _heroAttrUnits.Count + 1;//有一个是大于100级的突破属性
        }

        _passPortInfo = ActivityManager.Instance.GetPassPortInfo();
        if (this._passPortInfo.UnlockTier == 0)
        {
            this.RolePassportDetail.advanceBtn.visible = true;
            ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(int.Parse(_commonUnit700.Param3));
            if (payListUnit != null)
            {
                ((UI_AdvancePassportBtn) this.RolePassportDetail.advanceBtn).moneyType.selectedIndex = payListUnit.MoneyType-1;
                double price = double.Parse((payListUnit.RechargeAmount / 100f).ToString("f2"));
                ((UI_AdvancePassportBtn) this.RolePassportDetail.advanceBtn).moneyLb1.SetVar("value", price.ToString("f2")).FlushVars();
                ((UI_AdvancePassportBtn) this.RolePassportDetail.advanceBtn).moneyLb2.SetVar("value", price.ToString("f2")).FlushVars();
            }
            
        }
        else
        {
            this.RolePassportDetail.advanceBtn.visible = false;
        }
    }
    
    private void HeroLevelAttrItemRender(int index, GObject item)
    {
        if (index != _heroAttrUnits.Count)
        {
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 0;
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = 1 >= heroAttr.Level ? 0 : 1;
            ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = heroAttr.AttrId - 1;
            if (((UI_RoleAttrItem) item).lockCtrl.selectedIndex == 1)
            {
                ((UI_RoleAttrItem) item).lvLb.SetVar("value", heroAttr.Level.ToString()).FlushVars();
            }
        }
        else
        {
            var starParam = Utils.GetHeroStar(0);
            for (int i = 0; i < 3; i++)
            {
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).lockCtrl.selectedIndex= starParam.Item2 > i ? 0 : 1;
                ((UI_RoleStarItem) ((UI_RoleAttrItem) item).GetChild("star" + i)).type.selectedIndex = starParam.Item1;
            }
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroUnit.Id, 1);
            ((UI_RoleAttrItem) item).attrCtrl.selectedIndex = attrParam.Item2-1;
            ((UI_RoleAttrItem) item).starCount.selectedIndex = starParam.Item2;
            ((UI_RoleAttrItem) item).typeCtrl.selectedIndex = 1;
            ((UI_RoleAttrItem) item).lockCtrl.selectedIndex = starParam.Item2 == 0 ? 1 : 0;
            ((UI_RoleAttrItem) item).starLb.text = 0.ToString();
        }
        
        ((UI_RoleAttrItem) item).data = index;
        ((UI_RoleAttrItem) item).onClick.Set(OnHeroLevelAttrTips);
    }
    
    private void OnHeroLevelAttrTips(EventContext context)
    {
        int index = (int)((UI_RoleAttrItem)context.sender).data;
        if (index >= 0 && index < _heroAttrUnits .Count)
        {
            ConfigHeroAttrUnit heroAttr = _heroAttrUnits[index];
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, StringUtils.ConvertToAttributeValue(heroAttr.AttrId, heroAttr.Value), 1>=heroAttr.Level, heroAttr.Level,heroAttr.AttrId );
        }
        else
        {
            (double, int) attrParam = ConfigUtils.GetHeroAttrsByBreakLevel(_heroUnit.Id, 0);
            TipsManger.Instance.ShowPopupTip((UI_RoleAttrItem)context.sender, Tipstype.HeroLevelAttr, StringUtils.ConvertToAttributeValue((int) attrParam.Item2,  attrParam.Item1), true, 0,(int) attrParam.Item2);
        }
    }
    
    private void OnClickSkillIconToShowEff(EventContext context)
    {
        int skillLevelId = _heroUnit.ActiveSkill;
        ConfigSkillUnit skillLevel = ConfigUtils.GetSkillById(skillLevelId);
        UIManager.Instance.ShowUIPanel("SkillEffectPre", skillLevel.AttackEffect);
    }

    private void OnClickAdvanceBtn()
    {
        ConfigPayListUnit payListUnit = ConfigDataGroup.GetInstance<ConfigPayList>().Get(int.Parse(_commonUnit700.Param3));
        if (payListUnit != null)
        {
            UIManager.Instance.SendToApplyRecharge(payListUnit.Id);
        }

        UIManager.Instance.ShowUIPanel("PassportDetail");
    }
    
    private void OnUpdateUnlockAdvancePassport()
    {
        this.RolePassportDetail.advanceBtn.visible = ActivityManager.Instance.GetPassPortInfo().UnlockTier == 0;
    }
}
