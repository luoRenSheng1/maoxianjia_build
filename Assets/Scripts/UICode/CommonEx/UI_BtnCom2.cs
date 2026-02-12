/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BtnCom2 : GButton
    {
        public GLoader itemIcon;
        public GTextField num;
        public const string URL = "ui://5moj1x39hz5cdxy9b";

        public static UI_BtnCom2 CreateInstance()
        {
            return (UI_BtnCom2)UIPackage.CreateObject("CommonEx", "BtnCom2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            itemIcon = (GLoader)GetChild("itemIcon");
            num = (GTextField)GetChild("num");
        }
    }
}