/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_EmptyAdBtn2 : GButton
    {
        public GTextField adCntLb;
        public const string URL = "ui://5moj1x39ju03dxy59";

        public static UI_EmptyAdBtn2 CreateInstance()
        {
            return (UI_EmptyAdBtn2)UIPackage.CreateObject("CommonEx", "EmptyAdBtn2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            adCntLb = (GTextField)GetChild("adCntLb");
        }
    }
}