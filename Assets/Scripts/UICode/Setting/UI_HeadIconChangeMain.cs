/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_HeadIconChangeMain : GComponent
    {
        public GComponent frame1;
        public UI_HeadIconChange headIconChange;
        public GButton closeBtn;
        public const string URL = "ui://zs0w02qtbaxfy";

        public static UI_HeadIconChangeMain CreateInstance()
        {
            return (UI_HeadIconChangeMain)UIPackage.CreateObject("Setting", "HeadIconChangeMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame1 = (GComponent)GetChild("frame1");
            headIconChange = (UI_HeadIconChange)GetChild("headIconChange");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}