#if !UNITY_EDITOR && UNITY_ANDROID
#define USE_ANDROID
#endif

#if !UNITY_EDITOR && UNITY_IOS
#define USE_IOS
#endif

#if !UNITY_EDITOR && PF_WEIXIN
#define USE_WEIXIN
#endif

#if !UNITY_EDITOR && PF_DOUYIN
#define USE_DOUYIN
#endif

using System;
using UnityEngine;
using System.Collections;
#if USE_WEIXIN
using WeChatWASM;
#endif

#if USE_DOUYIN
using StarkSDKSpace;
#endif

using EngineBase;

namespace Engine
{
    public enum Z_ShowAdStatus
    {
        Start, //开始显示广告
        Clicked, //广告被点击
        Closed, //广告被关闭
        Rewarded, //广告播放完成,发放奖励
    }
    
#if USE_DOUYIN 
    public class DouyinVideoAd : StarkAdManager.VideoAdCallback
    {
        /// <summary>广告加载成功</summary>
        public void OnVideoLoaded()
        {
            LogUtils.LogWarning("OnVideoLoaded success");
            //VideoAdsHelper.Instance._isLoading = false;
            //GoogleMobileAdsManager.Instance.OnPreloadAdFinish();
        }

        /// <summary>广告播放成功，Unity测触发该回调时间不准确，具体触发Show的时间参考返回毫秒时间戳</summary>
        public void OnVideoShow(long timestamp)
        {
            LogUtils.LogWarning($"OnVideoShow timestamp:{timestamp}");
            //GoogleMobileAdsManager.Instance.OnShowAdResult(Z_ShowAdStatus.Start, "", "");
        }

        /// <summary>广告失败，错误码同 ShowVideoAd</summary>
        public void OnError(int errCode, string errorMessage)
        {
            LogUtils.LogWarning($"OnVideoLoaded errCode:{errCode}, errMsg:{errorMessage}");
            //VideoAdsHelper.Instance._isLoading = false;
            //GoogleMobileAdsManager.Instance.OnPreloadAdError(errorMessage, errCode);
        }

        /// <summary>广告关闭回调</summary>
        /// <param name="watchedTime">已播放时长.</param>
        /// <param name="effectiveTime">有效播放时长(超过此时长可以授予激励).</param>
        /// <param name="duration">视频总时长.</param>
        public void OnVideoClose(int watchedTime, int effectiveTime, int duration)
        {
            LogUtils.LogWarning($"OnVideoClose watchedTime:{watchedTime}, effectiveTime:{effectiveTime}, duration:{duration}");
        }
    }
#endif

    public class VideoAdsHelper : TSingleton<VideoAdsHelper>
    {
        internal bool _isLoading = false;
        private string _placementId = "";

#if USE_WEIXIN
        private WXRewardedVideoAd _adsWraper = null;

        private void OnWxAdLoad(WXADLoadResponse res)
        {
            LogUtils.LogWarning($"OnPreloadAdResult-A: success, res:{JsonUtility.ToJson(res)}");

            _isLoading = false;
            //GoogleMobileAdsManager.Instance.OnPreloadAdFinish(res.errMsg);
        }

        private void OnWxAdError(WXADErrorResponse err)
        {
            LogUtils.LogWarning($"OnPreloadAdResult-A: fail, err:{JsonUtility.ToJson(err)}");

            _isLoading = false;
            //GoogleMobileAdsManager.Instance.OnPreloadAdError(err.errMsg, err.errCode);
        }

        private void OnWxAdClose(WXRewardedVideoAdOnCloseResponse res)
        {
            LogUtils.LogWarning($"[RewardedVideoAd] OnShowAdResult-A: close res:{JsonUtility.ToJson(res)}");

            if (res.isEnded)
            {
                OnShowWxAdResult(Z_ShowAdStatus.Rewarded, res.errMsg);
            }
            else
            {
                OnShowWxAdResult(Z_ShowAdStatus.Closed, res.errMsg);
            }
        }

        private void OnShowWxAdResult(Z_ShowAdStatus status, string errMsg)
        {
            LogUtils.LogWarning($"[RewardedVideoAd] OnShowAdResult-A: success, status:{status}, errMsg:{errMsg}");

            //GoogleMobileAdsManager.Instance.OnShowAdResult(status, "", errMsg);
        }

