/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteryGetReward : GComponent
    {
        public GComponent frame;
        public GButton adGetBtn;
        public GButton item;
        public GTextField itemNameLb;
        public GButton normalGet;
        public const string URL = "ui://6izp804wjxfvz";

        public static UI_LotteryGetReward CreateInstance()
        {
            return (UI_LotteryGetReward)UIPackage.CreateObject("Lottery", "LotteryGetReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            adGetBtn = (GButton)GetChild("adGetBtn");
            item = (GButton)GetChild("item");
            itemNameLb = (GTextField)GetChild("itemNameLb");
            normalGet = (GButton)GetChild("normalGet");
        }
    }
}