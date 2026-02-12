/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSkillTips : GComponent
    {
        public Controller typeCtrl;
        public Controller status;
        public GComponent frame;
        public UI_PetSkillBtn skillItem;
        public GTextField skillName;
        public GTextField skillType;
        public UI_PetQualityLb petQuality;
        public GRichTextField skillDesc;
        public GButton closeBtn;
        public const string URL = "ui://lxs2h4ifhz5cdxy7f";

        public static UI_PetSkillTips CreateInstance()
        {
            return (UI_PetSkillTips)UIPackage.CreateObject("Pet", "PetSkillTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            status = GetController("status");
            frame = (GComponent)GetChild("frame");
            skillItem = (UI_PetSkillBtn)GetChild("skillItem");
            skillName = (GTextField)GetChild("skillName");
            skillType = (GTextField)GetChild("skillType");
            petQuality = (UI_PetQualityLb)GetChild("petQuality");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}