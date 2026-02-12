using CommonEx;
using Engine;
using RoleMain;
using Spine;
using UnityEngine;

public class RoleBreakView : UIViewBase
{
    private UI_RoleBreak roleUI => this.main as UI_RoleBreak;
    
    private HeroInfo _heroInfo;

    public RoleBreakView()
    {
        this.name = "RoleBreak";
        this.package = "RoleMain";
        this.component = "RoleBreak";
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
        
        this.roleUI.starSpine.spineAnimation.skeleton.SetToSetupPose();
        this.roleUI.starSpine.spineAnimation.state.ClearTracks();
        if (_heroInfo.BreakLevelLayer == 4)
        {
            TrackEntry trackEntry = this.roleUI.starSpine.spineAnimation.AnimationState.SetAnimation(0, "star_moon", false);
            trackEntry.AnimationStart = 0;
            this.roleUI.starSpine.spineAnimation.AnimationState.AddAnimation(0, GetStarSpineAniName(_heroInfo.BreakLevelLayer), false, 0);
        }else if (_heroInfo.BreakLevelLayer == 7)
        {
            TrackEntry trackEntry = this.roleUI.starSpine.spineAnimation.AnimationState.SetAnimation(0, "moon_sun", false);
            trackEntry.AnimationStart = 0;
            this.roleUI.starSpine.spineAnimation.AnimationState.AddAnimation(0, GetStarSpineAniName(_heroInfo.BreakLevelLayer), false, 0);
        }
        else
        {
            this.roleUI.starSpine.spineAnimation.AnimationState.SetAnimation(0, GetStarSpineAniName(_heroInfo.BreakLevelLayer), false);
        }


    }

    private string GetStarSpineAniName(int breakLevel)
    {
        if (breakLevel <= 3)
        {
            return "star"+breakLevel;
        }
        else if (breakLevel <= 6)
        {
            return "moon" + (breakLevel - 3);
        }
        else
        {
            return "sun"+ Mathf.Min(3, breakLevel - 6);
        }
    }
    
}
