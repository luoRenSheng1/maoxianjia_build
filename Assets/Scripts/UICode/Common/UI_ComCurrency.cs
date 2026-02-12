/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ComCurrency : GComponent
    {
        public UI_BtnCurrency btnGold;
        public UI_BtnCurrency btnDia;
        public const string URL = "ui://0anhreylbtyu3f";

        public static UI_ComCurrency CreateInstance()
        {
            return (UI_ComCurrency)UIPackage.CreateObject("Common", "ComCurrency");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            btnGold = (UI_BtnCurrency)GetChild("btnGold");
            btnDia = (UI_BtnCurrency)GetChild("btnDia");
        }
    }
}