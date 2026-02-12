/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinBuffTips : GComponent
    {
        public GComponent frame;
        public GTextField name;
        public UI_RuinBtn ruinBtn;
        public GList attrList;
        public GTextField time;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3kep2jcids";

        public static UI_RuinBuffTips CreateInstance()
        {
            return (UI_RuinBuffTips)UIPackage.CreateObject("BigMap", "RuinBuffTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            name = (GTextField)GetChild("name");
            ruinBtn = (UI_RuinBtn)GetChild("ruinBtn");
            attrList = (GList)GetChild("attrList");
            time = (GTextField)GetChild("time");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}