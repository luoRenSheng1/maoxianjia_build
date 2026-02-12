/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_battleBg : GComponent
    {
        public GLoader map_bg0;
        public GLoader map_bg;
        public GLoader map_middle;
        public GLoader cloud0;
        public GLoader cloud1;
        public GList mapList2;
        public Transition sceneT;
        public const string URL = "ui://s7x7ku0nwrejdxy0x";

        public static UI_battleBg CreateInstance()
        {
            return (UI_battleBg)UIPackage.CreateObject("Lobby", "battleBg");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            map_bg0 = (GLoader)GetChild("map_bg0");
            map_bg = (GLoader)GetChild("map_bg");
            map_middle = (GLoader)GetChild("map_middle");
            cloud0 = (GLoader)GetChild("cloud0");
            cloud1 = (GLoader)GetChild("cloud1");
            mapList2 = (GList)GetChild("mapList2");
            sceneT = GetTransition("sceneT");
        }
    }
}