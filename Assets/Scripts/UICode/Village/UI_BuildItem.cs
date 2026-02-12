/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_BuildItem : GButton
    {
        public Controller lockCtrl;
        public const string URL = "ui://8glegefcpchvi";

        public static UI_BuildItem CreateInstance()
        {
            return (UI_BuildItem)UIPackage.CreateObject("Village", "BuildItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
        }
    }
}