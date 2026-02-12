/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_GoldMine : GComponent
    {
        public GLoader icon;
        public const string URL = "ui://pdufy3ke100ni8h";

        public static UI_GoldMine CreateInstance()
        {
            return (UI_GoldMine)UIPackage.CreateObject("BigMap", "GoldMine");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GLoader)GetChild("icon");
        }
    }
}