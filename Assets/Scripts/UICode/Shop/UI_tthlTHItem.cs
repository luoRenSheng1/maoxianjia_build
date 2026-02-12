/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_tthlTHItem : GComponent
    {
        public Controller rewardCtrl;
        public Controller reachCtrl;
        public GTextField ttTitle;
        public GTextField dayTitle;
        public GList rewardList;
        public GButton gotoBtn;
        public GButton getRwBtn;
        public GButton waitGetBtn;
        public const string URL = "ui://nmzfxo89ftc9j";

        public static UI_tthlTHItem CreateInstance()
        {
            return (UI_tthlTHItem)UIPackage.CreateObject("Shop", "tthlTHItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            reachCtrl = GetController("reachCtrl");
            ttTitle = (GTextField)GetChild("ttTitle");
            dayTitle = (GTextField)GetChild("dayTitle");
            rewardList = (GList)GetChild("rewardList");
            gotoBtn = (GButton)GetChild("gotoBtn");
            getRwBtn = (GButton)GetChild("getRwBtn");
            waitGetBtn = (GButton)GetChild("waitGetBtn");
        }
    }
}