/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_battleBg : GComponent
    {
        public Transition sceneT;
        public const string URL = "ui://zoxecbv2qc4n8";

        public static UI_battleBg CreateInstance()
        {
            return (UI_battleBg)UIPackage.CreateObject("PVPMap", "battleBg");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            sceneT = GetTransition("sceneT");
        }
    }
}