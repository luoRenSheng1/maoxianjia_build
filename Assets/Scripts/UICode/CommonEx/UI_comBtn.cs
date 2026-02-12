/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_comBtn : GButton
    {
        public Controller isMax;
        public const string URL = "ui://5moj1x39p9n0dxy87";

        public static UI_comBtn CreateInstance()
        {
            return (UI_comBtn)UIPackage.CreateObject("CommonEx", "comBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isMax = GetController("isMax");
        }
    }
}