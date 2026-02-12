/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_MapLoadItem3 : GComponent
    {
        public GLoader bg3;
        public const string URL = "ui://s7x7ku0nucd4dxybv";

        public static UI_MapLoadItem3 CreateInstance()
        {
            return (UI_MapLoadItem3)UIPackage.CreateObject("Lobby", "MapLoadItem3");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg3 = (GLoader)GetChild("bg3");
        }
    }
}