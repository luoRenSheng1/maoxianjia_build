/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_HuntingShopBuyTips : GComponent
    {
        public GComponent frame;
        public GButton itemCom;
        public GTextField name;
        public GTextField desc;
        public GTextInput num;
        public GButton reduceBtn;
        public GButton addBtn;
        public UI_ShopBuyBtn buyBtn;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3ketdju5p";

        public static UI_HuntingShopBuyTips CreateInstance()
        {
            return (UI_HuntingShopBuyTips)UIPackage.CreateObject("BigMap", "HuntingShopBuyTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            itemCom = (GButton)GetChild("itemCom");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            num = (GTextInput)GetChild("num");
            reduceBtn = (GButton)GetChild("reduceBtn");
            addBtn = (GButton)GetChild("addBtn");
            buyBtn = (UI_ShopBuyBtn)GetChild("buyBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}