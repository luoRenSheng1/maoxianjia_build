/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FightLoseAndWin
{
    public partial class UI_FailItem : GComponent
    {
        public Controller tjCtrl;
        public GLoader iconLoader;
        public GTextField descLb;
        public GButton gotoBtn;
        public const string URL = "ui://h7b921iwvcwq4";

        public static UI_FailItem CreateInstance()
        {
            return (UI_FailItem)UIPackage.CreateObject("FightLoseAndWin", "FailItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tjCtrl = GetController("tjCtrl");
            iconLoader = (GLoader)GetChild("iconLoader");
            descLb = (GTextField)GetChild("descLb");
            gotoBtn = (GButton)GetChild("gotoBtn");
        }
    }
}