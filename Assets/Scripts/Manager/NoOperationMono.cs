using Engine;
using UnityEngine;

public class NoOperationMono : MonoBehaviour
{
    public float TimeOffset = 5f;//检测间隔时间

    private float _lasterTime;//上次的时间

    public bool isOpenCheck = false;

    private void Start()
    {
        // AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
    }
    
    /// <summary>
    /// 切换设备（蓝牙）后重新播放背景音乐
    /// </summary>
    /// <param name="deviceWasChanged"></param>
    void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        if (deviceWasChanged)
        {
            AudioConfiguration config = AudioSettings.GetConfiguration();
            config.dspBufferSize = 64;
            AudioSettings.Reset(config);
        }
        GameManager.Instance.SoundManager.PlayMusic();   //Music为背景音乐的AudioSoure   切换设备就重新播放音效
    }

    // Update is called once per frame
    void Update()
    {
        float nowTime = Time.time;
        if (Input.GetMouseButtonDown(0))
        {
            _lasterTime = nowTime;//更新触摸时间
        }
        
        if(isOpenCheck == false)
            return;

        float offsetTime = Mathf.Abs(nowTime - _lasterTime);
        if (offsetTime > TimeOffset)
        {
            Application.targetFrameRate = SystemAdapter.S_FRAMERATE_LOW;
            UIManager.Instance.ShowUIPanel("NoOperation");
            isOpenCheck = false;
        }
    }

    public void Reset()
    {
        float nowTime = Time.time;
        _lasterTime = nowTime;//更新触摸时间
    }
}
