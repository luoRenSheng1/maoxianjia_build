/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_ExchangeCode : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GButton exchangeBtn;
        public GTextInput ipt;
        public const string URL = "ui://zs0w02qtqt85j";

        public static UI_ExchangeCode CreateInstance()
        {
            return (UI_ExchangeCode)UIPackage.CreateObject("Setting", "ExchangeCode");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            exchangeBtn = (GButton)GetChild("exchangeBtn");
            ipt = (GTextInput)GetChild("ipt");
        }
    }
}