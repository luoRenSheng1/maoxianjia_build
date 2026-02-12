/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Lobby
{
    public partial class UI_BtnBoxIcon : GButton
    {
        public GLoader3D spineEff;
        public GLoader3D fx;
        public GLoader3D fx1;
        public const string URL = "ui://s7x7ku0nt5mtdxy0v";

        public static UI_BtnBoxIcon CreateInstance()
        {
            return (UI_BtnBoxIcon)UIPackage.CreateObject("Lobby", "BtnBoxIcon");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spineEff = (GLoader3D)GetChild("spineEff");
            fx = (GLoader3D)GetChild("fx");
            fx1 = (GLoader3D)GetChild("fx1");
        }
    }
}