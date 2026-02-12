/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanMapClip : GComponent
    {
        public UI_SokobanMapClipPanel clip;
        public const string URL = "ui://2nawooiys3cg1w";

        public static UI_SokobanMapClip CreateInstance()
        {
            return (UI_SokobanMapClip)UIPackage.CreateObject("Sokoban", "SokobanMapClip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            clip = (UI_SokobanMapClipPanel)GetChild("clip");
        }
    }
}