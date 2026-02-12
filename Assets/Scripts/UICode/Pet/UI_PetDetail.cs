/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetDetail : GComponent
    {
        public Controller ctrl;
        public Controller tipsCtrl;
        public Controller isStation;
        public Controller TalentUnlock;
        public Controller SkillBookUnlock;
        public UI_PetItem petItem;
        public GTextField petName;
        public GTextField gorwValue;
        public GTextField inherit;
        public UI_PetQualityLb petQuality;
        public UI_PetSkillBtn petSkill;
        public GTextField skillName;
        public GRichTextField skillDesc;
        public GList petTalentList;
        public GTextField TalentUnlockText;
        public GList petBookList;
        public GTextField SkillBookUnlockText;
        public UI_RecyclePetBtn recyclePetBtn;
        public GButton downBtn;
        public GButton uploadBtn;
        public GButton replaceBtn;
        public GButton getBtn;
        public GButton closeBtn;
        public const string URL = "ui://lxs2h4ifk6272k";

        public static UI_PetDetail CreateInstance()
        {
            return (UI_PetDetail)UIPackage.CreateObject("Pet", "PetDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            tipsCtrl = GetController("tipsCtrl");
            isStation = GetController("isStation");
            TalentUnlock = GetController("TalentUnlock");
            SkillBookUnlock = GetController("SkillBookUnlock");
            petItem = (UI_PetItem)GetChild("petItem");
            petName = (GTextField)GetChild("petName");
            gorwValue = (GTextField)GetChild("gorwValue");
            inherit = (GTextField)GetChild("inherit");
            petQuality = (UI_PetQualityLb)GetChild("petQuality");
            petSkill = (UI_PetSkillBtn)GetChild("petSkill");
            skillName = (GTextField)GetChild("skillName");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            petTalentList = (GList)GetChild("petTalentList");
            TalentUnlockText = (GTextField)GetChild("TalentUnlockText");
            petBookList = (GList)GetChild("petBookList");
            SkillBookUnlockText = (GTextField)GetChild("SkillBookUnlockText");
            recyclePetBtn = (UI_RecyclePetBtn)GetChild("recyclePetBtn");
            downBtn = (GButton)GetChild("downBtn");
            uploadBtn = (GButton)GetChild("uploadBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
            getBtn = (GButton)GetChild("getBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}