/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_HuntingShopItem : GButton
    {
        public Controller statuCtrl;
        public GTextField name;
        public GButton itemCom;
        public GTextField num;
        public const string URL = "ui://pdufy3ketdju5o";

        public static UI_HuntingShopItem CreateInstance()
        {
            return (UI_HuntingShopItem)UIPackage.CreateObject("BigMap", "HuntingShopItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            statuCtrl = GetController("statuCtrl");
            name = (GTextField)GetChild("name");
            itemCom = (GButton)GetChild("itemCom");
            num = (GTextField)GetChild("num");
        }
    }
}