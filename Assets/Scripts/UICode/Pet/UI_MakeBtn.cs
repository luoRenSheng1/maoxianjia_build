/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_MakeBtn : GButton
    {
        public GLoader itemIcon;
        public GTextField num;
        public const string URL = "ui://lxs2h4ifllb1dxyc6";

        public static UI_MakeBtn CreateInstance()
        {
            return (UI_MakeBtn)UIPackage.CreateObject("Pet", "MakeBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemIcon = (GLoader)GetChild("itemIcon");
            num = (GTextField)GetChild("num");
        }
    }
}