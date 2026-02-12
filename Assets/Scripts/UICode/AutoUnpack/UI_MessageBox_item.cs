/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_MessageBox_item : GButton
    {
        public Controller suoCtrl;
        public const string URL = "ui://rr0734r5mmjj2c";

        public static UI_MessageBox_item CreateInstance()
        {
            return (UI_MessageBox_item)UIPackage.CreateObject("AutoUnpack", "MessageBox_item");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            suoCtrl = GetController("suoCtrl");
        }
    }
}