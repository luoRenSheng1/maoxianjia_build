/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FrameMask_1 : GComponent
    {
        public Controller alphaType;
        public GGraph frame;
        public const string URL = "ui://0anhreyls2xhdxy7a";

        public static UI_FrameMask_1 CreateInstance()
        {
            return (UI_FrameMask_1)UIPackage.CreateObject("Common", "FrameMask_1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            alphaType = GetController("alphaType");
            frame = (GGraph)GetChild("frame");
        }
    }
}