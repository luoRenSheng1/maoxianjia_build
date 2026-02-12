/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_dailyTH : GComponent
    {
        public GList dailyList;
        public GLoader bgUrl;
        public UI_fengkuangItemCom freeItem;
        public UI_dailyTHItemAll rw60Item;
        public GButton tipsBtn;
        public const string URL = "ui://nmzfxo89ftc9a";

        public static UI_dailyTH CreateInstance()
        {
            return (UI_dailyTH)UIPackage.CreateObject("Shop", "dailyTH");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            dailyList = (GList)GetChild("dailyList");
            bgUrl = (GLoader)GetChild("bgUrl");
            freeItem = (UI_fengkuangItemCom)GetChild("freeItem");
            rw60Item = (UI_dailyTHItemAll)GetChild("rw60Item");
            tipsBtn = (GButton)GetChild("tipsBtn");
        }
    }
}