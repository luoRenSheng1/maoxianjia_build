/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_VipRewardItem : GComponent
    {
        public Controller itemCtrl;
        public GList itemList;
        public GButton chargeBtn;
        public const string URL = "ui://fqmq13s1sc7hg";

        public static UI_VipRewardItem CreateInstance()
        {
            return (UI_VipRewardItem)UIPackage.CreateObject("Vip", "VipRewardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemCtrl = GetController("itemCtrl");
            itemList = (GList)GetChild("itemList");
            chargeBtn = (GButton)GetChild("chargeBtn");
        }
    }
}