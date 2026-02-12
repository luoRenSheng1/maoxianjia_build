/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBoxLv : GButton
    {
        public GTextField txtBoxLv;
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0nt5mtdxy0s";

        public static UI_BtnBoxLv CreateInstance()
        {
            return (UI_BtnBoxLv)UIPackage.CreateObject("Lobby", "BtnBoxLv");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtBoxLv = (GTextField)GetChild("txtBoxLv");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}