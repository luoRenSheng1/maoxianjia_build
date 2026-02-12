/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FirstPay
{
    public partial class UI_FirstPayBtn : GButton
    {
        public Controller moneyType;
        public GTextField money;
        public GTextField money2;
        public const string URL = "ui://ksm9s29pez2lh";

        public static UI_FirstPayBtn CreateInstance()
        {
            return (UI_FirstPayBtn)UIPackage.CreateObject("FirstPay", "FirstPayBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            moneyType = GetController("moneyType");
            money = (GTextField)GetChild("money");
            money2 = (GTextField)GetChild("money2");
        }
    }
}