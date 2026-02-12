/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonHeroReward : GComponent
    {
        public GComponent frame;
        public UI_ItemList rewardList;
        public GButton closeBtn;
        public UI_SummonHeroBtn summon1;
        public UI_SummonHeroBtn summon10;
        public GLoader3D gxhdSpine;
        public GLoader img;
        public Transition shake;
        public Transition t1;
        public const string URL = "ui://i7ojazuuq9cg1a";

        public static UI_SummonHeroReward CreateInstance()
        {
            return (UI_SummonHeroReward)UIPackage.CreateObject("Summon", "SummonHeroReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            rewardList = (UI_ItemList)GetChild("rewardList");
            closeBtn = (GButton)GetChild("closeBtn");
            summon1 = (UI_SummonHeroBtn)GetChild("summon1");
            summon10 = (UI_SummonHeroBtn)GetChild("summon10");
            gxhdSpine = (GLoader3D)GetChild("gxhdSpine");
            img = (GLoader)GetChild("img");
            shake = GetTransition("shake");
            t1 = GetTransition("t1");
        }
    }
}