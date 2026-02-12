/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonProbability : GComponent
    {
        public GComponent frame;
        public GTextField lvLb;
        public GButton preBtn;
        public GButton nextBtn;
        public GList propList;
        public GButton closeBtn;
        public const string URL = "ui://i7ojazuusurfg";

        public static UI_SummonProbability CreateInstance()
        {
            return (UI_SummonProbability)UIPackage.CreateObject("Summon", "SummonProbability");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            lvLb = (GTextField)GetChild("lvLb");
            preBtn = (GButton)GetChild("preBtn");
            nextBtn = (GButton)GetChild("nextBtn");
            propList = (GList)GetChild("propList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}