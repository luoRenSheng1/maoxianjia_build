using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using Engine;
using EngineBase;
using FairyGUI;
using Login;
using msg;
using UnityEngine;
using EventDispatcher = EngineBase.EventDispatcher;

public class LoginView : UIViewBase
{
    private Login.UI_Main loginMain
    {
        get { return this.main as Login.UI_Main; }
    }

    private List<ConfigServerUnit> _serverUnits;
    private string ip;
    private int port;
    public LoginView()
    {
        this.name = "Login";
        this.package = "Login";
        this.component = "Main";
        this.removePackage = true;
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.loginMain.loginPage.taptapBtn.onClick.Add(OnClickTapSDK);
        
        this.loginMain.ctrlPage.selectedIndex = 1;
        this.loginMain.login.selectedIndex = 0;
        this.loginMain.loginPage.btnClose.onClick.Add(OnClickLoginClose);
        this.loginMain.loginPage.btnLogin.onClick.Add(this.OnClickLogin);
        this.loginMain.plus_12.onClick.Add(this.OnClickPlus12);
        this.loginMain.loginPage.linkLb.onClickLink.Add(this.OnClickLinkTxt);

        EventDispatcher.GameWorld.Regist(EventDefine.STR_LOGIN_CHECK_SUCCESS, OnAccountCheckSuccess);
        EventDispatcher.GameWorld.Regist(EventDefine.STR_LOGIN_CHECK_FAIL, OnAccountCheckFail);
        EventDispatcher.GameWorld.Regist(EventDefine.STR_LOGIN_PLAYER_SUCCESS, OnPlayerLoginSuccess);
        EventDispatcher.GameWorld.Regist(EventDefine.STR_LOGIN_PLAYER_FAIL, OnPlayerLoginFail);
        EventDispatcher.GameWorld.Regist<bool>(EventDefine.STR_LOGIN_PLAYER_User_check, this.UserCheckSucc);
        EventDispatcher.GameWorld.Regist(EventDefine.STR_TAP_SDK_LOGIN_SUCCESS, OnSDKLoginSuccess);
        
        _serverUnits = ConfigDataGroup.GetInstance<ConfigServer>().Data.Values.ToList();
        List<string> serverNames = new List<string>();
        foreach (var item in _serverUnits)
        {
            serverNames.Add(item.Name);
        }

        bool isSelect = LocalSave.GetBool("userCheck", false);
        this.loginMain.loginPage.userCheck.selected = isSelect;
        this.loginMain.loginPage.serverPop.items = serverNames.ToArray();
    }

    protected override void OnDispose()
    {
        base.OnDispose();
        UIManager.Instance.CloseUIPanel("Notice");
        EventDispatcher.GameWorld.UnRegist(EventDefine.STR_LOGIN_CHECK_SUCCESS, OnAccountCheckSuccess);
        EventDispatcher.GameWorld.UnRegist(EventDefine.STR_LOGIN_CHECK_FAIL, OnAccountCheckFail);
        EventDispatcher.GameWorld.UnRegist(EventDefine.STR_LOGIN_PLAYER_SUCCESS, OnPlayerLoginSuccess);
        EventDispatcher.GameWorld.UnRegist(EventDefine.STR_LOGIN_PLAYER_FAIL, OnPlayerLoginFail);
        EventDispatcher.GameWorld.UnRegist<bool>(EventDefine.STR_LOGIN_PLAYER_User_check, this.UserCheckSucc);
        EventDispatcher.GameWorld.UnRegist(EventDefine.STR_TAP_SDK_LOGIN_SUCCESS, OnSDKLoginSuccess);
    }

    public override void BindAll()
    {
        base.BindAll();

        LoginBinder.BindAll();
    }

    private void UserCheckSucc(bool isSelected)
    {
        this.loginMain.loginPage.userCheck.selected = isSelected;
    }

    private void OnClickPlus12()
    {
        UIManager.Instance.ShowUIPanel("Plus12");
    }
    private void OnClickLogin()
    {
        if (!this.loginMain.loginPage.userCheck.selected)
        {
            UIManager.Instance.ShowUIPanel("GameAgreement");
            return;
        }

        EngineBase.PlayerPrefs.SetString("userId", this.loginMain.loginPage.txtAccount.text);
        this.loginMain.loginPage.btnLogin.visible = false;
        GameManager.Instance.TimerManager?.SetTimer(1.5f, () =>
        {
            if(this.loginMain != null)
                this.loginMain.loginPage.btnLogin.visible = true;
        });
        ConfigServerUnit serverUnit = _serverUnits[this.loginMain.loginPage.serverPop.selectedIndex];
        ip = serverUnit.IP;
        port = serverUnit.Port;
        GameManager.Instance.CurServerUnit = serverUnit;
        GameManager.Instance.CurUserName = this.loginMain.loginPage.txtAccount.text;
        OnClickStartLobby();
    }

    private void OnClickStartLobby()
    {
        LogUtils.LogWarning("OnBtnStartLobbyClick");

        if (string.IsNullOrEmpty(this.loginMain.loginPage.txtAccount.text))
        {
            UIManager.Instance.ToastByKey(47);
            return;
        }
        
        GameManager.Instance.Init(ip, port);
        GameManager.Instance.PreLoginServer(this.loginMain.loginPage.txtAccount.text, "");
        GameManager.Instance.LoginCheckServer();
    }

