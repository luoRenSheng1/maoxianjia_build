/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_BattleReportMain : GComponent
    {
        public GComponent frame;
        public UI_BattleReport battleReport;
        public GButton closeBtn;
        public const string URL = "ui://zoxecbv2u5mt1a";

        public static UI_BattleReportMain CreateInstance()
        {
            return (UI_BattleReportMain)UIPackage.CreateObject("PVPMap", "BattleReportMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            battleReport = (UI_BattleReport)GetChild("battleReport");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}