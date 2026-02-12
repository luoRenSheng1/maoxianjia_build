
using System;
using Common;
using CommonEx;
using Config;
using Engine;

public class GetPetRewardView : UIViewBase
{
    public struct GetPetParam
    {
        public int Id;
        public bool IsRole;
        public Action CloseCallback;
    }
    
    private UI_GetPetReward GetPetReward => this.main as UI_GetPetReward;
    private GetPetParam _petParam;
    public GetPetRewardView()
    {
        this.name = "GetPetReward";
        this.package = "Common";
        this.component = "GetPetReward";
        this.removePackage = true;
        this.safeAreaInset = false;
        this.type = UIType.Tip;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _petParam = (GetPetParam) values[0];
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.GetPetReward.sortingOrder = 99999;
        this.GetPetReward.closeBtn.onClick.Add(this.Hide);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.GetPetReward.showCtrl.selectedIndex = 0;
        Utils.PlaySpineAnim(this.GetPetReward.baozhaSpine, "Chuxian", false);
        Utils.PlaySpineAnim(this.GetPetReward.gxhdSpine, "Chuxian", false, () =>
        {
            Utils.PlaySpineAnim(this.GetPetReward.gxhdSpine, "Loop", true);
        });
        GameManager.Instance.TimerManager.SetTimer(0.3f, ShowPetInfo);
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralGainRewardSE);
        this.GetPetReward.img.url = UIResource.GetImageUrlWithLang("g2", "Common");
    }

    private void ShowPetInfo()
    {
        GameManager.Instance.TimerManager.ClearTimer(ShowPetInfo);
        this.GetPetReward.showCtrl.selectedIndex = 1;
        if (_petParam.IsRole)
        {
            int roleId = _petParam.Id;
            ConfigHeroUnit heroUnit = ConfigUtils.GetHeroById(roleId);
            this.GetPetReward.petName.visible = true;
            // this.GetPetReward.petName.text = heroUnit.Name;
            this.GetPetReward.petName.text = ConfigUtils.GetTextById(heroUnit.Name);
            ((UI_qualityLabel) this.GetPetReward.petName).qualityCtrl.selectedIndex = heroUnit.HeroQuality - 1;
            Utils.SetSpineModelOnFGUI(this.GetPetReward.petSpine, heroUnit.Model, 160);
        }
        else
        {
            int curPetId = _petParam.Id;
            ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(curPetId);
            this.GetPetReward.petName.visible = true;
            // this.GetPetReward.petName.text = petBasisUnit.Name;
            this.GetPetReward.petName.text = ConfigUtils.GetTextById(petBasisUnit.Name);
            ((UI_qualityLabel) this.GetPetReward.petName).qualityCtrl.selectedIndex = petBasisUnit.Quality - 1;
            Utils.SetSpineModelOnFGUI(this.GetPetReward.petSpine, petBasisUnit.PetModel, 160);
        }
    }

    protected override void OnHide()
    {
        GameManager.Instance.TimerManager.ClearTimer(ShowPetInfo);
        
        if (_petParam.IsRole)
        {
            Utils.ClearSpineModelOnFGUI(this.GetPetReward.petSpine);
        }
        else
        {
            Utils.ClearSpineModelOnFGUI(this.GetPetReward.petSpine);
        }
        
        _petParam.CloseCallback?.Invoke();
        base.OnHide();
    }
}
