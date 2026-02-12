/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_TalentItem : GButton
    {
        public Controller typeCtrl;
        public UI_TalentSmallItem talentSmallItem;
        public UI_TalentMiddleItem talentMiddleItem;
        public UI_TalentBigItem talentBigItem;
        public const string URL = "ui://m37flevdp9n0dxy7g";

        public static UI_TalentItem CreateInstance()
        {
            return (UI_TalentItem)UIPackage.CreateObject("RoleMain", "TalentItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            talentSmallItem = (UI_TalentSmallItem)GetChild("talentSmallItem");
            talentMiddleItem = (UI_TalentMiddleItem)GetChild("talentMiddleItem");
            talentBigItem = (UI_TalentBigItem)GetChild("talentBigItem");
        }
    }
}