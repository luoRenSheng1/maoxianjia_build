/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_SkillDetail : GComponent
    {
        public Controller ctrl;
        public Controller tipsCtrl;
        public GComponent frame;
        public GButton skilltem;
        public GTextField cdLb;
        public GProgressBar petExp;
        public GComponent petQ;
        public GTextField skillName;
        public GTextField skillLv;
        public UI_skillAttrLb pdLb;
        public UI_skillAttrLb qjLb;
        public GRichTextField skillDesc;
        public GButton downBtn;
        public GButton replaceBtn;
        public GButton uploadBtn;
        public GButton getBtn;
        public GButton closeBtn;
        public const string URL = "ui://m37flevdrm2cdxy6h";

        public static UI_SkillDetail CreateInstance()
        {
            return (UI_SkillDetail)UIPackage.CreateObject("RoleMain", "SkillDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            tipsCtrl = GetController("tipsCtrl");
            frame = (GComponent)GetChild("frame");
            skilltem = (GButton)GetChild("skilltem");
            cdLb = (GTextField)GetChild("cdLb");
            petExp = (GProgressBar)GetChild("petExp");
            petQ = (GComponent)GetChild("petQ");
            skillName = (GTextField)GetChild("skillName");
            skillLv = (GTextField)GetChild("skillLv");
            pdLb = (UI_skillAttrLb)GetChild("pdLb");
            qjLb = (UI_skillAttrLb)GetChild("qjLb");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            downBtn = (GButton)GetChild("downBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
            uploadBtn = (GButton)GetChild("uploadBtn");
            getBtn = (GButton)GetChild("getBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}