/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Chat
{
    public partial class UI_TabBtn : GButton
    {
        public GComponent redDot;
        public const string URL = "ui://x1rxl2idr13re";

        public static UI_TabBtn CreateInstance()
        {
            return (UI_TabBtn)UIPackage.CreateObject("Chat", "TabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (GComponent)GetChild("redDot");
        }
    }
}