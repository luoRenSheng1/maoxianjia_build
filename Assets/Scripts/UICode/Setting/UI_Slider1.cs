/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_Slider1 : GSlider
    {
        public Controller ctrl;
        public const string URL = "ui://zs0w02qtha87v";

        public static UI_Slider1 CreateInstance()
        {
            return (UI_Slider1)UIPackage.CreateObject("Setting", "Slider1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
        }
    }
}