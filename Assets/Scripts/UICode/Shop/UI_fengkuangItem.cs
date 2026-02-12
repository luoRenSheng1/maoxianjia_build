/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_fengkuangItem : GComponent
    {
        public Controller reachCtrl;
        public GImage bg;
        public UI_fengkuangItemCom fkItemCom;
        public GTextField num;
        public GTextField title;
        public const string URL = "ui://nmzfxo89plioz";

        public static UI_fengkuangItem CreateInstance()
        {
            return (UI_fengkuangItem)UIPackage.CreateObject("Shop", "fengkuangItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            reachCtrl = GetController("reachCtrl");
            bg = (GImage)GetChild("bg");
            fkItemCom = (UI_fengkuangItemCom)GetChild("fkItemCom");
            num = (GTextField)GetChild("num");
            title = (GTextField)GetChild("title");
        }
    }
}