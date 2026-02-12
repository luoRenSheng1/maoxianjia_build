/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_GrimoireItem : GComponent
    {
        public GLoader bg;
        public GLoader icon;
        public GButton grimoireBtn;
        public GTextField name;
        public GTextField Lv;
        public GList attrList;
        public UI_UpAniBtn upAniBtn;
        public const string URL = "ui://s7x7ku0nax20dxy88";

        public static UI_GrimoireItem CreateInstance()
        {
            return (UI_GrimoireItem)UIPackage.CreateObject("Lobby", "GrimoireItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bg = (GLoader)GetChild("bg");
            icon = (GLoader)GetChild("icon");
            grimoireBtn = (GButton)GetChild("grimoireBtn");
            name = (GTextField)GetChild("name");
            Lv = (GTextField)GetChild("Lv");
            attrList = (GList)GetChild("attrList");
            upAniBtn = (UI_UpAniBtn)GetChild("upAniBtn");
        }
    }
}