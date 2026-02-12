/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_BoxItem : GComponent
    {
        public Controller rewardCtrl;
        public GProgressBar tempExp;
        public UI_PassBarExp expBar;
        public GLoader boxDetail;
        public GComponent redDot;
        public GButton boxGetBtn;
        public GTextField boxGetLb;
        public GTextField boxDesc;
        public const string URL = "ui://2pcsnr2kr0ab2";

        public static UI_BoxItem CreateInstance()
        {
            return (UI_BoxItem)UIPackage.CreateObject("Passport", "BoxItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            tempExp = (GProgressBar)GetChild("tempExp");
            expBar = (UI_PassBarExp)GetChild("expBar");
            boxDetail = (GLoader)GetChild("boxDetail");
            redDot = (GComponent)GetChild("redDot");
            boxGetBtn = (GButton)GetChild("boxGetBtn");
            boxGetLb = (GTextField)GetChild("boxGetLb");
            boxDesc = (GTextField)GetChild("boxDesc");
        }
    }
}