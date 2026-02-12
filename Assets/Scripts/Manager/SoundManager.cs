using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Config;
using Engine;
using EngineBase;
using FairyGUI;
using EventDispatcher = EngineBase.EventDispatcher;
using Object = UnityEngine.Object;

public class SoundEffect
{
    public string strName;
    public AudioSource audioSource;
    public ConfigSoundUnit cfgSound;
    public bool bLoop = false;
    public AssetBundleInfo uiAssetbundle;
    public AudioClip audioClip;
    public float fTimeOut = 0.0f; // 过期时间
    public bool bDestroy = false;
    public float fTimeFadeOut = 0.0f; // 淡出时间
    public float fVolumeFadeOutInit = 0.0f; // 淡出音量

    public void Init(string strName, Transform root, ConfigSoundUnit cfgSound, bool bLoop)
    {
        this.strName = strName;
        GameObject go = new GameObject(strName);
        go.transform.parent = root;
        this.audioSource = go.AddComponent<AudioSource>();
        this.cfgSound = cfgSound;
        this.bLoop = bLoop;
    }
}

public class SoundManager
{
    private MonoBehaviour MonoHost { set; get; }

    public static float MIN_SOUND_THRESHOLD = 0.09f; // 小于阀值的认为不播放声音
    public static float MAX_EFFECTOUT_TIME = 30.0f; // 声音过期检测间隔
    public static float MAX_CACHE_TIME = 10.0f; // 声音缓存时间
    public static float EFFECT_FADEOUT_TIME = 0.5f; // 声音淡出时间

    private float m_fMusicVolume = 1.0f; // 音乐大小
    private float m_fSoundVolume = 1.0f; // 音效大小

    // 音效
    private List<SoundEffect> m_lstSoundEffectRuning = new List<SoundEffect>(); // 播放中的声音
    private List<SoundEffect> m_lstSoundEffectOut = new List<SoundEffect>(); // 已经失效的声音，等待下一个周期清除内存
    private List<SoundEffect> m_lstSoundEffectLoading = new List<SoundEffect>(); // 加载中的声音
    private List<SoundEffect> m_lstSoundEffectFadeOut = new List<SoundEffect>(); // 淡出中的声音

    private float m_fTimerEffectRuningOut = 0.0f;
    //private CTimer m_timerEffectOut = new CTimer();

    // 背景音乐
    private AudioSource audioSoundBG;
    private int m_nMusicSoundID = 0;

    private int m_nMusicSoundIDLast = 0;

    //private CTimer m_timerMusicFadeOut = new CTimer();
    private float m_fMusicVolumeFadeOutInit = 0.0f;

    // 音效
    struct AudioInfo
    {
        public AudioClip clip;
        public int pendingNum;
    }

    private AudioSource audioSoundFx;
    private Dictionary<string, AudioInfo> m_dicFxAudioClips = new Dictionary<string, AudioInfo>();

    public void Init()
    {

        this.InitParam();

        this.InitMonoHost();

        //Stage.inst.RegisterOnPlaySoundAtion(this.PlayEffectWithoutLoop);
    }

    public void InitParam()
    {
        this.Clear();
    }

    public void InitMonoHost()
    {
        if (this.MonoHost != null)
        {
            return;
        }

        GameObject go = new GameObject();
        go.name = "SoundManager";
        GameObject.DontDestroyOnLoad(go);

        this.MonoHost = go.AddComponent<LoaderBehaviour>();
        this.audioSoundBG = go.AddComponent<AudioSource>();
        this.audioSoundBG.loop = true;
        this.audioSoundBG.priority = 0; //music has most important priority

        audioSoundFx = go.AddComponent<AudioSource>();
        audioSoundFx.priority = 32;
    }

    public void SetMusicVolume(float volume)
    {
        if (this.audioSoundBG != null)
        {
            float v = Mathf.Clamp01(volume);
            if (audioSoundBG.enabled)
            {
                if (Mathf.Approximately(v, 0))
                    audioSoundBG.enabled = false;
            }
            else
            {
                if (!Mathf.Approximately(v, 0))
                {
                    audioSoundBG.enabled = true;
                }
            }

            audioSoundBG.volume = v;
        }
    }

