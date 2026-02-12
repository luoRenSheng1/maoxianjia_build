/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_GameAgreement : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GRichTextField linkLb;
        public GButton RefusedBtn;
        public GButton agreeBtn;
        public const string URL = "ui://h85hm7vmuq40xxph";

        public static UI_GameAgreement CreateInstance()
        {
            return (UI_GameAgreement)UIPackage.CreateObject("Login", "GameAgreement");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            linkLb = (GRichTextField)GetChild("linkLb");
            RefusedBtn = (GButton)GetChild("RefusedBtn");
            agreeBtn = (GButton)GetChild("agreeBtn");
        }
    }
}