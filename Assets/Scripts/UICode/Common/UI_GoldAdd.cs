/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GoldAdd : GComponent
    {
        public GImage icon;
        public GTextField num;
        public const string URL = "ui://0anhreylqfmcdxyh4";

        public static UI_GoldAdd CreateInstance()
        {
            return (UI_GoldAdd)UIPackage.CreateObject("Common", "GoldAdd");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GImage)GetChild("icon");
            num = (GTextField)GetChild("num");
        }
    }
}