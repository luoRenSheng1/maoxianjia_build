/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Common
{
    public partial class UI_SingleReward : GComponent
    {
        public UI_FrameMask_Normal frame;
        public GTextField Title;
        public GTextField Desc;
        public GList rwList;
        public GButton getBtn;
        public GTextField btnTxt;
        public GTextField time;
        public const string URL = "ui://0anhreylsyvcdxy8j";

        public static UI_SingleReward CreateInstance()
        {
            return (UI_SingleReward)UIPackage.CreateObject("Common", "SingleReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (UI_FrameMask_Normal)GetChild("frame");
            Title = (GTextField)GetChild("Title");
            Desc = (GTextField)GetChild("Desc");
            rwList = (GList)GetChild("rwList");
            getBtn = (GButton)GetChild("getBtn");
            btnTxt = (GTextField)GetChild("btnTxt");
            time = (GTextField)GetChild("time");
        }
    }
}