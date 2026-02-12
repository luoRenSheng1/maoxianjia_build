/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_TaskBarExp : GProgressBar
    {
        public Controller maxCtrl;
        public const string URL = "ui://2pcsnr2kxezf2n";

        public static UI_TaskBarExp CreateInstance()
        {
            return (UI_TaskBarExp)UIPackage.CreateObject("Passport", "TaskBarExp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
        }
    }
}