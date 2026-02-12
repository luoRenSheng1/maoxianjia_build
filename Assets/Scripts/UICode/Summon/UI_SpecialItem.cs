/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SpecialItem : GComponent
    {
        public Controller lockCtrl;
        public GLoader bgLoader;
        public GTextField desc;
        public GButton goBtn;
        public GComponent redDot;
        public GTextField lockDesc;
        public const string URL = "ui://i7ojazuusurfb";

        public static UI_SpecialItem CreateInstance()
        {
            return (UI_SpecialItem)UIPackage.CreateObject("Summon", "SpecialItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            bgLoader = (GLoader)GetChild("bgLoader");
            desc = (GTextField)GetChild("desc");
            goBtn = (GButton)GetChild("goBtn");
            redDot = (GComponent)GetChild("redDot");
            lockDesc = (GTextField)GetChild("lockDesc");
        }
    }
}