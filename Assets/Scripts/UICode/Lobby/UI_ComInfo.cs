/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ComInfo : GButton
    {
        public Controller rewardCtrl;
        public Controller showTask;
        public Controller taskType;
        public Controller showHandle;
        public GButton rewardItem;
        public GTextField typeName;
        public GTextField txtProgress;
        public UI_TaskBarExp taskBar;
        public GTextField typeName2;
        public GTextField txtProgress2;
        public GTextField reward;
        public GTextField taskName;
        public const string URL = "ui://s7x7ku0npdpadxxyv";

        public static UI_ComInfo CreateInstance()
        {
            return (UI_ComInfo)UIPackage.CreateObject("Lobby", "ComInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            showTask = GetController("showTask");
            taskType = GetController("taskType");
            showHandle = GetController("showHandle");
            rewardItem = (GButton)GetChild("rewardItem");
            typeName = (GTextField)GetChild("typeName");
            txtProgress = (GTextField)GetChild("txtProgress");
            taskBar = (UI_TaskBarExp)GetChild("taskBar");
            typeName2 = (GTextField)GetChild("typeName2");
            txtProgress2 = (GTextField)GetChild("txtProgress2");
            reward = (GTextField)GetChild("reward");
            taskName = (GTextField)GetChild("taskName");
        }
    }
}