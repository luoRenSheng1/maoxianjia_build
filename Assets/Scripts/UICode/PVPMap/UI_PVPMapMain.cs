/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_PVPMapMain : GComponent
    {
        public GComponent frame;
        public UI_Panel panel;
        public GProgressBar myHpBar;
        public GLabel myHeadIcon;
        public GProgressBar enemyHpBar;
        public GLabel enemyHeadIcon;
        public UI_Vs vsCom;
        public Transition showAni;
        public Transition hideAni;
        public const string URL = "ui://zoxecbv2qc4n0";

        public static UI_PVPMapMain CreateInstance()
        {
            return (UI_PVPMapMain)UIPackage.CreateObject("PVPMap", "PVPMapMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            panel = (UI_Panel)GetChild("panel");
            myHpBar = (GProgressBar)GetChild("myHpBar");
            myHeadIcon = (GLabel)GetChild("myHeadIcon");
            enemyHpBar = (GProgressBar)GetChild("enemyHpBar");
            enemyHeadIcon = (GLabel)GetChild("enemyHeadIcon");
            vsCom = (UI_Vs)GetChild("vsCom");
            showAni = GetTransition("showAni");
            hideAni = GetTransition("hideAni");
        }
    }
}