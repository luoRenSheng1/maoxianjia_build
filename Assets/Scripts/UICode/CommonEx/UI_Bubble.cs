/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_Bubble : GLabel
    {
        public GLoader bubbleBg;
        public GTextField talkDes;
        public const string URL = "ui://5moj1x39sshpdxycv";

        public static UI_Bubble CreateInstance()
        {
            return (UI_Bubble)UIPackage.CreateObject("CommonEx", "Bubble");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bubbleBg = (GLoader)GetChild("bubbleBg");
            talkDes = (GTextField)GetChild("talkDes");
        }
    }
}