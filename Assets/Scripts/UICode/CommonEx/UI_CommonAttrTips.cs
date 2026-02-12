/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonAttrTips : GComponent
    {
        public GTextField pContentName;
        public GButton bossAttr;
        public GRichTextField pContentDesc;
        public const string URL = "ui://5moj1x39i18ndxy7t";

        public static UI_CommonAttrTips CreateInstance()
        {
            return (UI_CommonAttrTips)UIPackage.CreateObject("CommonEx", "CommonAttrTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pContentName = (GTextField)GetChild("pContentName");
            bossAttr = (GButton)GetChild("bossAttr");
            pContentDesc = (GRichTextField)GetChild("pContentDesc");
        }
    }
}