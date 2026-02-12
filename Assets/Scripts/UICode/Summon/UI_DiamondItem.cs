/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_DiamondItem : GComponent
    {
        public Controller doubleCtrl;
        public Controller itemCtrl;
        public GLoader iconUrl;
        public GLoader img;
        public GButton chargeBtn;
        public GTextField curLb;
        public UI_ChargeBtn chargeBtn2;
        public const string URL = "ui://i7ojazuup3wt2q";

        public static UI_DiamondItem CreateInstance()
        {
            return (UI_DiamondItem)UIPackage.CreateObject("Summon", "DiamondItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            doubleCtrl = GetController("doubleCtrl");
            itemCtrl = GetController("itemCtrl");
            iconUrl = (GLoader)GetChild("iconUrl");
            img = (GLoader)GetChild("img");
            chargeBtn = (GButton)GetChild("chargeBtn");
            curLb = (GTextField)GetChild("curLb");
            chargeBtn2 = (UI_ChargeBtn)GetChild("chargeBtn2");
        }
    }
}