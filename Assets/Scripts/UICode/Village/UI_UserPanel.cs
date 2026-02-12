/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_UserPanel : GComponent
    {
        public GButton userInfo;
        public const string URL = "ui://8glegefcvnmw22";

        public static UI_UserPanel CreateInstance()
        {
            return (UI_UserPanel)UIPackage.CreateObject("Village", "UserPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            userInfo = (GButton)GetChild("userInfo");
        }
    }
}