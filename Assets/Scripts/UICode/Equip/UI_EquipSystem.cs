/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_EquipSystem : GComponent
    {
        public Controller typeCtrl;
        public UI_EquipSystemEquip commonEquip;
        public GList tabList;
        public const string URL = "ui://ddc23erlef8udxy9y";

        public static UI_EquipSystem CreateInstance()
        {
            return (UI_EquipSystem)UIPackage.CreateObject("Equip", "EquipSystem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            commonEquip = (UI_EquipSystemEquip)GetChild("commonEquip");
            tabList = (GList)GetChild("tabList");
        }
    }
}