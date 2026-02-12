/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BarHpTween : GProgressBar
    {
        public Controller camp;
        public const string URL = "ui://5moj1x39ct04dxxzo";

        public static UI_BarHpTween CreateInstance()
        {
            return (UI_BarHpTween)UIPackage.CreateObject("CommonEx", "BarHpTween");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            camp = GetController("camp");
        }
    }
}