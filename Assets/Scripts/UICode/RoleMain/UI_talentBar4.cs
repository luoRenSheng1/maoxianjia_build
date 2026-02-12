/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar4 : GComponent
    {
        public Controller status;
        public const string URL = "ui://m37flevdp9n0dxy99";

        public static UI_talentBar4 CreateInstance()
        {
            return (UI_talentBar4)UIPackage.CreateObject("RoleMain", "talentBar4");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
        }
    }
}