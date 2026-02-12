/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_LotteryItem : GComponent
    {
        public Controller ctrl;
        public Controller rewardType;
        public GLoader itemIcon;
        public GTextField idLb;
        public const string URL = "ui://6izp804wtcgl2";

        public static UI_LotteryItem CreateInstance()
        {
            return (UI_LotteryItem)UIPackage.CreateObject("Lottery", "LotteryItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
            rewardType = GetController("rewardType");
            itemIcon = (GLoader)GetChild("itemIcon");
            idLb = (GTextField)GetChild("idLb");
        }
    }
}