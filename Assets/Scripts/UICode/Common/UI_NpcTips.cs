/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_NpcTips : GComponent
    {
        public Controller ctrl;
        public GTextField txt;
        public GGraph npc;
        public GButton skip;
        public const string URL = "ui://0anhreylua62dxyhv";

        public static UI_NpcTips CreateInstance()
        {
            return (UI_NpcTips)UIPackage.CreateObject("Common", "NpcTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            txt = (GTextField)GetChild("txt");
            npc = (GGraph)GetChild("npc");
            skip = (GButton)GetChild("skip");
        }
    }
}