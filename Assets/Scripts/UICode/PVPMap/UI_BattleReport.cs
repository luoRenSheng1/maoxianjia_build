/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_BattleReport : GComponent
    {
        public Controller typeCtrl;
        public GTextField title;
        public GList reportList;
        public const string URL = "ui://zoxecbv2u5mt17";

        public static UI_BattleReport CreateInstance()
        {
            return (UI_BattleReport)UIPackage.CreateObject("PVPMap", "BattleReport");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            title = (GTextField)GetChild("title");
            reportList = (GList)GetChild("reportList");
        }
    }
}