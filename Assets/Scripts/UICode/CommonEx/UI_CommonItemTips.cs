/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonItemTips : GComponent
    {
        public UI_ItemCom item;
        public UI_qualityLabel pName;
        public GTextField pContent;
        public GTextField pLv;
        public const string URL = "ui://5moj1x39w61tdxy38";

        public static UI_CommonItemTips CreateInstance()
        {
            return (UI_CommonItemTips)UIPackage.CreateObject("CommonEx", "CommonItemTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            item = (UI_ItemCom)GetChild("item");
            pName = (UI_qualityLabel)GetChild("pName");
            pContent = (GTextField)GetChild("pContent");
            pLv = (GTextField)GetChild("pLv");
        }
    }
}