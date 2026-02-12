/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace PVPMap
{
    public partial class UI_battleScene : GComponent
    {
        public GComponent obj;
        public GComponent fly;
        public Transition sceneT;
        public const string URL = "ui://zoxecbv2qc4nb";

        public static UI_battleScene CreateInstance()
        {
            return (UI_battleScene)UIPackage.CreateObject("PVPMap", "battleScene");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            obj = (GComponent)GetChild("obj");
            fly = (GComponent)GetChild("fly");
            sceneT = GetTransition("sceneT");
        }
    }
}