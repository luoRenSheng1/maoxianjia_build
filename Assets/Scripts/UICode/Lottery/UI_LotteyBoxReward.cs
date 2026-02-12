/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteyBoxReward : GComponent
    {
        public UI_boxExpBar expBar1;
        public UI_boxExpBar expBar2;
        public UI_boxItem box1;
        public UI_boxItem box2;
        public GTextField lvLb;
        public GTextField maxExp;
        public const string URL = "ui://6izp804wju03s";

        public static UI_LotteyBoxReward CreateInstance()
        {
            return (UI_LotteyBoxReward)UIPackage.CreateObject("Lottery", "LotteyBoxReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            expBar1 = (UI_boxExpBar)GetChild("expBar1");
            expBar2 = (UI_boxExpBar)GetChild("expBar2");
            box1 = (UI_boxItem)GetChild("box1");
            box2 = (UI_boxItem)GetChild("box2");
            lvLb = (GTextField)GetChild("lvLb");
            maxExp = (GTextField)GetChild("maxExp");
        }
    }
}