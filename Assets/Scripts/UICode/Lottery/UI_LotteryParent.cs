/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteryParent : GComponent
    {
        public GLoader bg;
        public GComponent lotteryParent;
        public const string URL = "ui://6izp804wtcgl3";

        public static UI_LotteryParent CreateInstance()
        {
            return (UI_LotteryParent)UIPackage.CreateObject("Lottery", "LotteryParent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GLoader)GetChild("bg");
            lotteryParent = (GComponent)GetChild("lotteryParent");
        }
    }
}