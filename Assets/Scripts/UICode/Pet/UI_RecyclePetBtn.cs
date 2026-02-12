/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_RecyclePetBtn : GButton
    {
        public GLoader itemIcon;
        public GTextField num;
        public const string URL = "ui://lxs2h4ifllb1dxyc7";

        public static UI_RecyclePetBtn CreateInstance()
        {
            return (UI_RecyclePetBtn)UIPackage.CreateObject("Pet", "RecyclePetBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemIcon = (GLoader)GetChild("itemIcon");
            num = (GTextField)GetChild("num");
        }
    }
}