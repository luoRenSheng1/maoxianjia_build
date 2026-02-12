/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_FishingBossStageDetail : GComponent
    {
        public Controller bossType;
        public Controller isShowTx;
        public GComponent frame1;
        public GTextField title;
        public GGraph monsterIcon;
        public GTextField txtDesc;
        public GList rewardList;
        public GList attrList;
        public GButton btnFight;
        public GTextField txtTitleTime;
        public const string URL = "ui://pdufy3keka12igz";

        public static UI_FishingBossStageDetail CreateInstance()
        {
            return (UI_FishingBossStageDetail)UIPackage.CreateObject("BigMap", "FishingBossStageDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bossType = GetController("bossType");
            isShowTx = GetController("isShowTx");
            frame1 = (GComponent)GetChild("frame1");
            title = (GTextField)GetChild("title");
            monsterIcon = (GGraph)GetChild("monsterIcon");
            txtDesc = (GTextField)GetChild("txtDesc");
            rewardList = (GList)GetChild("rewardList");
            attrList = (GList)GetChild("attrList");
            btnFight = (GButton)GetChild("btnFight");
            txtTitleTime = (GTextField)GetChild("txtTitleTime");
        }
    }
}