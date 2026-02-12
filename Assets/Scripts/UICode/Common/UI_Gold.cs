/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_Gold : GComponent
    {
        public GMovieClip gold;
        public const string URL = "ui://0anhreylf124dxygv";

        public static UI_Gold CreateInstance()
        {
            return (UI_Gold)UIPackage.CreateObject("Common", "Gold");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            gold = (GMovieClip)GetChild("gold");
        }
    }
}