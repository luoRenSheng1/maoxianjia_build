/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DailyTask
{
    public partial class UI_DailyTaskItem : GComponent
    {
        public Controller flagCtrl;
        public GButton item;
        public GTextField desc;
        public GProgressBar taskBar;
        public GButton rewardBtn;
        public const string URL = "ui://fdbg11jxsc7hf";

        public static UI_DailyTaskItem CreateInstance()
        {
            return (UI_DailyTaskItem)UIPackage.CreateObject("DailyTask", "DailyTaskItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            flagCtrl = GetController("flagCtrl");
            item = (GButton)GetChild("item");
            desc = (GTextField)GetChild("desc");
            taskBar = (GProgressBar)GetChild("taskBar");
            rewardBtn = (GButton)GetChild("rewardBtn");
        }
    }
}