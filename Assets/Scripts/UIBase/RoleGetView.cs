using CommonEx;
using Engine;
using RoleMain;
using Spine;
using UnityEngine;

public class RoleGetView : UIViewBase
{
    private UI_RoleGet roleUI => this.main as UI_RoleGet;
    
    private HeroInfo _heroInfo;

    public RoleGetView()
    {
        this.name = "RoleGet";
        this.package = "RoleMain";
        this.component = "RoleGet";
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
