/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_Hint : GComponent
    {
        public GComponent frame;
        public GTextField desc;
        public GButton yesBtn;
        public GButton noBtn;
        public GButton closeBtn2;
        public const string URL = "ui://8glegefcoppex";

        public static UI_Hint CreateInstance()
        {
            return (UI_Hint)UIPackage.CreateObject("Village", "Hint");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            desc = (GTextField)GetChild("desc");
            yesBtn = (GButton)GetChild("yesBtn");
            noBtn = (GButton)GetChild("noBtn");
            closeBtn2 = (GButton)GetChild("closeBtn2");
        }
    }
}