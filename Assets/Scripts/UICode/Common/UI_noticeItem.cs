/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_noticeItem : GComponent
    {
        public GTextField title;
        public GRichTextField content;
        public const string URL = "ui://0anhreyl8824dxy3z";

        public static UI_noticeItem CreateInstance()
        {
            return (UI_noticeItem)UIPackage.CreateObject("Common", "noticeItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            title = (GTextField)GetChild("title");
            content = (GRichTextField)GetChild("content");
        }
    }
}