/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingBar : GComponent
    {
        public GLoader bg;
        public GLoader yellowBar;
        public GLoader greenBar;
        public GLoader blueBar;
        public GLoader fishFlag;
        public const string URL = "ui://pdufy3keka12igy";

        public static UI_FishingBar CreateInstance()
        {
            return (UI_FishingBar)UIPackage.CreateObject("BigMap", "FishingBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GLoader)GetChild("bg");
            yellowBar = (GLoader)GetChild("yellowBar");
            greenBar = (GLoader)GetChild("greenBar");
            blueBar = (GLoader)GetChild("blueBar");
            fishFlag = (GLoader)GetChild("fishFlag");
        }
    }
}