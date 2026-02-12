/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_AllHeroAttrPanel : GComponent
    {
        public Controller typeCtrl;
        public GComponent frame;
        public GList basicAttrList;
        public GList advancedAttrList;
        public GButton closeBtn2;
        public const string URL = "ui://s7x7ku0nlir3dxy38";

        public static UI_AllHeroAttrPanel CreateInstance()
        {
            return (UI_AllHeroAttrPanel)UIPackage.CreateObject("Lobby", "AllHeroAttrPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            typeCtrl = GetController("typeCtrl");
            frame = (GComponent)GetChild("frame");
            basicAttrList = (GList)GetChild("basicAttrList");
            advancedAttrList = (GList)GetChild("advancedAttrList");
            closeBtn2 = (GButton)GetChild("closeBtn2");
        }
    }
}