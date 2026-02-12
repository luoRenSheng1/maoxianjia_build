/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_HelpItemPicture : GComponent
    {
        public GLoader icon;
        public GRichTextField title;
        public const string URL = "ui://0anhreylqofb1f";

        public static UI_HelpItemPicture CreateInstance()
        {
            return (UI_HelpItemPicture)UIPackage.CreateObject("Common", "HelpItemPicture");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
            title = (GRichTextField)GetChild("title");
        }
    }
}