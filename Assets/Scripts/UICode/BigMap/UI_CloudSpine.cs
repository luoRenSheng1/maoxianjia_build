/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace BigMap
{
    public partial class UI_CloudSpine : GComponent
    {
        public GLoader3D cloudSpine;
        public GGraph weatherPos;
        public Transition moveFog;
        public Transition shake;
        public const string URL = "ui://pdufy3kekh6r6";

        public static UI_CloudSpine CreateInstance()
        {
            return (UI_CloudSpine)UIPackage.CreateObject("BigMap", "CloudSpine");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            cloudSpine = (GLoader3D)GetChild("cloudSpine");
            weatherPos = (GGraph)GetChild("weatherPos");
            moveFog = GetTransition("moveFog");
            shake = GetTransition("shake");
        }
    }
}