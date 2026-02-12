/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_DungeonItem : GLabel
    {
        public Controller countCtr;
        public Controller lockCtrl;
        public Controller adCtrl;
        public GList stageItemList;
        public GLoader itemIcon;
        public GTextField itemCntLb;
        public GButton tipBtn;
        public GButton gotoBtn;
        public GButton adBtn;
        public GTextField lockDesc;
        public GComponent gotoBtnRed;
        public GComponent adBtnRed;
        public const string URL = "ui://57yc4rb0b0e435";

        public static UI_DungeonItem CreateInstance()
        {
            return (UI_DungeonItem)UIPackage.CreateObject("DungeonMap", "DungeonItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            countCtr = GetController("countCtr");
            lockCtrl = GetController("lockCtrl");
            adCtrl = GetController("adCtrl");
            stageItemList = (GList)GetChild("stageItemList");
            itemIcon = (GLoader)GetChild("itemIcon");
            itemCntLb = (GTextField)GetChild("itemCntLb");
            tipBtn = (GButton)GetChild("tipBtn");
            gotoBtn = (GButton)GetChild("gotoBtn");
            adBtn = (GButton)GetChild("adBtn");
            lockDesc = (GTextField)GetChild("lockDesc");
            gotoBtnRed = (GComponent)GetChild("gotoBtnRed");
            adBtnRed = (GComponent)GetChild("adBtnRed");
        }
    }
}