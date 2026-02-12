/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_GiftListItem : GComponent
    {
        public Controller bgCtrl;
        public Controller limitCtrl;
        public GTextField nameLb;
        public GTextField limitLb;
        public GTextField timeLb;
        public GLoader itemIcon;
        public GGraph spine;
        public GButton buyBtn;
        public GList rewardList;
        public GTextField rebate;
        public Transition t0;
        public const string URL = "ui://i7ojazuuftc92n";

        public static UI_GiftListItem CreateInstance()
        {
            return (UI_GiftListItem)UIPackage.CreateObject("Summon", "GiftListItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bgCtrl = GetController("bgCtrl");
            limitCtrl = GetController("limitCtrl");
            nameLb = (GTextField)GetChild("nameLb");
            limitLb = (GTextField)GetChild("limitLb");
            timeLb = (GTextField)GetChild("timeLb");
            itemIcon = (GLoader)GetChild("itemIcon");
            spine = (GGraph)GetChild("spine");
            buyBtn = (GButton)GetChild("buyBtn");
            rewardList = (GList)GetChild("rewardList");
            rebate = (GTextField)GetChild("rebate");
            t0 = GetTransition("t0");
        }
    }
}