/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FightLoseAndWin
{
    public partial class UI_FightWinWindow : GComponent
    {
        public GComponent frame;
        public GLoader3D spine;
        public GTextField desc;
        public GButton nextStageBtn;
        public GTextField timeLb;
        public const string URL = "ui://h7b921iwj3d5dxy2a";

        public static UI_FightWinWindow CreateInstance()
        {
            return (UI_FightWinWindow)UIPackage.CreateObject("FightLoseAndWin", "FightWinWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            spine = (GLoader3D)GetChild("spine");
            desc = (GTextField)GetChild("desc");
            nextStageBtn = (GButton)GetChild("nextStageBtn");
            timeLb = (GTextField)GetChild("timeLb");
        }
    }
}