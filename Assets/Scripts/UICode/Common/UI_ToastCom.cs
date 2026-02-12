/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ToastCom : GComponent
    {
        public GImage bg;
        public GRichTextField txt_title;
        public const string URL = "ui://0anhreylqrl6te";

        public static UI_ToastCom CreateInstance()
        {
            return (UI_ToastCom)UIPackage.CreateObject("Common", "ToastCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GImage)GetChild("bg");
            txt_title = (GRichTextField)GetChild("txt_title");
        }
    }
}