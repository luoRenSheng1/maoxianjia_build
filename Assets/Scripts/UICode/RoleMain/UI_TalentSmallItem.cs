/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_TalentSmallItem : GButton
    {
        public Controller status;
        public GLoader bg;
        public GTextField Lv;
        public const string URL = "ui://m37flevdllb1dxyfl";

        public static UI_TalentSmallItem CreateInstance()
        {
            return (UI_TalentSmallItem)UIPackage.CreateObject("RoleMain", "TalentSmallItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            bg = (GLoader)GetChild("bg");
            Lv = (GTextField)GetChild("Lv");
        }
    }
}