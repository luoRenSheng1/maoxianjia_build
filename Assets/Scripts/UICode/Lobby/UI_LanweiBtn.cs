/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_LanweiBtn : GButton
    {
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0noceedxybr";

        public static UI_LanweiBtn CreateInstance()
        {
            return (UI_LanweiBtn)UIPackage.CreateObject("Lobby", "LanweiBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (GComponent)GetChild("redDot");
        }
    }
}