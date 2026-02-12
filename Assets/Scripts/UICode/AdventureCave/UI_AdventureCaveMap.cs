/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AdventureCave
{
    public partial class UI_AdventureCaveMap : GComponent
    {
        public Controller c1;
        public GLoader map;
        public GImage map0;
        public GImage map1;
        public GLoader p1;
        public GLoader p2;
        public GLoader p3;
        public GLoader p4;
        public GLoader walk1;
        public GLoader walk2;
        public GLoader3D terminus;
        public GLoader rolePos;
        public GLoader car;
        public GGraph npc;
        public GLoader3D homeGate;
        public GLabel qipao;
        public GGraph hero;
        public GLoader npcTouch;
        public const string URL = "ui://z350mxkhrjj26";

        public static UI_AdventureCaveMap CreateInstance()
        {
            return (UI_AdventureCaveMap)UIPackage.CreateObject("AdventureCave", "AdventureCaveMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
            map = (GLoader)GetChild("map");
            map0 = (GImage)GetChild("map0");
            map1 = (GImage)GetChild("map1");
            p1 = (GLoader)GetChild("p1");
            p2 = (GLoader)GetChild("p2");
            p3 = (GLoader)GetChild("p3");
            p4 = (GLoader)GetChild("p4");
            walk1 = (GLoader)GetChild("walk1");
            walk2 = (GLoader)GetChild("walk2");
            terminus = (GLoader3D)GetChild("terminus");
            rolePos = (GLoader)GetChild("rolePos");
            car = (GLoader)GetChild("car");
            npc = (GGraph)GetChild("npc");
            homeGate = (GLoader3D)GetChild("homeGate");
            qipao = (GLabel)GetChild("qipao");
            hero = (GGraph)GetChild("hero");
            npcTouch = (GLoader)GetChild("npcTouch");
        }
    }
}