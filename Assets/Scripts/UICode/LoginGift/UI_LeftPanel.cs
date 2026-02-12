/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace LoginGift
{
    public partial class UI_LeftPanel : GComponent
    {
        public GList rewardList;
        public GTextField titleLb;
        public const string URL = "ui://z3lv15h6khmxh";

        public static UI_LeftPanel CreateInstance()
        {
            return (UI_LeftPanel)UIPackage.CreateObject("LoginGift", "LeftPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardList = (GList)GetChild("rewardList");
            titleLb = (GTextField)GetChild("titleLb");
        }
    }
}