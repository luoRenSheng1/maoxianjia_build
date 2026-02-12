/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_ChapterEventBossStageDetail : GComponent
    {
        public Controller isTx;
        public Controller isRandom;
        public GComponent frame;
        public GTextField txtName;
        public GGraph monsterIcon;
        public GList rewardList;
        public GTextField txtDesc;
        public GButton btnFight;
        public GList attrList;
        public GButton closeBtn;
        public GTextField txtTitleTime;
        public const string URL = "ui://pdufy3kep9n01o";

        public static UI_ChapterEventBossStageDetail CreateInstance()
        {
            return (UI_ChapterEventBossStageDetail)UIPackage.CreateObject("BigMap", "ChapterEventBossStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isTx = GetController("isTx");
            isRandom = GetController("isRandom");
            frame = (GComponent)GetChild("frame");
            txtName = (GTextField)GetChild("txtName");
            monsterIcon = (GGraph)GetChild("monsterIcon");
            rewardList = (GList)GetChild("rewardList");
            txtDesc = (GTextField)GetChild("txtDesc");
            btnFight = (GButton)GetChild("btnFight");
            attrList = (GList)GetChild("attrList");
            closeBtn = (GButton)GetChild("closeBtn");
            txtTitleTime = (GTextField)GetChild("txtTitleTime");
        }
    }
}