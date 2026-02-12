/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GetPetReward : GComponent
    {
        public Controller showCtrl;
        public GLoader img;
        public GLoader3D gxhdSpine;
        public GLoader3D bgSpine;
        public GGraph petSpine;
        public GLoader3D baozhaSpine;
        public GLabel petName;
        public GButton closeBtn;
        public Transition t0;
        public const string URL = "ui://0anhreyls503dxy2o";

        public static UI_GetPetReward CreateInstance()
        {
            return (UI_GetPetReward)UIPackage.CreateObject("Common", "GetPetReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            showCtrl = GetController("showCtrl");
            img = (GLoader)GetChild("img");
            gxhdSpine = (GLoader3D)GetChild("gxhdSpine");
            bgSpine = (GLoader3D)GetChild("bgSpine");
            petSpine = (GGraph)GetChild("petSpine");
            baozhaSpine = (GLoader3D)GetChild("baozhaSpine");
            petName = (GLabel)GetChild("petName");
            closeBtn = (GButton)GetChild("closeBtn");
            t0 = GetTransition("t0");
        }
    }
}