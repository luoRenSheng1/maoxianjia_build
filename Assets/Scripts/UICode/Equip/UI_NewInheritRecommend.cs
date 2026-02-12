/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_NewInheritRecommend : GComponent
    {
        public Controller type;
        public GTextField fightLb;
        public GComponent equipItem;
        public GTextField name;
        public UI_ComAttributeItem mainAttr;
        public GList attrList;
        public GButton recycleBtn;
        public GButton downBtn;
        public GButton upLoadBtn;
        public const string URL = "ui://ddc23erlkqmidxy9c";

        public static UI_NewInheritRecommend CreateInstance()
        {
            return (UI_NewInheritRecommend)UIPackage.CreateObject("Equip", "NewInheritRecommend");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            type = GetController("type");
            fightLb = (GTextField)GetChild("fightLb");
            equipItem = (GComponent)GetChild("equipItem");
            name = (GTextField)GetChild("name");
            mainAttr = (UI_ComAttributeItem)GetChild("mainAttr");
            attrList = (GList)GetChild("attrList");
            recycleBtn = (GButton)GetChild("recycleBtn");
            downBtn = (GButton)GetChild("downBtn");
            upLoadBtn = (GButton)GetChild("upLoadBtn");
        }
    }
}