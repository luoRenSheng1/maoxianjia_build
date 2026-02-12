/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_MapLoadItem : GComponent
    {
        public GLoader mapLoader;
        public const string URL = "ui://zoxecbv2qc4nd";

        public static UI_MapLoadItem CreateInstance()
        {
            return (UI_MapLoadItem)UIPackage.CreateObject("PVPMap", "MapLoadItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapLoader = (GLoader)GetChild("mapLoader");
        }
    }
}