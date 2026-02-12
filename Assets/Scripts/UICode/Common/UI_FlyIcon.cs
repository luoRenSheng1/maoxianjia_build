/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_FlyIcon : GComponent
    {
        public GLoader iconUrl;
        public const string URL = "ui://0anhreylktw8dxy7d";

        public static UI_FlyIcon CreateInstance()
        {
            return (UI_FlyIcon)UIPackage.CreateObject("Common", "FlyIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            iconUrl = (GLoader)GetChild("iconUrl");
        }
    }
}