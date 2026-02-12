/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_SkillCostBtn : GButton
    {
        public Controller status;
        public GTextField num;
        public const string URL = "ui://m37flevdf34jdxyak";

        public static UI_SkillCostBtn CreateInstance()
        {
            return (UI_SkillCostBtn)UIPackage.CreateObject("RoleMain", "SkillCostBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            num = (GTextField)GetChild("num");
        }
    }
}