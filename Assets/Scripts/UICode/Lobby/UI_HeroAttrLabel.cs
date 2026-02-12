/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_HeroAttrLabel : GLabel
    {
        public Controller shuxingCtrl;
        public GTextField attrLb;
        public Transition t0;
        public const string URL = "ui://s7x7ku0nkhmxdxy5j";

        public static UI_HeroAttrLabel CreateInstance()
        {
            return (UI_HeroAttrLabel)UIPackage.CreateObject("Lobby", "HeroAttrLabel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            shuxingCtrl = GetController("shuxingCtrl");
            attrLb = (GTextField)GetChild("attrLb");
            t0 = GetTransition("t0");
        }
    }
}