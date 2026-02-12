/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_HeroAttrItem : GComponent
    {
        public Controller status;
        public GTextField attrName;
        public GTextField attrValue;
        public const string URL = "ui://s7x7ku0na2kwdxyav";

        public static UI_HeroAttrItem CreateInstance()
        {
            return (UI_HeroAttrItem)UIPackage.CreateObject("Lobby", "HeroAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            attrName = (GTextField)GetChild("attrName");
            attrValue = (GTextField)GetChild("attrValue");
        }
    }
}