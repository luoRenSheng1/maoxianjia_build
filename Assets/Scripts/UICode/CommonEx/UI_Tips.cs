/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_Tips : GComponent
    {
        public GTextField name;
        public GTextField num;
        public const string URL = "ui://5moj1x39p9n0dxy8a";

        public static UI_Tips CreateInstance()
        {
            return (UI_Tips)UIPackage.CreateObject("CommonEx", "Tips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            name = (GTextField)GetChild("name");
            num = (GTextField)GetChild("num");
        }
    }
}