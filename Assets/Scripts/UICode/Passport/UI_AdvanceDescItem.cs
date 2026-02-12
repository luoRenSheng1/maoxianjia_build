/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Passport
{
    public partial class UI_AdvanceDescItem : GLabel
    {
        public Controller ctrl;
        public const string URL = "ui://2pcsnr2kauqc1p";

        public static UI_AdvanceDescItem CreateInstance()
        {
            return (UI_AdvanceDescItem)UIPackage.CreateObject("Passport", "AdvanceDescItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrl = GetController("ctrl");
        }
    }
}