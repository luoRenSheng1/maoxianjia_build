
using System.Collections.Generic;
using System.Text;
using Common;
using Config;
using Engine;
using FairyGUI;

public class PetTipShowDetailView : UIViewBase
{
    private UI_PetTipShowDetail PetTipShowDetail => this.main as UI_PetTipShowDetail;
    private int _curPetId;

    public PetTipShowDetailView()
    {
        this.name = "PetTipShowDetail";
        this.package = "Common";
        this.component = "PetTipShowDetail";
        this.removePackage = true;
        this.safeAreaInset = false;
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.PetTipShowDetail.sortingOrder = 100000;
    }

    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        _curPetId = (int) values[0];
    }

    protected override void OnShow()
    {
        base.OnShow();
        ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(_curPetId);
        // this.PetTipShowDetail.gender.selectedIndex = petBasisUnit.Gender;
        // this.PetTipShowDetail.occupationType.selectedIndex = petBasisUnit.Occupation - 1;
        // var attr = Utils.GetAttributeAddStr(new List<ConfigAttrUnit>(){ConfigUtils.GetAttrById(petBasisUnit.BasisAttrId)});
        // this.PetTipShowDetail.atkLb.text = attr.Item2.Atk.ToString("");
        // this.PetTipShowDetail.gjxsLb.text = (attr.Item2.AttackCoefficient*100f).ToString("f0") + "%";
        // this.PetTipShowDetail.gsLb.text = attr.Item2.AtkSpeed.ToString("f0");
        // this.PetTipShowDetail.ljLb.text = (attr.Item2.ComboAtk*100f).ToString("f0")  + "%";
        // this.PetTipShowDetail.ljxsLb.text = (attr.Item2.ComboCoefficient*100f).ToString("f0") + "%";
        // this.PetTipShowDetail.bjLb.text = (attr.Item2.CriticalStrike*100f).ToString("f0") + "%";
        // this.PetTipShowDetail.bsLb.text = (attr.Item2.CriticalInjury*100f).ToString("f0") + "%";
        // this.PetTipShowDetail.ctLb.text = attr.Item2.Penetration.ToString("");
        // this.PetTipShowDetail.petIcon.url = UIResource.GetPetIcon(petBasisUnit.IconPath);
        Utils.SetSpineModelOnFGUI(this.PetTipShowDetail.petIcon, ConfigUtils.GePetModelPathByID(petBasisUnit.Id), 100);
        
        // ConfigSkillUnit skillUnit = ConfigUtils.GetSkillById(petBasisUnit.SkillId);
        // this.PetTipShowDetail.skillIcon.url = UIResource.GetSkillIcon(skillUnit.SkillIcon);
        // this.PetTipShowDetail.skillName.text = skillUnit.Name;
        // this.PetTipShowDetail.skillDesc.text = skillUnit.SkillDes;
        // this.PetTipShowDetail.petDesc.text = petBasisUnit.Story;
        // this.PetTipShowDetail.petName.text = petBasisUnit.Name;
        this.PetTipShowDetail.skillIcon.onClick.Set(this.OnClickSkillIconToShowEff);
        
    }

    protected override void OnHide()
    {
        base.OnHide();
        Utils.ClearSpineModelOnFGUI(this.PetTipShowDetail.petIcon);
    }

    private void OnClickSkillIconToShowEff(EventContext context)
    {
        // ConfigPetBasisUnit petBasisUnit = ConfigUtils.GetPetById(_curPetId);
        // ConfigSkillUnit skillLevel = ConfigUtils.GetSkillById(petBasisUnit.SkillId);
        // UIManager.Instance.ShowUIPanel("SkillEffectPre", skillLevel.AttackEffect);
    }
}
