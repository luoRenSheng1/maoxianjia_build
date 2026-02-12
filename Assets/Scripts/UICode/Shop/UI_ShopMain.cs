/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_ShopMain : GComponent
    {
        public Controller tabCtrl;
        public Controller fkznBtnCtrl;
        public GLoader bg;
        public GGraph spine;
        public GList tabList;
        public UI_dailyTH dailyTH;
        public UI_tthlTH tthlTH;
        public UI_tqTH tqTH;
        public UI_fengkuangTH fkTH;
        public GButton closeBtn;
        public UI_fengkuangBuyBtn fkznBtn;
        public UI_fengkuangBuyBtn fkznBtn2;
        public UI_fengkuangBuyBtn fkznBtn3;
        public const string URL = "ui://nmzfxo89ftc95";

        public static UI_ShopMain CreateInstance()
        {
            return (UI_ShopMain)UIPackage.CreateObject("Shop", "ShopMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tabCtrl = GetController("tabCtrl");
            fkznBtnCtrl = GetController("fkznBtnCtrl");
            bg = (GLoader)GetChild("bg");
            spine = (GGraph)GetChild("spine");
            tabList = (GList)GetChild("tabList");
            dailyTH = (UI_dailyTH)GetChild("dailyTH");
            tthlTH = (UI_tthlTH)GetChild("tthlTH");
            tqTH = (UI_tqTH)GetChild("tqTH");
            fkTH = (UI_fengkuangTH)GetChild("fkTH");
            closeBtn = (GButton)GetChild("closeBtn");
            fkznBtn = (UI_fengkuangBuyBtn)GetChild("fkznBtn");
            fkznBtn2 = (UI_fengkuangBuyBtn)GetChild("fkznBtn2");
            fkznBtn3 = (UI_fengkuangBuyBtn)GetChild("fkznBtn3");
        }
    }
}