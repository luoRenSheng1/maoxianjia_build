/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_battleScene : GComponent
    {
        public UI_battleMap map;
        public GComponent obj;
        public GComponent fly;
        public Transition sceneT;
        public const string URL = "ui://s7x7ku0nf646dxy0w";

        public static UI_battleScene CreateInstance()
        {
            return (UI_battleScene)UIPackage.CreateObject("Lobby", "battleScene");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            map = (UI_battleMap)GetChild("map");
            obj = (GComponent)GetChild("obj");
            fly = (GComponent)GetChild("fly");
            sceneT = GetTransition("sceneT");
        }
    }
}