/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_Bar : GProgressBar
    {
        public Controller status;
        public const string URL = "ui://b8bvql0irwnpl";

        public static UI_Bar CreateInstance()
        {
            return (UI_Bar)UIPackage.CreateObject("Achievement", "Bar");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
        }
    }
}