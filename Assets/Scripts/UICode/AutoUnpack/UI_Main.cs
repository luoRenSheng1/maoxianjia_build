/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_Main : GComponent
    {
        public GComponent frame1;
        public UI_Panel panel;
        public const string URL = "ui://rr0734r5un3j2h";

        public static UI_Main CreateInstance()
        {
            return (UI_Main)UIPackage.CreateObject("AutoUnpack", "Main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame1 = (GComponent)GetChild("frame1");
            panel = (UI_Panel)GetChild("panel");
        }
    }
}