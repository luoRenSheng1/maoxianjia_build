/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonBarExp : GProgressBar
    {
        public Controller maxCtrl;
        public Controller barType;
        public GTextField maxLevel;
        public const string URL = "ui://i7ojazuullb152";

        public static UI_SummonBarExp CreateInstance()
        {
            return (UI_SummonBarExp)UIPackage.CreateObject("Summon", "SummonBarExp");
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