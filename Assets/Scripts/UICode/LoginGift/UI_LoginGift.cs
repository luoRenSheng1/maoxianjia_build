/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace LoginGift
{
    public partial class UI_LoginGift : GComponent
    {
        public GComponent frame;
        public UI_RightPanel rightPane;
        public UI_LeftPanel leftPane;
        public GButton closeBtn;
        public GLoader img;
        public const string URL = "ui://z3lv15h6khmxe";

        public static UI_LoginGift CreateInstance()
        {
            return (UI_LoginGift)UIPackage.CreateObject("LoginGift", "LoginGift");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            rightPane = (UI_RightPanel)GetChild("rightPane");
            leftPane = (UI_LeftPanel)GetChild("leftPane");
            closeBtn = (GButton)GetChild("closeBtn");
            img = (GLoader)GetChild("img");
        }
    }
}