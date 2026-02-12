/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_MapBg : GComponent
    {
        public GLoader bg1;
        public GLoader bg2;
        public GLoader bg3;
        public GLoader bg4;
        public GLoader bg5;
        public GLoader bg6;
        public const string URL = "ui://pdufy3kefpdrid3";

        public static UI_MapBg CreateInstance()
        {
            return (UI_MapBg)UIPackage.CreateObject("BigMap", "MapBg");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg1 = (GLoader)GetChild("bg1");
            bg2 = (GLoader)GetChild("bg2");
            bg3 = (GLoader)GetChild("bg3");
            bg4 = (GLoader)GetChild("bg4");
            bg5 = (GLoader)GetChild("bg5");
            bg6 = (GLoader)GetChild("bg6");
        }
    }
}