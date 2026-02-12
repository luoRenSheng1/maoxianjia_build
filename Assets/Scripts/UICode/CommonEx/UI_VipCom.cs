/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_VipCom : GButton
    {
        public UI_RedDotComponent redDot;
        public const string URL = "ui://5moj1x39vnmwdxy4l";

        public static UI_VipCom CreateInstance()
        {
            return (UI_VipCom)UIPackage.CreateObject("CommonEx", "VipCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (UI_RedDotComponent)GetChild("redDot");
        }
    }
}