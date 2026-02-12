/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_MessageBox : GComponent
    {
        public Controller typeOp;
        public GButton close;
        public GTextField titleLb;
        public GRichTextField content;
        public GButton okBtn;
        public GButton cancleBtn;
        public UI_HandTips HandTips;
        public const string URL = "ui://0anhreylfgvo1t";

        public static UI_MessageBox CreateInstance()
        {
            return (UI_MessageBox)UIPackage.CreateObject("Common", "MessageBox");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeOp = GetController("typeOp");
            close = (GButton)GetChild("close");
            titleLb = (GTextField)GetChild("titleLb");
            content = (GRichTextField)GetChild("content");
            okBtn = (GButton)GetChild("okBtn");
            cancleBtn = (GButton)GetChild("cancleBtn");
            HandTips = (UI_HandTips)GetChild("HandTips");
        }
    }
}