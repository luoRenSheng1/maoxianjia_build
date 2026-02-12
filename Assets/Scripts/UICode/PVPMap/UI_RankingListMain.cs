/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_RankingListMain : GComponent
    {
        public GLoader bg;
        public GLoader bg2;
        public GButton helpBtn;
        public GTextField num1Lb;
        public GTextField num2Lb;
        public GTextField num3Lb;
        public GGraph spine1;
        public GGraph spine2;
        public GGraph spine3;
        public GTextField timeLeft;
        public GTextField cntLb;
        public GButton addNumBtn;
        public GList rankingList;
        public UI_MyselfItem myselfItem;
        public GButton closeBtn;
        public GButton reportBtn;
        public GLoader img;
        public const string URL = "ui://zoxecbv2u5mt1b";

        public static UI_RankingListMain CreateInstance()
        {
            return (UI_RankingListMain)UIPackage.CreateObject("PVPMap", "RankingListMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GLoader)GetChild("bg");
            bg2 = (GLoader)GetChild("bg2");
            helpBtn = (GButton)GetChild("helpBtn");
            num1Lb = (GTextField)GetChild("num1Lb");
            num2Lb = (GTextField)GetChild("num2Lb");
            num3Lb = (GTextField)GetChild("num3Lb");
            spine1 = (GGraph)GetChild("spine1");
            spine2 = (GGraph)GetChild("spine2");
            spine3 = (GGraph)GetChild("spine3");
            timeLeft = (GTextField)GetChild("timeLeft");
            cntLb = (GTextField)GetChild("cntLb");
            addNumBtn = (GButton)GetChild("addNumBtn");
            rankingList = (GList)GetChild("rankingList");
            myselfItem = (UI_MyselfItem)GetChild("myselfItem");
            closeBtn = (GButton)GetChild("closeBtn");
            reportBtn = (GButton)GetChild("reportBtn");
            img = (GLoader)GetChild("img");
        }
    }
}