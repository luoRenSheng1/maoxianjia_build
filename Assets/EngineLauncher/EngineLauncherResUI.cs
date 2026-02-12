using FairyGUI;
using System;
using ThirdParty;
using UnityEngine;

namespace Engine
{
    // 游戏启动资源加载管理器
    public class EngineLauncherResUI : MonoBehaviour
    {
        public enum TIP_KEY
        {
            KEY_INITSDK = 1,
            KEY_DECOMPRESS = 2,
            KEY_DECOMPRESS_FAIL = 3,
            KEY_UPDATE_RESOURCE = 4,
            KEY_UPDATE_RESOURCE_SUCCESS = 5,
            KEY_DOWNLOADSYNC = 6,
            KEY_COMPRESS_RESOURCE = 7,
            KEY_COMPRESS_RESOURCE_FAIL = 8,

            KEY_TIP = 101,
            KEY_OK = 102,
            KEY_QUIT = 103,
            KEY_BIGVERSION_TIP = 104,
            KEY_UPDATEFAIL_TIP = 105,
            KEY_DOWNLOAD_TIP = 106,
            KEY_LOADCONFIGFAIL_TIP = 107,
            KEY_MAINTAIN = 108,
            KEY_MAINTAIN_TIME_TIP = 109,
            KEY_REFRESH = 110,
            KEY_LOGINBOX = 111,
            KEY_UPDATE_AWARD = 112,
            KEY_MAINTAIN_URL_TITLE = 113,
            KEY_APP_VERSION = 114,
            KEY_RES_VERSION = 115,
        }

        private const string C_RESPACKAGE = "EngineLauncherRes";
        private const string C_RESOBJECT = "EngineLauncherRes";
        private const string C_RESOBJECT_NAME = "Main";

        private GObject m_goUIRes;
        private GLoader m_imgBg;
        public GLoader m_titleBg;
        private GTextField m_txtUIResTip;
        private GTextField m_txtAppVersion;
        private GComponent m_goTipWindows;
        private GComponent m_goTipWindowsCompContent;
        private GTextField m_goTipWindowsCompContentValue;
        private GTextField m_goTipWindowsTitle;
        private FairyGUI.Controller m_ctrlTipWindowsOp;
        private FairyGUI.Controller m_ctrlTipWindowsClose;
        private GButton m_goTipWindowsOK;
        private GButton m_goTipWindowsCancel;
        private GButton m_goTipWindowsClose;
        private bool m_bTipWindowsHideAfterAction;
        private System.Action m_actionTipWindowsOk;
        private System.Action m_actionTipWindowsCancel;
        private System.Action m_actionTipWindowsClose;
        private GComponent m_goMaintainWindows;
        private GComponent m_goMaintainWindowsContent;
        private GTextField m_goMaintainWindowsContentValue;
        private GTextField m_goMaintainWindowsTitle;
        private GButton m_goMaintainWindowsOK;
        private GButton m_goMaintainWindowsClose;
        private CBaseTimer m_timerMaintain;

        private GComponent m_tapSDKWindows;
        private GComponent m_tapSDKContents;
        private GTextField m_tapSDKContentsTitle;
        private GRichTextField m_tapSDKContentsContent;
        private GButton m_tapSDKContentsOK;
        private GButton m_tapSDKContentsClose;
        private FairyGUI.Controller m_tapSDKContentsOp;
        private FairyGUI.Controller m_tapSDKContentsCClose;
        private bool m_tapSDKWindowsHideAfterAction;
        private System.Action m_actiontapSDKWindowsOk;
        private System.Action m_actiontapSDKWindowsCancel;
        
        private FairyGUI.Controller m_ctrlLoading;
        private GProgressBar m_proLoading;

        private FairyGUI.Controller m_ctrlLanguage;
        
        //加载转圈圈改sipne转圈圈
        private GComponent m_loadingCom;
        private GLoader3D m_loadingSpine;
        
        public static EngineLauncherResUI Init()
        {
            UIPackage.AddPackage(C_RESPACKAGE);

            GObject go = UIPackage.CreateObject(C_RESOBJECT, C_RESOBJECT_NAME);

            if (go != null && go.displayObject != null && go.displayObject.gameObject != null)
            {
                EngineLauncherResUI comp = go.displayObject.gameObject.AddComponent<EngineLauncherResUI>();
                comp.DoInit(go);
                return comp;
            }

            return null;
        }

