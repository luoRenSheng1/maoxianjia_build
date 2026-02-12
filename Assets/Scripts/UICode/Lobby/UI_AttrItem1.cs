/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_AttrItem1 : GComponent
    {
        public Controller attrCtrl;
        public GLoader icon;
        public GButton attrBtn;
        public GTextField num;
        public const string URL = "ui://s7x7ku0nax20dxy89";

        public static UI_AttrItem1 CreateInstance()
        {
            return (UI_AttrItem1)UIPackage.CreateObject("Lobby", "AttrItem1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrCtrl = GetController("attrCtrl");
            icon = (GLoader)GetChild("icon");
            attrBtn = (GButton)GetChild("attrBtn");
            num = (GTextField)GetChild("num");
        }
    }
}