/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_LobbyHeroAttrAdd : GComponent
    {
        public Controller showHandle;
        public Controller attrCtrl;
        public GLoader attrIcon;
        public GTextField attrName;
        public UI_HeroAttrAniBtn strengthBtn;
        public GTextField lvLb;
        public UI_HeroAttrLabel attrVal;
        public const string URL = "ui://s7x7ku0nozj9dxy54";

        public static UI_LobbyHeroAttrAdd CreateInstance()
        {
            return (UI_LobbyHeroAttrAdd)UIPackage.CreateObject("Lobby", "LobbyHeroAttrAdd");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            showHandle = GetController("showHandle");
            attrCtrl = GetController("attrCtrl");
            attrIcon = (GLoader)GetChild("attrIcon");
            attrName = (GTextField)GetChild("attrName");
            strengthBtn = (UI_HeroAttrAniBtn)GetChild("strengthBtn");
            lvLb = (GTextField)GetChild("lvLb");
            attrVal = (UI_HeroAttrLabel)GetChild("attrVal");
        }
    }
}