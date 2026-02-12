/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ShopBuyBtn : GButton
    {
        public GTextField num;
        public const string URL = "ui://pdufy3ketdju5q";

        public static UI_ShopBuyBtn CreateInstance()
        {
            return (UI_ShopBuyBtn)UIPackage.CreateObject("BigMap", "ShopBuyBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            num = (GTextField)GetChild("num");
        }
    }
}