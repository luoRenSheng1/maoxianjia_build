/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnFucntionIcon : GButton
    {
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0nlx83dxxyq";

        public static UI_BtnFucntionIcon CreateInstance()
        {
            return (UI_BtnFucntionIcon)UIPackage.CreateObject("Lobby", "BtnFucntionIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (GComponent)GetChild("redDot");
        }
    }
}