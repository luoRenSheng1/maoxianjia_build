/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleCurrency : GLabel
    {
        public GTextField txtValue;
        public const string URL = "ui://m37flevducd4dxyde";

        public static UI_RoleCurrency CreateInstance()
        {
            return (UI_RoleCurrency)UIPackage.CreateObject("RoleMain", "RoleCurrency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtValue = (GTextField)GetChild("txtValue");
        }
    }
}