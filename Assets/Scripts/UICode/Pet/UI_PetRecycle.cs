/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Pet
{
    public partial class UI_PetRecycle : GComponent
    {
        public Controller status;
        public Controller type;
        public GComponent frame;
        public GList list;
        public GComponent tabCom;
        public GButton recycleBtn;
        public GButton recycleBtn2;
        public GButton closeBtn;
        public const string URL = "ui://lxs2h4ifhz5cdxy7g";

        public static UI_PetRecycle CreateInstance()
        {
            return (UI_PetRecycle)UIPackage.CreateObject("Pet", "PetRecycle");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            type = GetController("type");
            frame = (GComponent)GetChild("frame");
            list = (GList)GetChild("list");
            tabCom = (GComponent)GetChild("tabCom");
            recycleBtn = (GButton)GetChild("recycleBtn");
            recycleBtn2 = (GButton)GetChild("recycleBtn2");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}