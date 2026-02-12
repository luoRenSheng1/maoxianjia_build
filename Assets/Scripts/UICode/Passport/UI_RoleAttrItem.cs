/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_RoleAttrItem : GButton
    {
        public Controller lockCtrl;
        public Controller typeCtrl;
        public Controller starCount;
        public Controller attrCtrl;
        public GLoader pIcon;
        public GTextField lvLb;
        public GComponent star2;
        public GComponent star1;
        public GComponent star0;
        public GTextField starLb;
        public const string URL = "ui://2pcsnr2kr0ab1b";

        public static UI_RoleAttrItem CreateInstance()
        {
            return (UI_RoleAttrItem)UIPackage.CreateObject("Passport", "RoleAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            lockCtrl = GetController("lockCtrl");
            typeCtrl = GetController("typeCtrl");
            starCount = GetController("starCount");
            attrCtrl = GetController("attrCtrl");
            pIcon = (GLoader)GetChild("pIcon");
            lvLb = (GTextField)GetChild("lvLb");
            star2 = (GComponent)GetChild("star2");
            star1 = (GComponent)GetChild("star1");
            star0 = (GComponent)GetChild("star0");
            starLb = (GTextField)GetChild("starLb");
        }
    }
}