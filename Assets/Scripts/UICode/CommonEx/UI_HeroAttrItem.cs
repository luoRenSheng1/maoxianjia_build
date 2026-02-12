/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_HeroAttrItem : GLabel
    {
        public Controller attrType;
        public const string URL = "ui://5moj1x39ozj9dxy5a";

        public static UI_HeroAttrItem CreateInstance()
        {
            return (UI_HeroAttrItem)UIPackage.CreateObject("CommonEx", "HeroAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrType = GetController("attrType");
        }
    }
}