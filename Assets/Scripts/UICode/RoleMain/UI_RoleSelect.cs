/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleSelect : GComponent
    {
        public Controller optCtrl;
        public Controller maxCtrl;
        public Controller hasCtrl;
        public Controller strongCtrl;
        public GGraph spine;
        public GButton playSpineBtn;
        public GLabel roleName;
        public GList starList;
        public GLoader occupationLb;
        public GLoader attrLb;
        public GComponent qIcon;
        public UI_HeroBarExp expBar;
        public GTextField roleLv;
        public GButton tipsbtn;
        public GList levelAttrList;
        public GLoader roleSkillIcon;
        public GTextField skillLv;
        public GTextField skillNameLb;
        public GRichTextField skillDesc;
        public UI_SkillCostBtn cost1;
        public UI_SkillCostBtn cost2;
        public GButton strongBtn;
        public GLoader strongLockIcon;
        public GList heroList;
        public UI_RoleSelectBottom roleSelectBottom;
        public UI_Currency btnGold;
        public UI_Currency btnDia;
        public UI_Currency btnSplitItem;
        public UI_RoleCurrency btnGold2;
        public UI_RoleCurrency btnStone;
        public UI_RoleCurrency btnUpLv;
        public UI_RoleCurrency btnBreak;
        public const string URL = "ui://m37flevdozj91p";

        public static UI_RoleSelect CreateInstance()
        {
            return (UI_RoleSelect)UIPackage.CreateObject("RoleMain", "RoleSelect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            optCtrl = GetController("optCtrl");
            maxCtrl = GetController("maxCtrl");
            hasCtrl = GetController("hasCtrl");
            strongCtrl = GetController("strongCtrl");
            spine = (GGraph)GetChild("spine");
            playSpineBtn = (GButton)GetChild("playSpineBtn");
            roleName = (GLabel)GetChild("roleName");
            starList = (GList)GetChild("starList");
            occupationLb = (GLoader)GetChild("occupationLb");
            attrLb = (GLoader)GetChild("attrLb");
            qIcon = (GComponent)GetChild("qIcon");
            expBar = (UI_HeroBarExp)GetChild("expBar");
            roleLv = (GTextField)GetChild("roleLv");
            tipsbtn = (GButton)GetChild("tipsbtn");
            levelAttrList = (GList)GetChild("levelAttrList");
            roleSkillIcon = (GLoader)GetChild("roleSkillIcon");
            skillLv = (GTextField)GetChild("skillLv");
            skillNameLb = (GTextField)GetChild("skillNameLb");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            cost1 = (UI_SkillCostBtn)GetChild("cost1");
            cost2 = (UI_SkillCostBtn)GetChild("cost2");
            strongBtn = (GButton)GetChild("strongBtn");
            strongLockIcon = (GLoader)GetChild("strongLockIcon");
            heroList = (GList)GetChild("heroList");
            roleSelectBottom = (UI_RoleSelectBottom)GetChild("roleSelectBottom");
            btnGold = (UI_Currency)GetChild("btnGold");
            btnDia = (UI_Currency)GetChild("btnDia");
            btnSplitItem = (UI_Currency)GetChild("btnSplitItem");
            btnGold2 = (UI_RoleCurrency)GetChild("btnGold2");
            btnStone = (UI_RoleCurrency)GetChild("btnStone");
            btnUpLv = (UI_RoleCurrency)GetChild("btnUpLv");
            btnBreak = (UI_RoleCurrency)GetChild("btnBreak");
        }
    }
}