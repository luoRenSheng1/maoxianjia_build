/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingFail : GComponent
    {
        public GComponent frame;
        public GButton againFishingBtn;
        public const string URL = "ui://pdufy3keka12igl";

        public static UI_FishingFail CreateInstance()
        {
            return (UI_FishingFail)UIPackage.CreateObject("BigMap", "FishingFail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            againFishingBtn = (GButton)GetChild("againFishingBtn");
        }
    }
}