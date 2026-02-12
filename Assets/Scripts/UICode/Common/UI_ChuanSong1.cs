/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_ChuanSong1 : GComponent
    {
        public GLoader3D effect;
        public const string URL = "ui://0anhreylosqgdxyhl";

        public static UI_ChuanSong1 CreateInstance()
        {
            return (UI_ChuanSong1)UIPackage.CreateObject("Common", "ChuanSong1");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            effect = (GLoader3D)GetChild("effect");
        }
    }
}