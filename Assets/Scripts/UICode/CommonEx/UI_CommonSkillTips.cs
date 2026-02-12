/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_CommonSkillTips : GComponent
    {
        public GTextField pContent;
        public const string URL = "ui://5moj1x39kn0fdxy3r";

        public static UI_CommonSkillTips CreateInstance()
        {
            return (UI_CommonSkillTips)UIPackage.CreateObject("CommonEx", "CommonSkillTips");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            pContent = (GTextField)GetChild("pContent");
        }
    }
}