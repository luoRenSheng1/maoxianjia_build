/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_PlayerNameEdit : GComponent
    {
        public Controller freeType;
        public GComponent frame;
        public GButton closeBtn;
        public GTextInput ipt;
        public GTextField tips;
        public GButton freeBtn;
        public GButton moneyBtn;
        public const string URL = "ui://zs0w02qtqt85k";

        public static UI_PlayerNameEdit CreateInstance()
        {
            return (UI_PlayerNameEdit)UIPackage.CreateObject("Setting", "PlayerNameEdit");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            freeType = GetController("freeType");
            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            ipt = (GTextInput)GetChild("ipt");
            tips = (GTextField)GetChild("tips");
            freeBtn = (GButton)GetChild("freeBtn");
            moneyBtn = (GButton)GetChild("moneyBtn");
        }
    }
}