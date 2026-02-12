/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Mail
{
    public partial class UI_MailItem : GComponent
    {
        public Controller mailState;
        public GTextField titleLb;
        public GTextField endTimeLb;
        public GList rewardList;
        public GTextField sendTimeLb;
        public GButton getBtn;
        public const string URL = "ui://h1wohbjovcwq8";

        public static UI_MailItem CreateInstance()
        {
            return (UI_MailItem)UIPackage.CreateObject("Mail", "MailItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mailState = GetController("mailState");
            titleLb = (GTextField)GetChild("titleLb");
            endTimeLb = (GTextField)GetChild("endTimeLb");
            rewardList = (GList)GetChild("rewardList");
            sendTimeLb = (GTextField)GetChild("sendTimeLb");
            getBtn = (GButton)GetChild("getBtn");
        }
    }
}