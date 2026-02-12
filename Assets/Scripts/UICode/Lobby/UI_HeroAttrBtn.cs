/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_HeroAttrBtn : GButton
    {
        public Controller ctrl;
        public GTextField costLb;
        public const string URL = "ui://s7x7ku0nozj9dxy55";

        public static UI_HeroAttrBtn CreateInstance()
        {
            return (UI_HeroAttrBtn)UIPackage.CreateObject("Lobby", "HeroAttrBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            costLb = (GTextField)GetChild("costLb");
        }
    }
}