/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_help : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GList helpList;
        public GButton closeBtn;
        public const string URL = "ui://0anhreylplio1b";

        public static UI_help CreateInstance()
        {
            return (UI_help)UIPackage.CreateObject("Common", "help");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            helpList = (GList)GetChild("helpList");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}