/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_DungeonFailed : GComponent
    {
        public GComponent frame;
        public GLoader itemIcon;
        public GTextField itemCntLb;
        public GButton againBtn;
        public const string URL = "ui://57yc4rb0tw1c39";

        public static UI_DungeonFailed CreateInstance()
        {
            return (UI_DungeonFailed)UIPackage.CreateObject("DungeonMap", "DungeonFailed");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            itemIcon = (GLoader)GetChild("itemIcon");
            itemCntLb = (GTextField)GetChild("itemCntLb");
            againBtn = (GButton)GetChild("againBtn");
        }
    }
}