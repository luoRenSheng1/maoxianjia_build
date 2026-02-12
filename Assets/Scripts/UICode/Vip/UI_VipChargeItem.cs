/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_VipChargeItem : GComponent
    {
        public Controller itemCtrl;
        public Controller doubleCtrl;
        public Controller priceType;
        public GLoader chargeIcon;
        public UI_VipChargeBtn chargeBtn;
        public GTextField curLb;
        public const string URL = "ui://fqmq13s1sc7hh";

        public static UI_VipChargeItem CreateInstance()
        {
            return (UI_VipChargeItem)UIPackage.CreateObject("Vip", "VipChargeItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemCtrl = GetController("itemCtrl");
            doubleCtrl = GetController("doubleCtrl");
            priceType = GetController("priceType");
            chargeIcon = (GLoader)GetChild("chargeIcon");
            chargeBtn = (UI_VipChargeBtn)GetChild("chargeBtn");
            curLb = (GTextField)GetChild("curLb");
        }
    }
}