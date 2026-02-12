/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleHelp : GComponent
    {
        public GComponent frame;
        public GList helpList;
        public GButton closeBtn;
        public const string URL = "ui://m37flevdozj91v";

        public static UI_RoleHelp CreateInstance()
        {
            return (UI_RoleHelp)UIPackage.CreateObject("RoleMain", "RoleHelp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            helpList = (GList)GetChild("helpList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}