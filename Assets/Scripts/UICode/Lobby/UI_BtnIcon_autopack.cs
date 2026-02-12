/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnIcon_autopack : GButton
    {
        public Controller lockCtrl;
        public Transition autoEff;
        public const string URL = "ui://s7x7ku0nkv23dxy3v";

        public static UI_BtnIcon_autopack CreateInstance()
        {
            return (UI_BtnIcon_autopack)UIPackage.CreateObject("Lobby", "BtnIcon_autopack");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            autoEff = GetTransition("autoEff");
        }
    }
}