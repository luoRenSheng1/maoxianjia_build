/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanBoxClip : GComponent
    {
        public Controller c1;
        public UI_SokobanBox icon;
        public Transition t0;
        public const string URL = "ui://2nawooiypmwj1x";

        public static UI_SokobanBoxClip CreateInstance()
        {
            return (UI_SokobanBoxClip)UIPackage.CreateObject("Sokoban", "SokobanBoxClip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
            icon = (UI_SokobanBox)GetChild("icon");
            t0 = GetTransition("t0");
        }
    }
}