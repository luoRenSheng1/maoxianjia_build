/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_LoginPage : GComponent
    {
        public Controller hasAcount;
        public GButton btnClose;
        public GButton btnLogin;
        public GComboBox serverPop;
        public GTextInput txtAccount;
        public GButton taptapBtn;
        public GButton userCheck;
        public GRichTextField linkLb;
        public const string URL = "ui://h85hm7vmnxr7p";

        public static UI_LoginPage CreateInstance()
        {
            return (UI_LoginPage)UIPackage.CreateObject("Login", "LoginPage");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasAcount = GetController("hasAcount");
            btnClose = (GButton)GetChild("btnClose");
            btnLogin = (GButton)GetChild("btnLogin");
            serverPop = (GComboBox)GetChild("serverPop");
            txtAccount = (GTextInput)GetChild("txtAccount");
            taptapBtn = (GButton)GetChild("taptapBtn");
            userCheck = (GButton)GetChild("userCheck");
            linkLb = (GRichTextField)GetChild("linkLb");
        }
    }
}