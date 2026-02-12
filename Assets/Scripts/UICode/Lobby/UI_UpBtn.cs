/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_UpBtn : GButton
    {
        public Controller status;
        public Controller enough;
        public GLoader bg;
        public GTextField goldNum;
        public const string URL = "ui://s7x7ku0nax20dxy8c";

        public static UI_UpBtn CreateInstance()
        {
            return (UI_UpBtn)UIPackage.CreateObject("Lobby", "UpBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            enough = GetController("enough");
            bg = (GLoader)GetChild("bg");
            goldNum = (GTextField)GetChild("goldNum");
        }
    }
}