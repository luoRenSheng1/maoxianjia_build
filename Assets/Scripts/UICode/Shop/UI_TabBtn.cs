/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_TabBtn : GButton
    {
        public Controller redCtrl;
        public const string URL = "ui://nmzfxo89ftc96";

        public static UI_TabBtn CreateInstance()
        {
            return (UI_TabBtn)UIPackage.CreateObject("Shop", "TabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redCtrl = GetController("redCtrl");
        }
    }
}