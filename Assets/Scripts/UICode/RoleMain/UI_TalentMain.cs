/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_TalentMain : GComponent
    {
        public Controller type;
        public Controller upCtrl;
        public UI_Currency talentPoint;
        public UI_Currency talentToken;
        public UI_TalentUI phyTalent;
        public UI_TalentUI spellTalent;
        public UI_TalentUI defTalent;
        public UI_TalentSelect talentSelect;
        public UI_TalentItem talent;
        public GTextField name;
        public GTextField desc;
        public UI_ResetBtn resetBtn;
        public UI_ResetBtn upLvBtn;
        public GButton activateBtn;
        public GButton maxBtn;
        public const string URL = "ui://m37flevdp9n0dxy7b";

        public static UI_TalentMain CreateInstance()
        {
            return (UI_TalentMain)UIPackage.CreateObject("RoleMain", "TalentMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            upCtrl = GetController("upCtrl");
            talentPoint = (UI_Currency)GetChild("talentPoint");
            talentToken = (UI_Currency)GetChild("talentToken");
            phyTalent = (UI_TalentUI)GetChild("phyTalent");
            spellTalent = (UI_TalentUI)GetChild("spellTalent");
            defTalent = (UI_TalentUI)GetChild("defTalent");
            talentSelect = (UI_TalentSelect)GetChild("talentSelect");
            talent = (UI_TalentItem)GetChild("talent");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            resetBtn = (UI_ResetBtn)GetChild("resetBtn");
            upLvBtn = (UI_ResetBtn)GetChild("upLvBtn");
            activateBtn = (GButton)GetChild("activateBtn");
            maxBtn = (GButton)GetChild("maxBtn");
        }
    }
}