/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_HeadIcon : GButton
    {
        public Controller status;
        public Controller isUse;
        public const string URL = "ui://zs0w02qtbaxf10";

        public static UI_HeadIcon CreateInstance()
        {
            return (UI_HeadIcon)UIPackage.CreateObject("Setting", "HeadIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            isUse = GetController("isUse");
        }
    }
}