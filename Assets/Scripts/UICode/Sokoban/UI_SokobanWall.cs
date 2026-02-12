/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanWall : GComponent
    {
        public GLoader icon1;
        public GLoader icon2;
        public const string URL = "ui://2nawooiyosqg1j";

        public static UI_SokobanWall CreateInstance()
        {
            return (UI_SokobanWall)UIPackage.CreateObject("Sokoban", "SokobanWall");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon1 = (GLoader)GetChild("icon1");
            icon2 = (GLoader)GetChild("icon2");
        }
    }
}