/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_Plus12 : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GLabel txtLb;
        public const string URL = "ui://h85hm7vmuq40xxpi";

        public static UI_Plus12 CreateInstance()
        {
            return (UI_Plus12)UIPackage.CreateObject("Login", "Plus12");
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