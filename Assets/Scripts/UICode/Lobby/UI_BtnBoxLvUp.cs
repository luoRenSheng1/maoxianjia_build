/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBoxLvUp : GButton
    {
        public GComponent redDot;
        public const string URL = "ui://s7x7ku0nv65gdxy87";

        public static UI_BtnBoxLvUp CreateInstance()
        {
            return (UI_BtnBoxLvUp)UIPackage.CreateObject("Lobby", "BtnBoxLvUp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (GComponent)GetChild("redDot");
        }
    }
}