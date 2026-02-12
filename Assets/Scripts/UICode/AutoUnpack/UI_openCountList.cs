/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_openCountList : GComponent
    {
        public GList list;
        public const string URL = "ui://rr0734r5x3dk2v";

        public static UI_openCountList CreateInstance()
        {
            return (UI_openCountList)UIPackage.CreateObject("AutoUnpack", "openCountList");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}