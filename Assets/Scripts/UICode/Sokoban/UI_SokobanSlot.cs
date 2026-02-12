/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanSlot : GComponent
    {
        public GLoader icon;
        public const string URL = "ui://2nawooiyosqg1p";

        public static UI_SokobanSlot CreateInstance()
        {
            return (UI_SokobanSlot)UIPackage.CreateObject("Sokoban", "SokobanSlot");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
        }
    }
}