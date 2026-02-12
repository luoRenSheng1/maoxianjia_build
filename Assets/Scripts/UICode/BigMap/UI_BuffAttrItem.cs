/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_BuffAttrItem : GComponent
    {
        public GTextField attr;
        public const string URL = "ui://pdufy3kep2jcidr";

        public static UI_BuffAttrItem CreateInstance()
        {
            return (UI_BuffAttrItem)UIPackage.CreateObject("BigMap", "BuffAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attr = (GTextField)GetChild("attr");
        }
    }
}