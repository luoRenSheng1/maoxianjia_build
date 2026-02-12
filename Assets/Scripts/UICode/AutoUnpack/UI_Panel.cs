/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AutoUnpack
{
    public partial class UI_Panel : GComponent
    {
        public GButton btnStart;
        public GButton checkBoxFullTickets;
        public GButton checkBoxFilter1;
        public GButton entry1;
        public GButton openNumBtn;
        public GButton equipBtn;
        public GButton checkBoxFilter2;
        public GButton entry2;
        public GButton entry3;
        public GButton entry4;
        public GButton closeBtn;
        public const string URL = "ui://rr0734r5mmjj20";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("AutoUnpack", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            btnStart = (GButton)GetChild("btnStart");
            checkBoxFullTickets = (GButton)GetChild("checkBoxFullTickets");
            checkBoxFilter1 = (GButton)GetChild("checkBoxFilter1");
            entry1 = (GButton)GetChild("entry1");
            openNumBtn = (GButton)GetChild("openNumBtn");
            equipBtn = (GButton)GetChild("equipBtn");
            checkBoxFilter2 = (GButton)GetChild("checkBoxFilter2");
            entry2 = (GButton)GetChild("entry2");
            entry3 = (GButton)GetChild("entry3");
            entry4 = (GButton)GetChild("entry4");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}