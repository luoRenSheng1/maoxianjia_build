/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_ServerItem : GButton
    {
        public GTextField serverName;
        public const string URL = "ui://zs0w02qtq36tq";

        public static UI_ServerItem CreateInstance()
        {
            return (UI_ServerItem)UIPackage.CreateObject("Setting", "ServerItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            serverName = (GTextField)GetChild("serverName");
        }
    }
}