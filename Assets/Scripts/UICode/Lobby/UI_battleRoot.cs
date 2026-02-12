/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_battleRoot : GComponent
    {
        public Controller skillCtrl;
        public Controller copyCtrl;
        public UI_battleBg bg;
        public UI_battleScene scene;
        public UI_ComMessage comMessage;
        public UI_ComSkills comSkills;
        public GTextField stageName;
        public GComponent barBossTime;
        public GComponent monsterGroup;
        public GImage loopIcon;
        public UI_BtnToppedAni btnToppedAni;
        public GTextField zctz;
        public GButton btnStage;
        public GTextField copyBossStageName;
        public GComponent copyBlood;
        public GList bossTxList;
        public GComponent copyBossTime;
        public GComponent copyBossTime1;
        public GGroup topGroup;
        public GComponent fightBoss;
        public GGraph commonTimeSp;
        public GGraph touchPanel;
        public Transition loopEff;
        public const string URL = "ui://s7x7ku0ncj33dxxzg";

        public static UI_battleRoot CreateInstance()
        {
            return (UI_battleRoot)UIPackage.CreateObject("Lobby", "battleRoot");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            skillCtrl = GetController("skillCtrl");
            copyCtrl = GetController("copyCtrl");
            bg = (UI_battleBg)GetChild("bg");
            scene = (UI_battleScene)GetChild("scene");
            comMessage = (UI_ComMessage)GetChild("comMessage");
            comSkills = (UI_ComSkills)GetChild("comSkills");
            stageName = (GTextField)GetChild("stageName");
            barBossTime = (GComponent)GetChild("barBossTime");
            monsterGroup = (GComponent)GetChild("monsterGroup");
            loopIcon = (GImage)GetChild("loopIcon");
            btnToppedAni = (UI_BtnToppedAni)GetChild("btnToppedAni");
            zctz = (GTextField)GetChild("zctz");
            btnStage = (GButton)GetChild("btnStage");
            copyBossStageName = (GTextField)GetChild("copyBossStageName");
            copyBlood = (GComponent)GetChild("copyBlood");
            bossTxList = (GList)GetChild("bossTxList");
            copyBossTime = (GComponent)GetChild("copyBossTime");
            copyBossTime1 = (GComponent)GetChild("copyBossTime1");
            topGroup = (GGroup)GetChild("topGroup");
            fightBoss = (GComponent)GetChild("fightBoss");
            commonTimeSp = (GGraph)GetChild("commonTimeSp");
            touchPanel = (GGraph)GetChild("touchPanel");
            loopEff = GetTransition("loopEff");
        }
    }
}