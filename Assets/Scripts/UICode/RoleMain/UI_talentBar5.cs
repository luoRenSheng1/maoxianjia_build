/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar5 : GComponent
    {
        public Controller status;
        public const string URL = "ui://m37flevdp9n0dxy9a";

        public static UI_talentBar5 CreateInstance()
        {
            return (UI_talentBar5)UIPackage.CreateObject("RoleMain", "talentBar5");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
        }
    }
}