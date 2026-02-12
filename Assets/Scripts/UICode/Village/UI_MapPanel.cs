/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_MapPanel : GComponent
    {
        public UI_MapBg mapBg;
        public UI_VillageBuildItem villageBuild1;
        public UI_VillageBuildItem villageBuild4;
        public UI_VillageBuildItem villageBuild5;
        public UI_VillageBuildItem villageBuild6;
        public UI_VillageBuildItem villageBuild3;
        public UI_VillageBuildItem villageBuild2;
        public GGraph p0;
        public GGraph p1;
        public GGraph p2;
        public GGraph p3;
        public GGraph p4;
        public GGraph p5;
        public GGraph p6;
        public GGraph p7;
        public GGraph p8;
        public GGraph p9;
        public GGraph p10;
        public GGraph p11;
        public GGraph p12;
        public GGraph p13;
        public GGroup petRandPatrol;
        public const string URL = "ui://8glegefcuobv1";

        public static UI_MapPanel CreateInstance()
        {
            return (UI_MapPanel)UIPackage.CreateObject("Village", "MapPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapBg = (UI_MapBg)GetChild("mapBg");
            villageBuild1 = (UI_VillageBuildItem)GetChild("villageBuild1");
            villageBuild4 = (UI_VillageBuildItem)GetChild("villageBuild4");
            villageBuild5 = (UI_VillageBuildItem)GetChild("villageBuild5");
            villageBuild6 = (UI_VillageBuildItem)GetChild("villageBuild6");
            villageBuild3 = (UI_VillageBuildItem)GetChild("villageBuild3");
            villageBuild2 = (UI_VillageBuildItem)GetChild("villageBuild2");
            p0 = (GGraph)GetChild("p0");
            p1 = (GGraph)GetChild("p1");
            p2 = (GGraph)GetChild("p2");
            p3 = (GGraph)GetChild("p3");
            p4 = (GGraph)GetChild("p4");
            p5 = (GGraph)GetChild("p5");
            p6 = (GGraph)GetChild("p6");
            p7 = (GGraph)GetChild("p7");
            p8 = (GGraph)GetChild("p8");
            p9 = (GGraph)GetChild("p9");
            p10 = (GGraph)GetChild("p10");
            p11 = (GGraph)GetChild("p11");
            p12 = (GGraph)GetChild("p12");
            p13 = (GGraph)GetChild("p13");
            petRandPatrol = (GGroup)GetChild("petRandPatrol");
        }
    }
}