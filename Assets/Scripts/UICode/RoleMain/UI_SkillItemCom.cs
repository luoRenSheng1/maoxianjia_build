/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_SkillItemCom : GButton
    {
        public Controller status;
        public GLoader bg;
        public const string URL = "ui://m37flevdp9n0dxy8h";

        public static UI_SkillItemCom CreateInstance()
        {
            return (UI_SkillItemCom)UIPackage.CreateObject("RoleMain", "SkillItemCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            bg = (GLoader)GetChild("bg");
        }
    }
}