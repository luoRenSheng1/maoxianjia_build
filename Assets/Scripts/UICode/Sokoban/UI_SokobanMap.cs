/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanMap : GComponent
    {
        public GLoader touch;
        public UI_SokobanMapClip map;
        public GLoader close;
        public GButton btnReset;
        public const string URL = "ui://2nawooiyosqg1f";

        public static UI_SokobanMap CreateInstance()
        {
            return (UI_SokobanMap)UIPackage.CreateObject("Sokoban", "SokobanMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            touch = (GLoader)GetChild("touch");
            map = (UI_SokobanMapClip)GetChild("map");
            close = (GLoader)GetChild("close");
            btnReset = (GButton)GetChild("btnReset");
        }
    }
}