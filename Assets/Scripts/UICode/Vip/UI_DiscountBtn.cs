/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_DiscountBtn : GComponent
    {
        public GButton chargeBtn;
        public GTextField totalLb;
        public GTextField curLb;
        public GTextField distLb;
        public const string URL = "ui://fqmq13s1sc7hr";

        public static UI_DiscountBtn CreateInstance()
        {
            return (UI_DiscountBtn)UIPackage.CreateObject("Vip", "DiscountBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            chargeBtn = (GButton)GetChild("chargeBtn");
            totalLb = (GTextField)GetChild("totalLb");
            curLb = (GTextField)GetChild("curLb");
            distLb = (GTextField)GetChild("distLb");
        }
    }
}