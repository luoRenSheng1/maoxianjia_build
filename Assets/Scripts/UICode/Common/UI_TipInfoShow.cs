/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_TipInfoShow : GComponent
    {
        public UI_FrameMask_Tips frame;
        public GButton close;
        public GTextField titleLb;
        public GRichTextField content;
        public GButton okBtn;
        public const string URL = "ui://0anhreylkv23dxy4f";

        public static UI_TipInfoShow CreateInstance()
        {
            return (UI_TipInfoShow)UIPackage.CreateObject("Common", "TipInfoShow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Tips)GetChild("frame");
            close = (GButton)GetChild("close");
            titleLb = (GTextField)GetChild("titleLb");
            content = (GRichTextField)GetChild("content");
            okBtn = (GButton)GetChild("okBtn");
        }
    }
}