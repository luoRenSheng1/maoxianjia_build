/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace InheritRecycle
{
    public partial class UI_InheritRecycle : GComponent
    {
        public GComponent frame;
        public GList list;
        public GComponent tabCom;
        public GButton recycleBtn;
        public GButton closeBtn;
        public const string URL = "ui://vt5xv73ukqmidxy8s";

        public static UI_InheritRecycle CreateInstance()
        {
            return (UI_InheritRecycle)UIPackage.CreateObject("InheritRecycle", "InheritRecycle");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            list = (GList)GetChild("list");
            tabCom = (GComponent)GetChild("tabCom");
            recycleBtn = (GButton)GetChild("recycleBtn");
            closeBtn = (GButton)GetChild("closeBtn");
        }
    }
}