//数据分析打点
#define USE_ANALYTIC_TRACK
using System;
using System.Collections.Generic;
using EngineBase;

namespace Engine
{
    public class DataAnalyticsWrapper : TSingleton<DataAnalyticsWrapper>
    {
        /// <summary>
        /// sdk登入
        /// </summary>
        public void OnLogin()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// sdk登入成功
        /// </summary>
        public void OnLoginSuccess()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// sdk登入失败
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="code"></param>
        public void OnLoginFail(string errMsg, int code = 0)
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnLogout()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnLogoutSuccess()
        {
#if USE_ANALYTIC_TRACK

#endif
        }

        /// <summary>
        /// Http登录
        /// </summary>
        public void OnLoginHttpServer()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// Http登录完成
        /// </summary>
        public void OnLoginHttpServerDone()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// Http登录失败
        /// </summary>
        /// <param name="code"></param>
        public void OnLoginHttpServerFail(int code)
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }


        /// <summary>
        /// 登入大厅服
        /// </summary>
        /// <param name="roleId">游戏方用户Id</param>
        /// <param name="account">SDK方账号id，ComboId</param>
        /// <param name="createTime">0不是新创角，1新创角</param>
        public void OnLoginLobbyServer(string roleId, string account, int createTime)
        {
#if USE_ANALYTIC_TRACK
            try
            {

            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 购买事件
        /// </summary>
        /// <param name="strProductID"></param>
        /// <param name="goodsID"></param>
        /// <param name="orderID"></param>
        public void OnPay(string strProductID, int goodsID, string orderID)
        {

#if USE_ANALYTIC_TRACK
            try
            {

            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cpOrderId"></param>
        /// <param name="extraParam"></param>
        public void OnPaySuccess(string orderId, string cpOrderId, string extraParam)
        {
#if USE_ANALYTIC_TRACK

#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cpOrderId"></param>
        /// <param name="extraParam"></param>
        public void OnPayCancel(string orderId, string cpOrderId, string extraParam)
        {
#if USE_ANALYTIC_TRACK

#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cpOrderId"></param>
        /// <param name="extraParam"></param>
        public void OnPayFailed(string orderId, string cpOrderId, string extraParam)
        {
#if USE_ANALYTIC_TRACK

#endif
        }

        /// <summary>
        /// 进入大世界事件
        /// </summary>
        public void OnEnterGame()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 回到游戏
        /// </summary>
        public void OnReturnGame()
        {
#if USE_ANALYTIC_TRACK
            try
            {
            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdateGold()
        {
#if USE_ANALYTIC_TRACK
            try
            {

            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdateGem()
        {
#if USE_ANALYTIC_TRACK
            try
            {

            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnUpdateEnergy()
        {
#if USE_ANALYTIC_TRACK
            try
            {

            }
            catch (Exception e)
            {
                LogUtils.LogError(e.Message);
            }
#endif
        }
    }
}