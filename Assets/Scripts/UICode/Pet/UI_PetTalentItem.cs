/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetTalentItem : GButton
    {
        public Controller lockCtrl;
        public Controller qualityCtrl;
        public GLoader qualityIcon;
        public GTextField talentName;
        public const string URL = "ui://lxs2h4ifhz5cdxy79";

        public static UI_PetTalentItem CreateInstance()
        {
            return (UI_PetTalentItem)UIPackage.CreateObject("Pet", "PetTalentItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            qualityCtrl = GetController("qualityCtrl");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            talentName = (GTextField)GetChild("talentName");
        }
    }
}