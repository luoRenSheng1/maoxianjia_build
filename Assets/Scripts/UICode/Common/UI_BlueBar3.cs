/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_BlueBar3 : GComponent
    {
        public GProgressBar bar;
        public GTextField title;
        public const string URL = "ui://0anhreylaytsdxxwo";

        public static UI_BlueBar3 CreateInstance()
        {
            return (UI_BlueBar3)UIPackage.CreateObject("Common", "BlueBar3");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            bar = (GProgressBar)GetChild("bar");
            title = (GTextField)GetChild("title");
        }
    }
}