/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetSystemTalent : GComponent
    {
        public Controller status;
        public Controller hasUpload;
        public GButton tipsBtn;
        public GLoader3D spine;
        public GList list;
        public GList talentList;
        public GButton talentBtn;
        public GComponent talentRedPoint;
        public GButton currency1;
        public GButton currency2;
        public const string URL = "ui://lxs2h4ifs444e";

        public static UI_PetSystemTalent CreateInstance()
        {
            return (UI_PetSystemTalent)UIPackage.CreateObject("Pet", "PetSystemTalent");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            hasUpload = GetController("hasUpload");
            tipsBtn = (GButton)GetChild("tipsBtn");
            spine = (GLoader3D)GetChild("spine");
            list = (GList)GetChild("list");
            talentList = (GList)GetChild("talentList");
            talentBtn = (GButton)GetChild("talentBtn");
            talentRedPoint = (GComponent)GetChild("talentRedPoint");
            currency1 = (GButton)GetChild("currency1");
            currency2 = (GButton)GetChild("currency2");
        }
    }
}