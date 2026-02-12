/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_HolyCost : GComponent
    {
        public Controller status;
        public GLoader icon;
        public GTextField num;
        public const string URL = "ui://m37flevda2kwdxyb0";

        public static UI_HolyCost CreateInstance()
        {
            return (UI_HolyCost)UIPackage.CreateObject("RoleMain", "HolyCost");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            icon = (GLoader)GetChild("icon");
            num = (GTextField)GetChild("num");
        }
    }
}