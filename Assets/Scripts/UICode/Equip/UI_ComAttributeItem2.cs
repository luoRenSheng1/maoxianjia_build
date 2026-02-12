/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ComAttributeItem2 : GComponent
    {
        public GTextField txtAttrName;
        public GTextField txtAttrValue;
        public const string URL = "ui://ddc23erlucd4dxy9u";

        public static UI_ComAttributeItem2 CreateInstance()
        {
            return (UI_ComAttributeItem2)UIPackage.CreateObject("Equip", "ComAttributeItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtAttrName = (GTextField)GetChild("txtAttrName");
            txtAttrValue = (GTextField)GetChild("txtAttrValue");
        }
    }
}