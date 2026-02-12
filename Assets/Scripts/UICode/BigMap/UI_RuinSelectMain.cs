/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinSelectMain : GComponent
    {
        public GList ruinList;
        public GTextField time;
        public GButton submitBtn;
        public const string URL = "ui://pdufy3kew224id6";

        public static UI_RuinSelectMain CreateInstance()
        {
            return (UI_RuinSelectMain)UIPackage.CreateObject("BigMap", "RuinSelectMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ruinList = (GList)GetChild("ruinList");
            time = (GTextField)GetChild("time");
            submitBtn = (GButton)GetChild("submitBtn");
        }
    }
}