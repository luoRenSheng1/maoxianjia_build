
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using Setting;
using UnityEngine;
using UnityEngine.Networking;

public class ProblemEditView : UIViewBase
{
    private UI_ProblemEdit ProblemEdit => this.main as UI_ProblemEdit;
    private string PostURL = "http://121.36.216.198:8081/submitquestion";
    public ProblemEditView()
    {
        this.name = "ProblemEdit";
        this.package = "Setting";
        this.component = "ProblemEdit";
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
        // this.ProblemEdit.closeBtn.onClick.Add(this.Hide);
        this.ProblemEdit.closeBtn.onClick.Add(this.HideWithSoundEffect);
        this.ProblemEdit.commitBtn.onClick.Add(this.OnClickCommitBtn);
    }

    protected override void OnShow()
    {
        base.OnShow();
        this.ProblemEdit.ipt.text = "";
    }

    private void OnClickCommitBtn()
    {
        if(string.IsNullOrEmpty(this.ProblemEdit.ipt.text))
            return;
        SetVisible(false);
        //提交
        LogUtils.LogFormat("提交的问题:{0}", this.ProblemEdit.ipt.text);
        SendProblem(this.ProblemEdit.ipt.text);
    }
    
    private void SendProblem(string question)
    {
        string url = "http://121.36.216.198:8081/submitquestion";

        HttpWebRequest  request = WebRequest.Create(url) as HttpWebRequest;

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";

        StringBuilder buffer = new StringBuilder();
        buffer.AppendFormat("{0}={1}", "gameId", "1");
        buffer.AppendFormat("&{0}={1}", "userId", DataManager.Instance.GetRoleData().userID);
        buffer.AppendFormat("&{0}={1}", "title", "问题反馈");
        buffer.AppendFormat("&{0}={1}", "content", question);
        byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        string[] values = request.Headers.GetValues("Content-Type");
        HttpWebResponse rsp = request.GetResponse() as HttpWebResponse;

    }
}
