/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar1 : GComponent
    {
        public Controller status;
        public Controller type;
        public const string URL = "ui://m37flevdp9n0dxy96";

        public static UI_talentBar1 CreateInstance()
        {
            return (UI_talentBar1)UIPackage.CreateObject("RoleMain", "talentBar1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            type = GetController("type");
        }
    }
}