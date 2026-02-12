/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace NoOperation
{
    public partial class UI_Slider : GSlider
    {
        public GLoader A1;
        public GLoader A2;
        public GLoader A3;
        public GLoader A4;
        public GGroup groupArrow;
        public Transition t0;
        public const string URL = "ui://88ncwg57uobv2";

        public static UI_Slider CreateInstance()
        {
            return (UI_Slider)UIPackage.CreateObject("NoOperation", "Slider");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            A1 = (GLoader)GetChild("A1");
            A2 = (GLoader)GetChild("A2");
            A3 = (GLoader)GetChild("A3");
            A4 = (GLoader)GetChild("A4");
            groupArrow = (GGroup)GetChild("groupArrow");
            t0 = GetTransition("t0");
        }
    }
}