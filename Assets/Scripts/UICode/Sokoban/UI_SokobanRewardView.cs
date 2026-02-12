/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace Sokoban
{
    public partial class UI_SokobanRewardView : GComponent
    {
        public GComponent frame;
        public GTextField title;
        public GButton btn;
        public GList rewardList;
        public const string URL = "ui://2nawooiyjnq814";

        public static UI_SokobanRewardView CreateInstance()
        {
            return (UI_SokobanRewardView)UIPackage.CreateObject("Sokoban", "SokobanRewardView");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            frame = (GComponent)GetChild("frame");
            title = (GTextField)GetChild("title");
            btn = (GButton)GetChild("btn");
            rewardList = (GList)GetChild("rewardList");
        }
    }
}