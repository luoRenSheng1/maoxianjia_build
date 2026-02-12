/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lottery
{
    public partial class UI_boxExpBar : GProgressBar
    {
        public Controller c1;
        public const string URL = "ui://6izp804wju03u";

        public static UI_boxExpBar CreateInstance()
        {
            return (UI_boxExpBar)UIPackage.CreateObject("Lottery", "boxExpBar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
        }
    }
}