/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSkillBtn : GButton
    {
        public Controller qualityCtrl;
        public GTextField num;
        public const string URL = "ui://lxs2h4ifhz5cdxy78";

        public static UI_PetSkillBtn CreateInstance()
        {
            return (UI_PetSkillBtn)UIPackage.CreateObject("Pet", "PetSkillBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            num = (GTextField)GetChild("num");
        }
    }
}