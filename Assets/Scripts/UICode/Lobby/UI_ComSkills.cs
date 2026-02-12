/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ComSkills : GComponent
    {
        public UI_PassiveBtnSkill xpskill;
        public UI_BtnIconSkillBig btnSkillAuto;
        public UI_BtnSkill activeSkill;
        public GList listSkills;
        public const string URL = "ui://s7x7ku0npdpadxxz6";

        public static UI_ComSkills CreateInstance()
        {
            return (UI_ComSkills)UIPackage.CreateObject("Lobby", "ComSkills");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            xpskill = (UI_PassiveBtnSkill)GetChild("xpskill");
            btnSkillAuto = (UI_BtnIconSkillBig)GetChild("btnSkillAuto");
            activeSkill = (UI_BtnSkill)GetChild("activeSkill");
            listSkills = (GList)GetChild("listSkills");
        }
    }
}