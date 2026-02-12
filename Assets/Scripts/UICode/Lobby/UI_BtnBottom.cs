/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBottom : GButton
    {
        public Controller hasRed;
        public Controller lockCtrl;
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0ndd3xdxxym";

        public static UI_BtnBottom CreateInstance()
        {
            return (UI_BtnBottom)UIPackage.CreateObject("Lobby", "BtnBottom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasRed = GetController("hasRed");
            lockCtrl = GetController("lockCtrl");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}