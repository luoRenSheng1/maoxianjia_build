/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnTopped : GButton
    {
        public Transition eff;
        public const string URL = "ui://s7x7ku0nw02ndxy2s";

        public static UI_BtnTopped CreateInstance()
        {
            return (UI_BtnTopped)UIPackage.CreateObject("Lobby", "BtnTopped");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            eff = GetTransition("eff");
        }
    }
}