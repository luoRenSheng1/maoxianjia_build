/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BuryGiftPack
{
    public partial class UI_BuryGiftPack : GComponent
    {
        public Controller ctrl;
        public Controller typeCtrl;
        public Controller buyMax;
        public GButton buyBtn;
        public GList rewardList;
        public GButton closeBtn;
        public GButton gotoBtn;
        public GTextField titleLb;
        public GTextField cntLb;
        public GTextField rebate;
        public const string URL = "ui://zjhq3sc3khmx0";

        public static UI_BuryGiftPack CreateInstance()
        {
            return (UI_BuryGiftPack)UIPackage.CreateObject("BuryGiftPack", "BuryGiftPack");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            typeCtrl = GetController("typeCtrl");
            buyMax = GetController("buyMax");
            buyBtn = (GButton)GetChild("buyBtn");
            rewardList = (GList)GetChild("rewardList");
            closeBtn = (GButton)GetChild("closeBtn");
            gotoBtn = (GButton)GetChild("gotoBtn");
            titleLb = (GTextField)GetChild("titleLb");
            cntLb = (GTextField)GetChild("cntLb");
            rebate = (GTextField)GetChild("rebate");
        }
    }
}