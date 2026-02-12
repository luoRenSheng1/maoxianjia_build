/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PmItem_popup : GComponent
    {
        public GList list;
        public const string URL = "ui://0anhreylot6fdxy2m";

        public static UI_PmItem_popup CreateInstance()
        {
            return (UI_PmItem_popup)UIPackage.CreateObject("Common", "PmItem_popup");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}