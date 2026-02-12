/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_Panel : GComponent
    {
        public Controller zzcCtrl;
        public Controller xialaCtrl;
        public Controller packageCtrl;
        public Controller xialaCtrl2;
        public Controller isShowOther;
        public Controller showTask;
        public Controller taskPos;
        public UI_BtnFucntionIcon sevenDayBtn;
        public UI_BtnFucntionIcon dailyBtn;
        public UI_BtnFucntionIcon tujianBtn;
        public UI_BtnFucntionIcon mailBtn;
        public UI_BtnFucntionIcon villageBtn;
        public UI_BtnFucntionIcon copyBtn;
        public UI_battleRoot battleRoot;
        public UI_RestView RestView;
        public GButton userInfo;
        public UI_ComBuff comBuff;
        public UI_ComInfo comPandaInfo;
        public GButton taskXialaBtn;
        public GLoader rightBg;
        public UI_BtnFucntionIcon superTXZBtn;
        public UI_BtnFucntionIcon firstChargeBtn;
        public UI_OnlineBox onlineBtn;
        public UI_BtnFucntionIcon achieveBtn;
        public GList rightFunctionList;
        public GButton rightBtn;
        public GLoader leftBg;
        public UI_BtnFucntionIcon PVPBtn;
        public UI_BtnFucntionIcon loginGiftBtn;
        public UI_BtnFucntionIcon shopBtn;
        public GList leftFunctionList;
        public GButton leftBtn;
        public GButton btnSpeed;
        public GButton btnHelp;
        public GLabel hpLb;
        public GLabel atkLb;
        public GLabel criticalStrikeLb;
        public GLabel criticalInjuryLb;
        public GButton equipFlyPos;
        public GList inheritList;
        public GButton packageBtn;
        public GLoader packageLockIcon;
        public GList listEquip2;
        public GButton upLvBtn;
        public GLoader lvUpLockIcon;
        public GComponent red;
        public GList listEquip1;
        public UI_BtnBox comBox;
        public UI_BtnIcon_autopack btnSetupAuto;
        public UI_TempEquipIcon tempEquipIcon;
        public UI_EquipSplitEffect equipSplitEff;
        public GButton recycleBtn;
        public GList inheritBagList;
        public GButton closeBtn;
        public GList recycleBagList;
        public GList shuxingList;
        public GList classicList;
        public GTextField noClassicTips;
        public GComponent clickAttr;
        public GButton openPmBtn;
        public GLoader listBottomBg;
        public GList listBottom;
        public GGroup bottomGroup;
        public GTextField fightValLb;
        public GButton showOtherBtn;
        public GLabel headIcon;
        public GTextField playerName;
        public GList otherList;
        public GButton colseOtherBtn;
        public const string URL = "ui://s7x7ku0ni9sldxxzp";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("Lobby", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            zzcCtrl = GetController("zzcCtrl");
            xialaCtrl = GetController("xialaCtrl");
            packageCtrl = GetController("packageCtrl");
            xialaCtrl2 = GetController("xialaCtrl2");
            isShowOther = GetController("isShowOther");
            showTask = GetController("showTask");
            taskPos = GetController("taskPos");
            sevenDayBtn = (UI_BtnFucntionIcon)GetChild("sevenDayBtn");
            dailyBtn = (UI_BtnFucntionIcon)GetChild("dailyBtn");
            tujianBtn = (UI_BtnFucntionIcon)GetChild("tujianBtn");
            mailBtn = (UI_BtnFucntionIcon)GetChild("mailBtn");
            villageBtn = (UI_BtnFucntionIcon)GetChild("villageBtn");
            copyBtn = (UI_BtnFucntionIcon)GetChild("copyBtn");
            battleRoot = (UI_battleRoot)GetChild("battleRoot");
            RestView = (UI_RestView)GetChild("RestView");
            userInfo = (GButton)GetChild("userInfo");
            comBuff = (UI_ComBuff)GetChild("comBuff");
            comPandaInfo = (UI_ComInfo)GetChild("comPandaInfo");
            taskXialaBtn = (GButton)GetChild("taskXialaBtn");
            rightBg = (GLoader)GetChild("rightBg");
            superTXZBtn = (UI_BtnFucntionIcon)GetChild("superTXZBtn");
            firstChargeBtn = (UI_BtnFucntionIcon)GetChild("firstChargeBtn");
            onlineBtn = (UI_OnlineBox)GetChild("onlineBtn");
            achieveBtn = (UI_BtnFucntionIcon)GetChild("achieveBtn");
            rightFunctionList = (GList)GetChild("rightFunctionList");
            rightBtn = (GButton)GetChild("rightBtn");
            leftBg = (GLoader)GetChild("leftBg");
            PVPBtn = (UI_BtnFucntionIcon)GetChild("PVPBtn");
            loginGiftBtn = (UI_BtnFucntionIcon)GetChild("loginGiftBtn");
            shopBtn = (UI_BtnFucntionIcon)GetChild("shopBtn");
            leftFunctionList = (GList)GetChild("leftFunctionList");
            leftBtn = (GButton)GetChild("leftBtn");
            btnSpeed = (GButton)GetChild("btnSpeed");
            btnHelp = (GButton)GetChild("btnHelp");
            hpLb = (GLabel)GetChild("hpLb");
            atkLb = (GLabel)GetChild("atkLb");
            criticalStrikeLb = (GLabel)GetChild("criticalStrikeLb");
            criticalInjuryLb = (GLabel)GetChild("criticalInjuryLb");
            equipFlyPos = (GButton)GetChild("equipFlyPos");
            inheritList = (GList)GetChild("inheritList");
            packageBtn = (GButton)GetChild("packageBtn");
            packageLockIcon = (GLoader)GetChild("packageLockIcon");
            listEquip2 = (GList)GetChild("listEquip2");
            upLvBtn = (GButton)GetChild("upLvBtn");
            lvUpLockIcon = (GLoader)GetChild("lvUpLockIcon");
            red = (GComponent)GetChild("red");
            listEquip1 = (GList)GetChild("listEquip1");
            comBox = (UI_BtnBox)GetChild("comBox");
            btnSetupAuto = (UI_BtnIcon_autopack)GetChild("btnSetupAuto");
            tempEquipIcon = (UI_TempEquipIcon)GetChild("tempEquipIcon");
            equipSplitEff = (UI_EquipSplitEffect)GetChild("equipSplitEff");
            recycleBtn = (GButton)GetChild("recycleBtn");
            inheritBagList = (GList)GetChild("inheritBagList");
            closeBtn = (GButton)GetChild("closeBtn");
            recycleBagList = (GList)GetChild("recycleBagList");
            shuxingList = (GList)GetChild("shuxingList");
            classicList = (GList)GetChild("classicList");
            noClassicTips = (GTextField)GetChild("noClassicTips");
            clickAttr = (GComponent)GetChild("clickAttr");
            openPmBtn = (GButton)GetChild("openPmBtn");
            listBottomBg = (GLoader)GetChild("listBottomBg");
            listBottom = (GList)GetChild("listBottom");
            bottomGroup = (GGroup)GetChild("bottomGroup");
            fightValLb = (GTextField)GetChild("fightValLb");
            showOtherBtn = (GButton)GetChild("showOtherBtn");
            headIcon = (GLabel)GetChild("headIcon");
            playerName = (GTextField)GetChild("playerName");
            otherList = (GList)GetChild("otherList");
            colseOtherBtn = (GButton)GetChild("colseOtherBtn");
        }
    }
}