        public void DoInit(GObject go)
        {
            m_goUIRes = go;

            if (m_goUIRes != null)
            {
                m_goUIRes.MakeFullScreen();

                GRoot.inst.AddChild(m_goUIRes);

                m_imgBg = m_goUIRes.asCom.GetChild("bg") as GLoader;
                m_titleBg = m_goUIRes.asCom.GetChild("titlebg") as GLoader;

                m_txtUIResTip = m_goUIRes.asCom.GetChild("txtTip") as GTextField;
                m_txtAppVersion = m_goUIRes.asCom.GetChild("txtAppVersion") as GTextField;

                m_ctrlLoading = m_goUIRes.asCom.GetController("ctrlLoading");
                m_proLoading = m_goUIRes.asCom.GetChild("proLoading") as GProgressBar;
                m_proLoading.visible = false;

                m_ctrlLanguage = m_goUIRes.asCom.GetController("ctrlLanguage");

                m_goTipWindows = m_goUIRes.asCom.GetChild("tipWindow").asCom;
                m_goTipWindowsTitle = m_goTipWindows.GetChild("title") as GTextField;
                m_goTipWindowsCompContent = m_goTipWindows.GetChild("content") as GComponent;
                m_goTipWindowsCompContentValue = m_goTipWindowsCompContent.GetChild("content") as GTextField;
                m_ctrlTipWindowsOp = m_goTipWindows.GetController("ctrlOp");
                m_ctrlTipWindowsClose = m_goTipWindows.GetController("ctrlClose");
                m_goTipWindowsOK = m_goTipWindows.GetChild("btnOk").asButton;
                m_goTipWindowsCancel = m_goTipWindows.GetChild("btnCancel").asButton;
                m_goTipWindowsClose = m_goTipWindows.GetChild("btnClose").asButton;
                m_goTipWindowsOK.onClick.Add(OnBtnTipWindowsOKClick);
                m_goTipWindowsCancel.onClick.Add(OnBtnTipWindowsCancelClick);
                m_goTipWindowsClose.onClick.Add(OnBtnTipWindowsCloseClick);

                m_goMaintainWindows = m_goUIRes.asCom.GetChild("maintainWindow").asCom;
                m_goMaintainWindowsContent = m_goMaintainWindows.GetChild("content") as GComponent;
                m_goMaintainWindowsContentValue = m_goMaintainWindowsContent.GetChild("content") as GTextField;
                m_goMaintainWindowsTitle = m_goMaintainWindows.GetChild("title") as GTextField;
                m_goMaintainWindowsOK = m_goMaintainWindows.GetChild("btnOk").asButton;
                m_goMaintainWindowsClose = m_goMaintainWindows.GetChild("btnClose").asButton;
                m_goMaintainWindowsOK.onClick.Add(OnBtnMaintainWindowsOKClick);
                m_goMaintainWindowsClose.onClick.Add(OnBtnMaintainWindowsCloseClick);

                m_tapSDKWindows = m_goUIRes.asCom.GetChild("TapSDKPanel").asCom;
                m_tapSDKContents = m_tapSDKWindows.GetChild("Content").asCom;
                m_tapSDKContentsTitle = m_tapSDKContents.GetChild("title") as GTextField;
                m_tapSDKContentsContent = m_tapSDKContents.GetChild("content") as GRichTextField;
                m_tapSDKContentsOp = m_tapSDKContents.GetController("ctrlOp");
                m_tapSDKContentsCClose = m_tapSDKContents.GetController("ctrlClose");
                m_tapSDKContentsOK = m_tapSDKContents.GetChild("btnOk").asButton;
                m_tapSDKContentsClose = m_tapSDKContents.GetChild("btnCancel").asButton;
                m_tapSDKContentsOK.onClick.Add(OnBtnTapSDKWindowsOKClick);
                m_tapSDKContentsClose.onClick.Add(OnBtnTapSDKWindowsCloseClick);
                m_tapSDKContentsContent.onClickLink.Add(this.OnClickTapSDKLinkTxt);
                
                m_loadingCom = m_goUIRes.asCom.GetChild("loadingSpine") as GComponent;
                m_loadingSpine = m_loadingCom.GetChild("spine") as GLoader3D;

                this.HideUILoading();
            }
        }

