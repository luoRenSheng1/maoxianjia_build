#if PF_WEIXIN

using Engine;
using SimpleJson;
using System;
using System.Collections.Generic;
using ThirdParty.Util;
using ThirdParty.Wrapper;
using UnityEngine;
using WeChatWASM;
using Touch = UnityEngine.Touch;

public class SDKInterfaceWeiXin : TSingleton<SDKInterfaceWeiXin>
{
    private WXUserInfoButton btnUserInfo;

    private WXGameClubButton clubBtn = null;

    private int nBenchmarkLevel = 0;
    public bool m_isInitSystemInfo { get; private set; }
    public WeChatWASM.SystemInfo m_sdkSystemInfo { get; private set; }
    public WeChatWASM.ClientRect m_clientRect { get; private set; }

    public string AccountIDFrom { get; private set; } = "";

    private Dictionary<int, object[]> m_touchEvents = new Dictionary<int, object[]>();

    public void DoAwake()
    {
        if (IsMinigamePlatform(MinigamePlatform.ios))
        {
            WX.SetPreferredFramesPerSecond(30);
        }
        else
        {
            WX.SetPreferredFramesPerSecond(60);
        }

        var screenon = new SetKeepScreenOnOption();
        screenon.keepScreenOn = true;
        WX.SetKeepScreenOn(screenon);

        nBenchmarkLevel = GetBenchmarkLevel();

#if !UNITY_EDITOR && PF_WEIXIN
        WX.OnShow(OnSDKShow);
        WX.OnTouchStart(OnWxTouchStart);
        WX.OnTouchMove(OnWxTouchMove);
        WX.OnTouchEnd(OnWxTouchEnd);
        WX.OnTouchCancel(OnWxTouchCancel);
        
        UICamera.GetInputTouchCount = GetUITouchCount;
        UICamera.GetInputTouch = GetUITouch;
        
        WX.OnNeedPrivacyAuthorization((result) =>
        {
            LogUtils.LogWarning("WX.OnNeedPrivacyAuthorization");
                    
            //开发者弹出自定义的隐私弹窗，并调用告知平台已经弹窗
            WX.PrivacyAuthorizeResolve(new PrivacyAuthorizeResolveOption()
            {
                eventString = "exposureAuthorization"
            });
            
            CommonPopupManager.Instance.StartPopup(CommonPopupManager.ePopupType.YESNO_PRIVACY_AUTHORIZE, "", null);
                    
            //触摸回调必须在WX.OnTouchEnd
            Transform set_popup = UIManager.Instance.GetPopupTrans(CommonPopupManager.ePopupType.YESNO_PRIVACY_AUTHORIZE);
            Transform btnYes = set_popup.Find("bt_yes");
            Transform btnNo = set_popup.Find("bt_no");
            if (null != btnYes && null != btnNo)
            {
                Collider yesCollider = btnYes.GetComponent<Collider>();
                SDKInterface.Instance.AddTouchEventHandler(defMinigameTouchEvent.agreePrivacyContract, yesCollider);
                Collider noCollider = btnNo.GetComponent<Collider>();
                SDKInterface.Instance.AddTouchEventHandler(defMinigameTouchEvent.disagreePrivacyContract, noCollider);
            }
        });
        _layerMask = 1 << LayerMask.NameToLayer("UI");
#endif
    }

    public void DoDestroy()
    {
        HideGameClubButton();

#if !UNITY_EDITOR && PF_WEIXIN
        WX.OffShow(OnSDKShow);
        WX.OffTouchStart(OnWxTouchStart);
        WX.OffTouchMove(OnWxTouchMove);
        WX.OffTouchEnd(OnWxTouchEnd);
        WX.OffTouchCancel(OnWxTouchCancel);

        UICamera.GetInputTouchCount = null;
        UICamera.GetInputTouch = null;
#endif
    }

    private void InitSystemInfo()
    {
        if (!m_isInitSystemInfo)
        {
#if !UNITY_EDITOR
            m_sdkSystemInfo = WX.GetSystemInfoSync();
            m_clientRect = WX.GetMenuButtonBoundingClientRect();
#else
            m_sdkSystemInfo = new WeChatWASM.SystemInfo();
            m_sdkSystemInfo.platform = "";
            m_clientRect = new ClientRect();
#endif
            m_isInitSystemInfo = true;
        }
    }

    public WeChatWASM.SystemInfo GetSystemInfoSync()
    {
        InitSystemInfo();
        return m_sdkSystemInfo;
    }

    public WeChatWASM.ClientRect GetMenuButtonBoundingClientRect()
    {
        InitSystemInfo();
        return m_clientRect;
    }

    public bool IsMinigamePlatform(string platform)
    {
        InitSystemInfo();
        return m_sdkSystemInfo.platform.Equals(platform);
    }

    public string GetSystemLanguageWithMinigame()
    {
        var wxSystemInfo = GetSystemInfoSync();
        return wxSystemInfo.language;
    }

    private int GetBenchmarkLevel()
    {
        InitSystemInfo();

        if (IsMinigamePlatform(MinigamePlatform.ios))
        {
            var model = m_sdkSystemInfo.model;

            // iPhone 7 及以下
            string[] lowPhoneType =
            {
                "iPhone1,1", "iPhone1,2", "iPhone2,1", "iPhone3,1", "iPhone3,3", "iPhone4,1",
                "iPhone5,1", "iPhone5,2", "iPhone5,3", "iPhone5,4", "iPhone6,1", "iPhone6,2",
                "iPhone6,2", "iPhone7,1", "iPhone7,2", "iPhone8,1", "iPhone8,2", "iPhone8,4",
                "iPhone9,1", "iPhone9,2", "iPhone9,3", "iPhone9,4"
            };

            // iPhone 8 ~ iPhone XS
            string[] middlePhoneType =
            {
                "iPhone10,1", "iPhone10,2", "iPhone10,3", "iPhone10,4", "iPhone10,5", "iPhone10,6",
                "iPhone11,2", "iPhone11,4", "iPhone11,6",
            };

            // 低端机
            for (var i = 0; i < lowPhoneType.Length; i++)
            {
                if (model.Contains(lowPhoneType[i]))
                {
                    return 10;
                }
            }

            // 中端机
            for (var i = 0; i < middlePhoneType.Length; i++)
            {
                if (model.Contains(lowPhoneType[i]))
                {
                    return 20;
                }
            }

            // 默认高端机
            return 50;
        }
        else
        {
            return (int)m_sdkSystemInfo.benchmarkLevel;
        }
    }

    /// <summary>
    /// 低端机判断
    /// </summary>
    /// <returns></returns>
    public bool CheckIsLowPhone()
    {
        return nBenchmarkLevel < 22;
    }

