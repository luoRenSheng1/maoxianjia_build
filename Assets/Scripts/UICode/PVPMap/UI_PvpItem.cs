/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_PvpItem : GComponent
    {
        public GList pvpList;
        public const string URL = "ui://zoxecbv2xezf4d";

        public static UI_PvpItem CreateInstance()
        {
            return (UI_PvpItem)UIPackage.CreateObject("PVPMap", "PvpItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pvpList = (GList)GetChild("pvpList");
        }
    }
}