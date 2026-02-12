/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_RolePassportDetail : GComponent
    {
        public GGraph spine;
        public GButton playSpineBtn;
        public GComponent qIcon;
        public GLabel roleName;
        public GList levelAttrList;
        public GLoader roleSkillIcon;
        public GTextField skillNameLb;
        public GRichTextField skillDesc;
        public UI_AdvancePassportBtn advanceBtn;
        public GButton closeBtn;
        public const string URL = "ui://2pcsnr2kr0ab18";

        public static UI_RolePassportDetail CreateInstance()
        {
            return (UI_RolePassportDetail)UIPackage.CreateObject("Passport", "RolePassportDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GGraph)GetChild("spine");
            playSpineBtn = (GButton)GetChild("playSpineBtn");
            qIcon = (GComponent)GetChild("qIcon");
            roleName = (GLabel)GetChild("roleName");
            levelAttrList = (GList)GetChild("levelAttrList");
            roleSkillIcon = (GLoader)GetChild("roleSkillIcon");
            skillNameLb = (GTextField)GetChild("skillNameLb");
            skillDesc = (GRichTextField)GetChild("skillDesc");
            advanceBtn = (UI_AdvancePassportBtn)GetChild("advanceBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}