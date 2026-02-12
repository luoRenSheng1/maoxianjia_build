/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_TalentUI : GComponent
    {
        public UI_Talent talent;
        public const string URL = "ui://m37flevdllb1dxyfr";

        public static UI_TalentUI CreateInstance()
        {
            return (UI_TalentUI)UIPackage.CreateObject("RoleMain", "TalentUI");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            talent = (UI_Talent)GetChild("talent");
        }
    }
}