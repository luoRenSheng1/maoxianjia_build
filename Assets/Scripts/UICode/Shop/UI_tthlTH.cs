/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Shop
{
    public partial class UI_tthlTH : GComponent
    {
        public GList ttList;
        public GLoader bgUrl;
        public UI_fengkuangItemCom freeItem;
        public GButton tipsBtn;
        public const string URL = "ui://nmzfxo89ftc9i";

        public static UI_tthlTH CreateInstance()
        {
            return (UI_tthlTH)UIPackage.CreateObject("Shop", "tthlTH");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ttList = (GList)GetChild("ttList");
            bgUrl = (GLoader)GetChild("bgUrl");
            freeItem = (UI_fengkuangItemCom)GetChild("freeItem");
            tipsBtn = (GButton)GetChild("tipsBtn");
        }
    }
}