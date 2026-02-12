/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SpecialCard : GComponent
    {
        public Controller ctrl;
        public Controller getCtrl;
        public GButton closeBtn;
        public GButton buyBtn;
        public UI_CardItem item0;
        public UI_CardItem item1;
        public GTextField timeLb;
        public GButton getBtn;
        public const string URL = "ui://i7ojazuuq9cg13";

        public static UI_SpecialCard CreateInstance()
        {
            return (UI_SpecialCard)UIPackage.CreateObject("Summon", "SpecialCard");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            getCtrl = GetController("getCtrl");
            closeBtn = (GButton)GetChild("closeBtn");
            buyBtn = (GButton)GetChild("buyBtn");
            item0 = (UI_CardItem)GetChild("item0");
            item1 = (UI_CardItem)GetChild("item1");
            timeLb = (GTextField)GetChild("timeLb");
            getBtn = (GButton)GetChild("getBtn");
        }
    }
}