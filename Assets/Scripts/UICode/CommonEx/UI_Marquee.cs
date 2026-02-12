/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_Marquee : GComponent
    {
        public GImage bg;
        public GTextField content;
        public const string URL = "ui://5moj1x39du07dxy6p";

        public static UI_Marquee CreateInstance()
        {
            return (UI_Marquee)UIPackage.CreateObject("CommonEx", "Marquee");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GImage)GetChild("bg");
            content = (GTextField)GetChild("content");
        }
    }
}