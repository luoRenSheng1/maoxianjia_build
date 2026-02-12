/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_ResetBtn : GButton
    {
        public Controller status;
        public GLoader itemIcon;
        public GTextField num;
        public const string URL = "ui://m37flevdp9n0dxy7e";

        public static UI_ResetBtn CreateInstance()
        {
            return (UI_ResetBtn)UIPackage.CreateObject("RoleMain", "ResetBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            itemIcon = (GLoader)GetChild("itemIcon");
            num = (GTextField)GetChild("num");
        }
    }
}