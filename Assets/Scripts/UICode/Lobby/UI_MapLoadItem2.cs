/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_MapLoadItem2 : GComponent
    {
        public GLoader bg2;
        public const string URL = "ui://s7x7ku0nucd4dxybw";

        public static UI_MapLoadItem2 CreateInstance()
        {
            return (UI_MapLoadItem2)UIPackage.CreateObject("Lobby", "MapLoadItem2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg2 = (GLoader)GetChild("bg2");
        }
    }
}