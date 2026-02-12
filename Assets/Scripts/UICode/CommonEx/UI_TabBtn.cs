/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_TabBtn : GButton
    {
        public Controller redCtrl;
        public Controller lockCtrl;
        public const string URL = "ui://5moj1x39p4bnc";

        public static UI_TabBtn CreateInstance()
        {
            return (UI_TabBtn)UIPackage.CreateObject("CommonEx", "TabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redCtrl = GetController("redCtrl");
            lockCtrl = GetController("lockCtrl");
        }
    }
}