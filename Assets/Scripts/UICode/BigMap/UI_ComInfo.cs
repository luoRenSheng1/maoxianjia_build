/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ComInfo : GButton
    {
        public Controller rewardCtrl;
        public Controller showTask;
        public Controller taskType;
        public Controller showHandle;
        public Controller type;
        public GButton rewardItem;
        public GTextField taskName;
        public GTextField reward;
        public GTextField typeName;
        public GTextField typeName2;
        public GTextField txtProgress;
        public GTextField txtProgress2;
        public UI_TaskBarExp taskBar;
        public const string URL = "ui://pdufy3ke9kp72c";

        public static UI_ComInfo CreateInstance()
        {
            return (UI_ComInfo)UIPackage.CreateObject("BigMap", "ComInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            showTask = GetController("showTask");
            taskType = GetController("taskType");
            showHandle = GetController("showHandle");
            type = GetController("type");
            rewardItem = (GButton)GetChild("rewardItem");
            taskName = (GTextField)GetChild("taskName");
            reward = (GTextField)GetChild("reward");
            typeName = (GTextField)GetChild("typeName");
            typeName2 = (GTextField)GetChild("typeName2");
            txtProgress = (GTextField)GetChild("txtProgress");
            txtProgress2 = (GTextField)GetChild("txtProgress2");
            taskBar = (UI_TaskBarExp)GetChild("taskBar");
        }
    }
}