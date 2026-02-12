/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BossFightWindow : GComponent
    {
        public GGraph monster;
        public GGraph hero;
        public Transition vs;
        public const string URL = "ui://0anhreylmrdwdxy20";

        public static UI_BossFightWindow CreateInstance()
        {
            return (UI_BossFightWindow)UIPackage.CreateObject("Common", "BossFightWindow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            monster = (GGraph)GetChild("monster");
            hero = (GGraph)GetChild("hero");
            vs = GetTransition("vs");
        }
    }
}