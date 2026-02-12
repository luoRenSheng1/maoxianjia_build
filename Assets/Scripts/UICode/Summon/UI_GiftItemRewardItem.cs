/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_GiftItemRewardItem : GComponent
    {
        public GButton item;
        public GTextField count;
        public const string URL = "ui://i7ojazuuftc92o";

        public static UI_GiftItemRewardItem CreateInstance()
        {
            return (UI_GiftItemRewardItem)UIPackage.CreateObject("Summon", "GiftItemRewardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            item = (GButton)GetChild("item");
            count = (GTextField)GetChild("count");
        }
    }
}