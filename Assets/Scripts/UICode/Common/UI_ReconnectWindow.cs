/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ReconnectWindow : GComponent
    {
        public GImage bg;
        public GTextField txt_title;
        public Transition t0;
        public const string URL = "ui://0anhreylb71r32";

        public static UI_ReconnectWindow CreateInstance()
        {
            return (UI_ReconnectWindow)UIPackage.CreateObject("Common", "ReconnectWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GImage)GetChild("bg");
            txt_title = (GTextField)GetChild("txt_title");
            t0 = GetTransition("t0");
        }
    }
}