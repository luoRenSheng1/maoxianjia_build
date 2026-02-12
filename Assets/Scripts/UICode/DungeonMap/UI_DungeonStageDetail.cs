/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_DungeonStageDetail : GComponent
    {
        public Controller countCtrl;
        public Controller weakCtrl;
        public Controller sweepCtrl;
        public Controller sweepBtnCtrl;
        public Controller adCtrl;
        public GComponent frame;
        public GTextField stageName;
        public GLoader stageIcon;
        public GGraph spine;
        public GTextField stageIdLb;
        public GList itemList;
        public GButton preBtn;
        public GButton nextBtn;
        public GLoader itemIcon;
        public GTextField itemCntLb;
        public GButton gotoBtn;
        public GButton sweepBtn;
        public GButton adBtn;
        public GButton closeBtn;
        public const string URL = "ui://57yc4rb0pbu03d";

        public static UI_DungeonStageDetail CreateInstance()
        {
            return (UI_DungeonStageDetail)UIPackage.CreateObject("DungeonMap", "DungeonStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            countCtrl = GetController("countCtrl");
            weakCtrl = GetController("weakCtrl");
            sweepCtrl = GetController("sweepCtrl");
            sweepBtnCtrl = GetController("sweepBtnCtrl");
            adCtrl = GetController("adCtrl");
            frame = (GComponent)GetChild("frame");
            stageName = (GTextField)GetChild("stageName");
            stageIcon = (GLoader)GetChild("stageIcon");
            spine = (GGraph)GetChild("spine");
            stageIdLb = (GTextField)GetChild("stageIdLb");
            itemList = (GList)GetChild("itemList");
            preBtn = (GButton)GetChild("preBtn");
            nextBtn = (GButton)GetChild("nextBtn");
            itemIcon = (GLoader)GetChild("itemIcon");
            itemCntLb = (GTextField)GetChild("itemCntLb");
            gotoBtn = (GButton)GetChild("gotoBtn");
            sweepBtn = (GButton)GetChild("sweepBtn");
            adBtn = (GButton)GetChild("adBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}