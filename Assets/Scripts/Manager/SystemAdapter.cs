namespace Engine
{
    using UnityEngine;

    public class SystemAdapter
    {
        // 默认帧率
        public static int S_FRAMERATE_NORMAL = 45;
        public static int S_FRAMERATE_HIGH = -1;
        public static int S_FRAMERATE_LOW = 15;
        // 异步加载时间片(推荐值2~4)
        public static int S_ASYNCUPLOADTIMESLICE_NORMAL = 4;
        // 异步加载时间片-高加载量(推荐值8~16)
        public static int S_ASYNCUPLOADTIMESLICE_HIGH = 16;
        // 异步加载Buffer
        public static int S_ASYNCUPLOADBUFFERSIZE_NORMAL = 32;

        public static int MAX_LOAD_AMOUNT { get; private set; } = 5;
        public static float DEFAULT_UNLOAD_UNUSED_ASSET_TIME { get; private set; } = 120.0f;
        public static float DEFAULT_LOGIN_UNLOAD_UNUSED_ASSET_TIME { get; private set; } = 15.0f;

        public static ThreadPriority InitThreadPriority { get; set; }

        public static void InitSystemInfo()
        {
#if UNITY_STANDALONE
            Application.targetFrameRate = S_FRAMERATE_HIGH;
            MAX_LOAD_AMOUNT = 10;
            DEFAULT_UNLOAD_UNUSED_ASSET_TIME = 600.0f;
#elif UNITY_WEBGL
            MAX_LOAD_AMOUNT = 10;
#else
            Application.targetFrameRate = S_FRAMERATE_NORMAL;
#endif

            QualitySettings.asyncUploadTimeSlice = S_ASYNCUPLOADTIMESLICE_NORMAL;
            QualitySettings.asyncUploadBufferSize = S_ASYNCUPLOADBUFFERSIZE_NORMAL;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            InitThreadPriority = Application.backgroundLoadingPriority;
        }

        public static void EnableHighLoadingMode(bool bEnable)
        {
            if (bEnable)
            {
                QualitySettings.asyncUploadTimeSlice = S_ASYNCUPLOADTIMESLICE_HIGH;
            }
            else
            {
                QualitySettings.asyncUploadTimeSlice = S_ASYNCUPLOADTIMESLICE_NORMAL;
            }
        }

        public static void DisposeSystemInfo()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
