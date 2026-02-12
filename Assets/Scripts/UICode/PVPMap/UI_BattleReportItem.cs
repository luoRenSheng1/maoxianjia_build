/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_BattleReportItem : GComponent
    {
        public GTextField timeLb;
        public GTextField content;
        public const string URL = "ui://zoxecbv2u5mt18";

        public static UI_BattleReportItem CreateInstance()
        {
            return (UI_BattleReportItem)UIPackage.CreateObject("PVPMap", "BattleReportItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            timeLb = (GTextField)GetChild("timeLb");
            content = (GTextField)GetChild("content");
        }
    }
}