    public void SetSoundEffectVolume(float volume)
    {
        if (this.audioSoundFx != null)
        {
            float v = Mathf.Clamp01(volume);
            if (audioSoundFx.enabled)
            {
                if (Mathf.Approximately(v, 0))
                    audioSoundFx.enabled = false;
            }
            else
            {
                if (!Mathf.Approximately(v, 0))
                {
                    audioSoundFx.enabled = true;
                }
            }

            audioSoundFx.volume = v;
        }
    }

    public void Dispose()
    {
        this.Clear();
    }

    public void Clear()
    {
        StopMusic();

        foreach (var item in m_lstSoundEffectRuning)
        {
            ClearSoundEffect(item);
        }

        foreach (var item in m_lstSoundEffectOut)
        {
            ClearSoundEffect(item);
        }

        m_fTimerEffectRuningOut = 0.0f;
        //m_timerEffectOut.Startup(MAX_EFFECTOUT_TIME);
        m_lstSoundEffectRuning.Clear();
        m_lstSoundEffectOut.Clear();
        m_lstSoundEffectLoading.Clear();

        m_dicFxAudioClips.Clear();
    }

    public void Tick(float deltaSeconds)
    {
        float fTimeNow = Time.time;

        if (m_lstSoundEffectFadeOut.Count > 0)
        {
            for (int i = m_lstSoundEffectFadeOut.Count - 1; i >= 0; i--)
            {
                SoundEffect effect = m_lstSoundEffectFadeOut[i];

                if (effect != null)
                {
                    float fPrecent = Math.Max((effect.fTimeFadeOut - fTimeNow) / EFFECT_FADEOUT_TIME, 0);

                    if (effect.audioSource != null)
                    {
                        effect.audioSource.volume = effect.fVolumeFadeOutInit * fPrecent;
                    }

                    if (fPrecent <= 0)
                    {
                        m_lstSoundEffectFadeOut.RemoveAt(i);
                    }
                }
            }
        }

        if (fTimeNow > m_fTimerEffectRuningOut && m_lstSoundEffectRuning.Count > 0)
        {
            for (int i = m_lstSoundEffectRuning.Count - 1; i >= 0; i--)
            {
                SoundEffect effect = m_lstSoundEffectRuning[i];

                if (effect != null && effect.fTimeOut <= fTimeNow)
                {
                    m_lstSoundEffectRuning.RemoveAt(i);
                    m_lstSoundEffectOut.Add(effect);
                }
            }
        }

        /*
        if (m_timerEffectOut.ToNextTime())
        {
            if (m_lstSoundEffectOut.Count > 0)
            {
                for (int i = m_lstSoundEffectOut.Count - 1; i >= 0; i--)
                {
                    SoundEffect item = m_lstSoundEffectOut[i];

                    if (item == null || item.fTimeOut + MAX_CACHE_TIME <= fTimeNow)
                    {
                        ClearSoundEffect(item);
                        m_lstSoundEffectOut.RemoveAt(i);
                    }
                }
            }
        }

        if (m_timerMusicFadeOut.IsActive())
        {
            if (audioSoundBG != null)
            {
                float fPass = m_timerMusicFadeOut.GetPassPrecent();
                float fValue = m_fMusicVolumeFadeOutInit - m_fMusicVolumeFadeOutInit * fPass;
                audioSoundBG.volume = fValue;
            }

            if (m_timerMusicFadeOut.TimeOver())
            {
                StopMusic();
            }
        }
        */
    }

    private void ClearSoundEffect(SoundEffect item)
    {
        if (item == null)
        {
            return;
        }

        if (item.audioSource != null)
        {
            GameObject.Destroy(item.audioSource.gameObject);
        }

        if (ModelManager.Instance != null && item.uiAssetbundle != null)
        {
            ModelManager.Instance.ClearModel(item.uiAssetbundle.assetBundleName);
        }
    }

    public bool musicPlay
    {
        get { return m_fMusicVolume > MIN_SOUND_THRESHOLD; }
    }

    public bool effectPlay
    {
        get { return m_fSoundVolume > MIN_SOUND_THRESHOLD; }
    }

