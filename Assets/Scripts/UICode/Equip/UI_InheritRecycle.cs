/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Equip
{
    public partial class UI_InheritRecycle : GComponent
    {
        public Controller status;
        public GComponent frame;
        public GList list;
        public GComponent tabCom;
        public GButton recycleBtn;
        public GButton closeBtn;
        public const string URL = "ui://ddc23erlkqmidxy8s";

        public static UI_InheritRecycle CreateInstance()
        {
            return (UI_InheritRecycle)UIPackage.CreateObject("Equip", "InheritRecycle");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            status = GetController("status");
            frame = (GComponent)GetChild("frame");
            list = (GList)GetChild("list");
            tabCom = (GComponent)GetChild("tabCom");
            recycleBtn = (GButton)GetChild("recycleBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}