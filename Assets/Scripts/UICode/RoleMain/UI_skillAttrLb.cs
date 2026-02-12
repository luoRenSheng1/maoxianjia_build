/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_skillAttrLb : GLabel
    {
        public GTextField pContent;
        public const string URL = "ui://m37flevda2kwdxyan";

        public static UI_skillAttrLb CreateInstance()
        {
            return (UI_skillAttrLb)UIPackage.CreateObject("RoleMain", "skillAttrLb");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pContent = (GTextField)GetChild("pContent");
        }
    }
}