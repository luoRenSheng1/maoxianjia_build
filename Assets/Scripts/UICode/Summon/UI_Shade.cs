/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_Shade : GComponent
    {
        public GList summonList;
        public const string URL = "ui://i7ojazuuizi821";

        public static UI_Shade CreateInstance()
        {
            return (UI_Shade)UIPackage.CreateObject("Summon", "Shade");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            summonList = (GList)GetChild("summonList");
        }
    }
}