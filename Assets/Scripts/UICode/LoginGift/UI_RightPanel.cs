/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace LoginGift
{
    public partial class UI_RightPanel : GComponent
    {
        public Controller rewardCtrl;
        public GLoader firstItemIcon;
        public GButton buyBtn;
        public GGraph spine;
        public const string URL = "ui://z3lv15h6khmxg";

        public static UI_RightPanel CreateInstance()
        {
            return (UI_RightPanel)UIPackage.CreateObject("LoginGift", "RightPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rewardCtrl = GetController("rewardCtrl");
            firstItemIcon = (GLoader)GetChild("firstItemIcon");
            buyBtn = (GButton)GetChild("buyBtn");
            spine = (GGraph)GetChild("spine");
        }
    }
}