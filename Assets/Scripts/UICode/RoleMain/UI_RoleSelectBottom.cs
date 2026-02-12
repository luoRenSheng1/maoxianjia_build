/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleSelectBottom : GComponent
    {
        public Controller optCtrl;
        public Controller maxCtrl;
        public Controller uploadCtrl;
        public Controller tupoLock;
        public Controller levelLock;
        public GButton closeBtn;
        public UI_RoleOptAniBtn levelUpBtn;
        public GButton upLoadBtn;
        public GButton getHeroBtn;
        public GButton composeHeroBtn;
        public GButton breakBtn;
        public GGroup bottom;
        public GLoader lvItemIcon;
        public GTextField avGetLb;
        public GButton adBtn;
        public GGroup adWatchGroup;
        public const string URL = "ui://m37flevdrm2cdxy6c";

        public static UI_RoleSelectBottom CreateInstance()
        {
            return (UI_RoleSelectBottom)UIPackage.CreateObject("RoleMain", "RoleSelectBottom");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            optCtrl = GetController("optCtrl");
            maxCtrl = GetController("maxCtrl");
            uploadCtrl = GetController("uploadCtrl");
            tupoLock = GetController("tupoLock");
            levelLock = GetController("levelLock");
            closeBtn = (GButton)GetChild("closeBtn");
            levelUpBtn = (UI_RoleOptAniBtn)GetChild("levelUpBtn");
            upLoadBtn = (GButton)GetChild("upLoadBtn");
            getHeroBtn = (GButton)GetChild("getHeroBtn");
            composeHeroBtn = (GButton)GetChild("composeHeroBtn");
            breakBtn = (GButton)GetChild("breakBtn");
            bottom = (GGroup)GetChild("bottom");
            lvItemIcon = (GLoader)GetChild("lvItemIcon");
            avGetLb = (GTextField)GetChild("avGetLb");
            adBtn = (GButton)GetChild("adBtn");
            adWatchGroup = (GGroup)GetChild("adWatchGroup");
        }
    }
}