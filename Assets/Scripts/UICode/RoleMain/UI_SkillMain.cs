/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace RoleMain
{
    public partial class UI_SkillMain : GComponent
    {
        public GLoader3D spine;
        public GLoader rlLoader;
        public GLoader skillItemBg;
        public GList SkillAllList;
        public GButton allStrengthBtn;
        public GComponent redDot;
        public GTextField atkLb;
        public UI_SkillItem skillItem0;
        public UI_SkillItem skillItem1;
        public UI_SkillItem skillItem2;
        public UI_SkillItem skillItem3;
        public UI_SkillItem skillItem4;
        public UI_SkillItem skillItem5;
        public GGroup upGroup;
        public const string URL = "ui://m37flevdcjbedxy6b";

        public static UI_SkillMain CreateInstance()
        {
            return (UI_SkillMain)UIPackage.CreateObject("RoleMain", "SkillMain");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            spine = (GLoader3D)GetChild("spine");
            rlLoader = (GLoader)GetChild("rlLoader");
            skillItemBg = (GLoader)GetChild("skillItemBg");
            SkillAllList = (GList)GetChild("SkillAllList");
            allStrengthBtn = (GButton)GetChild("allStrengthBtn");
            redDot = (GComponent)GetChild("redDot");
            atkLb = (GTextField)GetChild("atkLb");
            skillItem0 = (UI_SkillItem)GetChild("skillItem0");
            skillItem1 = (UI_SkillItem)GetChild("skillItem1");
            skillItem2 = (UI_SkillItem)GetChild("skillItem2");
            skillItem3 = (UI_SkillItem)GetChild("skillItem3");
            skillItem4 = (UI_SkillItem)GetChild("skillItem4");
            skillItem5 = (UI_SkillItem)GetChild("skillItem5");
            upGroup = (GGroup)GetChild("upGroup");
        }
    }
}