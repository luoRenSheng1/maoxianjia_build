/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace AdventureCave
{
    public partial class UI_AdventureCaveShop : GComponent
    {
        public GComponent frame;
        public GLabel dim;
        public GLoader titIcon;
        public GTextField title;
        public GButton close;
        public GList list;
        public const string URL = "ui://z350mxkhrjj21b";

        public static UI_AdventureCaveShop CreateInstance()
        {
            return (UI_AdventureCaveShop)UIPackage.CreateObject("AdventureCave", "AdventureCaveShop");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            dim = (GLabel)GetChild("dim");
            titIcon = (GLoader)GetChild("titIcon");
            title = (GTextField)GetChild("title");
            close = (GButton)GetChild("close");
            list = (GList)GetChild("list");
        }
    }
}