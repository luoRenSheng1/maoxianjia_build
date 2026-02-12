/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FrameMask_Normal : GComponent
    {
        public Controller alphaType;
        public GGraph frame;
        public const string URL = "ui://0anhreylugrmtd";

        public static UI_FrameMask_Normal CreateInstance()
        {
            return (UI_FrameMask_Normal)UIPackage.CreateObject("Common", "FrameMask_Normal");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            alphaType = GetController("alphaType");
            frame = (GGraph)GetChild("frame");
        }
    }
}