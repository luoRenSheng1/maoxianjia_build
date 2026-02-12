/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Village
{
    public partial class UI_FrameMask_Build : GComponent
    {
        public Controller alphaType;
        public GGraph frame;
        public GButton closeBtn2;
        public const string URL = "ui://8glegefcnif6h";

        public static UI_FrameMask_Build CreateInstance()
        {
            return (UI_FrameMask_Build)UIPackage.CreateObject("Village", "FrameMask_Build");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            alphaType = GetController("alphaType");
            frame = (GGraph)GetChild("frame");
            closeBtn2 = (GButton)GetChild("closeBtn2");
        }
    }
}