    public float musicVolume
    {
        set
        {
            m_fMusicVolume = value;

            if (m_fMusicVolume > MIN_SOUND_THRESHOLD)
            {
                PlayMusic();
            }
            else
            {
                StopMusic();
            }

            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_SOUND_SETTING_CHANGE);
        }
        get { return m_fMusicVolume; }
    }

    public float soundVolume
    {
        set
        {
            m_fSoundVolume = value;
            GRoot.inst.soundVolume = m_fSoundVolume;
            EventDispatcher.GameWorld.DispatchEvent(EventDefine.STR_SOUND_SETTING_CHANGE);
        }
        get { return m_fSoundVolume; }
    }

    private string GetAssetBundleName(string strAssetBundle)
    {
        return string.Format("sound/{0}.bin", strAssetBundle.ToLower());
    }

    #region 背景音乐

    public void PlayMusic()
    {
        PlayMusic(m_nMusicSoundIDLast);
    }

    public void PlayMusic(int nMusicSoundID)
    {
        // Not Init
        if (audioSoundBG == null || !musicPlay)
        {
            return;
        }
        //如果之前没有播放过背景音乐，则默认为1
        // if (nMusicSoundID == 0) nMusicSoundID = 1;
        ConfigSoundUnit cfgSound = ConfigDataGroup.GetInstance<ConfigSound>().Get(nMusicSoundID);

        if (cfgSound == null)
        {
            return;
        }

        if (m_nMusicSoundID == nMusicSoundID)
        {
            // 容错，修正音量，清理淡出逻辑
            float volume = cfgSound.Volume * 0.01f * m_fMusicVolume;
            if (audioSoundBG.volume != volume)
            {
                audioSoundBG.volume = volume;
            }

            //m_timerMusicFadeOut.Clear();
            return;
        }

        StopMusic();

        m_nMusicSoundID = nMusicSoundID;
        m_nMusicSoundIDLast = m_nMusicSoundID;
#if UNITY_EDITOR
        if (!Utils.IsLoadModelFromAssetBundle())
        {
            var path = string.Format("Assets/Editor Default Resources/Sound/{0}.ogg", cfgSound.Filename);
            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (go == null)
            {
                LogUtils.LogErrorFormat(@"OnAsyncLoadMusicDone {0} Not Found", cfgSound.Filename);
            }
            if (go != null && cfgSound != null)
            {
                AudioClip audioClip = go as AudioClip;
        
                if (audioClip == null)
                {
                    LogUtils.LogError("OnAsyncLoadMusicDone LoadAsset is null!" + cfgSound.Filename);
                    return;
                }
        
                if (null != audioSoundBG)
                {
                    audioSoundBG.clip = audioClip;
                    audioSoundBG.loop = cfgSound.Loop != 0;
                    audioSoundBG.volume = cfgSound.Volume * 0.01f * m_fMusicVolume;
                    audioSoundBG.pitch = cfgSound.Pitch * 0.01f; // -300~300映射成-3~3
                    audioSoundBG.rolloffMode = AudioRolloffMode.Linear;
                    audioSoundBG.Play();
                }
            }
            return;
        }
#else
        ModelManager.Instance.AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_SOUND,cfgSound.Filename, this.OnAsyncLoadMusicDone,cfgSound.Filename, cfgSound);
