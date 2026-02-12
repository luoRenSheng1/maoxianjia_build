/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleMain : GComponent
    {
        public GGraph spine;
        public GButton playSpineBtn;
        public GList starList;
        public GLabel roleName;
        public GLoader occupationLb;
        public GLoader attrLb;
        public GComponent qIcon;
        public UI_HeroBarExp expBar;
        public GTextField roleLv;
        public GButton helpBtn;
        public GList levelAttrList;
        public UI_SkillItemCom skillItem;
        public UI_SkillItemCom skillItem2;
        public GList list;
        public GTextField desc;
        public GButton upLevelBtn;
        public GComponent redDot;
        public const string URL = "ui://m37flevdozj9f";

        public static UI_RoleMain CreateInstance()
        {
            return (UI_RoleMain)UIPackage.CreateObject("RoleMain", "RoleMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            playSpineBtn = (GButton)GetChild("playSpineBtn");
            starList = (GList)GetChild("starList");
            roleName = (GLabel)GetChild("roleName");
            occupationLb = (GLoader)GetChild("occupationLb");
            attrLb = (GLoader)GetChild("attrLb");
            qIcon = (GComponent)GetChild("qIcon");
            expBar = (UI_HeroBarExp)GetChild("expBar");
            roleLv = (GTextField)GetChild("roleLv");
            helpBtn = (GButton)GetChild("helpBtn");
            levelAttrList = (GList)GetChild("levelAttrList");
            skillItem = (UI_SkillItemCom)GetChild("skillItem");
            skillItem2 = (UI_SkillItemCom)GetChild("skillItem2");
            list = (GList)GetChild("list");
            desc = (GTextField)GetChild("desc");
            upLevelBtn = (GButton)GetChild("upLevelBtn");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}