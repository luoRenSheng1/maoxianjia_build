/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_TreasureItem : GComponent
    {
        public GLoader di;
        public GLoader icon;
        public GProgressBar bar;
        public GLoader3D cutSpine;
        public Transition wajue;
        public Transition xiaoshi;
        public Transition daiji;
        public const string URL = "ui://pdufy3ketdju5u";

        public static UI_TreasureItem CreateInstance()
        {
            return (UI_TreasureItem)UIPackage.CreateObject("BigMap", "TreasureItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            di = (GLoader)GetChild("di");
            icon = (GLoader)GetChild("icon");
            bar = (GProgressBar)GetChild("bar");
            cutSpine = (GLoader3D)GetChild("cutSpine");
            wajue = GetTransition("wajue");
            xiaoshi = GetTransition("xiaoshi");
            daiji = GetTransition("daiji");
        }
    }
}