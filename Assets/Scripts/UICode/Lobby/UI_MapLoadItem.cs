/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_MapLoadItem : GComponent
    {
        public GLoader mapLoader;
        public const string URL = "ui://s7x7ku0nktejdxy1g";

        public static UI_MapLoadItem CreateInstance()
        {
            return (UI_MapLoadItem)UIPackage.CreateObject("Lobby", "MapLoadItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapLoader = (GLoader)GetChild("mapLoader");
        }
    }
}