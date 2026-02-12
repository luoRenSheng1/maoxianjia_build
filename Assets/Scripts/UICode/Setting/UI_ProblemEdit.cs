/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Setting
{
    public partial class UI_ProblemEdit : GComponent
    {
        public GComponent frame;
        public GButton closeBtn;
        public GButton commitBtn;
        public GTextInput ipt;
        public const string URL = "ui://zs0w02qtqt85l";

        public static UI_ProblemEdit CreateInstance()
        {
            return (UI_ProblemEdit)UIPackage.CreateObject("Setting", "ProblemEdit");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            closeBtn = (GButton)GetChild("closeBtn");
            commitBtn = (GButton)GetChild("commitBtn");
            ipt = (GTextInput)GetChild("ipt");
        }
    }
}