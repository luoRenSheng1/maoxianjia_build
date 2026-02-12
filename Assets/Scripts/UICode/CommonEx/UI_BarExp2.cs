/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BarExp2 : GProgressBar
    {
        public Controller maxCtrl;
        public const string URL = "ui://5moj1x39p9n0dxy7w";

        public static UI_BarExp2 CreateInstance()
        {
            return (UI_BarExp2)UIPackage.CreateObject("CommonEx", "BarExp2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
        }
    }
}