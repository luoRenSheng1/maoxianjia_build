/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace CommonEx
{
    public partial class UI_BtnEquip2 : GButton
    {
        public Controller ctrbg;
        public Controller ctrQuality;
        public Controller hasCnt;
        public GLoader qualityIcon;
        public GTextField lvLb;
        public GLoader3D equipSpineEff;
        public const string URL = "ui://5moj1x39ef8udxycu";

        public static UI_BtnEquip2 CreateInstance()
        {
            return (UI_BtnEquip2)UIPackage.CreateObject("CommonEx", "BtnEquip2");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            ctrbg = GetController("ctrbg");
            ctrQuality = GetController("ctrQuality");
            hasCnt = GetController("hasCnt");
            qualityIcon = (GLoader)GetChild("qualityIcon");
            lvLb = (GTextField)GetChild("lvLb");
            equipSpineEff = (GLoader3D)GetChild("equipSpineEff");
        }
    }
}