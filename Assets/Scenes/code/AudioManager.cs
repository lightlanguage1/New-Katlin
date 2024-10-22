using AudioStrategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Global;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐库")]
    public SceneSoundList_SO AudioItemList;  // 场景音效列表的 ScriptableObject，用于存储每个场景的音效信息

    public SoundList_SO soundList;  // 音效列表的 ScriptableObject，用于存储不同音效类型的详细信息

    [Header("Audio Source")]
    public AudioSource ambientSource;   // 环境音源，用于播放环境音效

    public AudioSource bgSource;    // 背景音源，用于播放背景音乐
    public AudioSource efffectSource;   // 效果音源，用于播放各种效果音效

    // 作为主轨道方便控制子轨道以及可以使用一些专业高级的音频效果
    [Header("snapshots")]
    public AudioMixerSnapshot normalSnapShot;  // 正常音效快照

    public AudioMixerSnapshot muteSnapShot;    // 静音快照
    public AudioMixerSnapshot onlyBgSnapShot;  // 只有背景音乐的快照

    // 策略字典，将不同的音效类型与相应的音频策略关联起来
    private Dictionary<SoundType, IAudioStrategy> audioStrategyDic;

    // 控制播放空隙时间的变量
    private bool isAudioPlaying = false;  // 标记是否正在播放音频
    private float delayBetweenAudios = 2.0f;  // 设置音频之间的延迟时间

    // 记录音频的结束时间
    private Dictionary<AudioSource, float> audioEndTimeDictionary = new Dictionary<AudioSource, float>();

    protected override void Awake()
    {
        base.Awake();
        // 初始化音频策略字典
        audioStrategyDic = new Dictionary<SoundType, IAudioStrategy>
        {
            {SoundType.Walk,new WalkAudioStrategy()},
            {SoundType.MoveCursor,new MoveCusorAudioStrategy()},
            {SoundType.Click,new ButtonClickedAudioStrategy()},
            {SoundType.Daji,new DajiSkillAudioStrategy()},
            {SoundType.Xuli,new XuliSkillAudioStrategy()}
        };
    }

    private void OnEnable()
    {
        // 注册事件，用于处理场景加载和卸载时的音效播放和停止
        SceneManager.sceneLoaded += playBgAudio;
        SceneManager.sceneUnloaded += stopAudio;
        // 注册事件，用于处理自定义音效的播放
        EventHandler.onSoundChange += playCustomAudio;
    }

    public void playCustomAudio(SoundType type)    // 播放自定义音效
    {
        // 检查是否正在播放音频，如果是，则返回
        if (isAudioPlaying)
        {
            return;
        }

        // 从音频策略字典中获取相应的音频策略并播放音效
        if (audioStrategyDic.TryGetValue(type, out var audioStrategy))
        {
            SoundDetails soundDetails = soundList.getSoundDetails(type);

            // 获取对应的音频源
            AudioSource audioSource = GetAudioSource(type);

            // 检查音频源是否正在播放
            if (audioSource.isPlaying)
            {
                // 获取音频的结束时间
                float endTime;
                if (audioEndTimeDictionary.TryGetValue(audioSource, out endTime))
                {
                    // 如果当前时间未达到结束时间，则返回
                    if (Time.time < endTime)
                    {
                        return;
                    }
                }
            }

            // 计算新的结束时间并更新字典
            float newEndTime = Time.time + (soundDetails.playDuration > 0 ? soundDetails.playDuration : soundDetails.soundClip.length) + delayBetweenAudios;
            audioEndTimeDictionary[audioSource] = newEndTime;

            // 播放音效
            audioStrategy.PlayAudio(audioSource, soundDetails);

            // 标记音频正在播放
            isAudioPlaying = true;

            // 启动协程来等待一段时间后重置标志
            StartCoroutine(WaitAndResetAudioFlag(soundDetails.delayBetweenPlays));
        }
    }


    private IEnumerator WaitAndResetAudioFlag(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 等待一段时间后重置标志
        isAudioPlaying = false;
    }


    private AudioSource GetAudioSource(SoundType type)
    {
        // 根据音效类型返回相应的音频源
        switch (type)
        {
            case SoundType.None: return null;
            case SoundType.Walk: return efffectSource;
            case SoundType.MoveCursor: return efffectSource;
            case SoundType.Click: return efffectSource;
            case SoundType.Daji: return efffectSource;
            case SoundType.Xuli: return efffectSource;
            case SoundType.StartMenu: return bgSource;
            case SoundType.Main1L: return bgSource;
            case SoundType.Battle: return bgSource;
        }
        return null;
    }

    private void playBgAudio(Scene scene, LoadSceneMode mode)
    {
        // 获取当前场景的音效信息
        SceneSoundItem item = AudioItemList.getSceneSoundItem(scene.name);
        if (item == null) return;

        // 获取背景音乐的详细信息并播放
        SoundDetails bg = soundList.getSoundDetails(item.bgSoundType);
        playBgMusicClip(bg);    // 播放背景音乐
    }

    private void stopAudio(Scene scene)
    {
        bgSource.Stop();    // 停止音频播放，并将当前播放位置重置为音频开头
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playBgMusicClip(SoundDetails soundDetails)
    {
        bgSource.volume = soundDetails.volumeSize;  // 设置背景音乐音量
        bgSource.clip = soundDetails.soundClip;    // 设置背景音乐音频剪辑
        if (bgSource.isActiveAndEnabled) bgSource.Play();  // 激活并播放背景音乐
    }

    /// <summary>
    /// 播放环境音
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playAmbientMusicClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;  // 设置环境音音频剪辑
        if (ambientSource.isActiveAndEnabled) ambientSource.Play();  // 激活并播放环境音
    }
}
