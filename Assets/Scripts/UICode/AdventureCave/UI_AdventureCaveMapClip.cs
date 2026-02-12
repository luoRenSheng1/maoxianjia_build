/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AdventureCave
{
    public partial class UI_AdventureCaveMapClip : GComponent
    {
        public UI_AdventureCaveMap panel;
        public const string URL = "ui://z350mxkhkoot31";

        public static UI_AdventureCaveMapClip CreateInstance()
        {
            return (UI_AdventureCaveMapClip)UIPackage.CreateObject("AdventureCave", "AdventureCaveMapClip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            panel = (UI_AdventureCaveMap)GetChild("panel");
        }
    }
}