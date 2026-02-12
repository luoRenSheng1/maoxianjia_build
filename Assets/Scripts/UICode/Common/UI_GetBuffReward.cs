/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_GetBuffReward : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GButton getBtn;
        public GList list;
        public const string URL = "ui://0anhreylsyvcdxy8p";

        public static UI_GetBuffReward CreateInstance()
        {
            return (UI_GetBuffReward)UIPackage.CreateObject("Common", "GetBuffReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            getBtn = (GButton)GetChild("getBtn");
            list = (GList)GetChild("list");
        }
    }
}