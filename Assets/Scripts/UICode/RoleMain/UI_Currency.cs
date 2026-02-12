/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_Currency : GLabel
    {
        public GTextField txtValue;
        public GButton addBtn;
        public const string URL = "ui://m37flevdozj91l";

        public static UI_Currency CreateInstance()
        {
            return (UI_Currency)UIPackage.CreateObject("RoleMain", "Currency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
            addBtn = (GButton)GetChild("addBtn");
        }
    }
}