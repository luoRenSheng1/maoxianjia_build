/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ChuanSong3 : GComponent
    {
        public GLoader3D effect;
        public const string URL = "ui://0anhreylosqgdxyhq";

        public static UI_ChuanSong3 CreateInstance()
        {
            return (UI_ChuanSong3)UIPackage.CreateObject("Common", "ChuanSong3");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            effect = (GLoader3D)GetChild("effect");
        }
    }
}