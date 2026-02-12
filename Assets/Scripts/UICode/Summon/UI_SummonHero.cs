/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonHero : GComponent
    {
        public Controller freeCtrl;
        public GLoader3D girlSpine;
        public UI_functionBtn giftBtn;
        public GButton tipsBtn;
        public UI_Currency ticketLb;
        public GButton closeBtn;
        public UI_SummonHeroBtn summon1;
        public UI_SummonHeroBtn summon10;
        public GButton summonFree;
        public GTextField freeTimeLb;
        public GGroup group;
        public GLoader yxjj;
        public const string URL = "ui://i7ojazuusurfc";

        public static UI_SummonHero CreateInstance()
        {
            return (UI_SummonHero)UIPackage.CreateObject("Summon", "SummonHero");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            freeCtrl = GetController("freeCtrl");
            girlSpine = (GLoader3D)GetChild("girlSpine");
            giftBtn = (UI_functionBtn)GetChild("giftBtn");
            tipsBtn = (GButton)GetChild("tipsBtn");
            ticketLb = (UI_Currency)GetChild("ticketLb");
            closeBtn = (GButton)GetChild("closeBtn");
            summon1 = (UI_SummonHeroBtn)GetChild("summon1");
            summon10 = (UI_SummonHeroBtn)GetChild("summon10");
            summonFree = (GButton)GetChild("summonFree");
            freeTimeLb = (GTextField)GetChild("freeTimeLb");
            group = (GGroup)GetChild("group");
            yxjj = (GLoader)GetChild("yxjj");
        }
    }
}