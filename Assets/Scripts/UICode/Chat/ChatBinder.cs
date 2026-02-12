/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;

namespace Chat
{
    public class ChatBinder
    {
        public static void BindAll()
        {
            UIObjectFactory.SetPackageItemExtension(UI_ChatLeftItem.URL, typeof(UI_ChatLeftItem));
            UIObjectFactory.SetPackageItemExtension(UI_ChatRightItem.URL, typeof(UI_ChatRightItem));
            UIObjectFactory.SetPackageItemExtension(UI_ChatSysItem.URL, typeof(UI_ChatSysItem));
            UIObjectFactory.SetPackageItemExtension(UI_ChatView.URL, typeof(UI_ChatView));
            UIObjectFactory.SetPackageItemExtension(UI_TabBtn.URL, typeof(UI_TabBtn));
        }
    }
}