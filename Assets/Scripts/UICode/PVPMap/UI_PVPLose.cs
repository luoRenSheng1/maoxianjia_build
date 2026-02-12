/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_PVPLose : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GLabel myHeadIcon;
        public GLabel enemyHeadIcon;
        public GTextField myNameLb;
        public GTextField enemyNameLb;
        public const string URL = "ui://zoxecbv2auqc2d";

        public static UI_PVPLose CreateInstance()
        {
            return (UI_PVPLose)UIPackage.CreateObject("PVPMap", "PVPLose");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            myHeadIcon = (GLabel)GetChild("myHeadIcon");
            enemyHeadIcon = (GLabel)GetChild("enemyHeadIcon");
            myNameLb = (GTextField)GetChild("myNameLb");
            enemyNameLb = (GTextField)GetChild("enemyNameLb");
        }
    }
}