/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace NoOperation
{
    public partial class UI_PetRunRoot : GComponent
    {
        public GGraph p4;
        public GGraph p3;
        public GGraph p2;
        public GGraph p1;
        public GGraph p0;
        public Transition t0;
        public Transition t1;
        public Transition t2;
        public Transition t3;
        public Transition t4;
        public const string URL = "ui://88ncwg57v51s7";

        public static UI_PetRunRoot CreateInstance()
        {
            return (UI_PetRunRoot)UIPackage.CreateObject("NoOperation", "PetRunRoot");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            p4 = (GGraph)GetChild("p4");
            p3 = (GGraph)GetChild("p3");
            p2 = (GGraph)GetChild("p2");
            p1 = (GGraph)GetChild("p1");
            p0 = (GGraph)GetChild("p0");
            t0 = GetTransition("t0");
            t1 = GetTransition("t1");
            t2 = GetTransition("t2");
            t3 = GetTransition("t3");
            t4 = GetTransition("t4");
        }
    }
}