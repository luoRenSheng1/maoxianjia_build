/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_HeroAttrAniBtn : GComponent
    {
        public UI_HeroAttrBtn heroBtn;
        public Transition t0;
        public const string URL = "ui://s7x7ku0ndr5adxy5c";

        public static UI_HeroAttrAniBtn CreateInstance()
        {
            return (UI_HeroAttrAniBtn)UIPackage.CreateObject("Lobby", "HeroAttrAniBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            heroBtn = (UI_HeroAttrBtn)GetChild("heroBtn");
            t0 = GetTransition("t0");
        }
    }
}