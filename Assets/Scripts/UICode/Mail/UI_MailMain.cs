/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Mail
{
    public partial class UI_MailMain : GComponent
    {
        public Controller state;
        public GComponent frame;
        public GList mailList;
        public GButton oneKeyDel;
        public GButton oneKeyGet;
        public GButton closeBtn;
        public const string URL = "ui://h1wohbjovcwq7";

        public static UI_MailMain CreateInstance()
        {
            return (UI_MailMain)UIPackage.CreateObject("Mail", "MailMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            state = GetController("state");
            frame = (GComponent)GetChild("frame");
            mailList = (GList)GetChild("mailList");
            oneKeyDel = (GButton)GetChild("oneKeyDel");
            oneKeyGet = (GButton)GetChild("oneKeyGet");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}