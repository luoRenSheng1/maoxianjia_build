/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_helpItem : GComponent
    {
        public GRichTextField content;
        public const string URL = "ui://0anhreylplio1c";

        public static UI_helpItem CreateInstance()
        {
            return (UI_helpItem)UIPackage.CreateObject("Common", "helpItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            content = (GRichTextField)GetChild("content");
        }
    }
}