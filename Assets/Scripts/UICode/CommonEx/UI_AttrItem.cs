/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_AttrItem : GComponent
    {
        public GTextField attrName;
        public GTextField attValue;
        public const string URL = "ui://5moj1x39fr3pdxyd1";

        public static UI_AttrItem CreateInstance()
        {
            return (UI_AttrItem)UIPackage.CreateObject("CommonEx", "AttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrName = (GTextField)GetChild("attrName");
            attValue = (GTextField)GetChild("attValue");
        }
    }
}