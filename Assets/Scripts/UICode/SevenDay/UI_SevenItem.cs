/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace SevenDay
{
    public partial class UI_SevenItem : GButton
    {
        public Controller tag;
        public Controller day;
        public GButton item;
        public GButton rewardBtn;
        public const string URL = "ui://1tqiwa7amg7g7";

        public static UI_SevenItem CreateInstance()
        {
            return (UI_SevenItem)UIPackage.CreateObject("SevenDay", "SevenItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tag = GetController("tag");
            day = GetController("day");
            item = (GButton)GetChild("item");
            rewardBtn = (GButton)GetChild("rewardBtn");
        }
    }
}