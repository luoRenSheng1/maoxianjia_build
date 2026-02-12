/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_DungeonStage : GComponent
    {
        public GComponent frame;
        public GButton comMessage;
        public GList stageList;
        public GTextField tiemdesc;
        public GTextField timeLeft;
        public GButton closeBtn;
        public const string URL = "ui://57yc4rb0b0e431";

        public static UI_DungeonStage CreateInstance()
        {
            return (UI_DungeonStage)UIPackage.CreateObject("DungeonMap", "DungeonStage");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            comMessage = (GButton)GetChild("comMessage");
            stageList = (GList)GetChild("stageList");
            tiemdesc = (GTextField)GetChild("tiemdesc");
            timeLeft = (GTextField)GetChild("timeLeft");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}