        public void Dispose()
        {
            if (m_goUIRes != null)
            {
                GRoot.inst.RemoveChild(m_goUIRes);

                m_goUIRes.Dispose();

                m_goUIRes = null;
                m_txtUIResTip = null;
                m_txtAppVersion = null;
                m_ctrlLoading = null;
                m_goTipWindows = null;
                m_actionTipWindowsOk = null;
                m_actionTipWindowsCancel = null;
                m_goMaintainWindows = null;
                m_ctrlLanguage = null;
                m_loadingSpine = null;
                m_loadingCom = null;

                UIPackage.RemovePackage(C_RESPACKAGE);
            }
        }

        void Update()
        {
            CheckMaintainWindowsTime();
        }

        public void UpdateVersion(string value)
        {
            if (m_txtAppVersion != null)
            {
                m_txtAppVersion.text = value;
            }
        }
        
        public void UpdateTitleBg(string name)
        {
            Texture2D newTexture = Resources.Load<Texture2D>("logo_"+name);
            NTexture nTexture = new NTexture(newTexture);
            m_titleBg.texture = nTexture;
        }

        private void OnBtnTipWindowsOKClick()
        {
            if (m_bTipWindowsHideAfterAction)
            {
                if (m_goTipWindows != null)
                {
                    m_goTipWindows.visible = false;
                }

                if (m_actionTipWindowsOk != null)
                {
                    System.Action actionTmp = m_actionTipWindowsOk;
                    m_actionTipWindowsOk = null;
                    actionTmp.Invoke();
                }
            }
            else
            {
                if (m_actionTipWindowsOk != null)
                {
                    m_actionTipWindowsOk.Invoke();
                }
            }
        }

        private void OnBtnTipWindowsCancelClick()
        {
            if (m_bTipWindowsHideAfterAction)
            {
                if (m_goTipWindows != null)
                {
                    m_goTipWindows.visible = false;
                }

                if (m_actionTipWindowsCancel != null)
                {
                    System.Action actionTmp = m_actionTipWindowsCancel;
                    m_actionTipWindowsCancel = null;
                    actionTmp.Invoke();
                }
            }
            else
            {
                if (m_actionTipWindowsCancel != null)
                {
                    m_actionTipWindowsCancel.Invoke();
                }
            }
        }

        private void OnBtnTipWindowsCloseClick()
        {
            if (m_goTipWindows != null)
            {
                m_goTipWindows.visible = false;
            }

            if (m_actionTipWindowsClose != null)
            {
                System.Action actionTmp = m_actionTipWindowsClose;
                m_actionTipWindowsClose = null;
                actionTmp.Invoke();
            }
        }

        private void OnBtnMaintainWindowsOKClick()
        {
            //SDKInterface.Instance.LoadConf();
        }

        private void OnBtnMaintainWindowsCloseClick()
        {
            //EngineLauncher.ApplicationQuit();
        }

        public void ShowSingleTipWindows(string title, string content, string strBtnOK, bool bHasClose, Action callbackOk, Action callbackClose, bool bTipWindowsHideAfterAction = true)
        {
            if (m_goTipWindows != null)
            {
                m_goTipWindows.visible = true;

                m_goTipWindowsTitle.text = title;
                m_goTipWindowsCompContentValue.text = content;
                m_ctrlTipWindowsOp.selectedIndex = 0;
                m_ctrlTipWindowsClose.selectedIndex = bHasClose ? 1 : 0;
                m_goTipWindowsOK.text = strBtnOK;
                m_actionTipWindowsOk = callbackOk;
                m_actionTipWindowsClose = callbackClose;
                m_bTipWindowsHideAfterAction = bTipWindowsHideAfterAction;

                if (m_goTipWindowsCompContentValue.height >= m_goTipWindowsCompContent.height)
                {
                    m_goTipWindowsCompContentValue.SetXY(m_goTipWindowsCompContentValue.x, 0);
                }
                else
                {
                    m_goTipWindowsCompContentValue.SetXY(m_goTipWindowsCompContentValue.x,
                        (m_goTipWindowsCompContent.height - m_goTipWindowsCompContentValue.height) / 2);
                }
            }
        }

