/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_tqTHItem : GComponent
    {
        public Controller cardType;
        public Controller rewardCtrl;
        public GLoader bgUrl;
        public GList rewardList;
        public GRichTextField attrLb;
        public GLoader img;
        public GButton getRwBtn;
        public GButton buyBtn;
        public const string URL = "ui://nmzfxo89ftc9m";

        public static UI_tqTHItem CreateInstance()
        {
            return (UI_tqTHItem)UIPackage.CreateObject("Shop", "tqTHItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cardType = GetController("cardType");
            rewardCtrl = GetController("rewardCtrl");
            bgUrl = (GLoader)GetChild("bgUrl");
            rewardList = (GList)GetChild("rewardList");
            attrLb = (GRichTextField)GetChild("attrLb");
            img = (GLoader)GetChild("img");
            getRwBtn = (GButton)GetChild("getRwBtn");
            buyBtn = (GButton)GetChild("buyBtn");
        }
    }
}