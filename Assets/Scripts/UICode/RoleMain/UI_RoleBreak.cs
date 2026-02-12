/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleBreak : GComponent
    {
        public GGraph spine;
        public GLabel roleNameLb;
        public GButton closeBtn;
        public GLoader3D starSpine;
        public Transition t0;
        public const string URL = "ui://m37flevdozj91o";

        public static UI_RoleBreak CreateInstance()
        {
            return (UI_RoleBreak)UIPackage.CreateObject("RoleMain", "RoleBreak");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            roleNameLb = (GLabel)GetChild("roleNameLb");
            closeBtn = (GButton)GetChild("closeBtn");
            starSpine = (GLoader3D)GetChild("starSpine");
            t0 = GetTransition("t0");
        }
    }
}