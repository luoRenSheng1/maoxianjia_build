/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_SoftGuideLayer : GComponent
    {
        public Controller ctrl;
        public Controller rolePos;
        public GGraph window;
        public Transition t0;
        public const string URL = "ui://0anhreyluq40dxy44";

        public static UI_SoftGuideLayer CreateInstance()
        {
            return (UI_SoftGuideLayer)UIPackage.CreateObject("Common", "SoftGuideLayer");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            rolePos = GetController("rolePos");
            window = (GGraph)GetChild("window");
            t0 = GetTransition("t0");
        }
    }
}