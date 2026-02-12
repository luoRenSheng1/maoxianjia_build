using System.Collections.Generic;
using Config;
using Engine;
using Pet;

public class PetSkillTipsView : UIViewBase
{
    private UI_PetSkillTips PetSkillTip => this.main as UI_PetSkillTips;

    private int type;//1天赋、2技能书
    private int id;//技能书id
    private PetTalent talentInfo;//宠物天赋
    private PetSkillBookSlot petBookSlot;//宠物书

    public PetSkillTipsView()
    {
        this.name = "PetSkillTips";
        this.package = "Pet";
        this.component = "PetSkillTips";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        PetBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.PetSkillTip.closeBtn.onClick.Add(this.Hide);
        this.PetSkillTip.closeBtn.onClick.Add(this.HideWithSoundEffect);
    }
    
    protected override void OnUpdateParams(params object[] values)
    {
        base.OnUpdateParams(values);
        type =  (int)values[0];
        // id = (int)values[1];

        if (type == 1)//天赋
        {
            talentInfo  = (PetTalent)values[1];
        }

        if (type == 2)//技能书
        {
            // id = (int)values[1];
            petBookSlot = (PetSkillBookSlot)values[1];
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        if (type == 1)
        {
            this.PetSkillTip.status.selectedIndex = 0;
            UpdateTalentInfo();
        }

        if (type == 2)
        {
            this.PetSkillTip.status.selectedIndex = 1;
            UpdateSkillBookInfo();
        }
    }

    private void UpdateTalentInfo()
    {
        // var talent = ConfigUtils.GetPetAptitudeUnitById(id);
        PetTalent petTalent = talentInfo;
        var talent = ConfigUtils.GetPetAptitudeUnitById(petTalent.talentId);
        this.PetSkillTip.skillItem.icon = UIResource.GetPetTalentIcon(talent.Item.ToString());
        this.PetSkillTip.skillItem.qualityCtrl.selectedIndex = talent.Quality - 1;
        this.PetSkillTip.skillName.text = ConfigUtils.GetTextById(talent.Name);
        this.PetSkillTip.typeCtrl.selectedIndex = talent.Type - 1;
        
        // ConfigSkillAchieveUnit skillAchieveUnit = ConfigUtils.GetSkillAchieveById(talent.Id);
        // int attrId = int.Parse(skillAchieveUnit.CommonAttr.Split(',')[0]);
        // double value = double.Parse(skillAchieveUnit.CommonAttr.Split(',')[1]);

        List<PetBattleAttr> list = petTalent.BattleAttrs;
        
        this.PetSkillTip.petQuality.qualityCtrl.selectedIndex = talent.Quality - 1;
        // string lastValue = EquipManager.Instance.SetAttributeValue(attrId, value, true);
        string lastValue = EquipManager.Instance.SetAttributeValue(list[0].AttrId, list[0].AttrVal, true);
        this.PetSkillTip.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(talent.Doc), lastValue);;
    }

    private void UpdateSkillBookInfo()
    {
        var skillBook = ConfigUtils.GetPetSkillBookUnitById(petBookSlot.BookId);
        // this.PetSkillTip.skillItem.icon = UIResource.GetItemUrl(skillBook.Icon.ToString());
        this.PetSkillTip.skillItem.icon = UIResource.GetPetSkillBookIcon(skillBook.Icon.ToString());
        this.PetSkillTip.skillItem.qualityCtrl.selectedIndex = skillBook.Quality - 1;
        this.PetSkillTip.skillName.text = ConfigUtils.GetTextById(skillBook.Name);
        this.PetSkillTip.typeCtrl.selectedIndex = skillBook.Type - 1;
        this.PetSkillTip.petQuality.qualityCtrl.selectedIndex = skillBook.Quality - 1;
        // this.PetSkillTip.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(skillBook.Doc),skillBook.Time);;
        // var skillAchieveUnit = ConfigUtils.GetSkillAchieveById(skillBook.Id);
        // this.PetSkillTip.skillDesc.text = StringUtils.Format(skillAchieveUnit.Doc,skillAchieveUnit.LastTime);//ConfigUtils.GetTextById(skillAchieveUnit.Doc)
        
        if (skillBook.DocCommon.Equals("0"))//为0时服务器下发
        {
            PetBattleAttr petBattleAttr = new PetBattleAttr();
            petBattleAttr = petBookSlot.battleAttrList[0];
            string value = (petBattleAttr.AttrVal/100).ToString("f2");
            this.PetSkillTip.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(skillBook.Doc),value);
        }
        else
        {
            //读取配置表
            double value = double.Parse(skillBook.DocCommon)/100;
            this.PetSkillTip.skillDesc.text = StringUtils.Format(ConfigUtils.GetTextById(skillBook.Doc),value);
        }
        
    }

}