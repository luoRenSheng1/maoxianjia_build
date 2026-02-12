/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RuneRecyle : GComponent
    {
        public GButton reduceBtn;
        public GTextInput cntLb;
        public GButton addBtn;
        public GButton recyleBtn;
        public GButton closeBtn;
        public const string URL = "ui://m37flevdph24dxy70";

        public static UI_RuneRecyle CreateInstance()
        {
            return (UI_RuneRecyle)UIPackage.CreateObject("RoleMain", "RuneRecyle");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reduceBtn = (GButton)GetChild("reduceBtn");
            cntLb = (GTextInput)GetChild("cntLb");
            addBtn = (GButton)GetChild("addBtn");
            recyleBtn = (GButton)GetChild("recyleBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}