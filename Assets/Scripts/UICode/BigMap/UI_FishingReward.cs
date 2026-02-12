/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingReward : GComponent
    {
        public Controller status;
        public GComponent frame;
        public GList rewardList;
        public GButton getRewardBtn;
        public GButton againFishingBtn;
        public const string URL = "ui://pdufy3keka12igk";

        public static UI_FishingReward CreateInstance()
        {
            return (UI_FishingReward)UIPackage.CreateObject("BigMap", "FishingReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            frame = (GComponent)GetChild("frame");
            rewardList = (GList)GetChild("rewardList");
            getRewardBtn = (GButton)GetChild("getRewardBtn");
            againFishingBtn = (GButton)GetChild("againFishingBtn");
        }
    }
}