/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_RuinAttrTips : GComponent
    {
        public GComponent frame;
        public GTextField name;
        public UI_RuinBtn ruinBtn;
        public GTextField desc;
        public GTextField time;
        public GButton closeBtn;
        public const string URL = "ui://pdufy3kew224idl";

        public static UI_RuinAttrTips CreateInstance()
        {
            return (UI_RuinAttrTips)UIPackage.CreateObject("BigMap", "RuinAttrTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            name = (GTextField)GetChild("name");
            ruinBtn = (UI_RuinBtn)GetChild("ruinBtn");
            desc = (GTextField)GetChild("desc");
            time = (GTextField)GetChild("time");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}