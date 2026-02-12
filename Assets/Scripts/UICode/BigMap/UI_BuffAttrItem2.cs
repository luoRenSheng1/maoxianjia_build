/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_BuffAttrItem2 : GComponent
    {
        public GTextField attrValue;
        public const string URL = "ui://pdufy3kep2jcidu";

        public static UI_BuffAttrItem2 CreateInstance()
        {
            return (UI_BuffAttrItem2)UIPackage.CreateObject("BigMap", "BuffAttrItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrValue = (GTextField)GetChild("attrValue");
        }
    }
}