/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace LoginGift
{
    public partial class UI_RewardItem : GComponent
    {
        public GTextField cntLb;
        public GButton item;
        public const string URL = "ui://z3lv15h6khmxi";

        public static UI_RewardItem CreateInstance()
        {
            return (UI_RewardItem)UIPackage.CreateObject("LoginGift", "RewardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cntLb = (GTextField)GetChild("cntLb");
            item = (GButton)GetChild("item");
        }
    }
}