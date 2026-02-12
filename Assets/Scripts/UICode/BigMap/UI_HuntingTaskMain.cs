/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_HuntingTaskMain : GComponent
    {
        public GComponent frame;
        public GLabel currency;
        public GLabel goldCurrency;
        public GLabel diaCurrency;
        public GButton huntingShopBtn;
        public GTextField timeLb;
        public UI_TaskItem taskInfo;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3ketdju5m";

        public static UI_HuntingTaskMain CreateInstance()
        {
            return (UI_HuntingTaskMain)UIPackage.CreateObject("BigMap", "HuntingTaskMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            currency = (GLabel)GetChild("currency");
            goldCurrency = (GLabel)GetChild("goldCurrency");
            diaCurrency = (GLabel)GetChild("diaCurrency");
            huntingShopBtn = (GButton)GetChild("huntingShopBtn");
            timeLb = (GTextField)GetChild("timeLb");
            taskInfo = (UI_TaskItem)GetChild("taskInfo");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}