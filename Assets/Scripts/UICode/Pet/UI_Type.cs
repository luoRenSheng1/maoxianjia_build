/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_Type : GComponent
    {
        public Controller type;
        public GLoader icon;
        public GTextField title;
        public const string URL = "ui://lxs2h4ifhz5cdxy77";

        public static UI_Type CreateInstance()
        {
            return (UI_Type)UIPackage.CreateObject("Pet", "Type");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            icon = (GLoader)GetChild("icon");
            title = (GTextField)GetChild("title");
        }
    }
}