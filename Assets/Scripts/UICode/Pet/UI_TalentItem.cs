/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_TalentItem : GComponent
    {
        public Controller isCheck;
        public Controller isShow;
        public UI_PetTalentItem petTalentItem;
        public GTextField talentName;
        public GTextField talentdesc;
        public GLoader itemIcon;
        public GTextField goldNum;
        public GButton checkBtn;
        public GLoader3D spine;
        public const string URL = "ui://lxs2h4ifhz5cdxy7s";

        public static UI_TalentItem CreateInstance()
        {
            return (UI_TalentItem)UIPackage.CreateObject("Pet", "TalentItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isCheck = GetController("isCheck");
            isShow = GetController("isShow");
            petTalentItem = (UI_PetTalentItem)GetChild("petTalentItem");
            talentName = (GTextField)GetChild("talentName");
            talentdesc = (GTextField)GetChild("talentdesc");
            itemIcon = (GLoader)GetChild("itemIcon");
            goldNum = (GTextField)GetChild("goldNum");
            checkBtn = (GButton)GetChild("checkBtn");
            spine = (GLoader3D)GetChild("spine");
        }
    }
}