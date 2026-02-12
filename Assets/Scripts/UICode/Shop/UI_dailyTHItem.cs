/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_dailyTHItem : GComponent
    {
        public Controller rewardCtrl;
        public GTextField dailyTitle;
        public GTextField limitLb;
        public GList rewardList;
        public GButton getRwBtn;
        public GButton buyBtn;
        public const string URL = "ui://nmzfxo89ftc9c";

        public static UI_dailyTHItem CreateInstance()
        {
            return (UI_dailyTHItem)UIPackage.CreateObject("Shop", "dailyTHItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            dailyTitle = (GTextField)GetChild("dailyTitle");
            limitLb = (GTextField)GetChild("limitLb");
            rewardList = (GList)GetChild("rewardList");
            getRwBtn = (GButton)GetChild("getRwBtn");
            buyBtn = (GButton)GetChild("buyBtn");
        }
    }
}