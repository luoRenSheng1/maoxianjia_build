/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ChuanSong2 : GComponent
    {
        public GLoader3D effect;
        public const string URL = "ui://0anhreylosqgdxyhp";

        public static UI_ChuanSong2 CreateInstance()
        {
            return (UI_ChuanSong2)UIPackage.CreateObject("Common", "ChuanSong2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            effect = (GLoader3D)GetChild("effect");
        }
    }
}