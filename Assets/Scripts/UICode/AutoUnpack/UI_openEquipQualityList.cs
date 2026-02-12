/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_openEquipQualityList : GComponent
    {
        public GList list;
        public const string URL = "ui://rr0734r5x3dk33";

        public static UI_openEquipQualityList CreateInstance()
        {
            return (UI_openEquipQualityList)UIPackage.CreateObject("AutoUnpack", "openEquipQualityList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}