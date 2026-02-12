/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanBox : GComponent
    {
        public GLoader icon;
        public const string URL = "ui://2nawooiyosqg1i";

        public static UI_SokobanBox CreateInstance()
        {
            return (UI_SokobanBox)UIPackage.CreateObject("Sokoban", "SokobanBox");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
        }
    }
}