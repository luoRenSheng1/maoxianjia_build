/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_SkillShow : GComponent
    {
        public UI_FrameMask_Tips frame;
        public GGraph skillHolder;
        public const string URL = "ui://0anhreylnfkbdxy2x";

        public static UI_SkillShow CreateInstance()
        {
            return (UI_SkillShow)UIPackage.CreateObject("Common", "SkillShow");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Tips)GetChild("frame");
            skillHolder = (GGraph)GetChild("skillHolder");
        }
    }
}