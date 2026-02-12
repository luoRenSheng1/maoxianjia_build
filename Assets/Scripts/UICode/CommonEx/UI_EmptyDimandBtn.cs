/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_EmptyDimandBtn : GButton
    {
        public GLoader itemIcon;
        public GTextField diamondLb;
        public const string URL = "ui://5moj1x39qonndxy4r";

        public static UI_EmptyDimandBtn CreateInstance()
        {
            return (UI_EmptyDimandBtn)UIPackage.CreateObject("CommonEx", "EmptyDimandBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemIcon = (GLoader)GetChild("itemIcon");
            diamondLb = (GTextField)GetChild("diamondLb");
        }
    }
}