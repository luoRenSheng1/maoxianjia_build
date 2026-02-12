/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteryTask : GComponent
    {
        public Controller adCtrl;
        public Controller ctrl;
        public Controller rewardCtrl;
        public Controller doubleCtrl;
        public Controller lotterying;
        public Controller taskType;
        public GComponent frame;
        public UI_LotteryParent lotteryParent;
        public GImage lotteryPin;
        public GLoader bg;
        public GButton closeBtn;
        public GTextField countLb;
        public GButton check;
        public GButton lotteryBtn;
        public GList rewardList;
        public GTextField txtValue;
        public GTextField txtProgress;
        public GButton getRwBtn;
        public GTextField timeLeft;
        public GButton gotoBtn;
        public UI_LotteyBoxReward boxItem;
        public GButton tips;
        public GComponent enablePanel;
        public const string URL = "ui://6izp804wtcgl0";

        public static UI_LotteryTask CreateInstance()
        {
            return (UI_LotteryTask)UIPackage.CreateObject("Lottery", "LotteryTask");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            adCtrl = GetController("adCtrl");
            ctrl = GetController("ctrl");
            rewardCtrl = GetController("rewardCtrl");
            doubleCtrl = GetController("doubleCtrl");
            lotterying = GetController("lotterying");
            taskType = GetController("taskType");
            frame = (GComponent)GetChild("frame");
            lotteryParent = (UI_LotteryParent)GetChild("lotteryParent");
            lotteryPin = (GImage)GetChild("lotteryPin");
            bg = (GLoader)GetChild("bg");
            closeBtn = (GButton)GetChild("closeBtn");
            countLb = (GTextField)GetChild("countLb");
            check = (GButton)GetChild("check");
            lotteryBtn = (GButton)GetChild("lotteryBtn");
            rewardList = (GList)GetChild("rewardList");
            txtValue = (GTextField)GetChild("txtValue");
            txtProgress = (GTextField)GetChild("txtProgress");
            getRwBtn = (GButton)GetChild("getRwBtn");
            timeLeft = (GTextField)GetChild("timeLeft");
            gotoBtn = (GButton)GetChild("gotoBtn");
            boxItem = (UI_LotteyBoxReward)GetChild("boxItem");
            tips = (GButton)GetChild("tips");
            enablePanel = (GComponent)GetChild("enablePanel");
        }
    }
}