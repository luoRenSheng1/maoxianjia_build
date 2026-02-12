/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_HolyMain : GComponent
    {
        public Controller isMax;
        public Controller ctrl;
        public Controller upBtnCtrl;
        public Controller hasCtrl;
        public Controller strongBtnCtrl;
        public Controller hasMask;
        public Controller downBtnCtrl;
        public GGraph blank1;
        public UI_HolyUploadItem holyUploadItem0;
        public GButton replaceBtn0;
        public UI_HolyUploadItem holyUploadItem1;
        public GButton replaceBtn1;
        public UI_HolyUploadItem holyUploadItem2;
        public GButton replaceBtn2;
        public GLoader selectBg;
        public GGroup upGroup;
        public GGraph blank2;
        public GList holyList;
        public GGraph blank3;
        public GTextField holyName;
        public GTextField curLv;
        public GTextField curDesc;
        public GTextField nextLv;
        public GTextField nextDesc;
        public GTextField maxLv;
        public GTextField maxDesc;
        public UI_HolyCost cost1;
        public UI_HolyCost cost2;
        public GButton strongBtn;
        public GButton downBtn;
        public GButton maxBtn;
        public GButton upBtn;
        public GButton replaceBtn;
        public GButton noBtn;
        public UI_Currency currency1;
        public UI_Currency currency2;
        public GButton helpBtn;
        public const string URL = "ui://m37flevda2kwdxyao";

        public static UI_HolyMain CreateInstance()
        {
            return (UI_HolyMain)UIPackage.CreateObject("RoleMain", "HolyMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isMax = GetController("isMax");
            ctrl = GetController("ctrl");
            upBtnCtrl = GetController("upBtnCtrl");
            hasCtrl = GetController("hasCtrl");
            strongBtnCtrl = GetController("strongBtnCtrl");
            hasMask = GetController("hasMask");
            downBtnCtrl = GetController("downBtnCtrl");
            blank1 = (GGraph)GetChild("blank1");
            holyUploadItem0 = (UI_HolyUploadItem)GetChild("holyUploadItem0");
            replaceBtn0 = (GButton)GetChild("replaceBtn0");
            holyUploadItem1 = (UI_HolyUploadItem)GetChild("holyUploadItem1");
            replaceBtn1 = (GButton)GetChild("replaceBtn1");
            holyUploadItem2 = (UI_HolyUploadItem)GetChild("holyUploadItem2");
            replaceBtn2 = (GButton)GetChild("replaceBtn2");
            selectBg = (GLoader)GetChild("selectBg");
            upGroup = (GGroup)GetChild("upGroup");
            blank2 = (GGraph)GetChild("blank2");
            holyList = (GList)GetChild("holyList");
            blank3 = (GGraph)GetChild("blank3");
            holyName = (GTextField)GetChild("holyName");
            curLv = (GTextField)GetChild("curLv");
            curDesc = (GTextField)GetChild("curDesc");
            nextLv = (GTextField)GetChild("nextLv");
            nextDesc = (GTextField)GetChild("nextDesc");
            maxLv = (GTextField)GetChild("maxLv");
            maxDesc = (GTextField)GetChild("maxDesc");
            cost1 = (UI_HolyCost)GetChild("cost1");
            cost2 = (UI_HolyCost)GetChild("cost2");
            strongBtn = (GButton)GetChild("strongBtn");
            downBtn = (GButton)GetChild("downBtn");
            maxBtn = (GButton)GetChild("maxBtn");
            upBtn = (GButton)GetChild("upBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
            noBtn = (GButton)GetChild("noBtn");
            currency1 = (UI_Currency)GetChild("currency1");
            currency2 = (UI_Currency)GetChild("currency2");
            helpBtn = (GButton)GetChild("helpBtn");
        }
    }
}