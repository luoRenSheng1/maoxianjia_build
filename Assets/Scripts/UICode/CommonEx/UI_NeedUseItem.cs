/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_NeedUseItem : GButton
    {
        public Controller colorType;
        public Controller qualityCtrl;
        public GTextField hasNumLb;
        public GTextField totalLb;
        public const string URL = "ui://5moj1x39jpak1n";

        public static UI_NeedUseItem CreateInstance()
        {
            return (UI_NeedUseItem)UIPackage.CreateObject("CommonEx", "NeedUseItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            colorType = GetController("colorType");
            qualityCtrl = GetController("qualityCtrl");
            hasNumLb = (GTextField)GetChild("hasNumLb");
            totalLb = (GTextField)GetChild("totalLb");
        }
    }
}