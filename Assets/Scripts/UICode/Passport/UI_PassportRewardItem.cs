/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassportRewardItem : GComponent
    {
        public Controller rewardCtrl;
        public Controller lockCtrl;
        public GButton item;
        public GButton getBtn;
        public const string URL = "ui://2pcsnr2kr0ab14";

        public static UI_PassportRewardItem CreateInstance()
        {
            return (UI_PassportRewardItem)UIPackage.CreateObject("Passport", "PassportRewardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            lockCtrl = GetController("lockCtrl");
            item = (GButton)GetChild("item");
            getBtn = (GButton)GetChild("getBtn");
        }
    }
}