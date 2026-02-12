/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSystemBook : GComponent
    {
        public Controller makeStatus;
        public Controller openBookStatus;
        public Controller status;
        public Controller hasUpload;
        public Controller type;
        public Controller isShow;
        public Controller openType;
        public GButton tipsBtn;
        public GList skillBookList;
        public UI_PetSkillBtn petSkillBtn;
        public GTextField name;
        public GTextField desc;
        public GButton recycleBtn;
        public GComponent recycleBtnRedPoint;
        public GList list;
        public GButton makeBtn;
        public UI_makeBtnAni makeBtnAni;
        public GComponent makeBtnRedPoint;
        public GButton openBookBtn;
        public GComponent openBookBtnRedPoint;
        public GList petList;
        public GButton currency1;
        public GButton currency2;
        public const string URL = "ui://lxs2h4ifs444f";

        public static UI_PetSystemBook CreateInstance()
        {
            return (UI_PetSystemBook)UIPackage.CreateObject("Pet", "PetSystemBook");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            makeStatus = GetController("makeStatus");
            openBookStatus = GetController("openBookStatus");
            status = GetController("status");
            hasUpload = GetController("hasUpload");
            type = GetController("type");
            isShow = GetController("isShow");
            openType = GetController("openType");
            tipsBtn = (GButton)GetChild("tipsBtn");
            skillBookList = (GList)GetChild("skillBookList");
            petSkillBtn = (UI_PetSkillBtn)GetChild("petSkillBtn");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            recycleBtn = (GButton)GetChild("recycleBtn");
            recycleBtnRedPoint = (GComponent)GetChild("recycleBtnRedPoint");
            list = (GList)GetChild("list");
            makeBtn = (GButton)GetChild("makeBtn");
            makeBtnAni = (UI_makeBtnAni)GetChild("makeBtnAni");
            makeBtnRedPoint = (GComponent)GetChild("makeBtnRedPoint");
            openBookBtn = (GButton)GetChild("openBookBtn");
            openBookBtnRedPoint = (GComponent)GetChild("openBookBtnRedPoint");
            petList = (GList)GetChild("petList");
            currency1 = (GButton)GetChild("currency1");
            currency2 = (GButton)GetChild("currency2");
        }
    }
}