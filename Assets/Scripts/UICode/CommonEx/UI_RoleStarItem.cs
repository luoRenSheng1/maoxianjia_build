/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_RoleStarItem : GComponent
    {
        public Controller type;
        public Controller lockCtrl;
        public const string URL = "ui://5moj1x39ozj9dxy5m";

        public static UI_RoleStarItem CreateInstance()
        {
            return (UI_RoleStarItem)UIPackage.CreateObject("CommonEx", "RoleStarItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            lockCtrl = GetController("lockCtrl");
        }
    }
}