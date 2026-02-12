/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_pathSpineItem : GComponent
    {
        public GLoader3D pathSpine;
        public const string URL = "ui://pdufy3kellb1ii3";

        public static UI_pathSpineItem CreateInstance()
        {
            return (UI_pathSpineItem)UIPackage.CreateObject("BigMap", "pathSpineItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pathSpine = (GLoader3D)GetChild("pathSpine");
        }
    }
}