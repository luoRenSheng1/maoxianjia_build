/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassportItem : GComponent
    {
        public Controller reachCtrl;
        public GProgressBar expBar;
        public GLoader passIcon;
        public GTextField passLv;
        public UI_PassportRewardItem normalItem;
        public UI_PassportRewardItem superItem;
        public const string URL = "ui://2pcsnr2kr0ab3";

        public static UI_PassportItem CreateInstance()
        {
            return (UI_PassportItem)UIPackage.CreateObject("Passport", "PassportItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reachCtrl = GetController("reachCtrl");
            expBar = (GProgressBar)GetChild("expBar");
            passIcon = (GLoader)GetChild("passIcon");
            passLv = (GTextField)GetChild("passLv");
            normalItem = (UI_PassportRewardItem)GetChild("normalItem");
            superItem = (UI_PassportRewardItem)GetChild("superItem");
        }
    }
}