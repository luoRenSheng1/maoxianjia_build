/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace NoOperation
{
    public partial class UI_NoOperation : GComponent
    {
        public UI_Slider slider;
        public UI_PetRunRoot petRoot;
        public GTextField stageName;
        public GTextField dateLb;
        public Transition t0;
        public Transition t1;
        public const string URL = "ui://88ncwg57uobv0";

        public static UI_NoOperation CreateInstance()
        {
            return (UI_NoOperation)UIPackage.CreateObject("NoOperation", "NoOperation");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            slider = (UI_Slider)GetChild("slider");
            petRoot = (UI_PetRunRoot)GetChild("petRoot");
            stageName = (GTextField)GetChild("stageName");
            dateLb = (GTextField)GetChild("dateLb");
            t0 = GetTransition("t0");
            t1 = GetTransition("t1");
        }
    }
}