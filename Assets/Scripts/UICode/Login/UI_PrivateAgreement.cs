/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_PrivateAgreement : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GLabel txtLb;
        public const string URL = "ui://h85hm7vmuq40xxpg";

        public static UI_PrivateAgreement CreateInstance()
        {
            return (UI_PrivateAgreement)UIPackage.CreateObject("Login", "PrivateAgreement");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            txtLb = (GLabel)GetChild("txtLb");
        }
    }
}