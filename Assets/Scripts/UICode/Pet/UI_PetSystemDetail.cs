/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSystemDetail : GComponent
    {
        public Controller ctrl;
        public Controller tipsCtrl;
        public Controller upStatus;
        public Controller strongStatus;
        public Controller hasUpload;
        public Controller quality;
        public Controller isShow;
        public Controller TalentUnlock;
        public Controller SkillBookUnlock;
        public GButton currency1;
        public GButton currency2;
        public UI_Type type;
        public GButton btnAttr;
        public GList attrList;
        public UI_PetSkillBtn petSkillBtn;
        public GTextField skillName;
        public GRichTextField skillDesc;
        public GButton strongBtn;
        public GLoader lockIcon2;
        public GComponent strongBtnRed;
        public GLoader qualityIcon;
        public UI_PetQualityLb petQuality;
        public GGraph petSpine;
        public GTextField petName;
        public GTextField petLv;
        public GButton upLvBtn;
        public GLoader lockIcon;
        public GComponent upLvBtnRed;
        public GList petTalentList;
        public GTextField TalentUnlockText;
        public GList petBookList;
        public GTextField SkillBookUnlockText;
        public GList petList;
        public const string URL = "ui://lxs2h4ifs444c";

        public static UI_PetSystemDetail CreateInstance()
        {
            return (UI_PetSystemDetail)UIPackage.CreateObject("Pet", "PetSystemDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            tipsCtrl = GetController("tipsCtrl");
            upStatus = GetController("upStatus");
            strongStatus = GetController("strongStatus");
            hasUpload = GetController("hasUpload");
            quality = GetController("quality");
            isShow = GetController("isShow");
            TalentUnlock = GetController("TalentUnlock");
            SkillBookUnlock = GetController("SkillBookUnlock");
            currency1 = (GButton)GetChild("currency1");
            currency2 = (GButton)GetChild("currency2");
            type = (UI_Type)GetChild("type");
            btnAttr = (GButton)GetChild("btnAttr");
            attrList = (GList)GetChild("attrList");
            petSkillBtn = (UI_PetSkillBtn)GetChild("petSkillBtn");
            skillName = (GTextField)GetChild("skillName");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            strongBtn = (GButton)GetChild("strongBtn");
            lockIcon2 = (GLoader)GetChild("lockIcon2");
            strongBtnRed = (GComponent)GetChild("strongBtnRed");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            petQuality = (UI_PetQualityLb)GetChild("petQuality");
            petSpine = (GGraph)GetChild("petSpine");
            petName = (GTextField)GetChild("petName");
            petLv = (GTextField)GetChild("petLv");
            upLvBtn = (GButton)GetChild("upLvBtn");
            lockIcon = (GLoader)GetChild("lockIcon");
            upLvBtnRed = (GComponent)GetChild("upLvBtnRed");
            petTalentList = (GList)GetChild("petTalentList");
            TalentUnlockText = (GTextField)GetChild("TalentUnlockText");
            petBookList = (GList)GetChild("petBookList");
            SkillBookUnlockText = (GTextField)GetChild("SkillBookUnlockText");
            petList = (GList)GetChild("petList");
        }
    }
}