    private void OnAccountCheckSuccess()
    {
        LogUtils.Log("OnAccountCheckSuccess");
    }
    
    private void OnAccountCheckFail()
    {
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam();
        UIManager.Instance.ShowUIPanel("MessageBox", "登录Account失败", param);
    }
    
    private void OnPlayerLoginSuccess()
    {
        UIManager.Instance.HideLoading();
        UIManager.Instance.SpawnByName("Lobby");
        UIManager.Instance.ShowLoadingUI(() =>
        {
            SendMessageToReady("DestroyEngineLauncherResUI");
            // 显示主界面
            UIManager.Instance.ShowUIPanel("Lobby");
            UIManager.Instance.DestroyController(this);
        });
        LocalSave.SetBool("userCheck", true);
        EngineBase.PlayerPrefs.SetInt("server", this.loginMain.loginPage.serverPop.selectedIndex);
        EngineBase.PlayerPrefs.SetString("userId", this.loginMain.loginPage.txtAccount.text);
        EngineBase.PlayerPrefs.Save();
    }
    
    private void OnPlayerLoginFail()
    {
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam();
        UIManager.Instance.ShowUIPanel("MessageBox", "登录Player失败", param);
    }
    
    /// <summary>
    /// 调用ready节点上脚本的方法
    /// </summary>
    /// <param name="funName"></param>
    /// <param name="param"></param>
    private void SendMessageToReady(string funName, object param = null)
    {
        var goReady = GameObject.Find("Ready");

        if (goReady != null)
        {
            if (param == null)
            {
                goReady.SendMessage(funName);
            }
            else
            {
                goReady.SendMessage(funName, param);
            }
        }
    }

    private void OnClickLoginClose()
    {
        this.loginMain.ctrlPage.selectedIndex = 0;
    }

    protected override void OnShow()
    {
        base.OnShow();
        
        UpdateUI();
        Object.FindObjectOfType<NoOperationMono>().isOpenCheck = false;
        this.loginMain.loginPage.serverPop.selectedIndex = EngineBase.PlayerPrefs.GetInt("server");
        this.loginMain.loginPage.txtAccount.text = EngineBase.PlayerPrefs.GetString("userId");
    
        // GameManager.Instance.SoundManager.PlayMusic(1);
        GameManager.Instance.SoundManager.PlayMusic((int)SoundType.LoginBGM);
        // this.loginMain.bg.url = UIResource.GetImageUrlWithLang("dl_bg", "Login");
        this.loginMain.titlebg.url = UIResource.GetImageUrlWithLang("logo","Login");
        
        if (SDKInterface.Instance.IsTapTapPlatform())
        {
            this.loginMain.loginPage.hasAcount.selectedIndex = 1;
        }
        else
        {
            UIManager.Instance.ShowUIPanel("Notice");
            this.loginMain.loginPage.hasAcount.selectedIndex = 0;
        }
    }

    private void OnClickTapSDK()
    {
        Debug.Log("=========OnClickTapSDK========");
        //SDKInterface.Instance.ReportTapSDKEventData("login_start","clickTapSDKButton");
        if (SDKInterface.Instance.CheckUserLogin())
        {
            //已经登录
            OnSDKLoginSuccess();
        }
        else
        {
            //还没登录
            SDKInterface.Instance.Login();
        }
    }

    private void OnSDKLoginSuccess()
    {
        Debug.Log("=========sdk uid ="+SDKInterface.Instance.GetUserUin());
        this.loginMain.loginPage.txtAccount.text = SDKInterface.Instance.GetUserUin();
        
        this.loginMain.loginPage.serverPop.selectedIndex = 0;
        for (int i = 0; i < _serverUnits.Count; i++)
        {
            if (_serverUnits[i].IsShowPm == 0)
            {
                this.loginMain.loginPage.serverPop.selectedIndex = i;
                break;
            }
        }
        
        OnClickLogin();
    }

    protected void UpdateUI()
    {
        var resVersion = VersionManager.Instance.GetResVersion();
        
        if (!string.IsNullOrEmpty(resVersion))
        {
            this.loginMain.txtAppVersion.text = string.Format("App:{0}.{1}", VersionManager.Instance.GetAppVersion(),
                VersionManager.Instance.GetResVersion());
        }
        else
        {
            this.loginMain.txtAppVersion.text = string.Format("App:{0}", VersionManager.Instance.GetAppVersion());
        }
        

    }
    
    private void OnClickLinkTxt(EventContext context)
    {
        GRichTextField t = context.sender as GRichTextField;
        string[] eventData = ((string) context.data).Split(":");
        if (eventData[1] == "userTips")
        {
            // UIManager.Instance.ShowUIPanel("GameUserAgreement");
            // string url = "http://download.fkpd.cc/public/url/userTips.html";//((string) context.data);
            // Application.OpenURL(url);
            Application.OpenURL(ConstDefine.URL_USER);
        }else if (eventData[1] == "PrivacyTips")
        {
            // UIManager.Instance.ShowUIPanel("PrivateAgreement");
            // string url = "http://download.fkpd.cc/public/url/PrivacyTips.html";//((string) context.data);
            // Application.OpenURL(url);
            Application.OpenURL(ConstDefine.URL_PRIVACY);
        }
    }
}