/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_RoleItem : GButton
    {
        public Controller state;
        public Controller hasLv;
        public Controller compondCtrl;
        public Controller quality;
        public Controller upLoadCtrl;
        public GLoader bg;
        public GTextField lvLb;
        public GTextField idLb;
        public UI_roleQualityItem qIcon;
        public UI_RedDotComponent redDot;
        public GTextField roleName;
        public GLoader splitItemIcon;
        public GTextField needItem;
        public GImage hcBtn;
        public Transition addEff;
        public Transition dragEff;
        public const string URL = "ui://5moj1x39ozj9dxy5d";

        public static UI_RoleItem CreateInstance()
        {
            return (UI_RoleItem)UIPackage.CreateObject("CommonEx", "RoleItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            state = GetController("state");
            hasLv = GetController("hasLv");
            compondCtrl = GetController("compondCtrl");
            quality = GetController("quality");
            upLoadCtrl = GetController("upLoadCtrl");
            bg = (GLoader)GetChild("bg");
            lvLb = (GTextField)GetChild("lvLb");
            idLb = (GTextField)GetChild("idLb");
            qIcon = (UI_roleQualityItem)GetChild("qIcon");
            redDot = (UI_RedDotComponent)GetChild("redDot");
            roleName = (GTextField)GetChild("roleName");
            splitItemIcon = (GLoader)GetChild("splitItemIcon");
            needItem = (GTextField)GetChild("needItem");
            hcBtn = (GImage)GetChild("hcBtn");
            addEff = GetTransition("addEff");
            dragEff = GetTransition("dragEff");
        }
    }
}