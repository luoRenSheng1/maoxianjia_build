/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FirstPay
{
    public partial class UI_FirstItem : GComponent
    {
        public Controller isGet;
        public GButton item;
        public const string URL = "ui://ksm9s29pl9l9j";

        public static UI_FirstItem CreateInstance()
        {
            return (UI_FirstItem)UIPackage.CreateObject("FirstPay", "FirstItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isGet = GetController("isGet");
            item = (GButton)GetChild("item");
        }
    }
}