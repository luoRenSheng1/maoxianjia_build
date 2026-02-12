/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar7 : GComponent
    {
        public Controller status;
        public Controller type;
        public const string URL = "ui://m37flevdllb1dxyfo";

        public static UI_talentBar7 CreateInstance()
        {
            return (UI_talentBar7)UIPackage.CreateObject("RoleMain", "talentBar7");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            type = GetController("type");
        }
    }
}