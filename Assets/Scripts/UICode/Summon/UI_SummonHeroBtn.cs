/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonHeroBtn : GButton
    {
        public Controller typeCtrl;
        public Controller redCtrl;
        public GTextField tickNumLb;
        public const string URL = "ui://i7ojazuusurfd";

        public static UI_SummonHeroBtn CreateInstance()
        {
            return (UI_SummonHeroBtn)UIPackage.CreateObject("Summon", "SummonHeroBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            redCtrl = GetController("redCtrl");
            tickNumLb = (GTextField)GetChild("tickNumLb");
        }
    }
}