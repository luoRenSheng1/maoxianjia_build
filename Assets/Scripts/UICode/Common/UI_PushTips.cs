/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_PushTips : GComponent
    {
        public Controller ctrl;
        public GTextField txt;
        public GGraph npc;
        public GButton GoTo;
        public GGroup root;
        public Transition In;
        public Transition Out;
        public const string URL = "ui://0anhreylkv73dxyi0";

        public static UI_PushTips CreateInstance()
        {
            return (UI_PushTips)UIPackage.CreateObject("Common", "PushTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            txt = (GTextField)GetChild("txt");
            npc = (GGraph)GetChild("npc");
            GoTo = (GButton)GetChild("GoTo");
            root = (GGroup)GetChild("root");
            In = GetTransition("In");
            Out = GetTransition("Out");
        }
    }
}