/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBoxKey : GButton
    {
        public GTextField txtKeyValue;
        public const string URL = "ui://s7x7ku0nt5mtdxy0t";

        public static UI_BtnBoxKey CreateInstance()
        {
            return (UI_BtnBoxKey)UIPackage.CreateObject("Lobby", "BtnBoxKey");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            txtKeyValue = (GTextField)GetChild("txtKeyValue");
        }
    }
}