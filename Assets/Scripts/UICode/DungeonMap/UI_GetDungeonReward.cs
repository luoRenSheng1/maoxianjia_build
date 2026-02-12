/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_GetDungeonReward : GComponent
    {
        public Controller countCtrl;
        public Controller adCtrl;
        public GComponent frame;
        public GList itemList;
        public GButton sweepBtn;
        public GButton nextStageBtn;
        public GLoader itemIcon;
        public GTextField itemCntLb;
        public GButton adBtn;
        public const string URL = "ui://57yc4rb0tw1c36";

        public static UI_GetDungeonReward CreateInstance()
        {
            return (UI_GetDungeonReward)UIPackage.CreateObject("DungeonMap", "GetDungeonReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            countCtrl = GetController("countCtrl");
            adCtrl = GetController("adCtrl");
            frame = (GComponent)GetChild("frame");
            itemList = (GList)GetChild("itemList");
            sweepBtn = (GButton)GetChild("sweepBtn");
            nextStageBtn = (GButton)GetChild("nextStageBtn");
            itemIcon = (GLoader)GetChild("itemIcon");
            itemCntLb = (GTextField)GetChild("itemCntLb");
            adBtn = (GButton)GetChild("adBtn");
        }
    }
}