using UnityEngine;
using System;
using System.Collections.Generic;
using Engine;
using EngineBase;
using TapSDK.Compliance;
using TapSDK.Core;
using TapSDK.Login;
using TapSDK.Update;

    /// <summary>
    /// TapSDK管理器 - 集成登录、更新唤起、合规认证和数据分析功能
    /// </summary>
    public class TapSDKManager: TSingleton<TapSDKManager>// : MonoBehaviour
    {
        // 登录相关
        private bool _isLoggedIn = false;
        private TapTapAccount _currentUser;

        // 事件回调
        public event Action<bool> OnLoginResult;
        public event Action OnLogoutResult;
        public event Action<bool> OnUpdateCheckResult;
        public event Action<bool> OnComplianceResult;
        
        // 合规认证详细事件回调
        public event Action<int, string> OnComplianceCallback;

        // 是否已通过合规认证检查
        public bool hasCheckedCompliance { get; private set; }
        
        /// <summary>
        /// SDK 初始化
        /// </summary>
        public void InitTapSDK()
        {
            try
            {
                LogUtils.Log("========[Wrapper] 初始化TapSDK=========");
                LogUtils.Log("[Wrapper] 初始化TapSDK");
                
                // 初始化TapSDK管理器
                TapSDKManager.Instance.InitializeTapSDK();
                
                // 注册事件回调
                TapSDKManager.Instance.OnLoginResult += OnTapSDKLoginResult;
                TapSDKManager.Instance.OnLogoutResult += OnTapSDKLogoutResult;
                TapSDKManager.Instance.OnUpdateCheckResult += OnTapSDKUpdateResult;
                TapSDKManager.Instance.OnComplianceResult += OnTapSDKComplianceResult;
                TapSDKManager.Instance.OnComplianceCallback += OnTapSDKComplianceCallback;

                LogUtils.Log("========[Wrapper] TapSDK初始化完成=========");
                LogUtils.Log("[Wrapper] TapSDK初始化完成");
            }
            catch (Exception e)
            {
                LogUtils.LogError($"[Wrapper] TapSDK初始化失败: {e.Message}");
            }
        }

        public void DoDestroy()
        {
            try
            {
                Debug.Log("[TapSDK] 开始销毁TapSDK管理器");
                
                // 先执行正常的登出流程
                TapSDKLogout();

                // 清除用户信息
                ClearUser();
                
                // 强制清理合规模块相关的对象
                CleanupComplianceObjects();
                
                Debug.Log("[TapSDK] TapSDK管理器销毁完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 销毁过程中出现异常: {e.Message}");
            }
        }
        
        /// <summary>
        /// 强制清理合规模块相关的对象
        /// </summary>
        private void CleanupComplianceObjects()
        {
            try
            {
                // 查找并销毁CompliancePoll对象
                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.Contains("CompliancePoll") || obj.name.Contains("Compliance"))
                    {
                        Debug.Log($"[TapSDK] 清理合规模块对象: {obj.name}");
                        UnityEngine.Object.DestroyImmediate(obj);
                    }
                }
                
                // 清理可能存在的其他TapSDK相关对象
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.Contains("TapSDK") || obj.name.Contains("TapTap"))
                    {
                        Debug.Log($"[TapSDK] 清理TapSDK对象: {obj.name}");
                        UnityEngine.Object.DestroyImmediate(obj);
                    }
                }
                
                // 强制垃圾回收
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
                
                // 清理Unity资源
                Resources.UnloadUnusedAssets();
                
                Debug.Log("[TapSDK] 内存清理完成");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[TapSDK] 清理合规模块对象时出现异常: {e.Message}");
            }
        }
        
        /// <summary>
        /// 初始化TapSDK
        /// </summary>
        public async void InitializeTapSDK()
        {
            try
            {
                Debug.Log("[TapSDK] 开始初始化TapSDK");

                // 核心配置 // TapSDK配置
                TapTapSdkOptions coreOptions = new TapTapSdkOptions
                {
                    clientId = "qak8cszqpofin72ti9",    // 客户端 ID，开发者后台获取
                    clientToken = "ler4dw39339oFmfj3jFmTK6bVcTFDleMs2dvhb61",  // 客户端令牌，开发者后台获取
                    region = TapTapRegionType.CN,    //根据您的目标市场设置正确的地区（CN为国内，Overseas为海外）
                    preferredLanguage = TapTapLanguageType.zh_Hans,   // 语言，默认为 Auto，默认情况下，国内为 zh_Hans，海外为 en
                    enableLog = true,    // 是否开启日志，Release 版本请设置为 false
                    gameVersion = VersionManager.Instance.GetResVersion()  // 游戏版本号，如果不传则默认读取应用的版本号
                };
                
                // 合规配置
                TapTapComplianceOption complianceOption = new TapTapComplianceOption
                {
                    showSwitchAccount = true,  // 是否显示切换账号选项
                    useAgeRange = true   // 是否使用年龄范围功能
                };
                
                // 数据分析配置
                TapTapEventOptions eventOptions = new TapTapEventOptions
                {
                    enableTapTapEvent = true,  // 启用数据分析功能
                    channel = "Taptap",  // 渠道信息 - 使用真实的游戏名称
                    enableAutoIAPEvent = true,  // 启用自动IAP事件上报
                    disableAutoLogDeviceLogin = false  // 不禁用自动设备登录事件
                };
                
                // 初始化核心SDK，包含合规配置和数据分析配置
                TapTapSDK.Init(coreOptions, new TapTapSdkBaseOptions[] { complianceOption, eventOptions });

                Debug.Log("[TapSDK] TapSDK初始化完成");

                // 注册合规认证回调
                try
                {
                    TapTapCompliance.RegisterComplianceCallback(OnComplianceEvent);
                    Debug.Log("[TapSDK] 合规认证回调已注册，等待用户登录后进行合规检查");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[TapSDK] 注册合规认证回调失败: {e.Message}");
                }

                TapTapAccount account = null;
                try
                {
                    // 检查本地是否已存在 account 信息
                    account = await TapTapLogin.Instance.GetCurrentTapAccount();
                }
                catch (Exception e)
                {
                    Debug.Log("本地无有效用户信息");
                }
                
                // 本地存在用户信息且未通过合规认证时进行合规认证检查
                if (account != null && !hasCheckedCompliance)
                {
                    StartCheckCompliance();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 初始化失败: {e.Message}");
            }
        }

        /// <summary>
        /// 处理合规认证事件回调
        /// </summary>
        private void OnComplianceEvent(int code, string message)
        {
            Debug.Log($"[TapSDK] 合规认证事件: code={code}, message={message}");
            
            // 触发详细事件回调
            OnComplianceCallback?.Invoke(code, message);
            //UIManager.Instance.Toast($"[TapSDK] 合规认证事件: code={code}, message={message}");
            // 根据事件代码处理不同情况
            switch (code)
            {
                case 500: // LOGIN_SUCCESS - 登录成功
                    Debug.Log("[TapSDK] 合规认证成功");
                    hasCheckedCompliance = true;
                    OnComplianceResult?.Invoke(true);
                    break;
                    
                case 1000: // EXITED - 用户登出
                    Debug.Log("[TapSDK] 用户登出");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 1001: // SWITCH_ACCOUNT - 切换账号
                    Debug.Log("[TapSDK] 用户切换账号");
                    //UIManager.Instance.Toast("切换账号");
                    TapSDKLogout();
                    TapSDKLogin();
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 1030: // PERIOD_RESTRICT - 当前用户达到宵禁时长
                    Debug.Log("[TapSDK] 用户达到宵禁时长");
                    UIManager.Instance.Toast("达到宵禁时长");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 1050: // DURATION_LIMIT - 时长限制
                    Debug.Log("[TapSDK] 用户达到时长限制");
                    UIManager.Instance.Toast("达到时长限制");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 1100: // AGE_LIMIT - 适龄限制
                    Debug.Log("[TapSDK] 用户年龄限制");  // 您当前未满足本游戏最低年龄要求，请退出游戏！
                    UIManager.Instance.Toast("您当前未满足本游戏最低年龄要求，请退出游戏！");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 1200: // INVALID_CLIENT_OR_NETWORK_ERROR - 应用配置错误或网络异常
                    Debug.Log("[TapSDK] 合规认证配置错误或网络异常");  // 当前应用信息或网络连接异常，请检查后重试！
                    UIManager.Instance.Toast("当前应用信息或网络连接异常，请检查后重试！");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                case 9002: // REAL_NAME_STOP - 实名过程中点击了关闭实名窗
                    Debug.Log("[TapSDK] 用户取消实名认证");
                    OnComplianceResult?.Invoke(false);
                    break;
                    
                default:
                    Debug.LogWarning($"[TapSDK] 未知的合规认证事件代码: {code}");
                    OnComplianceResult?.Invoke(false);
                    break;
            }
        }

        /// <summary>
        /// 检查合规认证
        /// </summary>
        public void StartCheckCompliance()
        {
            try
            {
                Debug.Log("[TapSDK] 开始合规认证检查");

                // 检查是否已初始化
                if (!IsTapSDKInitialized())
                {
                    Debug.LogError("[TapSDK] TapSDK未初始化，无法进行合规检查");
                    OnComplianceResult?.Invoke(false);
                    return;
                }

                // 检查是否已登录
                if (!_isLoggedIn || _currentUser == null)
                {
                    Debug.LogError("[TapSDK] 用户未登录，无法进行合规检查");
                    OnComplianceResult?.Invoke(false);
                    return;
                }

                string userId = _currentUser.openId;
                if (string.IsNullOrEmpty(userId))
                {
                    Debug.LogError("[TapSDK] 用户ID为空，无法进行合规检查");
                    OnComplianceResult?.Invoke(false);
                    return;
                }
                
                Debug.Log($"[TapSDK] 使用用户ID进行合规认证: {userId}");
                
                // 启动合规检查
                TapTapCompliance.Startup(userId);
                
                // 获取年龄范围
                // TapTapCompliance.GetAgeRange().ContinueWith(task =>
                // {
                //     if (task.IsCompletedSuccessfully)
                //     {
                //         int ageRange = task.Result;
                //         Debug.Log($"[TapSDK] 年龄范围: {ageRange}");
                //         
                //         // 根据年龄范围判断是否合规
                //         bool isCompliant = ageRange >= 0; // 年龄范围有效表示合规
                //         OnComplianceResult?.Invoke(isCompliant);
                //     }
                //     else
                //     {
                //         Debug.LogError("[TapSDK] 合规检查失败");
                //         OnComplianceResult?.Invoke(false);
                //     }
                // });
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 合规检查失败: {e.Message}");
                OnComplianceResult?.Invoke(false);
            }
        }

        #region 登录

        /// <summary>
        /// 登录
        /// </summary>
        public async void TapSDKLogin()
        {
            try
            {
                Debug.Log("[TapSDK] 开始登录");

                // 检查是否已经登录
                if (_isLoggedIn)
                {
                    Debug.Log("[TapSDK] 用户已经登录");
                    OnLoginResult?.Invoke(true);
                    return;
                }

                // 执行登录  // TAP_LOGIN_SCOPE_BASIC_INFO  只获得 openId 和 unionId 登录更快捷  // TAP_LOGIN_SCOPE_PUBLIC_PROFILE  获得 openId、unionId、用户昵称、用户头像
                var account = await TapTapLogin.Instance.LoginWithScopes(new string[] { TapTapLogin.TAP_LOGIN_SCOPE_BASIC_INFO }); 
                if (account != null)
                {
                    _isLoggedIn = true;
                    _currentUser = account;
                    Debug.Log($"[TapSDK] 登录成功，用户ID: {account.openId}");
                    OnLoginResult?.Invoke(true);
                    
                    // 登录成功后进行合规检查
                    StartCheckCompliance();
                }
                else
                {
                    Debug.LogError("[TapSDK] 登录失败");
                    OnLoginResult?.Invoke(false);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 登录异常: {e.Message}");
                OnLoginResult?.Invoke(false);
            }
        }

        /// <summary>
        /// 检查TapSDK是否已初始化
        /// </summary>
        private bool IsTapSDKInitialized()
        {
            try
            {
                // 检查核心SDK是否已初始化 - 通过检查配置选项来判断
                if (TapTapSDK.taptapSdkOptions == null)
                {
                    return false;
                }

                // 检查合规模块是否已初始化 - 使用try-catch来安全检查
                try
                {
                    // 尝试调用合规模块的方法来检查是否已初始化
                    // 如果合规模块未初始化，调用任何方法都会抛出异常
                    TapTapCompliance.GetAgeRange();
                    return true;
                }
                catch
                {
                    // 如果出现异常，说明合规模块未初始化
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[TapSDK] 检查初始化状态时出现异常: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 检查是否正在退出应用
        /// </summary>
        private bool IsApplicationQuitting()
        {
            return Application.isPlaying == false || Application.isEditor == false;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void TapSDKLogout()
        {
            try
            {
                Debug.Log("[TapSDK] 开始登出");

                // 先登出登录模块
                if (TapTapLogin.Instance != null)
                {
                    TapTapLogin.Instance.Logout();
                }
                
                // 检查是否正在退出应用，如果是则跳过API调用
                // if (IsApplicationQuitting())
                // {
                //     Debug.Log("[TapSDK] 应用正在退出，跳过API调用，直接清理本地状态");
                //     CleanupLocalState();
                //     return;
                // }
                
                // 检查合规模块是否已初始化，如果已初始化则调用Exit
                try
                {
                    if (IsTapSDKInitialized())
                    {
                        
                        TapTapCompliance.Exit();
                        Debug.Log("[TapSDK] 合规模块登出成功");
                    }
                    else
                    {
                        Debug.Log("[TapSDK] TapSDK未完全初始化，跳过合规模块Exit调用");
                    }
                }
                catch (Exception complianceException)
                {
                    Debug.LogWarning($"[TapSDK] 合规模块登出异常（非致命）: {complianceException.Message}");
                }

                // 清理本地状态
                CleanupLocalState();
                
                Debug.Log("[TapSDK] 登出成功");
                OnLogoutResult?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 登出异常: {e.Message}");
                // 即使出现异常，也要清理本地状态
                CleanupLocalState();
                OnLogoutResult?.Invoke();
            }
        }
        
        /// <summary>
        /// 清理本地状态
        /// </summary>
        private void CleanupLocalState()
        {
            _isLoggedIn = false;
            _currentUser = null;
            hasCheckedCompliance = false;
        }

        #endregion

        #region 更新唤起

        /// <summary>
        /// 检查更新    唤醒更新方式 1、开发者中心配置更新  适合：无自有版本管理系统的游戏，特别是单机游戏。
        /// </summary>
        public void TapSDKCheckUpdate()
        {
            try
            {
                Debug.Log("[TapSDK] 开始检查更新");

                // 检查强制更新
                TapTapUpdate.CheckForceUpdate();
                
                // 这里可以根据需要添加更多更新逻辑
                OnUpdateCheckResult?.Invoke(false);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 检查更新异常: {e.Message}");
                OnUpdateCheckResult?.Invoke(false);
            }
        }

        /// <summary>
        /// 执行游戏更新   唤醒更新方式 2、游戏自行判断更新  适合：自有版本管理系统，期望更灵活触发、展示更新的网游。
        /// </summary>
        public void TapSDKUpdateGame()
        {
            try
            {
                Debug.Log("[TapSDK] 开始执行游戏更新");

                // 执行游戏更新，传入取消回调
                TapTapUpdate.UpdateGame(() =>
                {
                    Debug.Log("[TapSDK] 用户取消了游戏更新");
                    OnUpdateCheckResult?.Invoke(false);
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 执行游戏更新异常: {e.Message}");
                OnUpdateCheckResult?.Invoke(false);
            }
        }

        #endregion
        

        #region 数据分析

        /// <summary>
        /// 数据分析 - 记录事件
        /// </summary>
        public void TapSDKTrackEvent(string eventName, Dictionary<string, object> properties = null)
        {
            try
            {
                if (properties == null)
                {
                    properties = new Dictionary<string, object>();
                }

                // 添加用户信息
                if (_isLoggedIn && _currentUser != null)
                {
                    properties["user_id"] = _currentUser.openId;
                    properties["user_name"] = _currentUser.name;
                }

                // 将Dictionary转换为JSON字符串
                string propertiesJson = Newtonsoft.Json.JsonConvert.SerializeObject(properties);
                
                // 记录事件
                TapTapEvent.LogEvent(eventName, propertiesJson);
                Debug.Log($"[TapSDK] 记录事件: {eventName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 记录事件失败: {e.Message}");
            }
        }

        /// <summary>
        /// 数据分析 - 设置用户ID
        /// </summary>
        public void SetUserID(string userID)
        {
            try
            {
                // 记录登录事件
                // TapSDKManager.Instance.TrackLogin("taptap");
                
                TapTapEvent.SetUserID(userID);
                Debug.Log($"[TapSDK] 设置用户ID成功: {userID}");
                
                 var properties = new Dictionary<string, object>
                 {
                     ["login_method"] = "taptap",
                     ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                 };
                TapSDKTrackEvent("user_login", properties);
                
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 设置用户ID失败: {e.Message}");
            }
        }

        /// <summary>
        /// 数据分析 - 设置用户ID和属性
        /// </summary>
        public void SetUserID(string userID, Dictionary<string, object> properties)
        {
            try
            {
                string propertiesJson = Newtonsoft.Json.JsonConvert.SerializeObject(properties);
                TapTapEvent.SetUserID(userID, propertiesJson);
                Debug.Log($"[TapSDK] 设置用户ID和属性成功: {userID}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 设置用户ID和属性失败: {e.Message}");
            }
        }

        /// <summary>
        /// 数据分析 - 清除用户信息
        /// </summary>
        public void ClearUser()
        {
            try
            {
                TapTapEvent.ClearUser();
                Debug.Log("[TapSDK] 清除用户信息成功");
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 清除用户信息失败: {e.Message}");
            }
        }

        /// <summary>
        /// 数据分析 - 记录用户登录
        /// </summary>
        public void TrackLogin(string method = "taptap")
        {
            var properties = new Dictionary<string, object>
            {
                ["login_method"] = method,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            TapSDKTrackEvent("user_login", properties);
        }

        /// <summary>
        /// 数据分析 - 记录用户注册
        /// </summary>
        public void TrackRegister(string method = "taptap")
        {
            var properties = new Dictionary<string, object>
            {
                ["register_method"] = method,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            TapSDKTrackEvent("user_register", properties);
        }

        /// <summary>
        /// 数据分析 - 记录游戏开始
        /// </summary>
        public void TrackGameStart()
        {
            var properties = new Dictionary<string, object>
            {
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            TapSDKTrackEvent("game_start", properties);
        }

        /// <summary>
        /// 数据分析 - 记录游戏结束
        /// </summary>
        public void TrackGameEnd(int duration = 0)
        {
            var properties = new Dictionary<string, object>
            {
                ["duration"] = duration,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            TapSDKTrackEvent("game_end", properties);
        }
        
        /// <summary>
        /// 数据分析 - 上报自定义数据
        /// </summary>
        public void ReportTapSDKEventData(string eventName, string data)
        {
            var properties = new Dictionary<string, object>
            {
                ["#custom2"] = data,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            TapSDKTrackEvent(eventName, properties);
        }
        
        #endregion

        #region 充值和上报   暂时没接支付不用
        
        /// <summary>
        /// 开始充值流程，先通过防沉迷模块检查充值是否受限，当无限制时，继续后续充值流程，否则提示充值限制
        /// </summary>
        /// <param name="amount"> 充值金额，单位：分 </param>
        public void CheckCharge(float amount, Action<bool> callback)
        {
            try
            {
                // 检查是否已初始化
                if (!IsTapSDKInitialized())
                {
                    Debug.LogError("[TapSDK] TapSDK未初始化，无法进行充值检查");
                    callback?.Invoke(false);
                    return;
                }

                TapTapCompliance.CheckPaymentLimit((long)amount, (result) =>
                {
                    int status = result.status;
                    // 当前充值不受限
                    if (status == 1)
                    {
                        // TODO: 完成后续充值流程
                        Debug.Log("可以正常充值");
                        callback?.Invoke(true);
                        // 默认为充值成功,显示充值结果
                        // ShowPayResult(true, null);
                        // //上报充值金额
                        // SubmitPayResult(amount);
                    }
                    else // 充值受限
                    {
                        Debug.Log("当前充值受限");
                        callback?.Invoke(false);
                    }
                }, (exception) =>
                {
                    Debug.Log("当前网络异常，请稍后重试");
                    callback?.Invoke(false);
                    //ShowPayResult(false, "当前网络异常，请稍后重试");
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 充值检查异常: {e.Message}");
                callback?.Invoke(false);
            }
        }
        
        // 上报付费重试次数
        private int TryTimes = 0;

        // 最大重试次数
        private const int MaxTryTimes = 3;
        /// <summary>
        /// 上报充值
        /// </summary>
        /// <param name="amount"></param>
        public void SubmitPayResult(float amount)
        {
            try
            {
                // 检查是否已初始化
                if (!IsTapSDKInitialized())
                {
                    Debug.LogError("[TapSDK] TapSDK未初始化，无法上报充值结果");
                    return;
                }

                TapTapCompliance.SubmitPayment((long)amount, () =>
                {
                    // 上报成功
                    TryTimes = 0;
                    Debug.Log("[TapSDK] 充值结果上报成功");
                }, (exception) =>
                {
                    Debug.LogError($"[TapSDK] 充值结果上报失败: {exception}");
                    // 进行重试
                    if (TryTimes < MaxTryTimes)
                    {
                        TryTimes++;
                        Debug.Log($"[TapSDK] 充值结果上报重试第{TryTimes}次");
                        SubmitPayResult(amount);
                    }
                    else
                    {
                        Debug.LogError("[TapSDK] 充值结果上报重试次数已达上限");
                        TryTimes = 0; // 重置重试次数
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"[TapSDK] 充值结果上报异常: {e.Message}");
            }
        }
        
        #endregion
        
        /// <summary>
        /// 检查是否已登录
        /// </summary>
        public bool IsLoggedIn()
        {
            if (_isLoggedIn && hasCheckedCompliance)  //登录并且  合规认证通过
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        public TapTapAccount GetCurrentUser()
        {
            return _currentUser;
        }
        
        /// <summary>
        /// 获取用户ID
        /// </summary>
        public string GetUserId()
        {
            return _currentUser?.openId ?? "";
        }

        /// <summary>
        /// 获取用户昵称
        /// </summary>
        public string GetUserName()
        {
            return _currentUser?.name ?? "";
        }

        /// <summary>
        /// 获取用户头像
        /// </summary>
        public string GetUserAvatar()
        {
            return _currentUser?.avatar ?? "";
        }
        
        /// <summary>
        /// TapSDK登录结果回调
        /// </summary>
        private void OnTapSDKLoginResult(bool success)
        {
            if (success)
            {
                LogUtils.Log("[Wrapper] TapSDK登录成功");
                OnLoginSuccess();

                SetUserID(GetUserId());
            }
            else
            {
                LogUtils.LogError("[Wrapper] TapSDK登录失败");
            }
        }

        private void OnLoginSuccess()
        {
            if (_isLoggedIn && hasCheckedCompliance)
            {
                Debug.Log("=======OnLoginSuccess  通知进入游戏=======");
                EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_TAP_SDK_LOGIN_SUCCESS);
            }
        }
        
        /// <summary>
        /// TapSDK登出结果回调
        /// </summary>
        private void OnTapSDKLogoutResult()
        {
            LogUtils.Log("[Wrapper] TapSDK登出成功");
        }
        
        /// <summary>
        /// TapSDK更新检查结果回调
        /// </summary>
        private void OnTapSDKUpdateResult(bool hasUpdate)
        {
            if (hasUpdate)
            {
                LogUtils.Log("[Wrapper] TapSDK发现可用更新");
            }
            else
            {
                LogUtils.Log("[Wrapper] TapSDK没有可用更新");
            }
        }
        
        /// <summary>
        /// TapSDK合规检查结果回调
        /// </summary>
        private void OnTapSDKComplianceResult(bool compliant)
        {
            if (compliant)
            {
                OnLoginSuccess();
                LogUtils.Log("[Wrapper] TapSDK合规检查通过");
            }
            else
            {
                LogUtils.LogWarning("[Wrapper] TapSDK合规检查失败");
            }
        }

        private void OnTapSDKComplianceCallback(int code, string message)
        {
            LogUtils.Log($"[Wrapper] TapSDK合规认证事件: code={code}, message={message}");
        }
    }