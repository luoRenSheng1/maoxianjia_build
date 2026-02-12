/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RuneDetail : GComponent
    {
        public Controller ctrl;
        public Controller isTips;
        public GButton runeItem;
        public GTextField runeName;
        public GTextField skillDesc;
        public GButton downBtn;
        public GButton replaceBtn;
        public GButton uploadBtn;
        public UI_RecycleBtn RecycleBtn;
        public GButton closeBtn;
        public const string URL = "ui://m37flevdph24dxy6y";

        public static UI_RuneDetail CreateInstance()
        {
            return (UI_RuneDetail)UIPackage.CreateObject("RoleMain", "RuneDetail");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            isTips = GetController("isTips");
            runeItem = (GButton)GetChild("runeItem");
            runeName = (GTextField)GetChild("runeName");
            skillDesc = (GTextField)GetChild("skillDesc");
            downBtn = (GButton)GetChild("downBtn");
            replaceBtn = (GButton)GetChild("replaceBtn");
            uploadBtn = (GButton)GetChild("uploadBtn");
            RecycleBtn = (UI_RecycleBtn)GetChild("RecycleBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}