#endif
    }

    private void OnAsyncLoadMusicDone(ModelLoad loader, AssetBundleInfo ab, Object obj)
    {
        ConfigSoundUnit cfgSound = loader.param as ConfigSoundUnit;
        if (obj != null && cfgSound != null)
        {
            AudioClip audioClip = obj as AudioClip;
        
            if (audioClip == null)
            {
                LogUtils.LogError("OnAsyncLoadMusicDone LoadAsset is null!" + cfgSound.Filename);
                return;
            }
        
            if (null != audioSoundBG)
            {
                audioSoundBG.clip = audioClip;
                audioSoundBG.loop = cfgSound.Loop != 0;
                audioSoundBG.volume = cfgSound.Volume * 0.01f * m_fMusicVolume;
                audioSoundBG.pitch = cfgSound.Pitch * 0.01f; // -300~300映射成-3~3
                audioSoundBG.rolloffMode = AudioRolloffMode.Linear;
                audioSoundBG.Play();
            }
        }
    }

    public void StopMusic()
    {
        if (audioSoundBG != null)
        {
            audioSoundBG.Stop();
        }

        // 释放上一次背景音乐
        if (m_nMusicSoundID != 0)
        {
            var cfgSound = ConfigDataGroup.GetInstance<ConfigSound>().Get(m_nMusicSoundID);

            if (cfgSound != null)
            {
                ModelManager.Instance.ClearModel(GetAssetBundleName(cfgSound.Filename));
            }
        }

        m_nMusicSoundID = 0;
        //m_timerMusicFadeOut.Clear();
    }

    public void FadeOutStopMusic(float fTime)
    {
        if (audioSoundBG == null)
        {
            return;
        }

        m_fMusicVolumeFadeOutInit = audioSoundBG.volume;
        //m_timerMusicFadeOut.Startup(fTime);
    }

    #endregion

    #region 音效

    public void PlayEffectWithoutLoop(int nSoundID)
    {
        PlayEffect(nSoundID, true);
    }
    public void PlayEffectWithoutLoop(SoundType nSoundID)
    {
        PlayEffect((int)nSoundID, true);
    }

    public SoundEffect PlayEffect(int nSoundID, bool bForceNotLoop = false)
    {
        var cfgSound = ConfigDataGroup.GetInstance<ConfigSound>().Get(nSoundID);

        if (cfgSound != null)
        {
            return this.PlayEffect(cfgSound, bForceNotLoop);
        }

        return null;
    }

    private SoundEffect PlayEffect(ConfigSoundUnit cfgSound, bool bForceNotLoop = false)
    {
        if (cfgSound == null || string.IsNullOrEmpty(cfgSound.Filename))
        {
            return null;
        }

        if (!effectPlay)
        {
            return null;
        }

        bool bLoop = (cfgSound.Loop != 0) && !bForceNotLoop;

        string strAssetBundle = cfgSound.Filename;

        // 查找即将释放的声音中是否有需要的声音
        SoundEffect effect = GetOutEffectInfo(strAssetBundle);
        if (effect != null)
        {
            effect.cfgSound = cfgSound;
            effect.bLoop = bLoop;
            OnPlayEffect(effect);
            return effect;
        }

        if (bLoop)
        {
            // 加载声音
            SoundEffect effectNew = new SoundEffect();

            effectNew.Init(cfgSound.Filename, this.MonoHost.transform, cfgSound, bLoop);

            bool bSameNameLoading = false;

            foreach (var item in m_lstSoundEffectLoading)
            {
                if (item != null && item.strName == cfgSound.Filename)
                {
                    bSameNameLoading = true;
                    break;
                }
            }

            m_lstSoundEffectLoading.Add(effectNew);

            if (!bSameNameLoading)
            {
                ModelManager.Instance.AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_SOUND,strAssetBundle, this.OnAsyncLoadEffectDone, cfgSound.Filename, cfgSound.Filename);
            }

            return effectNew;
        }
        else
        {
            string audioName = cfgSound.Filename;
            AudioInfo info;
            if (m_dicFxAudioClips.TryGetValue(audioName, out info))
            {
                if (info.clip != null)
                {
                    this.audioSoundFx.PlayOneShot(info.clip, GetSoundVolume(cfgSound));
                }
                else
                {
                    info.pendingNum++;
                    m_dicFxAudioClips[audioName] = info;
                }
            }
            else
            {
                info.pendingNum++;
                m_dicFxAudioClips.Add(audioName, info);
                
#if UNITY_EDITOR
                if (!Utils.IsLoadModelFromAssetBundle())
                {
                    var path = string.Format("Assets/Editor Default Resources/Sound/{0}.ogg", cfgSound.Filename);
                    var go = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (go == null)
                    {
                        LogUtils.LogErrorFormat(@"OnAsyncLoadMusicDone {0} Not Found", cfgSound.Filename);
                    }

                    if (go != null && cfgSound != null)
                    {
                        AudioClip audioClip = go as AudioClip;
                        
                        if (audioClip != null)
                        {
                            if (m_dicFxAudioClips.TryGetValue(audioName, out info))
                            {
                                info.clip = audioClip;
                                info.pendingNum = 0;
                                m_dicFxAudioClips[audioName] = info;
                                audioSoundFx.PlayOneShot(audioClip, GetSoundVolume(cfgSound));

                                // 加载声音
                                SoundEffect effectNew = new SoundEffect();
                                effectNew.Init(cfgSound.Filename, this.MonoHost.transform, cfgSound, bLoop);
                                effectNew.audioClip = audioClip;
                                effectNew.audioSource = audioSoundFx;
                                m_lstSoundEffectLoading.Add(effectNew);
                                return effectNew;
                            }

                        }
                    }
                    
                }
#else
                ModelManager.Instance.AsyncLoadModel(MODEL_LOAD_TYPE.MODEL_LOAD_SOUND, strAssetBundle, this.OnAsyncLoadNoLoopEffectDone,cfgSound.Filename, cfgSound);
#endif
            }

            return null;
        }
    }

    private float GetSoundVolume(ConfigSoundUnit cfgSound)
    {
        return cfgSound.Volume * 0.01f * m_fSoundVolume;
    }

    private void OnAsyncLoadEffectDone(ModelLoad loader, AssetBundleInfo ab, Object obj)
    {
        string strFileName = loader.param as string;

        if (strFileName == null)
        {
            return;
        }

        for (int i = m_lstSoundEffectLoading.Count - 1; i >= 0; i--)
        {
            SoundEffect effect = m_lstSoundEffectLoading[i];

            if (effect == null)
            {
                continue;
            }

            if (effect.strName != strFileName)
            {
                continue;
            }

            if (!effect.bDestroy)
            {
                AudioClip audioClip = obj as AudioClip;
            
                if (ab != null && audioClip != null && effect != null && effect.audioSource != null)
                {
                    effect.uiAssetbundle = ab;
                    effect.audioClip = audioClip;
                    effect.audioSource.clip = audioClip;
            
                    OnPlayEffect(effect);
                }
            }

            m_lstSoundEffectLoading.RemoveAt(i);
        }
    }

    private void OnAsyncLoadNoLoopEffectDone(ModelLoad loader, AssetBundleInfo ab, Object obj)
    {
        //Debug.LogFormat("Load noloop clip {0}", ab.assetBundleName);
        AudioClip audioClip = obj as AudioClip;
        if (audioClip != null)
        {
            ConfigSoundUnit cfgSound = (ConfigSoundUnit) loader.param;
            string audioName = cfgSound.Filename;
            if (m_dicFxAudioClips.TryGetValue(audioName, out AudioInfo info))
            {
                info.clip = audioClip;
                info.pendingNum = 0;
                m_dicFxAudioClips[audioName] = info;
                audioSoundFx.PlayOneShot(audioClip, GetSoundVolume(cfgSound));
            }
            
        }
        
        if (ModelManager.Instance != null && ab != null)
        {
            ModelManager.Instance.ClearModel(ab.assetBundleName);
        }
    }

    public void StopEffect(SoundEffect effect)
    {
        if (effect != null)
        {
            effect.bDestroy = true;
            effect.fTimeOut = Time.time;

            if (effect.audioSource != null)
            {
                effect.audioSource.Stop();
            }
        }
    }

    private void OnPlayEffect(SoundEffect effect)
    {
        if (effect == null || effect.cfgSound == null)
        {
            return;
        }

        effect.bDestroy = false;

        float fTimeNow = Time.time;
        float fTime = fTimeNow;

        if (effect.audioClip != null)
        {
            fTime += effect.audioClip.length;

            if (effect.bLoop)
            {
                fTime += 999999999.0f;
            }
        }

        effect.fTimeOut = fTime;
        effect.fTimeFadeOut = 0.0f;
        effect.fVolumeFadeOutInit = 0.0f;

        if (fTime < m_fTimerEffectRuningOut)
        {
            m_fTimerEffectRuningOut = fTime;
        }

        if (effect.audioSource != null)
        {
            effect.audioSource.loop = effect.bLoop;
            effect.audioSource.volume = GetSoundVolume(effect.cfgSound);
            effect.audioSource.priority =
                Mathf.RoundToInt(effect.cfgSound.Priority * -1.28f + 128); // 0~100映射成128~0
            effect.audioSource.pitch = effect.cfgSound.Pitch * 0.01f; // -300~300映射成-3~3
            effect.audioSource.rolloffMode = AudioRolloffMode.Linear;
            effect.audioSource.Play();
        }

        m_lstSoundEffectRuning.Add(effect);
    }

    private SoundEffect GetOutEffectInfo(string strName)
    {
        for (int i = 0; i < m_lstSoundEffectOut.Count; i++)
        {
            SoundEffect item = m_lstSoundEffectOut[i];

            if (item != null && item.uiAssetbundle != null && item.uiAssetbundle.assetBundleName.Equals(strName))
            {
                m_lstSoundEffectOut.RemoveAt(i);
                return item;
            }
        }

        return null;
    }

    #endregion
}