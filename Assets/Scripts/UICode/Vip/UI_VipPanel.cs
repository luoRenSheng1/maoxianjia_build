/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Vip
{
    public partial class UI_VipPanel : GComponent
    {
        public Controller vipCtrl;
        public GComponent frame;
        public GProgressBar vipExpBar;
        public GTextField CurVipLvLb;
        public GTextField leftExpLb;
        public GButton tipBtn;
        public GTextField vipLbLb;
        public GTextField vipValueLb;
        public GButton preBtn;
        public GButton nextBtn;
        public GLabel vipDesc;
        public UI_VipRewardItem upItem;
        public UI_VipDiscountItem downItem;
        public GList vipChargeList;
        public GButton closeBtn;
        public const string URL = "ui://fqmq13s1sc7he";

        public static UI_VipPanel CreateInstance()
        {
            return (UI_VipPanel)UIPackage.CreateObject("Vip", "VipPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            vipCtrl = GetController("vipCtrl");
            frame = (GComponent)GetChild("frame");
            vipExpBar = (GProgressBar)GetChild("vipExpBar");
            CurVipLvLb = (GTextField)GetChild("CurVipLvLb");
            leftExpLb = (GTextField)GetChild("leftExpLb");
            tipBtn = (GButton)GetChild("tipBtn");
            vipLbLb = (GTextField)GetChild("vipLbLb");
            vipValueLb = (GTextField)GetChild("vipValueLb");
            preBtn = (GButton)GetChild("preBtn");
            nextBtn = (GButton)GetChild("nextBtn");
            vipDesc = (GLabel)GetChild("vipDesc");
            upItem = (UI_VipRewardItem)GetChild("upItem");
            downItem = (UI_VipDiscountItem)GetChild("downItem");
            vipChargeList = (GList)GetChild("vipChargeList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}