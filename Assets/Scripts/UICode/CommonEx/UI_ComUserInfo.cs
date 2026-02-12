/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_ComUserInfo : GButton
    {
        public GLabel headIcon;
        public GTextField txtName;
        public GComponent comCurrency;
        public const string URL = "ui://5moj1x39rx0xo87";

        public static UI_ComUserInfo CreateInstance()
        {
            return (UI_ComUserInfo)UIPackage.CreateObject("CommonEx", "ComUserInfo");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            headIcon = (GLabel)GetChild("headIcon");
            txtName = (GTextField)GetChild("txtName");
            comCurrency = (GComponent)GetChild("comCurrency");
        }
    }
}