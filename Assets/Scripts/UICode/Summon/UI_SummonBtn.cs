/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonBtn : GButton
    {
        public Controller ctrl;
        public Controller redCtrl;
        public GTextField Free;
        public GTextField tickNumLb;
        public const string URL = "ui://i7ojazuusurf8";

        public static UI_SummonBtn CreateInstance()
        {
            return (UI_SummonBtn)UIPackage.CreateObject("Summon", "SummonBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            redCtrl = GetController("redCtrl");
            Free = (GTextField)GetChild("Free");
            tickNumLb = (GTextField)GetChild("tickNumLb");
        }
    }
}