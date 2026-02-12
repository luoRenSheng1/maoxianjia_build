/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_petQualityItem : GComponent
    {
        public Controller quality;
        public const string URL = "ui://5moj1x39k627dxy68";

        public static UI_petQualityItem CreateInstance()
        {
            return (UI_petQualityItem)UIPackage.CreateObject("CommonEx", "petQualityItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            quality = GetController("quality");
        }
    }
}