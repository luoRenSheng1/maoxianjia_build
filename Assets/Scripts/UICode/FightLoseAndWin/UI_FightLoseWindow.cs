/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FightLoseAndWin
{
    public partial class UI_FightLoseWindow : GComponent
    {
        public GComponent frame;
        public GList failGoList;
        public const string URL = "ui://h7b921iwq38ddxy23";

        public static UI_FightLoseWindow CreateInstance()
        {
            return (UI_FightLoseWindow)UIPackage.CreateObject("FightLoseAndWin", "FightLoseWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            failGoList = (GList)GetChild("failGoList");
        }
    }
}