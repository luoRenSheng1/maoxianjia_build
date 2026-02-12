/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Artifact
{
    public partial class UI_ArtifactMain : GComponent
    {
        public Controller status;
        public Controller isMax;
        public GComponent frame;
        public GButton diaCurBtn;
        public GButton itemCurBtn;
        public GList tabList;
        public GTextField name;
        public GTextField desc;
        public UI_ArtifactItem itemBtn2;
        public GTextField curLv;
        public GTextField nextLv;
        public GList attrList;
        public GTextField maxLv;
        public GList maxList;
        public GButton upLvBtn;
        public GComponent tabCom1;
        public GComponent tabCom2;
        public GButton closeBtn;
        public const string URL = "ui://v9y2d69nkqmidxy95";

        public static UI_ArtifactMain CreateInstance()
        {
            return (UI_ArtifactMain)UIPackage.CreateObject("Artifact", "ArtifactMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            isMax = GetController("isMax");
            frame = (GComponent)GetChild("frame");
            diaCurBtn = (GButton)GetChild("diaCurBtn");
            itemCurBtn = (GButton)GetChild("itemCurBtn");
            tabList = (GList)GetChild("tabList");
            name = (GTextField)GetChild("name");
            desc = (GTextField)GetChild("desc");
            itemBtn2 = (UI_ArtifactItem)GetChild("itemBtn2");
            curLv = (GTextField)GetChild("curLv");
            nextLv = (GTextField)GetChild("nextLv");
            attrList = (GList)GetChild("attrList");
            maxLv = (GTextField)GetChild("maxLv");
            maxList = (GList)GetChild("maxList");
            upLvBtn = (GButton)GetChild("upLvBtn");
            tabCom1 = (GComponent)GetChild("tabCom1");
            tabCom2 = (GComponent)GetChild("tabCom2");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}