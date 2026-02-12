/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Mail
{
    public partial class UI_MailInfo : GComponent
    {
        public Controller stateType;
        public GComponent frame;
        public GTextField titleLb;
        public GTextField contentLb;
        public GButton getBtn;
        public GTextField endTimeLb;
        public GButton delBtn1;
        public GList rewardList;
        public GButton delBtn2;
        public const string URL = "ui://h1wohbjojmb99";

        public static UI_MailInfo CreateInstance()
        {
            return (UI_MailInfo)UIPackage.CreateObject("Mail", "MailInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            stateType = GetController("stateType");
            frame = (GComponent)GetChild("frame");
            titleLb = (GTextField)GetChild("titleLb");
            contentLb = (GTextField)GetChild("contentLb");
            getBtn = (GButton)GetChild("getBtn");
            endTimeLb = (GTextField)GetChild("endTimeLb");
            delBtn1 = (GButton)GetChild("delBtn1");
            rewardList = (GList)GetChild("rewardList");
            delBtn2 = (GButton)GetChild("delBtn2");
        }
    }
}