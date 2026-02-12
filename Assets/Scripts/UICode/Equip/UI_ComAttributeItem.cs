/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ComAttributeItem : GComponent
    {
        public Controller status;
        public GTextField txtAttrName;
        public GTextField txtAttrValue;
        public const string URL = "ui://ddc23erlskp6o";

        public static UI_ComAttributeItem CreateInstance()
        {
            return (UI_ComAttributeItem)UIPackage.CreateObject("Equip", "ComAttributeItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            txtAttrName = (GTextField)GetChild("txtAttrName");
            txtAttrValue = (GTextField)GetChild("txtAttrValue");
        }
    }
}