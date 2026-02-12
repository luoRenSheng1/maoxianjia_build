/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FrameMask_Tips : GComponent
    {
        public Controller alphaType;
        public GGraph frame;
        public GTextField textTips;
        public const string URL = "ui://0anhreylpmd7pq";

        public static UI_FrameMask_Tips CreateInstance()
        {
            return (UI_FrameMask_Tips)UIPackage.CreateObject("Common", "FrameMask_Tips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            alphaType = GetController("alphaType");
            frame = (GGraph)GetChild("frame");
            textTips = (GTextField)GetChild("textTips");
        }
    }
}