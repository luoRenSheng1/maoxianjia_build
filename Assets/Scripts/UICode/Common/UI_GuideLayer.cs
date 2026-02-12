/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GuideLayer : GComponent
    {
        public Controller ctrl;
        public Controller rolePos;
        public GImage touchEff;
        public GGraph window;
        public GGraph window1;
        public GLoader window2;
        public Transition t1;
        public const string URL = "ui://0anhreyluq40dxy42";

        public static UI_GuideLayer CreateInstance()
        {
            return (UI_GuideLayer)UIPackage.CreateObject("Common", "GuideLayer");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            rolePos = GetController("rolePos");
            touchEff = (GImage)GetChild("touchEff");
            window = (GGraph)GetChild("window");
            window1 = (GGraph)GetChild("window1");
            window2 = (GLoader)GetChild("window2");
            t1 = GetTransition("t1");
        }
    }
}