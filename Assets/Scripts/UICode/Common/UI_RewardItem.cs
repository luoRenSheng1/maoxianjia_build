/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_RewardItem : GComponent
    {
        public Controller state;
        public GImage selectBg;
        public GButton item;
        public GTextField name;
        public GImage maskBg;
        public const string URL = "ui://0anhreyl9k6jdxyht";

        public static UI_RewardItem CreateInstance()
        {
            return (UI_RewardItem)UIPackage.CreateObject("Common", "RewardItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            state = GetController("state");
            selectBg = (GImage)GetChild("selectBg");
            item = (GButton)GetChild("item");
            name = (GTextField)GetChild("name");
            maskBg = (GImage)GetChild("maskBg");
        }
    }
}