        public void ShowDoubleTipWindows(string title, string content, string strBtnOK, string strBtnCancel, bool bHasClose,
            Action callbackOk, Action callbackCancel, Action callbackClose, bool bTipWindowsHideAfterAction = true)
        {
            if (m_goTipWindows != null)
            {
                m_goTipWindows.visible = true;

                m_goTipWindowsTitle.text = title;
                m_goTipWindowsCompContentValue.text = content;
                m_ctrlTipWindowsOp.selectedIndex = 1;
                m_ctrlTipWindowsClose.selectedIndex = bHasClose ? 1 : 0;
                m_goTipWindowsOK.text = strBtnOK;
                m_actionTipWindowsOk = callbackOk;
                m_goTipWindowsCancel.text = strBtnCancel;
                m_actionTipWindowsCancel = callbackCancel;
                m_actionTipWindowsClose = callbackClose;
                m_bTipWindowsHideAfterAction = bTipWindowsHideAfterAction;

                if (m_goTipWindowsCompContentValue.height >= m_goTipWindowsCompContent.height)
                {
                    m_goTipWindowsCompContentValue.SetXY(m_goTipWindowsCompContentValue.x, 0);
                }
                else
                {
                    m_goTipWindowsCompContentValue.SetXY(m_goTipWindowsCompContentValue.x,
                        (m_goTipWindowsCompContent.height - m_goTipWindowsCompContentValue.height) / 2);
                }
            }
        }

        public void ShowMaintainWindows(string content)
        {
            if (m_goMaintainWindows != null)
            {
                m_goMaintainWindows.visible = true;

                m_goMaintainWindowsContentValue.text = content;
                //m_goMaintainWindowsTitle.text = ConfigUtils.GetCSStringByKey((int)TIP_KEY.KEY_MAINTAIN);
                //m_goMaintainWindowsOK.text = ConfigUtils.GetCSStringByKey((int)TIP_KEY.KEY_REFRESH);

                if (m_goMaintainWindowsContentValue.height >= m_goMaintainWindowsContent.height)
                {
                    m_goMaintainWindowsContentValue.SetXY(m_goMaintainWindowsContentValue.x, 0);
                }
                else
                {
                    m_goMaintainWindowsContentValue.SetXY(m_goMaintainWindowsContentValue.x,
                        (m_goMaintainWindowsContent.height - m_goMaintainWindowsContentValue.height) / 2);
                }
            }
        }

        private void CheckMaintainWindowsTime()
        {
            if (m_timerMaintain != null)
            {
                if (m_timerMaintain.ToNextTime())
                {
                    if (!UpdateMaintainWindowsTime())
                    {
                        // 时间结束
                        m_timerMaintain.Clear();

                        OnBtnMaintainWindowsOKClick();
                    }
                }
            }
        }

        private bool UpdateMaintainWindowsTime()
        {
            return false;
        }

        #region 个人信息保护弹窗
        public void ShowTapSDKWindows(string title, string content, string strBtnOK, string strBtnCancel, Action callbackOk, Action callbackCancel, bool tapSDKWindowsHideAfterAction = true)
        {
            if (m_tapSDKWindows != null)
            {
                m_tapSDKWindows.visible = true;

                m_tapSDKContentsTitle.text = title;
                m_tapSDKContentsContent.text = content;
                
                m_tapSDKContentsOp.selectedIndex = 1;
                m_tapSDKContentsCClose.selectedIndex = 0;
                
                m_tapSDKContentsOK.text = strBtnOK;
                m_actiontapSDKWindowsOk = callbackOk;
                m_tapSDKContentsClose.text = strBtnCancel;
                m_actiontapSDKWindowsCancel = callbackCancel;
                
                m_tapSDKWindowsHideAfterAction = tapSDKWindowsHideAfterAction;
            }
        }
        private void OnBtnTapSDKWindowsOKClick()
        {
            if (m_tapSDKWindowsHideAfterAction)
            {
                if (m_tapSDKWindows != null)
                {
                    m_tapSDKWindows.visible = false;
                }

                if (m_actiontapSDKWindowsOk != null)
                {
                    System.Action actionTmp = m_actiontapSDKWindowsOk;
                    m_actiontapSDKWindowsOk = null;
                    actionTmp.Invoke();
                }
            }
            else
            {
                if (m_actiontapSDKWindowsOk != null)
                {
                    m_actiontapSDKWindowsOk.Invoke();
                }
            }
        }

