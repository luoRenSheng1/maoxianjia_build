/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_NewInheritObtain : GComponent
    {
        public Controller status;
        public Controller Ctrl;
        public GComponent equip1;
        public GTextField fightLb1;
        public GTextField name;
        public UI_ComAttributeItem mainAttr1;
        public GList attrList1;
        public UI_ComAttributeItem mainAttr2;
        public GList attrList2;
        public GComponent equip2;
        public GTextField fightLb2;
        public GTextField name2;
        public GButton recycleBtn;
        public GButton replaceBtn;
        public const string URL = "ui://ddc23erlkqmidxy9b";

        public static UI_NewInheritObtain CreateInstance()
        {
            return (UI_NewInheritObtain)UIPackage.CreateObject("Equip", "NewInheritObtain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            Ctrl = GetController("Ctrl");
            equip1 = (GComponent)GetChild("equip1");
            fightLb1 = (GTextField)GetChild("fightLb1");
            name = (GTextField)GetChild("name");
            mainAttr1 = (UI_ComAttributeItem)GetChild("mainAttr1");
            attrList1 = (GList)GetChild("attrList1");
            mainAttr2 = (UI_ComAttributeItem)GetChild("mainAttr2");
            attrList2 = (GList)GetChild("attrList2");
            equip2 = (GComponent)GetChild("equip2");
            fightLb2 = (GTextField)GetChild("fightLb2");
            name2 = (GTextField)GetChild("name2");
            recycleBtn = (GButton)GetChild("recycleBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
        }
    }
}