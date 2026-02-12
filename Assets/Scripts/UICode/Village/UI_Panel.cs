/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_Panel : GComponent
    {
        public UI_MapPanel mapPanel;
        public GButton btnClose;
        public const string URL = "ui://8glegefcuobv0";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("Village", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            mapPanel = (UI_MapPanel)GetChild("mapPanel");
            btnClose = (GButton)GetChild("btnClose");
        }
    }
}