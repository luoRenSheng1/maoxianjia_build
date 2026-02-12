/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_HolyItem : GButton
    {
        public Controller status;
        public GTextField level;
        public GLoader flag;
        public GComponent redPoint;
        public const string URL = "ui://m37flevda2kwdxyb1";

        public static UI_HolyItem CreateInstance()
        {
            return (UI_HolyItem)UIPackage.CreateObject("RoleMain", "HolyItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            level = (GTextField)GetChild("level");
            flag = (GLoader)GetChild("flag");
            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}