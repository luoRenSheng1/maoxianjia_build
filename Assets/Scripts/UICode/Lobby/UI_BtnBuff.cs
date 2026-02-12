/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBuff : GButton
    {
        public Controller itemType;
        public GLoader clicker;
        public GTextField txtValue;
        public Transition reacts;
        public const string URL = "ui://s7x7ku0na2kwdxyas";

        public static UI_BtnBuff CreateInstance()
        {
            return (UI_BtnBuff)UIPackage.CreateObject("Lobby", "BtnBuff");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemType = GetController("itemType");
            clicker = (GLoader)GetChild("clicker");
            txtValue = (GTextField)GetChild("txtValue");
            reacts = GetTransition("reacts");
        }
    }
}