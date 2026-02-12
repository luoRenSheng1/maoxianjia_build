/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_LoreEquip : GComponent
    {
        public UI_EquipCurrency goldCur;
        public UI_EquipCurrency diaCur;
        public GList upLoreList;
        public UI_LoreEquipItem loreEquip0;
        public UI_LoreEquipItem loreEquip1;
        public UI_LoreEquipItem loreEquip2;
        public UI_LoreEquipItem loreEquip3;
        public GLoader vacancyBg;
        public GList loreEquipList;
        public GButton recycleBtn;
        public const string URL = "ui://ddc23erlef8udxyav";

        public static UI_LoreEquip CreateInstance()
        {
            return (UI_LoreEquip)UIPackage.CreateObject("Equip", "LoreEquip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            goldCur = (UI_EquipCurrency)GetChild("goldCur");
            diaCur = (UI_EquipCurrency)GetChild("diaCur");
            upLoreList = (GList)GetChild("upLoreList");
            loreEquip0 = (UI_LoreEquipItem)GetChild("loreEquip0");
            loreEquip1 = (UI_LoreEquipItem)GetChild("loreEquip1");
            loreEquip2 = (UI_LoreEquipItem)GetChild("loreEquip2");
            loreEquip3 = (UI_LoreEquipItem)GetChild("loreEquip3");
            vacancyBg = (GLoader)GetChild("vacancyBg");
            loreEquipList = (GList)GetChild("loreEquipList");
            recycleBtn = (GButton)GetChild("recycleBtn");
        }
    }
}