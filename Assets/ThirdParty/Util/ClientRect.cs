#if PF_DOUYIN
using StarkSDKSpace.UNBridgeLib.LitJson;
using LogUtils = ThirdParty.Wrapper.ExtensionsDefs.LogUtils;

namespace ThirdParty
{
    public class ClientRect
    {
        /// <summary>下边界坐标，单位：px</summary>
        public double bottom;
        /// <summary>高度，单位：px</summary>
        public double height;
        /// <summary>左边界坐标，单位：px</summary>
        public double left;
        /// <summary>右边界坐标，单位：px</summary>
        public double right;
        /// <summary>上边界坐标，单位：px</summary>
        public double top;
        /// <summary>宽度，单位：px</summary>
        public double width;
        
        public ClientRect(JsonData menuRect)
        {
            LogUtils.LogWarning($"ClientRect menuRect:{menuRect}");
            
            width = (double)menuRect["width"];
            height = (double)menuRect["height"];
            top = (double)menuRect["top"];
            right = (double)menuRect["right"];
            bottom = (double)menuRect["bottom"];
            left = (double)menuRect["left"];
        }
    }
}
#endif