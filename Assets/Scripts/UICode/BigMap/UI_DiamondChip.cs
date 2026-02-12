/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_DiamondChip : GComponent
    {
        public GLoader icon1;
        public GLoader icon2;
        public const string URL = "ui://pdufy3kexq5aid0";

        public static UI_DiamondChip CreateInstance()
        {
            return (UI_DiamondChip)UIPackage.CreateObject("BigMap", "DiamondChip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon1 = (GLoader)GetChild("icon1");
            icon2 = (GLoader)GetChild("icon2");
        }
    }
}