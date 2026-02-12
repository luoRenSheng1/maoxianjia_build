/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_dailyTHItemAll : GComponent
    {
        public Controller rewardCtrl;
        public GTextField titlelb;
        public GTextField limitLb;
        public GButton getRwBtn;
        public GButton buyBtn;
        public GButton item;
        public const string URL = "ui://nmzfxo89ftc9y";

        public static UI_dailyTHItemAll CreateInstance()
        {
            return (UI_dailyTHItemAll)UIPackage.CreateObject("Shop", "dailyTHItemAll");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            titlelb = (GTextField)GetChild("titlelb");
            limitLb = (GTextField)GetChild("limitLb");
            getRwBtn = (GButton)GetChild("getRwBtn");
            buyBtn = (GButton)GetChild("buyBtn");
            item = (GButton)GetChild("item");
        }
    }
}