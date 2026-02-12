/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AdventureCave
{
    public partial class UI_AdventureCaveView : GComponent
    {
        public UI_AdventureCaveMapClip map;
        public GTextField ziti;
        public GLoader mask;
        public GLoader close;
        public Transition t0;
        public const string URL = "ui://z350mxkhkoot1c";

        public static UI_AdventureCaveView CreateInstance()
        {
            return (UI_AdventureCaveView)UIPackage.CreateObject("AdventureCave", "AdventureCaveView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            map = (UI_AdventureCaveMapClip)GetChild("map");
            ziti = (GTextField)GetChild("ziti");
            mask = (GLoader)GetChild("mask");
            close = (GLoader)GetChild("close");
            t0 = GetTransition("t0");
        }
    }
}