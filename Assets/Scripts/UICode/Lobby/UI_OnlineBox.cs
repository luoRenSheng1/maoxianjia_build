/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_OnlineBox : GButton
    {
        public Controller openCtrl;
        public GTextField timeLb;
        public const string URL = "ui://s7x7ku0nng62dxy3s";

        public static UI_OnlineBox CreateInstance()
        {
            return (UI_OnlineBox)UIPackage.CreateObject("Lobby", "OnlineBox");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            openCtrl = GetController("openCtrl");
            timeLb = (GTextField)GetChild("timeLb");
        }
    }
}