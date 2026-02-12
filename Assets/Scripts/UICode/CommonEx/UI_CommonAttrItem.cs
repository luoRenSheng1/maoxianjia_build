/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonAttrItem : GLabel
    {
        public Controller type;
        public GTextField pContent;
        public const string URL = "ui://5moj1x39k627dxy6a";

        public static UI_CommonAttrItem CreateInstance()
        {
            return (UI_CommonAttrItem)UIPackage.CreateObject("CommonEx", "CommonAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            pContent = (GTextField)GetChild("pContent");
        }
    }
}