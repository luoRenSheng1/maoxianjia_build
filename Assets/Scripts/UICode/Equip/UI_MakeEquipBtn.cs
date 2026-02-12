/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_MakeEquipBtn : GButton
    {
        public Controller isLock;
        public GTextField num;
        public const string URL = "ui://ddc23erlef8udxyat";

        public static UI_MakeEquipBtn CreateInstance()
        {
            return (UI_MakeEquipBtn)UIPackage.CreateObject("Equip", "MakeEquipBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isLock = GetController("isLock");
            num = (GTextField)GetChild("num");
        }
    }
}