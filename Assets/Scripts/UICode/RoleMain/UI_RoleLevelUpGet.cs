/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleLevelUpGet : GComponent
    {
        public GGraph spine;
        public UI_RoleAttrItem attrItem;
        public GLabel roleNameLb;
        public GButton closeBtn;
        public Transition t0;
        public const string URL = "ui://m37flevdrm2cdxy6o";

        public static UI_RoleLevelUpGet CreateInstance()
        {
            return (UI_RoleLevelUpGet)UIPackage.CreateObject("RoleMain", "RoleLevelUpGet");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            attrItem = (UI_RoleAttrItem)GetChild("attrItem");
            roleNameLb = (GLabel)GetChild("roleNameLb");
            closeBtn = (GButton)GetChild("closeBtn");
            t0 = GetTransition("t0");
        }
    }
}