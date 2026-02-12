/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_openEntryList : GComponent
    {
        public GList list;
        public const string URL = "ui://rr0734r5x3dk2z";

        public static UI_openEntryList CreateInstance()
        {
            return (UI_openEntryList)UIPackage.CreateObject("AutoUnpack", "openEntryList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}