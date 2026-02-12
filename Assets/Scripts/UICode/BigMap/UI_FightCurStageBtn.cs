/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FightCurStageBtn : GButton
    {
        public Controller typeCtrl;
        public GTextField stageInfo;
        public const string URL = "ui://pdufy3kellb1ijl";

        public static UI_FightCurStageBtn CreateInstance()
        {
            return (UI_FightCurStageBtn)UIPackage.CreateObject("BigMap", "FightCurStageBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            stageInfo = (GTextField)GetChild("stageInfo");
        }
    }
}