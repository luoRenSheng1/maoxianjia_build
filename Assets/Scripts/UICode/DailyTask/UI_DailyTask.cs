/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DailyTask
{
    public partial class UI_DailyTask : GComponent
    {
        public GComponent frame;
        public GTextField timeLb;
        public GList dailyList;
        public GButton closeBtn;
        public const string URL = "ui://fdbg11jxsc7ha";

        public static UI_DailyTask CreateInstance()
        {
            return (UI_DailyTask)UIPackage.CreateObject("DailyTask", "DailyTask");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            timeLb = (GTextField)GetChild("timeLb");
            dailyList = (GList)GetChild("dailyList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}