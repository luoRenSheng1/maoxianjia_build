/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_NewEquipObtain : GComponent
    {
        public Controller fightCtrl;
        public Controller isNew;
        public Controller isGuide;
        public Controller btnCtrl;
        public Controller hasEntryOfOld;
        public Controller hasEntryOfNew;
        public GComponent equip3;
        public GTextField name3;
        public GTextField equipLv3;
        public GTextField fightLb3;
        public GList mainAttrList3;
        public GList attrList3;
        public GComponent equip4;
        public GTextField name4;
        public GTextField equipLv4;
        public GTextField fightLb4;
        public GList mainAttrList4;
        public GList attrList4;
        public GButton autoSell2;
        public GButton resolveBtn4;
        public GButton repalceBtn4;
        public GButton recycleBtn4;
        public GButton replaceBtn5;
        public const string URL = "ui://ddc23erlhdtee";

        public static UI_NewEquipObtain CreateInstance()
        {
            return (UI_NewEquipObtain)UIPackage.CreateObject("Equip", "NewEquipObtain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            fightCtrl = GetController("fightCtrl");
            isNew = GetController("isNew");
            isGuide = GetController("isGuide");
            btnCtrl = GetController("btnCtrl");
            hasEntryOfOld = GetController("hasEntryOfOld");
            hasEntryOfNew = GetController("hasEntryOfNew");
            equip3 = (GComponent)GetChild("equip3");
            name3 = (GTextField)GetChild("name3");
            equipLv3 = (GTextField)GetChild("equipLv3");
            fightLb3 = (GTextField)GetChild("fightLb3");
            mainAttrList3 = (GList)GetChild("mainAttrList3");
            attrList3 = (GList)GetChild("attrList3");
            equip4 = (GComponent)GetChild("equip4");
            name4 = (GTextField)GetChild("name4");
            equipLv4 = (GTextField)GetChild("equipLv4");
            fightLb4 = (GTextField)GetChild("fightLb4");
            mainAttrList4 = (GList)GetChild("mainAttrList4");
            attrList4 = (GList)GetChild("attrList4");
            autoSell2 = (GButton)GetChild("autoSell2");
            resolveBtn4 = (GButton)GetChild("resolveBtn4");
            repalceBtn4 = (GButton)GetChild("repalceBtn4");
            recycleBtn4 = (GButton)GetChild("recycleBtn4");
            replaceBtn5 = (GButton)GetChild("replaceBtn5");
        }
    }
}