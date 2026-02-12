/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ToastIconCom : GComponent
    {
        public GRichTextField txt_Icon_title;
        public GLoader loader_Icon;
        public const string URL = "ui://0anhreylhjvfxxqc";

        public static UI_ToastIconCom CreateInstance()
        {
            return (UI_ToastIconCom)UIPackage.CreateObject("Common", "ToastIconCom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txt_Icon_title = (GRichTextField)GetChild("txt_Icon_title");
            loader_Icon = (GLoader)GetChild("loader_Icon");
        }
    }
}