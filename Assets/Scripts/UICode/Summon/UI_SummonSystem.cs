/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_SummonSystem : GComponent
    {
        public Controller tabCtrl;
        public UI_SummonInfo summonInfo;
        public UI_SpecialInfo specialInfo;
        public UI_DimaondInfo diamondInfo;
        public UI_GiftInfo giftInfo;
        public GList tabList;
        public UI_Currency btnGold;
        public UI_Currency btnDia;
        public const string URL = "ui://i7ojazuusurf0";

        public static UI_SummonSystem CreateInstance()
        {
            return (UI_SummonSystem)UIPackage.CreateObject("Summon", "SummonSystem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            tabCtrl = GetController("tabCtrl");
            summonInfo = (UI_SummonInfo)GetChild("summonInfo");
            specialInfo = (UI_SpecialInfo)GetChild("specialInfo");
            diamondInfo = (UI_DimaondInfo)GetChild("diamondInfo");
            giftInfo = (UI_GiftInfo)GetChild("giftInfo");
            tabList = (GList)GetChild("tabList");
            btnGold = (UI_Currency)GetChild("btnGold");
            btnDia = (UI_Currency)GetChild("btnDia");
        }
    }
}