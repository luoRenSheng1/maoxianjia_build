/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonAdBtn : GButton
    {
        public Controller redCtrl;
        public GTextField adSummLb;
        public GTextField adLb;
        public const string URL = "ui://i7ojazuusurf9";

        public static UI_SummonAdBtn CreateInstance()
        {
            return (UI_SummonAdBtn)UIPackage.CreateObject("Summon", "SummonAdBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redCtrl = GetController("redCtrl");
            adSummLb = (GTextField)GetChild("adSummLb");
            adLb = (GTextField)GetChild("adLb");
        }
    }
}