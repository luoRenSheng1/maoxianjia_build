/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinMap : GComponent
    {
        public UI_RuinPanel ruinPanel;
        public GGraph mask;
        public const string URL = "ui://pdufy3kew224idn";

        public static UI_RuinMap CreateInstance()
        {
            return (UI_RuinMap)UIPackage.CreateObject("BigMap", "RuinMap");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ruinPanel = (UI_RuinPanel)GetChild("ruinPanel");
            mask = (GGraph)GetChild("mask");
        }
    }
}