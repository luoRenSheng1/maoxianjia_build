/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FirstPay
{
    public partial class UI_FirstPayMain : GComponent
    {
        public Controller isPay;
        public Controller c1;
        public GComponent frame1;
        public GTextField day1;
        public GList FPRewardList;
        public GTextField day2;
        public UI_FirstItem FPReward2;
        public GTextField day3;
        public UI_FirstItem FPReward3;
        public UI_FirstPayBtn payBtn;
        public GButton getFPRewardBtn;
        public GButton closeBtn;
        public const string URL = "ui://ksm9s29pez2lg";

        public static UI_FirstPayMain CreateInstance()
        {
            return (UI_FirstPayMain)UIPackage.CreateObject("FirstPay", "FirstPayMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isPay = GetController("isPay");
            c1 = GetController("c1");
            frame1 = (GComponent)GetChild("frame1");
            day1 = (GTextField)GetChild("day1");
            FPRewardList = (GList)GetChild("FPRewardList");
            day2 = (GTextField)GetChild("day2");
            FPReward2 = (UI_FirstItem)GetChild("FPReward2");
            day3 = (GTextField)GetChild("day3");
            FPReward3 = (UI_FirstItem)GetChild("FPReward3");
            payBtn = (UI_FirstPayBtn)GetChild("payBtn");
            getFPRewardBtn = (GButton)GetChild("getFPRewardBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}