/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_LangComboBox_popup : GComponent
    {
        public GList list;
        public const string URL = "ui://zs0w02qtogek12";

        public static UI_LangComboBox_popup CreateInstance()
        {
            return (UI_LangComboBox_popup)UIPackage.CreateObject("Setting", "LangComboBox_popup");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            list = (GList)GetChild("list");
        }
    }
}