        private void OnShowWxAdError(string errMsg)
        {
            LogUtils.LogWarning($"[RewardedVideoAd] OnShowAdResult-A: fail, errMsg:{errMsg}");

            //GoogleMobileAdsManager.Instance.OnShowAdError(errMsg);
        }
#endif
        
        
#if USE_DOUYIN
        private DouyinVideoAd _douyinVideoAd = new DouyinVideoAd();

        private void OnCloseDouyinVideoAd(bool isEnded)
        {
            LogUtils.LogWarning($"OnCloseDouyinVideoAd isEnded:{isEnded}");
            //GoogleMobileAdsManager.Instance.OnShowAdResult(isEnded ? Z_ShowAdStatus.Rewarded : Z_ShowAdStatus.Closed, "", "");
        }
        
        private void OnErrorDouyinVideoAd(int errCode, string errMsg)
        {
            LogUtils.LogWarning($"OnErrorDouyinVideoAd errCode:{errCode}, errMsg:{errMsg}");
            //GoogleMobileAdsManager.Instance.OnShowAdError(errMsg);
        }
#endif
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="call"></param>
        public void DoInitAds(Action call)
        {
            if (SDKInterface.Instance.IsNormalPlatform())
                return;
#if USE_WEIXIN
            if (!SDKInterface.Instance.IsMinigamePlatform(MinigamePlatform.devtools))
            {
                if (string.IsNullOrEmpty(PlacementId))
                    return;

                _adsWraper = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam() { adUnitId = PlacementId }); //自己申请广告单元ID
                _adsWraper.OnError(this.OnWxAdError);
                _adsWraper.OnLoad(this.OnWxAdLoad);
                _adsWraper.OnClose(this.OnWxAdClose);
            }
#elif USE_DOUYIN
            _douyinVideoAd = new DouyinVideoAd();
#endif
            
            call?.Invoke();
        }

        /// <summary>
        /// 广告加载
        /// </summary>
        public void DoStartPreloadAds()
        {
            _isLoading = true;
#if USE_WEIXIN
            _adsWraper?.Load();
#elif USE_DOUYIN

#endif
        }

        /// <summary>
        /// 广告展示
        /// </summary>
        public void DoStartShowAds()
        {
#if USE_WEIXIN
            _adsWraper?.Show((res) => { OnShowWxAdResult(Z_ShowAdStatus.Start, res.errMsg); }, (res) => { OnShowWxAdError(res.errMsg); });
#elif USE_DOUYIN
            //if (string.IsNullOrEmpty(PlacementId))
            //{
            //    LogUtils.LogWarning("placementId is null");
            //    return;
            //}
            StarkSDK.API.GetStarkAdManager().ShowVideoAdWithId(PlacementId, OnCloseDouyinVideoAd, OnErrorDouyinVideoAd, _douyinVideoAd);
#endif
        }

        /// <summary>
        /// 加载中
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
        }

        /// <summary>
        /// 广告位ID
        /// </summary>
        private string PlacementId
        {
            get
            {
                return _placementId;
            }
        }

        #region fix:为解决TopOn广告奖励事件时序性

        private Action _delayClearInvoke = null;
        private Coroutine _delayCoroutine = null;

        private IEnumerator DoDelayActionCoroutine(WaitForSeconds _wait)
        {
            yield return _wait;

            _delayClearInvoke?.Invoke();
            _delayClearInvoke = null;
        }

        public void DoStopDelayInvoke()
        {
#if PF_WEIXIN
#else
            if (null != _delayCoroutine)
            {
                GameManager.Instance.StopCoroutine(_delayCoroutine);
                _delayCoroutine = null;
            }

            _delayClearInvoke?.Invoke();
            _delayClearInvoke = null;
#endif
        }

        public void DoDelayInvoke(Action action, WaitForSeconds wait = null)
        {
#if PF_WEIXIN
            action?.Invoke();
#else
            DoStopDelayInvoke();

            _delayClearInvoke = action;
            _delayCoroutine = GameManager.Instance.StartCoroutine(DoDelayActionCoroutine(wait)); //微延迟
#endif
        }

        #endregion
    }
}