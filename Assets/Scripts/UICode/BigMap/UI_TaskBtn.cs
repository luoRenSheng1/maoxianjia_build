/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_TaskBtn : GButton
    {
        public GTextField num;
        public const string URL = "ui://pdufy3kemqvf35";

        public static UI_TaskBtn CreateInstance()
        {
            return (UI_TaskBtn)UIPackage.CreateObject("BigMap", "TaskBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            num = (GTextField)GetChild("num");
        }
    }
}