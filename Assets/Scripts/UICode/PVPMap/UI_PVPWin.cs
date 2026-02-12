/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_PVPWin : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GLabel myHeadIcon;
        public GLabel enemyHeadIcon;
        public GTextField myNameLb;
        public GTextField enemyNameLb;
        public const string URL = "ui://zoxecbv2auqc2c";

        public static UI_PVPWin CreateInstance()
        {
            return (UI_PVPWin)UIPackage.CreateObject("PVPMap", "PVPWin");
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