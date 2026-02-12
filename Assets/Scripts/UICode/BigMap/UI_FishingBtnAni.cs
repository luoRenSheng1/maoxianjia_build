/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingBtnAni : GComponent
    {
        public GButton fishingBtn;
        public Transition t0;
        public const string URL = "ui://pdufy3keka12igx";

        public static UI_FishingBtnAni CreateInstance()
        {
            return (UI_FishingBtnAni)UIPackage.CreateObject("BigMap", "FishingBtnAni");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            fishingBtn = (GButton)GetChild("fishingBtn");
            t0 = GetTransition("t0");
        }
    }
}