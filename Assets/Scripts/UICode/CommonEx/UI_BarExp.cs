/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BarExp : GProgressBar
    {
        public Controller maxCtrl;
        public Controller barType;
        public GTextField maxLevel;
        public const string URL = "ui://5moj1x39eb0tdxxzf";

        public static UI_BarExp CreateInstance()
        {
            return (UI_BarExp)UIPackage.CreateObject("CommonEx", "BarExp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            barType = GetController("barType");
            maxLevel = (GTextField)GetChild("maxLevel");
        }
    }
}