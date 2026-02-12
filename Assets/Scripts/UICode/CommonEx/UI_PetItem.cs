/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_PetItem : GButton
    {
        public Controller quality;
        public Controller occupationType;
        public Controller hasLv;
        public GTextField lvLb;
        public GTextField idLb;
        public GList starList;
        public UI_RedDotComponent redDot;
        public GButton dragBtn;
        public GTextField petName;
        public Transition addEff;
        public Transition dragEff;
        public const string URL = "ui://5moj1x39jp5j1";

        public static UI_PetItem CreateInstance()
        {
            return (UI_PetItem)UIPackage.CreateObject("CommonEx", "PetItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            quality = GetController("quality");
            occupationType = GetController("occupationType");
            hasLv = GetController("hasLv");
            lvLb = (GTextField)GetChild("lvLb");
            idLb = (GTextField)GetChild("idLb");
            starList = (GList)GetChild("starList");
            redDot = (UI_RedDotComponent)GetChild("redDot");
            dragBtn = (GButton)GetChild("dragBtn");
            petName = (GTextField)GetChild("petName");
            addEff = GetTransition("addEff");
            dragEff = GetTransition("dragEff");
        }
    }
}