/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_ServerSelect : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GList serverList;
        public GButton changeSerBtn;
        public const string URL = "ui://zs0w02qtq36tp";

        public static UI_ServerSelect CreateInstance()
        {
            return (UI_ServerSelect)UIPackage.CreateObject("Setting", "ServerSelect");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            serverList = (GList)GetChild("serverList");
            changeSerBtn = (GButton)GetChild("changeSerBtn");
        }
    }
}