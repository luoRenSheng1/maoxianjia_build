/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_DungeonMain : GComponent
    {
        public GComponent frame;
        public UI_Panel panel;
        public Transition showAni;
        public Transition hideAni;
        public const string URL = "ui://57yc4rb0gwxt0";

        public static UI_DungeonMain CreateInstance()
        {
            return (UI_DungeonMain)UIPackage.CreateObject("DungeonMap", "DungeonMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            panel = (UI_Panel)GetChild("panel");
            showAni = GetTransition("showAni");
            hideAni = GetTransition("hideAni");
        }
    }
}