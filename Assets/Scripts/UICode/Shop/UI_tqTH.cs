/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_tqTH : GComponent
    {
        public GList tqList;
        public GLoader bgUrl;
        public UI_fengkuangItemCom freeItem;
        public const string URL = "ui://nmzfxo89ftc9l";

        public static UI_tqTH CreateInstance()
        {
            return (UI_tqTH)UIPackage.CreateObject("Shop", "tqTH");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tqList = (GList)GetChild("tqList");
            bgUrl = (GLoader)GetChild("bgUrl");
            freeItem = (UI_fengkuangItemCom)GetChild("freeItem");
        }
    }
}