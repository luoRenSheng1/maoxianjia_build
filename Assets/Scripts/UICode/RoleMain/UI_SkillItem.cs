/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_SkillItem : GComponent
    {
        public Controller hasCtrl;
        public Controller ctrlQuality;
        public Controller showHandCtrl;
        public GLoader icon;
        public const string URL = "ui://m37flevdrm2cdxy6i";

        public static UI_SkillItem CreateInstance()
        {
            return (UI_SkillItem)UIPackage.CreateObject("RoleMain", "SkillItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasCtrl = GetController("hasCtrl");
            ctrlQuality = GetController("ctrlQuality");
            showHandCtrl = GetController("showHandCtrl");
            icon = (GLoader)GetChild("icon");
        }
    }
}