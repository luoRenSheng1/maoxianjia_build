/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_panelClip : GComponent
    {
        public UI_ChapterPanel panel;
        public GGraph mask;
        public const string URL = "ui://pdufy3kewp974a";

        public static UI_panelClip CreateInstance()
        {
            return (UI_panelClip)UIPackage.CreateObject("BigMap", "panelClip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            panel = (UI_ChapterPanel)GetChild("panel");
            mask = (GGraph)GetChild("mask");
        }
    }
}