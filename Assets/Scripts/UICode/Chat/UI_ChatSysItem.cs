/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Chat
{
    public partial class UI_ChatSysItem : GComponent
    {
        public GTextField content;
        public GTextField playerName;
        public const string URL = "ui://x1rxl2idkvszn";

        public static UI_ChatSysItem CreateInstance()
        {
            return (UI_ChatSysItem)UIPackage.CreateObject("Chat", "ChatSysItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            content = (GTextField)GetChild("content");
            playerName = (GTextField)GetChild("playerName");
        }
    }
}