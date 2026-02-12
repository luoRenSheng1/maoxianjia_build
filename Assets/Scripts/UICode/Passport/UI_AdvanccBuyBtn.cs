/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_AdvanccBuyBtn : GButton
    {
        public Controller moneyType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://2pcsnr2kauqc1q";

        public static UI_AdvanccBuyBtn CreateInstance()
        {
            return (UI_AdvanccBuyBtn)UIPackage.CreateObject("Passport", "AdvanccBuyBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            moneyType = GetController("moneyType");
            moneyLb1 = (GTextField)GetChild("moneyLb1");
            moneyLb2 = (GTextField)GetChild("moneyLb2");
        }
    }
}