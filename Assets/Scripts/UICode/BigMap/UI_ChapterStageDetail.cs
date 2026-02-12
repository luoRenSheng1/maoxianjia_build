/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterStageDetail : GComponent
    {
        public Controller bossStageCtrl;
        public Controller isTx;
        public Controller unlockTypeCtrl;
        public GComponent frame;
        public GGraph monsterIcon;
        public GTextField txtName;
        public GTextField txtRewardName;
        public GList rewardList;
        public GList badgeList;
        public GTextField txtMonsterAtk;
        public GTextField txtDesc;
        public GButton btnFight;
        public GButton btnFight2;
        public GList attrList;
        public GList firstRwList;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3kef7h7b";

        public static UI_ChapterStageDetail CreateInstance()
        {
            return (UI_ChapterStageDetail)UIPackage.CreateObject("BigMap", "ChapterStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bossStageCtrl = GetController("bossStageCtrl");
            isTx = GetController("isTx");
            unlockTypeCtrl = GetController("unlockTypeCtrl");
            frame = (GComponent)GetChild("frame");
            monsterIcon = (GGraph)GetChild("monsterIcon");
            txtName = (GTextField)GetChild("txtName");
            txtRewardName = (GTextField)GetChild("txtRewardName");
            rewardList = (GList)GetChild("rewardList");
            badgeList = (GList)GetChild("badgeList");
            txtMonsterAtk = (GTextField)GetChild("txtMonsterAtk");
            txtDesc = (GTextField)GetChild("txtDesc");
            btnFight = (GButton)GetChild("btnFight");
            btnFight2 = (GButton)GetChild("btnFight2");
            attrList = (GList)GetChild("attrList");
            firstRwList = (GList)GetChild("firstRwList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}