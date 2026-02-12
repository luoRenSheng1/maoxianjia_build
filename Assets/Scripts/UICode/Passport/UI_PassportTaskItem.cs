/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassportTaskItem : GComponent
    {
        public Controller taskType;
        public Controller advanceCtrl;
        public Controller rewardCtrl;
        public GTextField timeLb;
        public GButton item;
        public GTextField taskNameLb;
        public UI_TaskBarExp expBar;
        public GButton getRwBtn;
        public const string URL = "ui://2pcsnr2kr0ab4";

        public static UI_PassportTaskItem CreateInstance()
        {
            return (UI_PassportTaskItem)UIPackage.CreateObject("Passport", "PassportTaskItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            taskType = GetController("taskType");
            advanceCtrl = GetController("advanceCtrl");
            rewardCtrl = GetController("rewardCtrl");
            timeLb = (GTextField)GetChild("timeLb");
            item = (GButton)GetChild("item");
            taskNameLb = (GTextField)GetChild("taskNameLb");
            expBar = (UI_TaskBarExp)GetChild("expBar");
            getRwBtn = (GButton)GetChild("getRwBtn");
        }
    }
}