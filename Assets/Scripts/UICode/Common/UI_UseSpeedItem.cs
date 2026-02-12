/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_UseSpeedItem : GComponent
    {
        public UI_FrameMask_Tips frame;
        public GButton item;
        public GLabel itemName;
        public GRichTextField desc;
        public GTextField purposeLb;
        public GButton useBtn;
        public GButton reduceBtn;
        public GButton addBtn;
        public GSlider cntSlider;
        public GTextField cntLb;
        public GTextField timeLb;
        public const string URL = "ui://0anhreylsc7hdxy3p";

        public static UI_UseSpeedItem CreateInstance()
        {
            return (UI_UseSpeedItem)UIPackage.CreateObject("Common", "UseSpeedItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Tips)GetChild("frame");
            item = (GButton)GetChild("item");
            itemName = (GLabel)GetChild("itemName");
            desc = (GRichTextField)GetChild("desc");
            purposeLb = (GTextField)GetChild("purposeLb");
            useBtn = (GButton)GetChild("useBtn");
            reduceBtn = (GButton)GetChild("reduceBtn");
            addBtn = (GButton)GetChild("addBtn");
            cntSlider = (GSlider)GetChild("cntSlider");
            cntLb = (GTextField)GetChild("cntLb");
            timeLb = (GTextField)GetChild("timeLb");
        }
    }
}