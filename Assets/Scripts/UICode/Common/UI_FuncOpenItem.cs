/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FuncOpenItem : GComponent
    {
        public GLoader funcIcon;
        public GTextField funcName;
        public const string URL = "ui://0anhreylwpkodxy7e";

        public static UI_FuncOpenItem CreateInstance()
        {
            return (UI_FuncOpenItem)UIPackage.CreateObject("Common", "FuncOpenItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            funcIcon = (GLoader)GetChild("funcIcon");
            funcName = (GTextField)GetChild("funcName");
        }
    }
}