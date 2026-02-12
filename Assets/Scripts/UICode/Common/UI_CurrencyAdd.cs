/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_CurrencyAdd : GButton
    {
        public GTextField value;
        public GButton addBtn;
        public const string URL = "ui://0anhreylf34jdxy9l";

        public static UI_CurrencyAdd CreateInstance()
        {
            return (UI_CurrencyAdd)UIPackage.CreateObject("Common", "CurrencyAdd");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            value = (GTextField)GetChild("value");
            addBtn = (GButton)GetChild("addBtn");
        }
    }
}