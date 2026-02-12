/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonItem : GComponent
    {
        public Controller typeCtrl;
        public Controller lockCtrl;
        public GLoader itemBgUrl;
        public GTextField lvLb;
        public UI_SummonBarExp expBar;
        public GButton tipsBtn;
        public UI_SummonAdBtn adSummon;
        public UI_SummonBtn Summon10;
        public UI_SummonBtn Summon30;
        public GTextField lockDesc;
        public const string URL = "ui://i7ojazuusurf7";

        public static UI_SummonItem CreateInstance()
        {
            return (UI_SummonItem)UIPackage.CreateObject("Summon", "SummonItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            lockCtrl = GetController("lockCtrl");
            itemBgUrl = (GLoader)GetChild("itemBgUrl");
            lvLb = (GTextField)GetChild("lvLb");
            expBar = (UI_SummonBarExp)GetChild("expBar");
            tipsBtn = (GButton)GetChild("tipsBtn");
            adSummon = (UI_SummonAdBtn)GetChild("adSummon");
            Summon10 = (UI_SummonBtn)GetChild("Summon10");
            Summon30 = (UI_SummonBtn)GetChild("Summon30");
            lockDesc = (GTextField)GetChild("lockDesc");
        }
    }
}