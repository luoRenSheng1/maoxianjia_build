/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_Announcement : GLabel
    {
        public GRichTextField textPrivacyPolicy;
        public const string URL = "ui://h85hm7vmkgyuxxoh";

        public static UI_Announcement CreateInstance()
        {
            return (UI_Announcement)UIPackage.CreateObject("Login", "Announcement");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            textPrivacyPolicy = (GRichTextField)GetChild("textPrivacyPolicy");
        }
    }
}