/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_RoleToBreak : GComponent
    {
        public Controller attrCtrl;
        public GComponent frame;
        public GLoader bgLoader;
        public GGraph spine;
        public GList starList;
        public GLabel roleNameLb;
        public GLoader occupationLb;
        public GLoader attrLb;
        public GTextField curLb;
        public GTextField nextLb;
        public GLoader attrIcon;
        public GTextField attrName;
        public GTextField attrValue;
        public GList itemList;
        public GButton breakBtn;
        public GButton closeBtn;
        public const string URL = "ui://m37flevdrm2cdxy6q";

        public static UI_RoleToBreak CreateInstance()
        {
            return (UI_RoleToBreak)UIPackage.CreateObject("RoleMain", "RoleToBreak");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            attrCtrl = GetController("attrCtrl");
            frame = (GComponent)GetChild("frame");
            bgLoader = (GLoader)GetChild("bgLoader");
            spine = (GGraph)GetChild("spine");
            starList = (GList)GetChild("starList");
            roleNameLb = (GLabel)GetChild("roleNameLb");
            occupationLb = (GLoader)GetChild("occupationLb");
            attrLb = (GLoader)GetChild("attrLb");
            curLb = (GTextField)GetChild("curLb");
            nextLb = (GTextField)GetChild("nextLb");
            attrIcon = (GLoader)GetChild("attrIcon");
            attrName = (GTextField)GetChild("attrName");
            attrValue = (GTextField)GetChild("attrValue");
            itemList = (GList)GetChild("itemList");
            breakBtn = (GButton)GetChild("breakBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}