/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_AttrTips : GComponent
    {
        public GList attrList;
        public GLoader dirIcon;
        public const string URL = "ui://5moj1x39fr3pdxyd0";

        public static UI_AttrTips CreateInstance()
        {
            return (UI_AttrTips)UIPackage.CreateObject("CommonEx", "AttrTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrList = (GList)GetChild("attrList");
            dirIcon = (GLoader)GetChild("dirIcon");
        }
    }
}