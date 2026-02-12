/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_NewEquipRecommend : GComponent
    {
        public Controller typeCtrl;
        public Controller status;
        public Controller hasEntry;
        public GComponent comEquipItem2;
        public GTextField txtEquipItem;
        public GTextField equipLv;
        public GImage newIcon;
        public GTextField fightLb;
        public GList mainAttrList2;
        public GList attrsList2;
        public GButton btnEquip2;
        public GButton recycleBtn2;
        public GButton upLoadBtn2;
        public GButton downBtn2;
        public const string URL = "ui://ddc23erlskp6q";

        public static UI_NewEquipRecommend CreateInstance()
        {
            return (UI_NewEquipRecommend)UIPackage.CreateObject("Equip", "NewEquipRecommend");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            status = GetController("status");
            hasEntry = GetController("hasEntry");
            comEquipItem2 = (GComponent)GetChild("comEquipItem2");
            txtEquipItem = (GTextField)GetChild("txtEquipItem");
            equipLv = (GTextField)GetChild("equipLv");
            newIcon = (GImage)GetChild("newIcon");
            fightLb = (GTextField)GetChild("fightLb");
            mainAttrList2 = (GList)GetChild("mainAttrList2");
            attrsList2 = (GList)GetChild("attrsList2");
            btnEquip2 = (GButton)GetChild("btnEquip2");
            recycleBtn2 = (GButton)GetChild("recycleBtn2");
            upLoadBtn2 = (GButton)GetChild("upLoadBtn2");
            downBtn2 = (GButton)GetChild("downBtn2");
        }
    }
}