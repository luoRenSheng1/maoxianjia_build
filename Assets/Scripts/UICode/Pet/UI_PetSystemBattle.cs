/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSystemBattle : GComponent
    {
        public Controller status;
        public Controller isShow;
        public UI_PetUpLoadItem petUpItem0;
        public UI_PetUpLoadItem petUpItem1;
        public UI_PetUpLoadItem petUpItem2;
        public GGroup upGroup;
        public GList petAllList;
        public GButton recycleBtn;
        public GComponent recycleBtnRed;
        public GTextField limit;
        public GButton currency1;
        public GButton currency2;
        public const string URL = "ui://lxs2h4ifs444d";

        public static UI_PetSystemBattle CreateInstance()
        {
            return (UI_PetSystemBattle)UIPackage.CreateObject("Pet", "PetSystemBattle");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            isShow = GetController("isShow");
            petUpItem0 = (UI_PetUpLoadItem)GetChild("petUpItem0");
            petUpItem1 = (UI_PetUpLoadItem)GetChild("petUpItem1");
            petUpItem2 = (UI_PetUpLoadItem)GetChild("petUpItem2");
            upGroup = (GGroup)GetChild("upGroup");
            petAllList = (GList)GetChild("petAllList");
            recycleBtn = (GButton)GetChild("recycleBtn");
            recycleBtnRed = (GComponent)GetChild("recycleBtnRed");
            limit = (GTextField)GetChild("limit");
            currency1 = (GButton)GetChild("currency1");
            currency2 = (GButton)GetChild("currency2");
        }
    }
}