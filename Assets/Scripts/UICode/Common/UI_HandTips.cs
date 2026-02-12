/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_HandTips : GComponent
    {
        public Controller ctrl;
        public const string URL = "ui://0anhreylddc1dxy4v";

        public static UI_HandTips CreateInstance()
        {
            return (UI_HandTips)UIPackage.CreateObject("Common", "HandTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
        }
    }
}