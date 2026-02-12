/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace SevenDay
{
    public partial class UI_SevenDay : GComponent
    {
        public GComponent frame;
        public GList dayList;
        public GTextField timeLeft;
        public GButton closeBtn;
        public const string URL = "ui://1tqiwa7amg7g6";

        public static UI_SevenDay CreateInstance()
        {
            return (UI_SevenDay)UIPackage.CreateObject("SevenDay", "SevenDay");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            dayList = (GList)GetChild("dayList");
            timeLeft = (GTextField)GetChild("timeLeft");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}