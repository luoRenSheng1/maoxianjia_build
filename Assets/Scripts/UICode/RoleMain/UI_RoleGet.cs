/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleGet : GComponent
    {
        public GGraph spine;
        public GLabel roleNameLb;
        public GButton closeBtn;
        public Transition t0;
        public const string URL = "ui://m37flevdrm2cdxy6k";

        public static UI_RoleGet CreateInstance()
        {
            return (UI_RoleGet)UIPackage.CreateObject("RoleMain", "RoleGet");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            roleNameLb = (GLabel)GetChild("roleNameLb");
            closeBtn = (GButton)GetChild("closeBtn");
            t0 = GetTransition("t0");
        }
    }
}