/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_ComEquipItem : GComponent
    {
        public Controller ctrlQuality;
        public GLoader icon;
        public GTextField txtLv;
        public const string URL = "ui://5moj1x39skp6p";

        public static UI_ComEquipItem CreateInstance()
        {
            return (UI_ComEquipItem)UIPackage.CreateObject("CommonEx", "ComEquipItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrlQuality = GetController("ctrlQuality");
            icon = (GLoader)GetChild("icon");
            txtLv = (GTextField)GetChild("txtLv");
        }
    }
}