/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar2 : GComponent
    {
        public Controller status;
        public Controller type;
        public const string URL = "ui://m37flevdp9n0dxy97";

        public static UI_talentBar2 CreateInstance()
        {
            return (UI_talentBar2)UIPackage.CreateObject("RoleMain", "talentBar2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            type = GetController("type");
        }
    }
}