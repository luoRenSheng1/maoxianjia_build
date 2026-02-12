/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_VillagePetTalk : GLabel
    {
        public GTextField talkDes;
        public const string URL = "ui://5moj1x39yd2hdxy4e";

        public static UI_VillagePetTalk CreateInstance()
        {
            return (UI_VillagePetTalk)UIPackage.CreateObject("CommonEx", "VillagePetTalk");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            talkDes = (GTextField)GetChild("talkDes");
        }
    }
}