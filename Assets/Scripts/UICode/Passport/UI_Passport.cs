/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_Passport : GComponent
    {
        public Controller tabCtrl;
        public GGraph spine;
        public GTextField titleLb;
        public GTextField timeLb;
        public UI_LeftTabBtn leftBtn;
        public UI_RightTabBtn rightBtn;
        public UI_PassPortPanel Passport;
        public UI_PassTaskPanel PassTask;
        public UI_PassBarExp expBar;
        public GButton txzIcon;
        public GTextField passLvLb;
        public GButton closeBtn;
        public UI_AdvancePassportBtn advanceBtn;
        public const string URL = "ui://2pcsnr2kr0ab0";

        public static UI_Passport CreateInstance()
        {
            return (UI_Passport)UIPackage.CreateObject("Passport", "Passport");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tabCtrl = GetController("tabCtrl");
            spine = (GGraph)GetChild("spine");
            titleLb = (GTextField)GetChild("titleLb");
            timeLb = (GTextField)GetChild("timeLb");
            leftBtn = (UI_LeftTabBtn)GetChild("leftBtn");
            rightBtn = (UI_RightTabBtn)GetChild("rightBtn");
            Passport = (UI_PassPortPanel)GetChild("Passport");
            PassTask = (UI_PassTaskPanel)GetChild("PassTask");
            expBar = (UI_PassBarExp)GetChild("expBar");
            txzIcon = (GButton)GetChild("txzIcon");
            passLvLb = (GTextField)GetChild("passLvLb");
            closeBtn = (GButton)GetChild("closeBtn");
            advanceBtn = (UI_AdvancePassportBtn)GetChild("advanceBtn");
        }
    }
}