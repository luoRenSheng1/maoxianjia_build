/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleOptBtn : GButton
    {
        public Controller colorType;
        public GLoader itemIcon;
        public GTextField itemCnt;
        public const string URL = "ui://m37flevdozj91t";

        public static UI_RoleOptBtn CreateInstance()
        {
            return (UI_RoleOptBtn)UIPackage.CreateObject("RoleMain", "RoleOptBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            colorType = GetController("colorType");
            itemIcon = (GLoader)GetChild("itemIcon");
            itemCnt = (GTextField)GetChild("itemCnt");
        }
    }
}