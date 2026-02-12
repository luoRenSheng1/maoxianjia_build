/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_Sokoban : GComponent
    {
        public GImage icon;
        public const string URL = "ui://pdufy3keutr3id3";

        public static UI_Sokoban CreateInstance()
        {
            return (UI_Sokoban)UIPackage.CreateObject("BigMap", "Sokoban");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            icon = (GImage)GetChild("icon");
        }
    }
}