/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_EquipTabBtn : GButton
    {
        public Controller redCtrl;
        public Controller lockCtrl;
        public const string URL = "ui://ddc23erlef8udxy9z";

        public static UI_EquipTabBtn CreateInstance()
        {
            return (UI_EquipTabBtn)UIPackage.CreateObject("Equip", "EquipTabBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redCtrl = GetController("redCtrl");
            lockCtrl = GetController("lockCtrl");
        }
    }
}