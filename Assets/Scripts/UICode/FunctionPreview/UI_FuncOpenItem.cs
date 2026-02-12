/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FunctionPreview
{
    public partial class UI_FuncOpenItem : GComponent
    {
        public Controller canGet;
        public GLoader funcIcon;
        public GTextField funcName;
        public GTextField funcDesc;
        public GButton rwItem;
        public Transition t0;
        public const string URL = "ui://tekh4dt9v51s5";

        public static UI_FuncOpenItem CreateInstance()
        {
            return (UI_FuncOpenItem)UIPackage.CreateObject("FunctionPreview", "FuncOpenItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            canGet = GetController("canGet");
            funcIcon = (GLoader)GetChild("funcIcon");
            funcName = (GTextField)GetChild("funcName");
            funcDesc = (GTextField)GetChild("funcDesc");
            rwItem = (GButton)GetChild("rwItem");
            t0 = GetTransition("t0");
        }
    }
}