/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_VipDiscountItem : GComponent
    {
        public Controller itemCtrl;
        public GList itemList;
        public GButton chargeBtn;
        public UI_DiscountBtn distCom;
        public const string URL = "ui://fqmq13s1sc7hs";

        public static UI_VipDiscountItem CreateInstance()
        {
            return (UI_VipDiscountItem)UIPackage.CreateObject("Vip", "VipDiscountItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemCtrl = GetController("itemCtrl");
            itemList = (GList)GetChild("itemList");
            chargeBtn = (GButton)GetChild("chargeBtn");
            distCom = (UI_DiscountBtn)GetChild("distCom");
        }
    }
}