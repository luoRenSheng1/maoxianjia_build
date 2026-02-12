/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BtnCurrency : GButton
    {
        public Controller itemType;
        public GLoader clicker;
        public GTextField txtValue;
        public GButton btnAdd;
        public Transition reacts;
        public const string URL = "ui://0anhreylbtyu3g";

        public static UI_BtnCurrency CreateInstance()
        {
            return (UI_BtnCurrency)UIPackage.CreateObject("Common", "BtnCurrency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemType = GetController("itemType");
            clicker = (GLoader)GetChild("clicker");
            txtValue = (GTextField)GetChild("txtValue");
            btnAdd = (GButton)GetChild("btnAdd");
            reacts = GetTransition("reacts");
        }
    }
}