/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetStrength : GComponent
    {
        public GComponent frame;
        public UI_PetStrengList petList;
        public GButton closeBtn;
        public GLoader img;
        public GLoader3D gxhdSpine;
        public Transition t0;
        public const string URL = "ui://lxs2h4ifk6272y";

        public static UI_PetStrength CreateInstance()
        {
            return (UI_PetStrength)UIPackage.CreateObject("Pet", "PetStrength");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            petList = (UI_PetStrengList)GetChild("petList");
            closeBtn = (GButton)GetChild("closeBtn");
            img = (GLoader)GetChild("img");
            gxhdSpine = (GLoader3D)GetChild("gxhdSpine");
            t0 = GetTransition("t0");
        }
    }
}