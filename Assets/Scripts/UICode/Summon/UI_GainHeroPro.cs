/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Summon
{
    public partial class UI_GainHeroPro : GComponent
    {
        public Controller quality;
        public Controller type;
        public GTextField name;
        public GTextField value;
        public const string URL = "ui://i7ojazuul5cy3o";

        public static UI_GainHeroPro CreateInstance()
        {
            return (UI_GainHeroPro)UIPackage.CreateObject("Summon", "GainHeroPro");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            quality = GetController("quality");
            type = GetController("type");
            name = (GTextField)GetChild("name");
            value = (GTextField)GetChild("value");
        }
    }
}