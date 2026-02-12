/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_HuntingShopMain : GComponent
    {
        public GComponent frame;
        public GLabel currency;
        public GList list;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3ketdju5n";

        public static UI_HuntingShopMain CreateInstance()
        {
            return (UI_HuntingShopMain)UIPackage.CreateObject("BigMap", "HuntingShopMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            currency = (GLabel)GetChild("currency");
            list = (GList)GetChild("list");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}