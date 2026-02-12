/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_GameUserAgreement : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GLabel txtLb;
        public const string URL = "ui://h85hm7vmuq40xxpe";

        public static UI_GameUserAgreement CreateInstance()
        {
            return (UI_GameUserAgreement)UIPackage.CreateObject("Login", "GameUserAgreement");
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