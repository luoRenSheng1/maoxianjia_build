/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_EquipCurrency : GLabel
    {
        public GTextField txtValue;
        public const string URL = "ui://ddc23erlef8udxyau";

        public static UI_EquipCurrency CreateInstance()
        {
            return (UI_EquipCurrency)UIPackage.CreateObject("Equip", "EquipCurrency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
        }
    }
}