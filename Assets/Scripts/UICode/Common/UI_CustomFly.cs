/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_CustomFly : GComponent
    {
        public GRichTextField textNumber;
        public GTextField tip;
        public Transition nomral;
        public Transition baoji;
        public Transition buff;
        public Transition skill;
        public const string URL = "ui://0anhreylfcoenzh";

        public static UI_CustomFly CreateInstance()
        {
            return (UI_CustomFly)UIPackage.CreateObject("Common", "CustomFly");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            textNumber = (GRichTextField)GetChild("textNumber");
            tip = (GTextField)GetChild("tip");
            nomral = GetTransition("nomral");
            baoji = GetTransition("baoji");
            buff = GetTransition("buff");
            skill = GetTransition("skill");
        }
    }
}