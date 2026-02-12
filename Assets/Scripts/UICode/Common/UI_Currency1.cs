/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_Currency1 : GButton
    {
        public GTextField txtValue;
        public GButton addBtn;
        public const string URL = "ui://0anhreylhz5cdxy96";

        public static UI_Currency1 CreateInstance()
        {
            return (UI_Currency1)UIPackage.CreateObject("Common", "Currency1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
            addBtn = (GButton)GetChild("addBtn");
        }
    }
}