/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace DungeonMap
{
    public partial class UI_Panel : GComponent
    {
        public Controller copyType;
        public GComponent battleRoot;
        public GButton userInfo;
        public GTextField txtBattleContent;
        public GImage left;
        public GImage right;
        public GButton runBtn;
        public GTextField txtDesc;
        public Transition t0;
        public const string URL = "ui://57yc4rb0gwxt1";

        public static UI_Panel CreateInstance()
        {
            return (UI_Panel)UIPackage.CreateObject("DungeonMap", "Panel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            copyType = GetController("copyType");
            battleRoot = (GComponent)GetChild("battleRoot");
            userInfo = (GButton)GetChild("userInfo");
            txtBattleContent = (GTextField)GetChild("txtBattleContent");
            left = (GImage)GetChild("left");
            right = (GImage)GetChild("right");
            runBtn = (GButton)GetChild("runBtn");
            txtDesc = (GTextField)GetChild("txtDesc");
            t0 = GetTransition("t0");
        }
    }
}