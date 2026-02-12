/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ComMessage : GButton
    {
        public Controller type;
        public GButton btnChat;
        public GComponent redDot;
        public GButton chatBtn;
        public GTextField pNameLb;
        public GTextField contentLb;
        public GGroup pGroup;
        public const string URL = "ui://s7x7ku0npdpadxxyu";

        public static UI_ComMessage CreateInstance()
        {
            return (UI_ComMessage)UIPackage.CreateObject("Lobby", "ComMessage");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            btnChat = (GButton)GetChild("btnChat");
            redDot = (GComponent)GetChild("redDot");
            chatBtn = (GButton)GetChild("chatBtn");
            pNameLb = (GTextField)GetChild("pNameLb");
            contentLb = (GTextField)GetChild("contentLb");
            pGroup = (GGroup)GetChild("pGroup");
        }
    }
}