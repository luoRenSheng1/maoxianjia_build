/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_Notice : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GList noticeList;
        public GButton closeBtn;
        public Transition t0;
        public const string URL = "ui://0anhreyl8824dxy3y";

        public static UI_Notice CreateInstance()
        {
            return (UI_Notice)UIPackage.CreateObject("Common", "Notice");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            noticeList = (GList)GetChild("noticeList");
            closeBtn = (GButton)GetChild("closeBtn");
            t0 = GetTransition("t0");
        }
    }
}