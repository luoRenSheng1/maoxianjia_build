/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_TaskItem : GComponent
    {
        public Controller type;
        public Controller statuCtrl;
        public UI_TaskBtn item;
        public GTextField name;
        public GProgressBar bar;
        public GButton takeBtn;
        public GButton giveUpTaskBtn;
        public GButton getRewardBtn;
        public GComponent redPoint;
        public GButton submitBtn;
        public const string URL = "ui://pdufy3kep9n015";

        public static UI_TaskItem CreateInstance()
        {
            return (UI_TaskItem)UIPackage.CreateObject("BigMap", "TaskItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            statuCtrl = GetController("statuCtrl");
            item = (UI_TaskBtn)GetChild("item");
            name = (GTextField)GetChild("name");
            bar = (GProgressBar)GetChild("bar");
            takeBtn = (GButton)GetChild("takeBtn");
            giveUpTaskBtn = (GButton)GetChild("giveUpTaskBtn");
            getRewardBtn = (GButton)GetChild("getRewardBtn");
            redPoint = (GComponent)GetChild("redPoint");
            submitBtn = (GButton)GetChild("submitBtn");
        }
    }
}