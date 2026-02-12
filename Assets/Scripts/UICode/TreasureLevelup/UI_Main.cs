/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace TreasureLevelup
{
    public partial class UI_Main : GComponent
    {
        public GComponent frame;
        public UI_Panel panel;
        public const string URL = "ui://v1wfpt3li9slh";

        public static UI_Main CreateInstance()
        {
            return (UI_Main)UIPackage.CreateObject("TreasureLevelup", "Main");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            panel = (UI_Panel)GetChild("panel");
        }
    }
}