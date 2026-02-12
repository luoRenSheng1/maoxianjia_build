/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_MessageBoxWindow : GComponent
    {
        public UI_FrameMask_Normal frame;
        public UI_MessageBox messageBox;
        public const string URL = "ui://0anhreylueyx26";

        public static UI_MessageBoxWindow CreateInstance()
        {
            return (UI_MessageBoxWindow)UIPackage.CreateObject("Common", "MessageBoxWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            messageBox = (UI_MessageBox)GetChild("messageBox");
        }
    }
}