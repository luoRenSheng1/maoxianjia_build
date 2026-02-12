/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_AdvancePassportBtn : GButton
    {
        public Controller moneyType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://2pcsnr2kr0ab16";

        public static UI_AdvancePassportBtn CreateInstance()
        {
            return (UI_AdvancePassportBtn)UIPackage.CreateObject("Passport", "AdvancePassportBtn");
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