/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleOptAniBtn : GComponent
    {
        public UI_RoleOptBtn levelUpBtn;
        public Transition t0;
        public const string URL = "ui://m37flevddr5adxy6a";

        public static UI_RoleOptAniBtn CreateInstance()
        {
            return (UI_RoleOptAniBtn)UIPackage.CreateObject("RoleMain", "RoleOptAniBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            levelUpBtn = (UI_RoleOptBtn)GetChild("levelUpBtn");
            t0 = GetTransition("t0");
        }
    }
}