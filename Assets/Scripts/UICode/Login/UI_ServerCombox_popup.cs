/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Login
{
    public partial class UI_ServerCombox_popup : GComponent
    {
        public GList list;
        public const string URL = "ui://h85hm7vmpchvxxp7";

        public static UI_ServerCombox_popup CreateInstance()
        {
            return (UI_ServerCombox_popup)UIPackage.CreateObject("Login", "ServerCombox_popup");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}