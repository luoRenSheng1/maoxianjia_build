/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Achievement
{
    public partial class UI_GetBtn : GButton
    {
        public Controller status;
        public const string URL = "ui://b8bvql0irwnpk";

        public static UI_GetBtn CreateInstance()
        {
            return (UI_GetBtn)UIPackage.CreateObject("Achievement", "GetBtn");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
        }
    }
}