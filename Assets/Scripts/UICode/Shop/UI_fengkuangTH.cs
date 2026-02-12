/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_fengkuangTH : GComponent
    {
        public Controller titleCtrl;
        public GLoader bgUrl;
        public GList titleList;
        public GList itemList;
        public GButton tipsBtn;
        public const string URL = "ui://nmzfxo89plio15";

        public static UI_fengkuangTH CreateInstance()
        {
            return (UI_fengkuangTH)UIPackage.CreateObject("Shop", "fengkuangTH");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            titleCtrl = GetController("titleCtrl");
            bgUrl = (GLoader)GetChild("bgUrl");
            titleList = (GList)GetChild("titleList");
            itemList = (GList)GetChild("itemList");
            tipsBtn = (GButton)GetChild("tipsBtn");
        }
    }
}