
using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using FairyGUI;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

public class NoticeData
{
    public string title;
    public string content;
}

public class NoticeView : UIViewBase
{
    private UI_Notice Notice => this.main as UI_Notice;

    // private string strUrl = "https://pandoo.obs.cn-east-3.myhuaweicloud.com";
    private string strUrl = "http://download.fkpd.cc/public";
    private List<NoticeData> _noticeDatas = new List<NoticeData>();
    public NoticeView()
    {
        this.name = "Notice";
        this.package = "Common";
        this.component = "Notice";
    }

    public override void BindAll()
    {
        base.BindAll();
        CommonBinder.BindAll();
    }

    protected override void OnInit()
    {
        base.OnInit();
        this.Notice.closeBtn.onClick.Add(this.Hide);
        // this.Notice.closeBtn.onClick.Add(this.HideUI);
        this.Notice.noticeList.itemRenderer = NoticeListRender;
    }

    private void HideUI()
    {
        GameManager.Instance.SoundManager.PlayEffectWithoutLoop((int)SoundType.GeneralClickSE);
        this.Hide();
    }

    protected override void OnShow()
    {
        base.OnShow();
        _noticeDatas.Clear();
        this.Notice.closeBtn.visible = false;
        GameManager.Instance.StartCoroutine(GetNoticeData(GetNoticeDataSuccess));
    }

    protected override void OnHide()
    {
        base.OnHide();
        this.Notice.closeBtn.visible = true;
    }

    private void GetNoticeDataSuccess(string json)
    {
        var noticeObj = JsonMapper.ToObject(json);
        foreach (JsonData item in noticeObj)
        {
            LogUtils.Log(item.ToJson());
            NoticeData noticeData = JsonMapper.ToObject<NoticeData>(item.ToJson());
            _noticeDatas.Add(noticeData);
        }

        this.Notice.closeBtn.visible = true;
        this.Notice.noticeList.numItems = _noticeDatas.Count;
    }

    private IEnumerator GetNoticeData(Action<string> actionSuccess)
    {
        // string path = strUrl + "/CDN/Notice/notice.txt";
        string path = strUrl + "/Notice/notice.txt";
        
        UnityWebRequest httpRequest = UnityWebRequest.Get(path);

        httpRequest.timeout = 10;

        yield return httpRequest.SendWebRequest();

        if (httpRequest.error != null)
        {
            LogUtils.LogWarningFormat("OnHttpGetWithUnityWebRequest Fail {0} {1}", path, httpRequest.error);
            httpRequest.Dispose();
            yield break;
        }

        if (httpRequest.downloadHandler == null)
        {
            LogUtils.LogWarningFormat("OnHttpGetWithUnityWebRequest downloadHandler null {0}", path);
            
            httpRequest.Dispose();
            yield break;
        }

        yield return httpRequest.downloadHandler.text;

        var byData = httpRequest.downloadHandler.text;
        

        try
        {
            actionSuccess?.Invoke(byData);
        }
        catch (Exception e)
        {
            LogUtils.LogException(e);
        }

        httpRequest.Dispose();
    }

    private void NoticeListRender(int index, GObject item)
    {
        ((UI_noticeItem) item).title.text = _noticeDatas[index].title;
        ((UI_noticeItem) item).content.text = _noticeDatas[index].content;
        
        ((UI_noticeItem) item).onClickLink.Add(this.OnClickLink);
    }

    private void OnClickLink(EventContext context)
    {
        string url = ((string) context.data);
        Application.OpenURL(url);
    }
}
