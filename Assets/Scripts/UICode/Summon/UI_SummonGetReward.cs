/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonGetReward : GComponent
    {
        public GComponent frame;
        public GLoader3D gxhdSpine;
        public GLoader img;
        public UI_ItemList rewardList;
        public GButton closeBtn;
        public UI_SummonBtn summon10;
        public UI_SummonBtn summon30;
        public UI_SummonBtn summon300;
        public GLoader close;
        public Transition shake;
        public Transition t1;
        public const string URL = "ui://i7ojazuusurfe";

        public static UI_SummonGetReward CreateInstance()
        {
            return (UI_SummonGetReward)UIPackage.CreateObject("Summon", "SummonGetReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            gxhdSpine = (GLoader3D)GetChild("gxhdSpine");
            img = (GLoader)GetChild("img");
            rewardList = (UI_ItemList)GetChild("rewardList");
            closeBtn = (GButton)GetChild("closeBtn");
            summon10 = (UI_SummonBtn)GetChild("summon10");
            summon30 = (UI_SummonBtn)GetChild("summon30");
            summon300 = (UI_SummonBtn)GetChild("summon300");
            close = (GLoader)GetChild("close");
            shake = GetTransition("shake");
            t1 = GetTransition("t1");
        }
    }
}