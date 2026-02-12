using CommonEx;
using Config;
using Engine;
using RoleMain;
using Spine;
using UnityEngine;

public class RoleLevelUpGetView : UIViewBase
{
    private UI_RoleLevelUpGet roleUI => this.main as UI_RoleLevelUpGet;
    
    private HeroInfo _heroInfo;
    private ConfigHeroAttrUnit _heroAttrUnit;

    public RoleLevelUpGetView()
    {
        this.name = "RoleLevelUpGet";
        this.package = "RoleMain";
        this.component = "RoleLevelUpGet";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        RoleMainBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _heroInfo = values[0] as HeroInfo;
        _heroAttrUnit = values[1] as ConfigHeroAttrUnit;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.roleUI.closeBtn.onClick.Add(this.Hide);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateRoleInfo();
        this.roleUI.t0.Play();
        
        this.roleUI.attrItem.typeCtrl.selectedIndex = 0;
        ConfigHeroAttrUnit heroAttr = _heroAttrUnit;
        this.roleUI.attrItem.lockCtrl.selectedIndex = 0;
        // this.roleUI.attrItem.attrCtrl.selectedIndex = heroAttr.AttrId - 1;
        this.roleUI.attrItem.pIcon.url = UIResource.GetAttrIconById(heroAttr.AttrId.ToString());
        this.roleUI.attrItem.lvLb.SetVar("value", heroAttr.Level.ToString()).FlushVars();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.roleUI.spine);
    }
    
    private void UpdateRoleInfo()
    {
        string strResName = ConfigUtils.GetHeroModelPathByID(_heroInfo.HeroUnit.Id);
        Utils.SetSpineModelOnFGUI(this.roleUI.spine, strResName, 180);
        
        // this.roleUI.roleNameLb.text = _heroInfo.HeroUnit.Name;
        this.roleUI.roleNameLb.text = ConfigUtils.GetTextById(_heroInfo.HeroUnit.Name);
        ((UI_qualityLabel) this.roleUI.roleNameLb).qualityCtrl.selectedIndex = _heroInfo.HeroUnit.HeroQuality - 1;
    }
}