        private void OnBtnTapSDKWindowsCloseClick()
        {
            if (m_tapSDKWindowsHideAfterAction)
            {
                if (m_tapSDKWindows != null)
                {
                    m_tapSDKWindows.visible = false;
                }

                if (m_actiontapSDKWindowsCancel != null)
                {
                    System.Action actionTmp = m_actiontapSDKWindowsCancel;
                    m_actiontapSDKWindowsCancel = null;
                    actionTmp.Invoke();
                }
            }
            else
            {
                if (m_actiontapSDKWindowsCancel != null)
                {
                    m_actiontapSDKWindowsCancel.Invoke();
                }
            }
        }

        private void OnClickTapSDKLinkTxt(EventContext context)
        {
            GRichTextField t = context.sender as GRichTextField;
            string[] eventData = ((string) context.data).Split(":");
            if (eventData[1] == "userTips")
            {
                string url = "http://download.fkpd.cc/public/url/userTips.html";//((string) context.data);
                Application.OpenURL(url);
            }else if (eventData[1] == "PrivacyTips")
            {
                string url = "http://download.fkpd.cc/public/url/PrivacyTips.html";//((string) context.data);
                Application.OpenURL(url);
            }
        }
        #endregion
        
        public void ShowUILoading()
        {
            if (m_ctrlLoading != null)
            {
                m_ctrlLoading.selectedIndex = 1;
                this.playLoading();
            }
        }
        
        public void HideUILoading()
        {
            if (m_ctrlLoading != null)
            {
                m_ctrlLoading.selectedIndex = 0;
                this.stopLoading();
            }
        }

        public void TipStep(string strValue)
        {
            if (m_txtUIResTip != null)
            {
                m_txtUIResTip.text = strValue;
            }
        }

        public void TipUpdateProgress(float value)
        {
            if (m_txtUIResTip != null)
            {
                m_proLoading.value = value;
            }
        }

        public void TipUpdateProgress(int nKey, float value, params object[] args)
        {
            if (m_txtUIResTip != null)
            {
                m_proLoading.value = value;
            }
        }

        public void SetVisibleProgress(bool visible)
        {
            if (m_txtUIResTip != null)
            {
                m_proLoading.visible = visible;
            }
        }
        
        private void playLoading()
        {
            if (null != m_loadingSpine && !m_loadingSpine.playing)
            {
                m_loadingSpine.alpha = 0;
                m_loadingSpine.frame = 0;
                m_loadingSpine.loop = false;
                m_loadingSpine.playing = false;
                
                var tShow = m_loadingCom.GetTransition("show");
                if (null != tShow)
                {
                    tShow.Play();
                    tShow.SetHook("startSpine", () =>
                    {
                        if (null != m_loadingSpine)
                        {
                            m_loadingSpine.frame = 0;
                            m_loadingSpine.loop = true;
                            m_loadingSpine.playing = true;
                        }
                    });
                }
            }
        }
        
        private void stopLoading()
        {
            if (null != m_loadingSpine && m_loadingSpine.playing)
            {
                m_loadingSpine.alpha = 0;
                m_loadingSpine.frame = 0;
                m_loadingSpine.loop = false;
                m_loadingSpine.playing = false;
                
                var tShow = m_loadingCom.GetTransition("show");
                if (null != tShow && tShow.playing)
                {
                    tShow.Stop();
                }
            }
        }
        
        public void SetTipWindowsOKText(string strBtnOK)
        {
            if (m_goTipWindowsOK != null)
            {
                m_goTipWindowsOK.text = strBtnOK;
            }
        }
        
        public void SetTipWindowsCancelText(string strBtnCancel)
        {
            if (m_goTipWindowsCancel != null)
            {
                m_goTipWindowsCancel.text = strBtnCancel;
            }
        }

        public void HideTip()
        {
            if (m_goTipWindows != null)
            {
                m_goTipWindows.visible = false;
            }
        }
    }
}
