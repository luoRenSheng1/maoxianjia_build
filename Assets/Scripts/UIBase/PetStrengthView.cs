
using System.Collections.Generic;
using CommonEx;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Pet;

public class PetStrengthView : UIViewBase
{
    private UI_PetStrength PetStrength => this.main as UI_PetStrength;

    private List<PetStrengthVo> _petStrengthVos;
    float _delay = 0f;
    public PetStrengthView()
    {
        this.name = "PetStrength";
        this.package = "Pet";
        this.component = "PetStrength";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _petStrengthVos = values[0] as List<PetStrengthVo>;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.PetStrength.closeBtn.onClick.Add(this.Hide);
        this.PetStrength.petList.petList.itemRenderer = PetStrengthItemRender;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.UpLvSE);
        
        this.PetStrength.img.url = UIResource.GetImageUrlWithLang("g3", "Common");
        this.PetStrength.t0.Play(1, 0, null);
        Utils.PlaySpineAnim(this.PetStrength.gxhdSpine, "Chuxian", false, () =>
        {
            //Utils.PlaySpineAnim(this.GetRewardUI.gxhdSpine, "Loop", true);
        });
        _petStrengthVos.Sort((a, b)=>
        {
            int result = a.PetBasisUnit.Quality > b.PetBasisUnit.Quality ? 1 : (a.PetBasisUnit.Quality==b.PetBasisUnit.Quality ? 0 : -1);
            if (result == 0)
                result = a.PetBasisUnit.Id > b.PetBasisUnit.Id ? 1 : -1;
            return result;
        });
        this.PetStrength.petList.petList.numItems = _petStrengthVos.Count;
        this.PetStrength.petList.petList.ScrollToView(0);
    }
    
    private void PetStrengthItemRender(int index, GObject item)
    {
        PetStrengthVo petStrengthVo = _petStrengthVos[index];
        // ((UI_ItemCom)((UI_PetStrengthItem) item).item).icon = UIResource.GetPetIcon(petStrengthVo.PetBasisUnit.IconPath);
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).icon = UIResource.GetItemUrl(ConfigUtils.GetConfigItemTypeByParam(int.Parse(petStrengthVo.PetBasisUnit.IconPath)).Icon);
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).ctrlQuality.selectedIndex = petStrengthVo.PetBasisUnit.Quality - 1;
        ((UI_PetStrengthItem) item).curLb.text = petStrengthVo.PreLv.ToString();
        ((UI_PetStrengthItem) item).nextLb.text = petStrengthVo.CurLv.ToString();
        ((UI_PetStrengthItem) item).visible = false;
        ((UI_ItemCom)((UI_PetStrengthItem) item).item).itemSpineEff.visible = false;
        _delay = index * 0.02f;
        GameManager.Instance.TimerManager.SetTimer(EN_TIMER_SOURCE.UI, 1, _delay, () =>
        {
            ((UI_PetStrengthItem) item).visible = true;
            //((UI_ItemCom)((UI_PetStrengthItem) item).item).itemSpineEff.visible = true;
        });
    }

    protected override void OnHide()
    {
        base.OnHide();
        GameManager.Instance.TimerManager.ClearTimerBySourceID(EN_TIMER_SOURCE.UI, 1);
    }
}
