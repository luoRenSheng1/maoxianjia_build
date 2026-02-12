/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_RankingItem : GComponent
    {
        public Controller numCtrl;
        public Controller status;
        public GTextField num;
        public GLabel heroIcon;
        public GTextField pName;
        public GTextField fightingCapacity;
        public GList rewardList;
        public GButton challengeBtn;
        public GButton standCollarBtn;
        public const string URL = "ui://zoxecbv2u5mt1d";

        public static UI_RankingItem CreateInstance()
        {
            return (UI_RankingItem)UIPackage.CreateObject("PVPMap", "RankingItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            numCtrl = GetController("numCtrl");
            status = GetController("status");
            num = (GTextField)GetChild("num");
            heroIcon = (GLabel)GetChild("heroIcon");
            pName = (GTextField)GetChild("pName");
            fightingCapacity = (GTextField)GetChild("fightingCapacity");
            rewardList = (GList)GetChild("rewardList");
            challengeBtn = (GButton)GetChild("challengeBtn");
            standCollarBtn = (GButton)GetChild("standCollarBtn");
        }
    }
}