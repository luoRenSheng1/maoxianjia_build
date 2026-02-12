/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_fengkuangBuyBtn : GButton
    {
        public Controller moneyType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://nmzfxo89ftc9x";

        public static UI_fengkuangBuyBtn CreateInstance()
        {
            return (UI_fengkuangBuyBtn)UIPackage.CreateObject("Shop", "fengkuangBuyBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            moneyType = GetController("moneyType");
            moneyLb1 = (GTextField)GetChild("moneyLb1");
            moneyLb2 = (GTextField)GetChild("moneyLb2");
        }
    }
}