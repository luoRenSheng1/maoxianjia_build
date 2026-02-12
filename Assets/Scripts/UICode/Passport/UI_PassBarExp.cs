/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_PassBarExp : GProgressBar
    {
        public Controller maxCtrl;
        public Controller barType;
        public GTextField maxLevel;
        public const string URL = "ui://2pcsnr2kxezf2p";

        public static UI_PassBarExp CreateInstance()
        {
            return (UI_PassBarExp)UIPackage.CreateObject("Passport", "PassBarExp");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            maxCtrl = GetController("maxCtrl");
            barType = GetController("barType");
            maxLevel = (GTextField)GetChild("maxLevel");
        }
    }
}