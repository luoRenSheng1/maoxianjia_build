/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RuneItem : GComponent
    {
        public Controller hasCtrl;
        public Controller ctrlQuality;
        public Controller showHandCtrl;
        public Controller bgiCtrl;
        public Controller uploadCtrl;
        public Controller countCtrl;
        public GLoader icon;
        public GTextField countLb;
        public GComponent redDot;
        public const string URL = "ui://m37flevdeau0dxy6s";

        public static UI_RuneItem CreateInstance()
        {
            return (UI_RuneItem)UIPackage.CreateObject("RoleMain", "RuneItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hasCtrl = GetController("hasCtrl");
            ctrlQuality = GetController("ctrlQuality");
            showHandCtrl = GetController("showHandCtrl");
            bgiCtrl = GetController("bgiCtrl");
            uploadCtrl = GetController("uploadCtrl");
            countCtrl = GetController("countCtrl");
            icon = (GLoader)GetChild("icon");
            countLb = (GTextField)GetChild("countLb");
            redDot = (GComponent)GetChild("redDot");
        }
    }
}