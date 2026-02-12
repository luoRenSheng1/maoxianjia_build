/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar6 : GComponent
    {
        public Controller status;
        public Controller type;
        public const string URL = "ui://m37flevdp9n0dxy9b";

        public static UI_talentBar6 CreateInstance()
        {
            return (UI_talentBar6)UIPackage.CreateObject("RoleMain", "talentBar6");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            type = GetController("type");
        }
    }
}