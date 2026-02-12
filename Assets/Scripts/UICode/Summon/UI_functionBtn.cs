/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_functionBtn : GButton
    {
        public GComponent redDot;
        public const string URL = "ui://i7ojazuuq9cgy";

        public static UI_functionBtn CreateInstance()
        {
            return (UI_functionBtn)UIPackage.CreateObject("Summon", "functionBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redDot = (GComponent)GetChild("redDot");
        }
    }
}