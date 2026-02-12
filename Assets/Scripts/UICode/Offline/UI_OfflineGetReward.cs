/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Offline
{
    public partial class UI_OfflineGetReward : GComponent
    {
        public GComponent frame;
        public GList rwList;
        public const string URL = "ui://jbbd3ox4ng623";

        public static UI_OfflineGetReward CreateInstance()
        {
            return (UI_OfflineGetReward)UIPackage.CreateObject("Offline", "OfflineGetReward");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            rwList = (GList)GetChild("rwList");
        }
    }
}