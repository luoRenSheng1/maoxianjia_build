using System.Collections.Generic;
using System.Linq;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using Setting;
using EventDispatcher = EngineBase.EventDispatcher;

public class ServerSelectView : UIViewBase
{
    private UI_ServerSelect ServerSelect => this.main as UI_ServerSelect;
    
    private List<ConfigServerUnit> _serverUnits;
    public ServerSelectView()
    {
        this.name = "ServerSelect";
        this.package = "Setting";
        this.component = "ServerSelect";
        this.removePackage = true;
        this.safeAreaInset = true;
    }

    public override void BindAll()
    {
        base.BindAll();
        SettingBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        // this.ServerSelect.closeBtn.onClick.Add(this.Hide);
        this.ServerSelect.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.ServerSelect.serverList.itemRenderer = ServerListRender;
        
        this.ServerSelect.changeSerBtn.onClick.Add(this.OnClickChangeServerBtn);
        
        _serverUnits = ConfigDataGroup.GetInstance<ConfigServer>().Data.Values.ToList();
        this.ServerSelect.serverList.numItems = _serverUnits.Count;
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.ServerSelect.serverList.selectedIndex = GetCurServerIndex();
    }

    private int GetCurServerIndex()
    {
        for (int i = 0; i < _serverUnits.Count; i++)
        {
            if (_serverUnits[i].Id == GameManager.Instance.CurServerUnit.Id)
                return i;
        }

        return -1;
    }

    private void ServerListRender(int index, GObject item)
    {
        ((UI_ServerItem) item).serverName.text = _serverUnits[index].Name;
    }

    private void OnClickChangeServerBtn()
    {
        if (this.ServerSelect.serverList.selectedIndex == -1)
        {
            return;
        }
        int index = this.ServerSelect.serverList.selectedIndex;
        if (GameManager.Instance.CurServerUnit.Id == _serverUnits[index].Id)
        {
            UIManager.Instance.ToastByKey(10127);
            return;
        }
        MessageBoxView.MessageParam param = new MessageBoxView.MessageParam
        {
            OkCallBack = SendToChangeServer
        };
        UIManager.Instance.ShowUIPanel("MessageBox", ConfigUtils.GetStringByKey(10126), param);
    }

    private void SendToChangeServer()
    {
        int index = this.ServerSelect.serverList.selectedIndex;
        if (index != -1)
        {
           if(GameManager.Instance.CurServerUnit.Id == _serverUnits[index].Id)
               return;
           UIManager.Instance.CloseAllUIPanel();
           UIManager.Instance.ShowUIPanel("Login");
           GameManager.Instance.CurServerUnit = _serverUnits[index];
           EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_RECONNECT_SUCCESS);
           GameManager.Instance.ReLoginServer();
        }
    }
}
