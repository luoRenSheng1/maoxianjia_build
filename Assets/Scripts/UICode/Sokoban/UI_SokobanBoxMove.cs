/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanBoxMove : GComponent
    {
        public Controller c1;
        public UI_SokobanBoxClip Clip;
        public GLoader3D right;
        public GLoader3D left;
        public GLoader3D up1;
        public GLoader3D up2;
        public GGroup up;
        public GLoader3D down1;
        public GLoader3D down2;
        public GGroup down;
        public GImage jiantou;
        public Transition t0;
        public const string URL = "ui://2nawooiyazu01y";

        public static UI_SokobanBoxMove CreateInstance()
        {
            return (UI_SokobanBoxMove)UIPackage.CreateObject("Sokoban", "SokobanBoxMove");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
            Clip = (UI_SokobanBoxClip)GetChild("Clip");
            right = (GLoader3D)GetChild("right");
            left = (GLoader3D)GetChild("left");
            up1 = (GLoader3D)GetChild("up1");
            up2 = (GLoader3D)GetChild("up2");
            up = (GGroup)GetChild("up");
            down1 = (GLoader3D)GetChild("down1");
            down2 = (GLoader3D)GetChild("down2");
            down = (GGroup)GetChild("down");
            jiantou = (GImage)GetChild("jiantou");
            t0 = GetTransition("t0");
        }
    }
}