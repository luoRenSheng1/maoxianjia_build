/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonPetTips : GComponent
    {
        public GTextField pContent;
        public UI_PetItem petItem;
        public GButton detailBtn;
        public const string URL = "ui://5moj1x39gybrdxy3m";

        public static UI_CommonPetTips CreateInstance()
        {
            return (UI_CommonPetTips)UIPackage.CreateObject("CommonEx", "CommonPetTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pContent = (GTextField)GetChild("pContent");
            petItem = (UI_PetItem)GetChild("petItem");
            detailBtn = (GButton)GetChild("detailBtn");
        }
    }
}