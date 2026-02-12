/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_BtnBuff : GButton
    {
        public Controller itemType;
        public GLoader clicker;
        public Transition reacts;
        public const string URL = "ui://pdufy3kex2p02f";

        public static UI_BtnBuff CreateInstance()
        {
            return (UI_BtnBuff)UIPackage.CreateObject("BigMap", "BtnBuff");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemType = GetController("itemType");
            clicker = (GLoader)GetChild("clicker");
            reacts = GetTransition("reacts");
        }
    }
}