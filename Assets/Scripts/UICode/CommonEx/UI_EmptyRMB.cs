/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_EmptyRMB : GButton
    {
        public Controller moneyType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://5moj1x39cy7ndxy3a";

        public static UI_EmptyRMB CreateInstance()
        {
            return (UI_EmptyRMB)UIPackage.CreateObject("CommonEx", "EmptyRMB");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            moneyType = GetController("moneyType");
            moneyLb1 = (GTextField)GetChild("moneyLb1");
            moneyLb2 = (GTextField)GetChild("moneyLb2");
        }
    }
}