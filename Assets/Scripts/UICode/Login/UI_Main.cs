/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_Main : GComponent
    {
        public Controller login;
        public Controller ctrlPage;
        public Controller loading;
        public GLoader bg;
        public GLoader titlebg;
        public GButton plus_12;
        public GTextField txtAppVersion;
        public UI_Announcement announcement;
        public GGraph hero;
        public GButton btnLoginPage;
        public GButton btnServer;
        public GButton btnKeFu;
        public GButton btnGongGao;
        public UI_LoginPage loginPage;
        public Transition t0;
        public const string URL = "ui://h85hm7vmvggn0";

        public static UI_Main CreateInstance()
        {
            return (UI_Main)UIPackage.CreateObject("Login", "Main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            login = GetController("login");
            ctrlPage = GetController("ctrlPage");
            loading = GetController("loading");
            bg = (GLoader)GetChild("bg");
            titlebg = (GLoader)GetChild("titlebg");
            plus_12 = (GButton)GetChild("plus_12");
            txtAppVersion = (GTextField)GetChild("txtAppVersion");
            announcement = (UI_Announcement)GetChild("announcement");
            hero = (GGraph)GetChild("hero");
            btnLoginPage = (GButton)GetChild("btnLoginPage");
            btnServer = (GButton)GetChild("btnServer");
            btnKeFu = (GButton)GetChild("btnKeFu");
            btnGongGao = (GButton)GetChild("btnGongGao");
            loginPage = (UI_LoginPage)GetChild("loginPage");
            t0 = GetTransition("t0");
        }
    }
}