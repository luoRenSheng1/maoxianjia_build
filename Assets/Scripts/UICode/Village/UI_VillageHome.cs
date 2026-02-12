/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_VillageHome : GComponent
    {
        public UI_Panel panel;
        public UI_UserPanel userPanel;
        public GGraph weatherPos;
        public const string URL = "ui://8glegefcpchvl";

        public static UI_VillageHome CreateInstance()
        {
            return (UI_VillageHome)UIPackage.CreateObject("Village", "VillageHome");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            panel = (UI_Panel)GetChild("panel");
            userPanel = (UI_UserPanel)GetChild("userPanel");
            weatherPos = (GGraph)GetChild("weatherPos");
        }
    }
}