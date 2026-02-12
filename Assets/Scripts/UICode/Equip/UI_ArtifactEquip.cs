/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ArtifactEquip : GComponent
    {
        public Controller status;
        public Controller isMax;
        public Controller tabCtrl;
        public UI_ArtifactSelectBtn tab0;
        public UI_ArtifactSelectBtn tab1;
        public UI_ArtifactSelectBtn tab2;
        public UI_ArtifactSelectBtn tab3;
        public UI_ArtifactItem1 artifact;
        public GTextField name;
        public GTextField desc;
        public GTextField curLv;
        public GTextField nextLv;
        public GList attrList;
        public GTextField maxLv;
        public GList maxList;
        public GComponent tabCom1;
        public GComponent tabCom2;
        public GButton upLvBtn;
        public const string URL = "ui://ddc23erlef8udxyax";

        public static UI_ArtifactEquip CreateInstance()
        {
            return (UI_ArtifactEquip)UIPackage.CreateObject("Equip", "ArtifactEquip");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            isMax = GetController("isMax");
            tabCtrl = GetController("tabCtrl");
            tab0 = (UI_ArtifactSelectBtn)GetChild("tab0");
            tab1 = (UI_ArtifactSelectBtn)GetChild("tab1");
            tab2 = (UI_ArtifactSelectBtn)GetChild("tab2");
            tab3 = (UI_ArtifactSelectBtn)GetChild("tab3");
            artifact = (UI_ArtifactItem1)GetChild("artifact");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            curLv = (GTextField)GetChild("curLv");
            nextLv = (GTextField)GetChild("nextLv");
            attrList = (GList)GetChild("attrList");
            maxLv = (GTextField)GetChild("maxLv");
            maxList = (GList)GetChild("maxList");
            tabCom1 = (GComponent)GetChild("tabCom1");
            tabCom2 = (GComponent)GetChild("tabCom2");
            upLvBtn = (GButton)GetChild("upLvBtn");
        }
    }
}