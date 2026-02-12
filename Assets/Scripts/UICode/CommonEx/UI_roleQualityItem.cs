/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_roleQualityItem : GComponent
    {
        public Controller quality;
        public const string URL = "ui://5moj1x39ozj9dxy5e";

        public static UI_roleQualityItem CreateInstance()
        {
            return (UI_roleQualityItem)UIPackage.CreateObject("CommonEx", "roleQualityItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            quality = GetController("quality");
        }
    }
}