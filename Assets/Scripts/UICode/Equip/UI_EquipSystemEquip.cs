/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_EquipSystemEquip : GComponent
    {
        public Controller lockCtrl;
        public GGraph myHero;
        public UI_EquipCurrency goldCur;
        public UI_EquipCurrency hammerCur;
        public GList equipList;
        public UI_MakeEquipBtnAni makeBtnAni;
        public UI_UpLvBtn upLvBtn;
        public GComponent redPoint;
        public const string URL = "ui://ddc23erlef8udxyar";

        public static UI_EquipSystemEquip CreateInstance()
        {
            return (UI_EquipSystemEquip)UIPackage.CreateObject("Equip", "EquipSystemEquip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            myHero = (GGraph)GetChild("myHero");
            goldCur = (UI_EquipCurrency)GetChild("goldCur");
            hammerCur = (UI_EquipCurrency)GetChild("hammerCur");
            equipList = (GList)GetChild("equipList");
            makeBtnAni = (UI_MakeEquipBtnAni)GetChild("makeBtnAni");
            upLvBtn = (UI_UpLvBtn)GetChild("upLvBtn");
            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}