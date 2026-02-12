/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_VipChargeBtn : GButton
    {
        public Controller priceType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://fqmq13s1sc7ho";

        public static UI_VipChargeBtn CreateInstance()
        {
            return (UI_VipChargeBtn)UIPackage.CreateObject("Vip", "VipChargeBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            priceType = GetController("priceType");
            moneyLb1 = (GTextField)GetChild("moneyLb1");
            moneyLb2 = (GTextField)GetChild("moneyLb2");
        }
    }
}