    public bool CheckSDKApiLevel(string value)
    {
        InitSystemInfo();

        if (SDKInterface.Instance.CompareVersion(m_sdkSystemInfo.SDKVersion, value) > 0)
        {
            return false;
        }

        return true;
    }

    class TouchData
    {
        public Touch touch;
        public long timeStamp;
    }

    private readonly List<TouchData> _touches = new List<TouchData>();

    private UICamera.Touch touchTmp = new UICamera.Touch();

    public int touchCount
    {
        get { return _touches.Count; }
    }

    public Touch GetTouch(int index)
    {
        return _touches[index].touch;
    }

    public bool InputAnyKey
    {
        get
        {
            if (_touches.Count > 0)
            {
                foreach (var touchData in _touches)
                {
                    var touch = touchData.touch;

                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    private TouchData FindOrCreateTouchData(int identifier)
    {
        var touchData = FindTouchData(identifier);
        if (touchData != null) return touchData;

        var data = new TouchData();
        data.touch.pressure = 1.0f;
        data.touch.maximumPossiblePressure = 1.0f;
        data.touch.type = TouchType.Direct;
        data.touch.tapCount = 1;
        data.touch.fingerId = identifier;
        data.touch.radius = 0;
        data.touch.radiusVariance = 0;
        data.touch.altitudeAngle = 0;
        data.touch.azimuthAngle = 0;
        data.touch.deltaTime = 0;
        _touches.Add(data);
        return data;
    }

    private TouchData FindTouchData(int identifier)
    {
        foreach (var touchData in _touches)
        {
            var touch = touchData.touch;
            if (touch.fingerId == identifier)
            {
                return touchData;
            }
        }

        return null;
    }

    private static void UpdateTouchData(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
    {
        data.touch.phase = phase;
        data.touch.deltaPosition = pos - data.touch.position;
        data.touch.position = pos;
        data.touch.deltaTime = (timeStamp - data.timeStamp) / 1000000.0f;
    }

    private void OnSDKShow(OnShowListenerResult param)
    {
        if (param != null)
        {
            if (param.query != null)
            {
                string acFrom = "";

                if (param.query.TryGetValue("ac", out acFrom))
                {
                    LogUtils.LogWarningFormat("WX OnSDKShow Query ac {0}", acFrom);

                    AccountIDFrom = acFrom;
                }
            }
        }
    }

    private void OnWxTouchStart(OnTouchStartListenerResult touchEvent)
    {
        // LogUtils.LogWarning("OnWxTouchStart " + touchEvent.changedTouches.Length);

        foreach (var wxTouch in touchEvent.changedTouches)
        {
            var data = FindOrCreateTouchData(wxTouch.identifier);
            data.touch.phase = TouchPhase.Began;
            data.touch.position = new Vector2(wxTouch.clientX, wxTouch.clientY);
            data.touch.rawPosition = data.touch.position;
            data.timeStamp = touchEvent.timeStamp;

            // LogUtils.LogWarning($"OnWxTouchStart:{wxTouch.identifier}, {data.touch.phase}");
        }
    }

    private void OnWxTouchMove(OnTouchStartListenerResult touchEvent)
    {
        foreach (var wxTouch in touchEvent.changedTouches)
        {
            var data = FindOrCreateTouchData(wxTouch.identifier);
            UpdateTouchData(data, new Vector2(wxTouch.clientX, wxTouch.clientY), touchEvent.timeStamp, TouchPhase.Moved);
        }
    }

    private void OnWxTouchEnd(OnTouchStartListenerResult touchEvent)
    {
        // LogUtils.LogWarning("OnWxTouchEnd " + touchEvent.changedTouches.Length);

        foreach (var wxTouch in touchEvent.changedTouches)
        {
            TouchData data = FindTouchData(wxTouch.identifier);

            if (data == null)
            {
                // LogUtils.LogError($"OnWxTouchEnd, error identifier:{wxTouch.identifier}");
                continue;
            }

            /*if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
            {
                LogUtils.LogWarning($"OnWxTouchEnd, error phase:{wxTouch.identifier}, phase:{data.touch.phase}");
            }*/

            // LogUtils.LogWarning($"OnWxTouchEnd:{wxTouch.identifier}");

            UpdateTouchData(data, new Vector2(wxTouch.clientX, wxTouch.clientY), touchEvent.timeStamp, TouchPhase.Ended);
        }

        OnTouchEndEventWithFunc(touchEvent);
    }

    private void OnWxTouchCancel(OnTouchStartListenerResult touchEvent)
    {
        // LogUtils.LogWarning("OnWxTouchCancel " + touchEvent.changedTouches.Length);

        foreach (var wxTouch in touchEvent.changedTouches)
        {
            TouchData data = FindTouchData(wxTouch.identifier);

            if (data == null)
            {
                // LogUtils.LogError($"OnWxTouchCancel, error identifier:{wxTouch.identifier}");
                continue;
            }

            /*if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
            {
                LogUtils.LogWarning($"OnWxTouchCancel, error phase:{wxTouch.identifier}, phase:{data.touch.phase}");
            }*/

            // LogUtils.LogWarning($"OnWxTouchCancel:{wxTouch.identifier}");

            UpdateTouchData(data, new Vector2(wxTouch.clientX, wxTouch.clientY), touchEvent.timeStamp, TouchPhase.Canceled);
        }

        // Cancel 跟Android系统手势冲突，可能是微信的Bug，这里收到一个Canceled，取消全部点击事件
        /*if (IsMinigamePlatform(MinigamePlatform.android))
        {
            foreach (var touchData in _touches)
            {
                if (touchData == null || touchData.touch.phase == TouchPhase.Canceled)
                {
                    continue;
                }

                /*if (data.touch.phase == TouchPhase.Canceled || data.touch.phase == TouchPhase.Ended)
                {
                    LogUtils.LogWarning($"OnWxTouchCancel, error phase:{wxTouch.identifier}, phase:{data.touch.phase}");
                }#1#

                // LogUtils.LogWarning($"OnWxTouchCancel:{touchData.touch.fingerId}");

                UpdateTouchData(touchData, touchData.touch.position, touchEvent.timeStamp, TouchPhase.Canceled);
            }
        }*/
    }

    public void DoLateUpdate()
    {
        foreach (var t in _touches)
        {
            if (t.touch.phase == TouchPhase.Began)
            {
                t.touch.phase = TouchPhase.Stationary;
            }
        }

        RemoveEndedTouches();
    }

    private void RemoveEndedTouches()
    {
        if (_touches.Count > 0)
        {
            _touches.RemoveAll(touchData =>
            {
                var touch = touchData.touch;
                return (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
            });
        }
    }

    public int GetUITouchCount()
    {
        return touchCount;
    }

    public UICamera.Touch GetUITouch(int index)
    {
        var touch = GetTouch(index);
        touchTmp.fingerId = touch.fingerId;
        touchTmp.phase = touch.phase;
        touchTmp.position = touch.position;
        touchTmp.tapCount = touch.tapCount;
        return touchTmp;
    }

    public void ReportScene(int sceneId)
    {
        if (!CheckSDKApiLevel("2.26.2"))
        {
            return;
        }

        ReportSceneOption callback = new ReportSceneOption();
        callback.sceneId = sceneId;
        WX.ReportScene(callback);
    }

    public void OnLogin()
    {
        LoginOption callback = new LoginOption();

        callback.success = (res) =>
        {
            var param = WX.GetLaunchOptionsSync();

            if (param != null)
            {
                if (param.query != null)
                {
                    string acFrom = "";

                    if (param.query.TryGetValue("ac", out acFrom))
                    {
                        LogUtils.LogWarningFormat("WX OnLogin Query ac {0}", acFrom);

                        AccountIDFrom = acFrom;
                    }
                }
            }

            SDKInterface.Instance.OnLoginSuccess(res.code, res.code, res.code);
        };
        callback.fail = (res) => { SDKInterface.Instance.OnLoginFailed(res.errMsg); };

        WX.Login(callback);
    }

    public void OnLoginLobbyServer(string token, string roleId, string account)
    {
    }

    public void DestroyBtnUserInfo()
    {
        if (btnUserInfo != null)
        {
            btnUserInfo.Destroy();
            btnUserInfo = null;
        }
    }

    public void GetWXUserInfo()
    {
        bool bGetSettingDone = false;
        bool bHaveAuth = false;

        GetSettingOption actionSetting = new GetSettingOption();
        actionSetting.success = (res) =>
        {
            LogUtils.LogWarning("WX GetSetting Success");

            if (res != null && res.authSetting != null)
            {
                foreach (var item in res.authSetting)
                {
                    if (item.Key.ToLower().Equals("scope.userInfo"))
                    {
                        bHaveAuth = item.Value;
                        break;
                    }
                }
            }

            bGetSettingDone = true;

            if (!bHaveAuth)
            {
                WX.OpenSetting(null);
            }
        };

        actionSetting.fail = (res) =>
        {
            LogUtils.LogWarning("WX GetSetting Fail");

            bHaveAuth = false;
            bGetSettingDone = true;
        };

        WX.GetSetting(actionSetting);

        //         while (!bGetSettingDone)
        //         {
        //             yield return null;
        //         }

        bool bGetUserInfoDone = false;

        if (bHaveAuth)
        {
            LogUtils.LogWarning("WX HaveAuth GetUserInfo");

            GetUserInfoOption actionUserInfo = new GetUserInfoOption();

            actionUserInfo.success = (res) =>
            {
                LogUtils.LogWarning("User Info Success");

                var userInfo = res.userInfo;
                var nickName = userInfo.nickName;
                var avatarUrl = userInfo.avatarUrl;
                var gender = userInfo.gender; //性别 0：未知、1：男、2：女
                var province = userInfo.province;
                var city = userInfo.city;
                var country = userInfo.country;

                LogUtils.LogWarning($"User Info {nickName} {avatarUrl} {gender} {province} {city} {country}");

                DataAnalyticsWrapper.Instance.OnInitUserInfoByWeChat(nickName);

                bGetUserInfoDone = true;
            };

            actionUserInfo.fail = (res) =>
            {
                LogUtils.LogWarning("User Info Fail");

                bGetUserInfoDone = true;
            };

            WX.GetUserInfo(actionUserInfo);
        }
        else
        {
            LogUtils.LogWarning("WX No HaveAuth GetUserInfo");

            if (btnUserInfo == null)
            {
                btnUserInfo = WX.CreateUserInfoButton(10, 76, 200, 40, "zh_CN", false);

                Action<WXUserInfoResponse> action = (res) =>
                {
                    LogUtils.LogWarning("User Info Success");

                    var userInfo = res.userInfo;
                    var nickName = userInfo.nickName;
                    var avatarUrl = userInfo.avatarUrl;
                    var gender = userInfo.gender; //性别 0：未知、1：男、2：女
                    var province = userInfo.province;
                    var city = userInfo.city;
                    var country = userInfo.country;

                    LogUtils.LogWarning($"User Info {nickName} {avatarUrl} {gender} {province} {city} {country}");
                };

                btnUserInfo.OnTap(action);
            }
            else
            {
                btnUserInfo.Show();
            }

            bGetUserInfoDone = true;
        }

        if (bGetUserInfoDone)
        {
            LogUtils.LogWarning("bGetUserInfoDone");
        }

        if (bGetSettingDone)
        {
            LogUtils.LogWarning("bGetSettingDone");
        }
        //         while (!bGetUserInfoDone)
        //         {
        //             yield return null;
        //         }
    }

    /// <summary>
    /// 适配
    /// </summary>
    /// <param name="anchor"></param>
    public void OptionSafeAreaUIAnchor(UIAnchor anchor)
    {
        int topCorrectionOffset = (int)Screen.safeArea.yMax; // SafeAreaScreen.GetTopOffsetY();
        int bottomCorrectionOffset = (int)Screen.safeArea.yMin;

        if (SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.ios)
            || SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.android)
            || SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.devtools))
        {
            var sysInfo = GetSystemInfoSync();

            var botomOffset = (int)(sysInfo.screenHeight - sysInfo.safeArea.bottom);
            bottomCorrectionOffset = Mathf.Clamp(botomOffset, 0, 60) + 3; //优化显示多偏移3个像素
            if (anchor.side == UIAnchor.Side.Top
                || anchor.side == UIAnchor.Side.TopLeft
                || anchor.side == UIAnchor.Side.TopRight)
            {
                bool len = (sysInfo.screenHeight / sysInfo.screenWidth) > 1.8f;

                var menuRect = GetMenuButtonBoundingClientRect();
                if (SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.ios))
                {
                    topCorrectionOffset = (int)(menuRect.bottom * (Screen.height / sysInfo.screenHeight)) + (len ? 20 : 15); //优化显示多偏移20个像素
                }
                else
                {
                    topCorrectionOffset = (int)(menuRect.bottom * sysInfo.pixelRatio) + (len ? 20 : 15); //优化显示多偏移20个像素
                }
            }
        }
        else
        {
            return;
        }

        switch (anchor.side)
        {
            case UIAnchor.Side.Top:
            case UIAnchor.Side.TopLeft:
            case UIAnchor.Side.TopRight:
                anchor.pixelOffset.Set(0, -(topCorrectionOffset));
                break;

            case UIAnchor.Side.Bottom:
            case UIAnchor.Side.BottomLeft:
            case UIAnchor.Side.BottomRight:
                anchor.pixelOffset.Set(0, bottomCorrectionOffset);
                break;
        }
    }

    /// <summary>
    /// 登入信息打点
    /// </summary>
    /// <param name="action"></param>
    public void GetSystemInfoByTDLogin(Action<string, string> action)
    {
        if (action != null)
        {
            var sysInfo = GetSystemInfoSync();
            action.Invoke(sysInfo.model, sysInfo.platform);
        }
    }

    /// <summary>
    /// 问卷跳转
    /// </summary>
    public void NavigateToMiniProgram(string questionnaireId)
    {
        LogUtils.LogWarning("NavigateToMiniProgram");

        var que = AssetManager.Instance.asset_questionnaire.list.Find(m => m.qid == questionnaireId);
        if (que != null)
        {
            var path = string.Format(que.path, DataManager.Instance.GetUserID());
            LogUtils.LogWarning($"NavigateToMiniProgram path={path}");

            NavigateToMiniProgramOption param = new NavigateToMiniProgramOption();
            param.appId = "wxd947200f82267e58";
            param.path = path;
            param.success = (res) => { LogUtils.LogWarning("NavigateToMiniProgram success"); };
            param.fail = (res) => { LogUtils.LogWarning($"NavigateToMiniProgram fail res.errMsg = {res.errMsg}"); };
            param.complete = (res) => { LogUtils.LogWarning("NavigateToMiniProgram complete"); };
            WX.NavigateToMiniProgram(param);
        }
        else
        {
            LogUtils.LogWarning($"NavigateToMiniProgram questionnaireId={questionnaireId}");
        }
    }


    /// <summary>
    /// 创建微信游戏圈按钮--仅主场景使用
    /// </summary>
    /// <param name="screenPos"></param>
    /// <param name="index">右侧图标的位置</param>
    public void ShowGameClubButton(Vector2 screenPos, bool bChanged = false)
    {
        //LogUtils.LogWarningFormat($"wxsdk ShowGameClubButton {screenPos}");
        if (bChanged)
        {
            if (clubBtn != null)
            {
                clubBtn.Hide();
                clubBtn = null;
            }
        }

        if (clubBtn == null)
        {
            var wxSystemInfo = GetSystemInfoSync();
            var x = screenPos.x * (wxSystemInfo.screenWidth / Screen.width);
            var y = wxSystemInfo.screenHeight - (screenPos.y * (wxSystemInfo.screenHeight / Screen.height));
            int with = ConstDefine.CLUB_ICON_SIZE;
            int height = ConstDefine.CLUB_ICON_SIZE;

            int left = (int)Math.Round(x - height / 2);
            int top = (int)Math.Round(y - height / 2);

            LogUtils.LogWarning($"wx_W:{wxSystemInfo.screenWidth}, wx_h:{wxSystemInfo.screenHeight}, w:{Screen.width},h:{Screen.width}, x：{x}, y:{y},left:{left}, top:{top}");

            WXCreateGameClubButtonParam param = new WXCreateGameClubButtonParam();
            param.type = GameClubButtonType.image;
            param.icon = GameClubButtonIcon.light;
            param.image = "https://cs.seayooassets.com/Res/WebGLWX/Common/wxgameclub.png";
            param.style = new GameClubButtonStyle
            {
                left = left,
                top = top,
                width = with,
                height = height,
            };
            clubBtn = WX.CreateGameClubButton(param);
            clubBtn.Show();
        }
        else
        {
            clubBtn.Show();
        }
    }

    /// <summary>
    /// 隐藏微信游戏圈按钮
    /// </summary>
    public void HideGameClubButton()
    {
        //LogUtils.LogWarning("HideGameClubButton");
        if (clubBtn != null)
        {
            clubBtn.Hide();
        }
    }

    /// <summary>
    /// 微信点击事件监听注册,直接覆盖，用minigameTouchEvent区分不同功能的事件
    /// </summary>
    /// <param name="touchEvent">事件枚举</param>
    /// <param name="args">额外参数</param>
    public void AddTouchEventHandler(int touchEvent, params object[] args)
    {
        if (m_touchEvents == null)
        {
            m_touchEvents = new Dictionary<int, object[]>();
        }

        m_touchEvents[touchEvent] = args;

        LogUtils.LogWarningFormat("AddTouchEventHandler type:{0}", touchEvent);
    }

    /// <summary>
    /// 微信点击事件监听移除
    /// </summary>
    public void RemoveTouchEventHandler(int touchEvent)
    {
        if (m_touchEvents.Count <= 0)
            return;

        object[] value = null;
        if (m_touchEvents.TryGetValue(touchEvent, out value))
        {
            m_touchEvents.Remove(touchEvent);
        }
    }

    /// <summary>
    /// 微信触摸事件回调-功能事件
    /// </summary>
    /// <param name="touchEvent"></param>
    int _layerMask = 0;

    private void OnTouchEndEventWithFunc(OnTouchStartListenerResult touchEvent)
    {
        //LogUtils.LogWarning($"OnTouchEndEventWithFunc count:{m_touchEvents.Count}, touchEvent.changedTouches.Length={touchEvent.changedTouches.Length}");
        if (m_touchEvents.Count <= 0)
            return;

        var bFindTouch = false;
        int removeKey = -1; //是否移除

        foreach (var dicEvent in m_touchEvents)
        {
            //LogUtils.LogWarning($" m_touchEvents Key={dicEvent.Key} Value={dicEvent.Value}");
            Collider collider = null;
            if (dicEvent.Value.Length > 0)
            {
                collider = (Collider)dicEvent.Value[0];
            }

            foreach (var wxTouch in touchEvent.changedTouches)
            {
                Vector3 touchPosition = new Vector3(wxTouch.clientX, wxTouch.clientY, 0);
                Ray ray = UICamera.mainCamera.ScreenPointToRay(touchPosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, _layerMask);
                //LogUtils.LogWarning($" OnTouchEndEvent hits：{hits.Length}, touchPosition={touchPosition} wxTouch.pageX={wxTouch.pageX} wxTouch.pageY={wxTouch.pageY}");

                foreach (RaycastHit hit in hits)
                {
                    if (collider != null)
                    {
                        //LogUtils.LogWarningFormat("hit.name{0}, eventButtonName{1}", hit.collider.name, null != collider ? collider.name : "");
                        // 检查每个碰撞点处的物体，以确定点击是否在UI元素上
                        if (hit.collider == collider)
                        {
                            bFindTouch = true;
                            break;
                        }
                    }
                }

                // 判断触摸位置是否在按钮的区域内
                if (bFindTouch)
                {
                    switch (dicEvent.Key)
                    {
                        case defMinigameTouchEvent.agreePrivacyContract:
                            PrivacyAuthorizeResolve(true);
                            break;
                        case defMinigameTouchEvent.disagreePrivacyContract:
                            PrivacyAuthorizeResolve(false);
                            break;
                        case defMinigameTouchEvent.BuildSubscribe:
                        case defMinigameTouchEvent.Subscribe:
                        case defMinigameTouchEvent.AllClickSubscribe:
                            RequestSubscribeMessage();
                            break;
                    }

                    break;
                }
                else
                {
                    if (m_touchEvents.ContainsKey(defMinigameTouchEvent.AllClickSubscribe))
                    {
                        bFindTouch = true;
                        removeKey = defMinigameTouchEvent.AllClickSubscribe;
                        RequestSubscribeMessage();
                    }
                }
            }

            //确认触摸，break
            if (bFindTouch)
                break;
        }

        //只要有点击事件就移除一开始注册的全屏订阅点击
        if (removeKey != -1)
        {
            if (touchEvent.changedTouches.Length > 0)
            {
                if (m_touchEvents != null && m_touchEvents.Count > 0)
                {
                    object[] clickEvent = null;
                    if (m_touchEvents.TryGetValue(removeKey, out clickEvent))
                    {
                        m_touchEvents.Remove(removeKey);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 微信短时间震动(400MS)
    /// </summary>
    public void VibrateLong()
    {
        VibrateLongOption vibateOption = new VibrateLongOption();
        WX.VibrateLong(vibateOption);
    }

    /// <summary>
    /// 复制到剪贴板
    /// </summary>
    public void SetClipboardData(string content)
    {
        //新规需要隐私协议
        //https://developers.weixin.qq.com/community/minigame/doc/000aa25cf1c8a0e64310ac3ef66401
        CheckPrivacyAuthorize((pass, errMsg) =>
        {
            if (pass)
            {
                SetClipboardDataOption clipboardDataOption = new SetClipboardDataOption();
                clipboardDataOption.data = content;
                WX.SetClipboardData(clipboardDataOption);
            }
            else
            {
                LogUtils.LogWarning("SetClipboardData Error");
            }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    public void RestartMiniProgram()
    {
        WX.StorageDeleteAllSync();
        WX.RestartMiniProgram(new RestartMiniProgramOption());
    }

    /// <summary>
    /// 
    /// </summary>
    public void ExitMiniProgram()
    {
        ExitMiniProgramOption callback = new ExitMiniProgramOption();
        WX.ExitMiniProgram(callback);
    }

    /// <summary>
    /// 
    /// </summary>
    public void OpenSettings()
    {
        OpenSettingOption openSetting = new OpenSettingOption();
        WX.OpenSetting(openSetting);
    }

    /// <summary>
    /// 分享被动监听
    /// </summary>
    public void SetupShareParam()
    {
        WXShareAppMessageParam defaultParam = new WXShareAppMessageParam();
        Entity_SharePath.Param entity_SharePath = AssetManager.Instance.asset_sharePath.list[0];
        defaultParam.imageUrl = entity_SharePath.picaddress;
        defaultParam.imageUrlId = entity_SharePath.picId;
        defaultParam.query = string.Format("ac={0}", GameManager.Instance.ServerAccount);
        LogUtils.LogWarningFormat("SetupShareParam {0}", defaultParam.query);

        WX.OnShareAppMessage(defaultParam, (Action<WXShareAppMessageParam> action) =>
        {
            if (action != null)
            {
                action(defaultParam);
            }
        });
    }

    #region 订阅

    // success, errMsg, errCode
    private Action<bool, string, double> _subscriptionsSettingCallback = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public void InitSubscriptionsSettingHooks(Action<bool, string, double> callback)
    {
        _subscriptionsSettingCallback = callback;
    }

    /// <summary>
    /// 订阅消息调起
    /// </summary>
    public void RequestSubscribeMessage()
    {
        LogUtils.LogWarning("RequestSubscribeMessage");

        RequestSubscribeMessageOption param = new RequestSubscribeMessageOption();
        param.fail = (res) => { _subscriptionsSettingCallback?.Invoke(false, res.errMsg, res.errCode); };
        param.complete = (res) => { _subscriptionsSettingCallback?.Invoke(true, res.errMsg, -1); };
        if (AssetManager.Instance.asset_subscribe.list != null && AssetManager.Instance.asset_subscribe.list.Count > 0)
        {
            string[] tmplIds = new string[AssetManager.Instance.asset_subscribe.list.Count];
            for (int i = 0; i < AssetManager.Instance.asset_subscribe.list.Count; i++)
            {
                tmplIds[i] = AssetManager.Instance.asset_subscribe.list[i].template_id;
            }

            param.tmplIds = tmplIds;
        }
        else
        {
            param.tmplIds = new string[] { };
        }

        WX.RequestSubscribeMessage(param);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public void GetSubscriptionsSetting(Action<bool, string, Dictionary<string, string>, bool> callback)
    {
        //LogUtils.LogWarning("GetSubscriptionsSetting");

        GetSettingOption actionSetting = new GetSettingOption();
        actionSetting.withSubscriptions = true;
        actionSetting.success = (res) =>
        {
            if (null != res && null != res.subscriptionsSetting)
            {
                //LogUtils.LogWarning($"WX GetSetting Success mainSwitch:{res.subscriptionsSetting.mainSwitch} itemSettings.count:{res.subscriptionsSetting.itemSettings.Count}");
                callback?.Invoke(true, "", res.subscriptionsSetting.itemSettings, res.subscriptionsSetting.mainSwitch);
            }
            else
            {
                //LogUtils.LogWarning("WX GetSetting Success null == res");
                callback?.Invoke(false, "", null, false);
            }
        };
        actionSetting.fail = (res) =>
        {
            //LogUtils.LogWarning("WX GetSetting Fail");
            callback?.Invoke(false, res.errMsg, null, false);
        };
        WX.GetSetting(actionSetting);
    }

    #endregion

    #region 分享、截图、拍照相关

    private Action<bool, string> _shareOrSaveCompleteAction;

    /// <summary>
    /// 分享固定图
    /// </summary>
    /// <param name="imageUrl"></param>
    /// <param name="imageUrlId"></param>
    public void ShareApp(string imageUrl, string imageUrlId)
    {
        if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(imageUrlId))
        {
            Entity_SharePath.Param entitySharePath = AssetManager.Instance.asset_sharePath.list[0];
            if (entitySharePath != null)
            {
                imageUrl = entitySharePath.picaddress;
                imageUrlId = entitySharePath.picId;
            }
        }

        if (string.IsNullOrEmpty(imageUrlId))
            return;

        // 图片需要提前上传审核
        ShareAppMessageOption samo = new ShareAppMessageOption();
        samo.imageUrl = imageUrl;
        samo.imageUrlId = imageUrlId;
        samo.query = string.Format("ac={0}", GameManager.Instance.ServerAccount);
        LogUtils.LogWarningFormat("ShareApp {0}", samo.query);
        WX.ShareAppMessage(samo);
    }

    /// <summary>
    /// 分享照片（画布截图）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="callback"></param>
    public void ShareImageToAppMessage(int type, Action<bool, string> callback = null)
    {
        defImageWayType wayType = (defImageWayType)type;

        _shareOrSaveCompleteAction = callback;

        SaveOrShareScreenshot(wayType, false);
    }

    /// <summary>
    /// 保存图片到本地（画布截图）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="callback"></param>
    public void SaveImageToGallery(int type, Action<bool, string> callback = null)
    {
        _shareOrSaveCompleteAction = callback;

        //https://developers.weixin.qq.com/community/minigame/doc/000aa25cf1c8a0e64310ac3ef66401
        CheckPrivacyAuthorize((success, errMsg) =>
        {
            if (success)
            {
                defImageWayType wayType = (defImageWayType)type;

                GetSettingOption settingOption = new GetSettingOption();
                settingOption.fail = (result) =>
                {
                    LogUtils.LogWarning($"WX GetSetting fail:{result.errMsg}");

                    DoShareOrSaveCompleteAction(false, result.errMsg);
                };
                settingOption.success = (result) =>
                {
                    LogUtils.LogWarning($"WX.GetSetting success, errMsg:{result.errMsg}");

                    if (null != result.authSetting)
                    {
                        bool value = false;
                        result.authSetting.TryGetValue("scope.writePhotosAlbum", out value);
                        if (value)
                        {
                            SaveOrShareScreenshot(wayType, true);
                        }
                        else
                        {
                            AuthorizeOption authorizeOption = new AuthorizeOption();
                            authorizeOption.scope = "scope.writePhotosAlbum";
                            authorizeOption.fail = (authorizeResult) =>
                            {
                                LogUtils.LogWarning($"WX.Authorize fail:{authorizeResult.errMsg}");

                                DoShareOrSaveCompleteAction(false, authorizeResult.errMsg);
                            };
                            authorizeOption.success = (authorizeResult) =>
                            {
                                LogUtils.LogWarning($"WX.Authorize complete:{authorizeResult.errMsg}");

                                SaveOrShareScreenshot(wayType, true);
                            };

                            WX.Authorize(authorizeOption);
                        }
                    }
                    else
                    {
                        LogUtils.LogWarning($"WX GetSetting success result.authSetting == null");

                        DoShareOrSaveCompleteAction(false);
                    }
                };

                WX.GetSetting(settingOption);
            }
            else
            {
                //fix:解决同一帧，popup不提示问题
                GameManager.Instance.StartCoroutine(ResultEnumerator(false, errMsg));
            }
        });
    }

    System.Collections.IEnumerator ResultEnumerator(bool success, string errMsg = "")
    {
        yield return null;

        DoShareOrSaveCompleteAction(success, errMsg);
    }

    private void SaveOrShareScreenshot(defImageWayType type, bool bSaveToPhotosAlbum)
    {
        var param = GetScreenshotParam(type, bSaveToPhotosAlbum);
        ScreenshotToTempFilePath(param.Item1, param.Item2, param.Item3, param.Item4, bSaveToPhotosAlbum);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="pwidth"></param>
    /// <param name="pheight"></param>
    /// <param name="bSaveToPhotosAlbum"></param>
    public void ScreenshotToTempFilePath(float px, float py, float pwidth, float pheight, bool bSaveToPhotosAlbum = false)
    {
        // https://blog.csdn.net/weixin_42224055/article/details/118526012
        var wxSystemInfo = GetSystemInfoSync();
        var wxWidth = wxSystemInfo.screenWidth * wxSystemInfo.pixelRatio;
        var wxHeight = wxSystemInfo.screenHeight * wxSystemInfo.pixelRatio;
        //LogUtils.LogWarning($"SystemInfo {wxSystemInfo.screenWidth} {wxSystemInfo.screenHeight} {wxSystemInfo.pixelRatio}");

        WXToTempFilePathParam param = new WXToTempFilePathParam();
        param.x = (int)(px * wxWidth);
        param.y = (int)(py * wxHeight);
        param.width = (int)(pwidth * wxWidth);
        param.height = bSaveToPhotosAlbum ? (int)(pheight * wxHeight) : (int)(pwidth * wxWidth);
        param.destWidth = param.width;
        param.destHeight = param.height;
        param.fail = (res) =>
        {
            LogUtils.LogWarning($"WXCanvas.ToTempFilePath Fail: {res.errMsg}, {res.errCode.ToString()}");

            DoShareOrSaveCompleteAction(false, res.errMsg);
        };
        param.success = (res) =>
        {
            LogUtils.LogWarning("WXCanvas.ToTempFilePath success");

            TimerManagerEx.Instance.SetTimer(0.3f, () =>
            {
                if (res != null && !string.IsNullOrEmpty(res.tempFilePath))
                {
                    if (bSaveToPhotosAlbum)
                    {
                        //必须有权限
                        SaveScreenshotToPhotosAlbum(res.tempFilePath);
                    }
                    else
                    {
                        ShareScreenshot(res.tempFilePath);
                    }
                }
            });
        };
        WXCanvas.ToTempFilePath(param);

        LogUtils.LogWarning($"ScreenshotToTempFilePath {param.x}, {param.y}, {param.width}, {param.height}, {param.destWidth}, {param.destHeight}");
    }

    //tempFilePath:图片文件路径，可以是临时文件路径或永久文件路径 (本地路径) ，不支持网络路径
    private void SaveScreenshotToPhotosAlbum(string tempFilePath)
    {
        LogUtils.LogWarning($"SaveScreenshotToPhotosAlbum tempFilePath: {tempFilePath}");

        SaveImageToPhotosAlbumOption param = new SaveImageToPhotosAlbumOption();
        param.filePath = tempFilePath;
        param.fail = (result) =>
        {
            LogUtils.LogWarning($"WX.SaveImageToPhotosAlbum fail errMsg: {result.errMsg}");

            DoShareOrSaveCompleteAction(false, result.errMsg);
        };
        param.success = (result) =>
        {
            LogUtils.LogWarning($"WX.SaveImageToPhotosAlbum success errMsg: {result.errMsg}");

            DoShareOrSaveCompleteAction(true, result.errMsg);
        };

        WX.SaveImageToPhotosAlbum(param);
    }

    private void ShareScreenshot(string tempFilePath)
    {
        var query = string.Format("ac={0}", GameManager.Instance.ServerAccount);
        LogUtils.LogWarning($"ShareScreenshot tempFilePath: {tempFilePath}, query={query}");

        ShareAppMessageOption samo = new ShareAppMessageOption();
        samo.imageUrl = tempFilePath;
        samo.query = query;
        WX.ShareAppMessage(samo);

        DoShareOrSaveCompleteAction(true);
    }

    private void DoShareOrSaveCompleteAction(bool success, string errMsg = "")
    {
        LogUtils.LogWarning($"DoShareOrSaveCompleteAction.Invoke({success}, errMsg:{errMsg});");

        _shareOrSaveCompleteAction?.Invoke(success, errMsg);
        _shareOrSaveCompleteAction = null;
    }

    #endregion

    #region 隐私协议  注意小游戏隐私合规开发指南(用户数据，本地保存，订阅等相关接口，需要隐私协议)

    private bool bPrivacyAuthorizePass = false;
    private Action<bool, string> actionPrivacyAuthorizeNext = null;

    //检测隐私协议是否已经允许，允许继续执行回调，不允许终止执行回调
    //https://developers.weixin.qq.com/community/minigame/doc/000aa25cf1c8a0e64310ac3ef66401
    private void CheckPrivacyAuthorize(Action<bool, string> actionNext)
    {
        actionPrivacyAuthorizeNext = actionNext;

        if (bPrivacyAuthorizePass)
        {
            DoPrivacyAuthorizeNext(true, "");
            return;
        }

        GetPrivacySettingOption param = new GetPrivacySettingOption();
        param.fail = (result) =>
        {
            LogUtils.LogWarning($"WX.GetPrivacySettingOption fail errMsg: {result.errMsg}");
            DoPrivacyAuthorizeNext(false, result.errMsg);
        };
        param.success = (result) =>
        {
            LogUtils.LogWarning($"WX.GetPrivacySettingOption success errMsg: {result.needAuthorization}");

            if (!result.needAuthorization)
            {
                // 隐私协议 已通过
                DoPrivacyAuthorizeNext(true, "");
            }
            else
            {
                // 隐私协议 未通过 发起隐私协议授权，同时监听隐私协议弹窗
                bPrivacyAuthorizePass = false;
                var privacyAuthorizeOption = new RequirePrivacyAuthorizeOption();
                privacyAuthorizeOption.fail = (res) =>
                {
                    LogUtils.LogWarning($"WX.RequirePrivacyAuthorizeOption fail:{res.errMsg}");
                    DoPrivacyAuthorizeNext(false, res.errMsg);
                };
                privacyAuthorizeOption.success = (res) => { LogUtils.LogWarning($"WX.RequirePrivacyAuthorizeOption success errMsg: {res.errMsg}"); };
                WX.RequirePrivacyAuthorize(privacyAuthorizeOption);
            }
        };

        WX.GetPrivacySetting(param);
    }

    private void DoPrivacyAuthorizeNext(bool pass, string msg)
    {
        bPrivacyAuthorizePass = pass;
        if (actionPrivacyAuthorizeNext != null)
        {
            //LogUtils.LogWarning($"DoPrivacyAuthorizeNext.Invoke({pass}, msg:{msg});");
            actionPrivacyAuthorizeNext.Invoke(bPrivacyAuthorizePass, msg);
            actionPrivacyAuthorizeNext = null;
        }
    }

    private void PrivacyAuthorizeResolve(bool agree)
    {
        LogUtils.LogWarning($"PrivacyAuthorizeResolve: agree：{agree}");

        WX.PrivacyAuthorizeResolve(new PrivacyAuthorizeResolveOption()
        {
            eventString = agree ? "agree" : "disagree"
        });

        DoPrivacyAuthorizeNext(agree, agree ? "agree" : "disagree");
    }

    public void OpenPrivacyContract()
    {
        //LogUtils.LogWarning("OpenPrivacyContract");

        OpenPrivacyContractOption openPrivacyContractOption = new OpenPrivacyContractOption();
        openPrivacyContractOption.success = (result) => { LogUtils.LogWarning($"WX.OpenPrivacyContractOption success errMsg: {result.errMsg}"); };
        openPrivacyContractOption.fail = (result) => { LogUtils.LogWarning($"WX.OpenPrivacyContractOption success errMsg: {result.errMsg}"); };
        WX.OpenPrivacyContract(openPrivacyContractOption);
    }

    #endregion

    /// <summary>
    /// 米大师支付
    /// </summary>
    /// <param name="orderID"></param>
    /// <param name="callback"></param>
    public void MidasPay(string orderID, Action<bool, string> callback = null)
    {
        LogUtils.LogWarning($"MidasPay orderID:{orderID}");

        RequestMidasPaymentOption option = new RequestMidasPaymentOption();
        option.outTradeNo = orderID;
        option.mode = "game";
        option.buyQuantity = 10;
        option.env = Utils.IsLogSDK() ? 1 : 0;
        option.offerId = "";
        option.platform = "android";
        option.currencyType = "CNY";
        option.fail = (result) =>
        {
            LogUtils.LogWarning($"WX.RequestMidasPayment fail errMsg: {result.errMsg}");

            callback?.Invoke(true, result.errMsg);
        };
        option.success = (result) =>
        {
            LogUtils.LogWarning($"WX.RequestMidasPayment success errMsg: {result.errMsg}");

            callback?.Invoke(false, result.errMsg);
        };
        WX.RequestMidasPayment(option);
    }

    /// <summary>
    /// 客服消息（ios客服支付）
    /// </summary>
    /// <param name="title">会话内消息卡片路径</param>
    /// <param name="path">会话内消息卡片标题</param>
    /// <param name="sessionFrom">会话来源</param>
    /// <param name="cardImg">会话内消息卡片图片路径</param>
    /// <param name="isCard">是否显示会话内消息卡片</param>
    /// <param name="callback">是否成功，回调结果信息</param>
    public void OpenCustomerServiceConversation(string title, string path, string sessionFrom, string cardImg = "", bool isCard = false, Action<bool, string> callback = null)
    {
        //LogUtils.LogWarning($"OpenCustomerServiceConversation title:{title}, path:{path}, cardImg:{cardImg}, sessionFrom:{sessionFrom}");

        OpenCustomerServiceConversationOption option = new OpenCustomerServiceConversationOption();
        option.sessionFrom = sessionFrom;
        option.showMessageCard = isCard;
        option.sendMessageTitle = title;
        if (!string.IsNullOrEmpty(path))
        {
            option.sendMessagePath = path;
        }

        if (isCard && !string.IsNullOrEmpty(cardImg))
        {
            option.sendMessageImg = cardImg;
        }

        option.fail = (result) =>
        {
            LogUtils.LogWarning($"WX.OpenCustomerServiceConversation fail errMsg: {result.errMsg}");

            callback?.Invoke(true, result.errMsg);
        };
        option.success = (result) =>
        {
            LogUtils.LogWarning($"WX.OpenCustomerServiceConversation success errMsg: {result.errMsg}");

            callback?.Invoke(false, result.errMsg);
        };
        WX.OpenCustomerServiceConversation(option);
    }

    /// <summary>
    /// 微信客服聊天
    /// </summary>
    /// <param name="title">气泡消息标题</param>
    /// <param name="path">气泡消息小程序路径</param>
    /// <param name="cardImg">气泡消息图片</param>
    /// <param name="isCard">是否发送小程序气泡消息</param>
    /// <param name="callback">是否成功，回调结果信息</param>
    public void OpenCustomerServiceChat(string title, string path, string cardImg = "", bool isCard = false, Action<bool, string> callback = null)
    {
        //LogUtils.LogWarning($"OpenCustomerServiceChat title:{title}, path:{path}, cardImg:{cardImg}");

        OpenCustomerServiceChatOption option = new OpenCustomerServiceChatOption();
        option.corpId = "";
        option.extInfo.url = "";
        option.showMessageCard = isCard;
        if (!string.IsNullOrEmpty(title))
        {
            option.sendMessageTitle = title;
        }

        if (!string.IsNullOrEmpty(path))
        {
            option.sendMessagePath = path;
        }

        if (isCard && !string.IsNullOrEmpty(cardImg))
        {
            option.sendMessageImg = cardImg;
        }

        option.fail = (result) =>
        {
            LogUtils.LogWarning($"WX.OpenCustomerServiceChat fail errMsg: {result.errMsg}");

            callback?.Invoke(true, result.errMsg);
        };
        option.success = (result) =>
        {
            LogUtils.LogWarning($"WX.OpenCustomerServiceChat success errMsg: {result.errMsg}");

            callback?.Invoke(false, result.errMsg);
        };
        WX.OpenCustomerServiceChat(option);
    }

    /// <summary>
    /// 设备像素比
    /// </summary>
    /// <returns></returns>
    public double GetPixelRatio()
    {
        var wxSystemInfo = GetSystemInfoSync();
        return wxSystemInfo.pixelRatio;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="bSaveToPhotosAlbum"></param>
    /// <returns></returns>
    private (float, float, float, float) GetScreenshotParam(defImageWayType type, bool bSaveToPhotosAlbum)
    {
        switch (type) //换算：414*896
        {
            case defImageWayType.Normal: //x:37,y:56,w:340,h:726-(0.089,0.096,0.826,0.810)
                return (bSaveToPhotosAlbum ? 0.089f : 0.1f, bSaveToPhotosAlbum ? 0.0625f : 0.266f, bSaveToPhotosAlbum ? 0.821f : 0.8f, bSaveToPhotosAlbum ? 0.810f : 0.64f);
            case defImageWayType.FullScreen:
                return (0.1f, 0.266f, 0.8f, 0.64f);
            case defImageWayType.MiniRoom: //x:37,y:197,w:340,h:444-(0.089,0.220,0.826,0.495)
                return (bSaveToPhotosAlbum ? 0.089f : 0.1f, bSaveToPhotosAlbum ? 0.220f : 0.266f, bSaveToPhotosAlbum ? 0.821f : 0.8f, bSaveToPhotosAlbum ? 0.495f : 0.64f);
            case defImageWayType.KittyCat: //x:88,y:423,w:240,h:168-(0.212,0.505,0.579,0.187)
                return (0.212f, 0.472f, 0.579f, 0.187f);
            default:
                return (0.1f, 0.266f, 0.8f, 0.64f);
        }
    }

    #region 分享到游戏圈

    /// <summary>
    /// 分享图片到游戏圈
    /// </summary>
    /// <param name="url"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <param name="callback"></param>
    public void ShareImageToGameCenter(string url, string title = "", string content = "", Action<bool, int> callback = null)
    {
        ShareImageToGameCenterOption centerOption = new ShareImageToGameCenterOption();
        centerOption.url = url;
        centerOption.title = title;
        centerOption.content = content;
        centerOption.success = (res) =>
        {
            callback?.Invoke(true, 0);
            LogUtils.LogWarning($" ShareImageToGameCenter success res:{JsonUtility.ToJson(res)}");
        };
        centerOption.fail = (res) =>
        {
            callback?.Invoke(false, res.errCode);
            LogUtils.LogWarning($" ShareImageToGameCenter fail res:{JsonUtility.ToJson(res)}");
        };
        ThirdPartyWrapper.Instance.ShareImageToGameCenter(centerOption);
    }

    /// <summary>
    /// 分享截图到游戏圈
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    /// <param name="callback"></param>
    public void ShareCanvasToGameCenter(int type, string title = "", string content = "", Action<bool, int> callback = null)
    {
        var param = GetScreenshotParam((defImageWayType)type, true);
        float px = param.Item1;
        float py = param.Item2;
        float pwidth = param.Item3;
        float pheight = param.Item4;

        var wxSystemInfo = GetSystemInfoSync();
        var wxWidth = wxSystemInfo.screenWidth * wxSystemInfo.pixelRatio;
        var wxHeight = wxSystemInfo.screenHeight * wxSystemInfo.pixelRatio;

        var tarX = (int)(px * wxWidth);
        var tarY = (int)(py * wxHeight);
        var tarWidth = (int)(pwidth * wxWidth);
        var tarHeight = (int)(pheight * wxHeight);

        ShareImageToGameCenterOption centerOption = new ShareImageToGameCenterOption();
        centerOption.x = tarX;
        centerOption.y = tarY;
        centerOption.width = tarWidth;
        centerOption.height = tarHeight;
        centerOption.destWidth = tarWidth;
        centerOption.destHeight = tarHeight;
        centerOption.title = title;
        centerOption.content = content;
        centerOption.success = (res) =>
        {
            callback?.Invoke(true, 0);
            LogUtils.LogWarning($" ShareCanvasToGameCenter success res:{JsonUtility.ToJson(res)}");
        };
        centerOption.fail = (res) =>
        {
            callback?.Invoke(false, res.errCode);
            LogUtils.LogWarning($" ShareCanvasToGameCenter fail res:{JsonUtility.ToJson(res)}");
        };

        ThirdPartyWrapper.Instance.ShareCanvasToGameCenter(centerOption);
    }

    #endregion
}
#endif