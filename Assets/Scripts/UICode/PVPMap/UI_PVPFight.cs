/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_PVPFight : GComponent
    {
        public GLoader3D spine;
        public GGraph monster;
        public GGraph hero;
        public GLabel myFightLb;
        public GTextField myNameLb;
        public GLabel enemyFightLb;
        public GTextField otherNameLb;
        public GList myPetList;
        public GList enemyPetList;
        public Transition t0;
        public const string URL = "ui://zoxecbv2auqc28";

        public static UI_PVPFight CreateInstance()
        {
            return (UI_PVPFight)UIPackage.CreateObject("PVPMap", "PVPFight");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GLoader3D)GetChild("spine");
            monster = (GGraph)GetChild("monster");
            hero = (GGraph)GetChild("hero");
            myFightLb = (GLabel)GetChild("myFightLb");
            myNameLb = (GTextField)GetChild("myNameLb");
            enemyFightLb = (GLabel)GetChild("enemyFightLb");
            otherNameLb = (GTextField)GetChild("otherNameLb");
            myPetList = (GList)GetChild("myPetList");
            enemyPetList = (GList)GetChild("enemyPetList");
            t0 = GetTransition("t0");
        }
    }
}