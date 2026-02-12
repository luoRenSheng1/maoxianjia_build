/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_ClassicMaxAttrItem : GComponent
    {
        public GTextField maxAttrLb;
        public const string URL = "ui://s7x7ku0nmtmsdxycp";

        public static UI_ClassicMaxAttrItem CreateInstance()
        {
            return (UI_ClassicMaxAttrItem)UIPackage.CreateObject("Lobby", "ClassicMaxAttrItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxAttrLb = (GTextField)GetChild("maxAttrLb");
        }
    }
}