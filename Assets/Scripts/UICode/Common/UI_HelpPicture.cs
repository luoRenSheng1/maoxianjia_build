/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_HelpPicture : GComponent
    {
        public UI_FrameMask_Normal frame1;
        public GList helpList;
        public GButton closeBtn2;
        public const string URL = "ui://0anhreylil6l18";

        public static UI_HelpPicture CreateInstance()
        {
            return (UI_HelpPicture)UIPackage.CreateObject("Common", "HelpPicture");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame1 = (UI_FrameMask_Normal)GetChild("frame1");
            helpList = (GList)GetChild("helpList");
            closeBtn2 = (GButton)GetChild("closeBtn2");
        }
    }
}