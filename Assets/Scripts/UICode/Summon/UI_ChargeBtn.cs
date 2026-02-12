/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_ChargeBtn : GButton
    {
        public Controller moneyType;
        public GTextField moneyLb1;
        public GTextField moneyLb2;
        public const string URL = "ui://i7ojazuuf34j3e";

        public static UI_ChargeBtn CreateInstance()
        {
            return (UI_ChargeBtn)UIPackage.CreateObject("Summon", "ChargeBtn");
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