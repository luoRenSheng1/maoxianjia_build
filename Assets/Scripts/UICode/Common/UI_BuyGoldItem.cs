/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BuyGoldItem : GComponent
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
        public GTextField costLb;
        public GTextField getLb;
        public const string URL = "ui://0anhreyluq40dxy40";

        public static UI_BuyGoldItem CreateInstance()
        {
            return (UI_BuyGoldItem)UIPackage.CreateObject("Common", "BuyGoldItem");
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
            costLb = (GTextField)GetChild("costLb");
            getLb = (GTextField)GetChild("getLb");
        }
    }
}