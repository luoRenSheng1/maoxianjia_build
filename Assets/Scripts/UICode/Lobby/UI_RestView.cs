/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_RestView : GComponent
    {
        public GLoader3D spine;
        public GGraph hero;
        public const string URL = "ui://s7x7ku0nqalndxyca";

        public static UI_RestView CreateInstance()
        {
            return (UI_RestView)UIPackage.CreateObject("Lobby", "RestView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GLoader3D)GetChild("spine");
            hero = (GGraph)GetChild("hero");
        }
    }
}