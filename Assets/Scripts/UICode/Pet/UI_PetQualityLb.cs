/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetQualityLb : GComponent
    {
        public Controller qualityCtrl;
        public GLoader icon;
        public GTextField title;
        public const string URL = "ui://lxs2h4ifhz5cdxy7d";

        public static UI_PetQualityLb CreateInstance()
        {
            return (UI_PetQualityLb)UIPackage.CreateObject("Pet", "PetQualityLb");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            qualityCtrl = GetController("qualityCtrl");
            icon = (GLoader)GetChild("icon");
            title = (GTextField)GetChild("title");
        }
    }
}