/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_UpLvBtn : GButton
    {
        public GTextField lvLb;
        public const string URL = "ui://ddc23erlef8udxyas";

        public static UI_UpLvBtn CreateInstance()
        {
            return (UI_UpLvBtn)UIPackage.CreateObject("Equip", "UpLvBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lvLb = (GTextField)GetChild("lvLb");
        }
    }
}