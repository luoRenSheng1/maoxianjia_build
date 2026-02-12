/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace FunctionPreview
{
    public partial class UI_FuncPreview : GComponent
    {
        public GComponent frame;
        public GList funcList;
        public const string URL = "ui://tekh4dt9v51s0";

        public static UI_FuncPreview CreateInstance()
        {
            return (UI_FuncPreview)UIPackage.CreateObject("FunctionPreview", "FuncPreview");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            funcList = (GList)GetChild("funcList");
        }
    }
}