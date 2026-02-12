/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_ArtifactSelectBtn : GButton
    {
        public GComponent redPoint;
        public const string URL = "ui://ddc23erlef8udxybb";

        public static UI_ArtifactSelectBtn CreateInstance()
        {
            return (UI_ArtifactSelectBtn)UIPackage.CreateObject("Equip", "ArtifactSelectBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            redPoint = (GComponent)GetChild("redPoint");
        }
    }
}