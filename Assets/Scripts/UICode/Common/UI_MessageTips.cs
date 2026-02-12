/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_MessageTips : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GButton closeBtn;
        public GRichTextField txtContent;
        public GButton RefusedBtn;
        public GButton agreeBtn;
        public const string URL = "ui://0anhreyl9kp7dxy8s";

        public static UI_MessageTips CreateInstance()
        {
            return (UI_MessageTips)UIPackage.CreateObject("Common", "MessageTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            txtContent = (GRichTextField)GetChild("txtContent");
            RefusedBtn = (GButton)GetChild("RefusedBtn");
            agreeBtn = (GButton)GetChild("agreeBtn");
        }
    }
}