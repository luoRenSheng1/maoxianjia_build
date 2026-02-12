/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_HeroBarExp : GProgressBar
    {
        public Controller maxCtrl;
        public GTextField maxLevel;
        public const string URL = "ui://m37flevducd4dxydd";

        public static UI_HeroBarExp CreateInstance()
        {
            return (UI_HeroBarExp)UIPackage.CreateObject("RoleMain", "HeroBarExp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            maxLevel = (GTextField)GetChild("maxLevel");
        }
    }
}