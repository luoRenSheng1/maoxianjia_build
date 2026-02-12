/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_AthleticsMain : GComponent
    {
        public GComponent frame;
        public GTextField title;
        public UI_PvpItem PvpItem;
        public GButton closeBtn;
        public const string URL = "ui://zoxecbv2u5mt16";

        public static UI_AthleticsMain CreateInstance()
        {
            return (UI_AthleticsMain)UIPackage.CreateObject("PVPMap", "AthleticsMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            title = (GTextField)GetChild("title");
            PvpItem = (UI_PvpItem)GetChild("PvpItem");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}