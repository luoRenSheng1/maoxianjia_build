/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_battleRoot : GComponent
    {
        public UI_battleBg bg;
        public UI_battleScene scene;
        public GComponent comSkills;
        public GLabel myFightLb;
        public GLabel otherFightLb;
        public Transition loopEff;
        public const string URL = "ui://zoxecbv2qc4n7";

        public static UI_battleRoot CreateInstance()
        {
            return (UI_battleRoot)UIPackage.CreateObject("PVPMap", "battleRoot");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (UI_battleBg)GetChild("bg");
            scene = (UI_battleScene)GetChild("scene");
            comSkills = (GComponent)GetChild("comSkills");
            myFightLb = (GLabel)GetChild("myFightLb");
            otherFightLb = (GLabel)GetChild("otherFightLb");
            loopEff = GetTransition("loopEff");
        }
    }
}