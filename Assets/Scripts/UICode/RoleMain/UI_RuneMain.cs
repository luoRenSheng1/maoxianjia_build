/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RuneMain : GComponent
    {
        public GLoader rlLoader;
        public GImage runeItemBg;
        public GList runeAllList;
        public GButton recycleBtn;
        public UI_RuneItem runeItem0;
        public UI_RuneItem runeItem1;
        public UI_RuneItem runeItem2;
        public UI_RuneItem runeItem3;
        public UI_RuneItem runeItem4;
        public UI_RuneItem runeItem5;
        public GGroup upGroup;
        public GList runDetailList;
        public const string URL = "ui://m37flevdeau0dxy6r";

        public static UI_RuneMain CreateInstance()
        {
            return (UI_RuneMain)UIPackage.CreateObject("RoleMain", "RuneMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            rlLoader = (GLoader)GetChild("rlLoader");
            runeItemBg = (GImage)GetChild("runeItemBg");
            runeAllList = (GList)GetChild("runeAllList");
            recycleBtn = (GButton)GetChild("recycleBtn");
            runeItem0 = (UI_RuneItem)GetChild("runeItem0");
            runeItem1 = (UI_RuneItem)GetChild("runeItem1");
            runeItem2 = (UI_RuneItem)GetChild("runeItem2");
            runeItem3 = (UI_RuneItem)GetChild("runeItem3");
            runeItem4 = (UI_RuneItem)GetChild("runeItem4");
            runeItem5 = (UI_RuneItem)GetChild("runeItem5");
            upGroup = (GGroup)GetChild("upGroup");
            runDetailList = (GList)GetChild("runDetailList");
        }
    }
}