/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterMap : GComponent
    {
        public Controller showTask;
        public Controller showFucn;
        public UI_panelClip panelClip;
        public GComponent mapEX;
        public GButton userInfo;
        public GButton zhuZaoChuiCur;
        public GList functionList;
        public GButton funcXialaBtn;
        public UI_ComInfo comPandaInfo;
        public UI_ComInfo mainComPandaInfo;
        public GButton comMessage;
        public UI_ComBuff comBuff;
        public UI_FightCurStageBtn fightCurStageBtn;
        public UI_CloudSpine cloudPanel;
        public GButton exitBtn;
        public GLoader maskBG;
        public GLabel LoadTitle;
        public GGroup Loading;
        public GLoader video;
        public const string URL = "ui://pdufy3kekh6r3";

        public static UI_ChapterMap CreateInstance()
        {
            return (UI_ChapterMap)UIPackage.CreateObject("BigMap", "ChapterMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            showTask = GetController("showTask");
            showFucn = GetController("showFucn");
            panelClip = (UI_panelClip)GetChild("panelClip");
            mapEX = (GComponent)GetChild("mapEX");
            userInfo = (GButton)GetChild("userInfo");
            zhuZaoChuiCur = (GButton)GetChild("zhuZaoChuiCur");
            functionList = (GList)GetChild("functionList");
            funcXialaBtn = (GButton)GetChild("funcXialaBtn");
            comPandaInfo = (UI_ComInfo)GetChild("comPandaInfo");
            mainComPandaInfo = (UI_ComInfo)GetChild("mainComPandaInfo");
            comMessage = (GButton)GetChild("comMessage");
            comBuff = (UI_ComBuff)GetChild("comBuff");
            fightCurStageBtn = (UI_FightCurStageBtn)GetChild("fightCurStageBtn");
            cloudPanel = (UI_CloudSpine)GetChild("cloudPanel");
            exitBtn = (GButton)GetChild("exitBtn");
            maskBG = (GLoader)GetChild("maskBG");
            LoadTitle = (GLabel)GetChild("LoadTitle");
            Loading = (GGroup)GetChild("Loading");
            video = (GLoader)GetChild("video");
        }
    }
}