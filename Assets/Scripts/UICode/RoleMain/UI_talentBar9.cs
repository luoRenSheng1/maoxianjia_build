/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_talentBar9 : GComponent
    {
        public Controller status;
        public const string URL = "ui://m37flevdllb1dxyfq";

        public static UI_talentBar9 CreateInstance()
        {
            return (UI_talentBar9)UIPackage.CreateObject("RoleMain", "talentBar9");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
        }
    }
}