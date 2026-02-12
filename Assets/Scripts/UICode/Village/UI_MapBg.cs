/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_MapBg : GComponent
    {
        public GList mapList;
        public const string URL = "ui://8glegefcpchvs";

        public static UI_MapBg CreateInstance()
        {
            return (UI_MapBg)UIPackage.CreateObject("Village", "MapBg");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapList = (GList)GetChild("mapList");
        }
    }
}