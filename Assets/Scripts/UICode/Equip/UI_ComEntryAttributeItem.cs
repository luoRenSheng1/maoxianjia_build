/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ComEntryAttributeItem : GComponent
    {
        public GTextField txtAttrName;
        public GTextField txtAttrValue;
        public const string URL = "ui://ddc23erlx3dk1f";

        public static UI_ComEntryAttributeItem CreateInstance()
        {
            return (UI_ComEntryAttributeItem)UIPackage.CreateObject("Equip", "ComEntryAttributeItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtAttrName = (GTextField)GetChild("txtAttrName");
            txtAttrValue = (GTextField)GetChild("txtAttrValue");
        }
    }
}