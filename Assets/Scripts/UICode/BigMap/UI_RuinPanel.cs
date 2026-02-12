/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinPanel : GComponent
    {
        public GLoader mapBg;
        public GLoader3D step0;
        public GLoader3D step1;
        public GLoader pos1;
        public GLoader pos2;
        public GLoader pos3;
        public GLoader pos4;
        public GLoader pos5;
        public GLoader pos6;
        public GLoader pos7;
        public GLoader pos8;
        public GLoader pos9;
        public GLoader pos10;
        public GLoader pos11;
        public GLoader reward0;
        public GComponent rewardBuild0;
        public GLoader reward1;
        public GComponent rewardBuild1;
        public GLoader reward2;
        public GComponent rewardBuild2;
        public GGraph boss0;
        public GGraph boss1;
        public GGraph boss2;
        public GButton bossBtn0;
        public GButton bossBtn1;
        public GButton bossBtn2;
        public GList buffList;
        public GGraph hero;
        public const string URL = "ui://pdufy3kew224idm";

        public static UI_RuinPanel CreateInstance()
        {
            return (UI_RuinPanel)UIPackage.CreateObject("BigMap", "RuinPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapBg = (GLoader)GetChild("mapBg");
            step0 = (GLoader3D)GetChild("step0");
            step1 = (GLoader3D)GetChild("step1");
            pos1 = (GLoader)GetChild("pos1");
            pos2 = (GLoader)GetChild("pos2");
            pos3 = (GLoader)GetChild("pos3");
            pos4 = (GLoader)GetChild("pos4");
            pos5 = (GLoader)GetChild("pos5");
            pos6 = (GLoader)GetChild("pos6");
            pos7 = (GLoader)GetChild("pos7");
            pos8 = (GLoader)GetChild("pos8");
            pos9 = (GLoader)GetChild("pos9");
            pos10 = (GLoader)GetChild("pos10");
            pos11 = (GLoader)GetChild("pos11");
            reward0 = (GLoader)GetChild("reward0");
            rewardBuild0 = (GComponent)GetChild("rewardBuild0");
            reward1 = (GLoader)GetChild("reward1");
            rewardBuild1 = (GComponent)GetChild("rewardBuild1");
            reward2 = (GLoader)GetChild("reward2");
            rewardBuild2 = (GComponent)GetChild("rewardBuild2");
            boss0 = (GGraph)GetChild("boss0");
            boss1 = (GGraph)GetChild("boss1");
            boss2 = (GGraph)GetChild("boss2");
            bossBtn0 = (GButton)GetChild("bossBtn0");
            bossBtn1 = (GButton)GetChild("bossBtn1");
            bossBtn2 = (GButton)GetChild("bossBtn2");
            buffList = (GList)GetChild("buffList");
            hero = (GGraph)GetChild("hero");
        }
    }
}