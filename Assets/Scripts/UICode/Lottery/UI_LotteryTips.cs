/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteryTips : GComponent
    {
        public GComponent frame;
        public GRichTextField content;
        public GButton closeBtn;
        public const string URL = "ui://6izp804wju03w";

        public static UI_LotteryTips CreateInstance()
        {
            return (UI_LotteryTips)UIPackage.CreateObject("Lottery", "LotteryTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            content = (GRichTextField)GetChild("content");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}