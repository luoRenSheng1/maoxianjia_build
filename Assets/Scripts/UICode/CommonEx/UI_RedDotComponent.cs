/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_RedDotComponent : GComponent
    {
        public Controller countType;
        public GImage redDotObj;
        public GTextField countText;
        public const string URL = "ui://5moj1x39jl15dxy3s";

        public static UI_RedDotComponent CreateInstance()
        {
            return (UI_RedDotComponent)UIPackage.CreateObject("CommonEx", "RedDotComponent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            countType = GetController("countType");
            redDotObj = (GImage)GetChild("redDotObj");
            countText = (GTextField)GetChild("countText");
        }
    }
}