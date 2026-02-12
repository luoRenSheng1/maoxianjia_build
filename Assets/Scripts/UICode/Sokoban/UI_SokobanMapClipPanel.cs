/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanMapClipPanel : GComponent
    {
        public Controller c1;
        public GLoader map1;
        public GLoader map2;
        public GLoader3D boxSpine;
        public GLoader box;
        public GLoader3D boxTips;
        public GLoader boxBar;
        public GLoader3D TenMan;
        public GComponent panel0;
        public GComponent panel1;
        public GComponent panel2;
        public GLoader3D terminus;
        public GGraph hero;
        public GLoader bottom;
        public GLoader pos1;
        public GLoader pos2;
        public GTextField num;
        public const string URL = "ui://2nawooiyosqg1h";

        public static UI_SokobanMapClipPanel CreateInstance()
        {
            return (UI_SokobanMapClipPanel)UIPackage.CreateObject("Sokoban", "SokobanMapClipPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            c1 = GetController("c1");
            map1 = (GLoader)GetChild("map1");
            map2 = (GLoader)GetChild("map2");
            boxSpine = (GLoader3D)GetChild("boxSpine");
            box = (GLoader)GetChild("box");
            boxTips = (GLoader3D)GetChild("boxTips");
            boxBar = (GLoader)GetChild("boxBar");
            TenMan = (GLoader3D)GetChild("TenMan");
            panel0 = (GComponent)GetChild("panel0");
            panel1 = (GComponent)GetChild("panel1");
            panel2 = (GComponent)GetChild("panel2");
            terminus = (GLoader3D)GetChild("terminus");
            hero = (GGraph)GetChild("hero");
            bottom = (GLoader)GetChild("bottom");
            pos1 = (GLoader)GetChild("pos1");
            pos2 = (GLoader)GetChild("pos2");
            num = (GTextField)GetChild("num");
        }
    }
}