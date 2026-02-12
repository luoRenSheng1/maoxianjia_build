/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_DiamondAdd : GComponent
    {
        public GImage icon;
        public GTextField num;
        public const string URL = "ui://0anhreylqfmcdxyh6";

        public static UI_DiamondAdd CreateInstance()
        {
            return (UI_DiamondAdd)UIPackage.CreateObject("Common", "DiamondAdd");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GImage)GetChild("icon");
            num = (GTextField)GetChild("num");
        }
    }
}