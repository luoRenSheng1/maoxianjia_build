/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace TreasureLevelup
{
    public partial class UI_BtnBuy : GButton
    {
        public GTextField money;
        public const string URL = "ui://v1wfpt3li9sl2a";

        public static UI_BtnBuy CreateInstance()
        {
            return (UI_BtnBuy)UIPackage.CreateObject("TreasureLevelup", "BtnBuy");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            money = (GTextField)GetChild("money");
        }
    }
}