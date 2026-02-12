/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_battleMap : GComponent
    {
        public GList mapList;
        public const string URL = "ui://s7x7ku0nvs59dxy1a";

        public static UI_battleMap CreateInstance()
        {
            return (UI_battleMap)UIPackage.CreateObject("Lobby", "battleMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapList = (GList)GetChild("mapList");
        }
    }
}