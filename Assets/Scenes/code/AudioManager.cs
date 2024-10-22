using AudioStrategy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Global;

public class AudioManager : Singleton<AudioManager>
{
    [Header("���ֿ�")]
    public SceneSoundList_SO AudioItemList;  // ������Ч�б�� ScriptableObject�����ڴ洢ÿ����������Ч��Ϣ

    public SoundList_SO soundList;  // ��Ч�б�� ScriptableObject�����ڴ洢��ͬ��Ч���͵���ϸ��Ϣ

    [Header("Audio Source")]
    public AudioSource ambientSource;   // ������Դ�����ڲ��Ż�����Ч

    public AudioSource bgSource;    // ������Դ�����ڲ��ű�������
    public AudioSource efffectSource;   // Ч����Դ�����ڲ��Ÿ���Ч����Ч

    // ��Ϊ�������������ӹ���Լ�����ʹ��һЩרҵ�߼�����ƵЧ��
    [Header("snapshots")]
    public AudioMixerSnapshot normalSnapShot;  // ������Ч����

    public AudioMixerSnapshot muteSnapShot;    // ��������
    public AudioMixerSnapshot onlyBgSnapShot;  // ֻ�б������ֵĿ���

    // �����ֵ䣬����ͬ����Ч��������Ӧ����Ƶ���Թ�������
    private Dictionary<SoundType, IAudioStrategy> audioStrategyDic;

    // ���Ʋ��ſ�϶ʱ��ı���
    private bool isAudioPlaying = false;  // ����Ƿ����ڲ�����Ƶ
    private float delayBetweenAudios = 2.0f;  // ������Ƶ֮����ӳ�ʱ��

    // ��¼��Ƶ�Ľ���ʱ��
    private Dictionary<AudioSource, float> audioEndTimeDictionary = new Dictionary<AudioSource, float>();

    protected override void Awake()
    {
        base.Awake();
        // ��ʼ����Ƶ�����ֵ�
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
        // ע���¼������ڴ��������غ�ж��ʱ����Ч���ź�ֹͣ
        SceneManager.sceneLoaded += playBgAudio;
        SceneManager.sceneUnloaded += stopAudio;
        // ע���¼������ڴ����Զ�����Ч�Ĳ���
        EventHandler.onSoundChange += playCustomAudio;
    }

    public void playCustomAudio(SoundType type)    // �����Զ�����Ч
    {
        // ����Ƿ����ڲ�����Ƶ������ǣ��򷵻�
        if (isAudioPlaying)
        {
            return;
        }

        // ����Ƶ�����ֵ��л�ȡ��Ӧ����Ƶ���Բ�������Ч
        if (audioStrategyDic.TryGetValue(type, out var audioStrategy))
        {
            SoundDetails soundDetails = soundList.getSoundDetails(type);

            // ��ȡ��Ӧ����ƵԴ
            AudioSource audioSource = GetAudioSource(type);

            // �����ƵԴ�Ƿ����ڲ���
            if (audioSource.isPlaying)
            {
                // ��ȡ��Ƶ�Ľ���ʱ��
                float endTime;
                if (audioEndTimeDictionary.TryGetValue(audioSource, out endTime))
                {
                    // �����ǰʱ��δ�ﵽ����ʱ�䣬�򷵻�
                    if (Time.time < endTime)
                    {
                        return;
                    }
                }
            }

            // �����µĽ���ʱ�䲢�����ֵ�
            float newEndTime = Time.time + (soundDetails.playDuration > 0 ? soundDetails.playDuration : soundDetails.soundClip.length) + delayBetweenAudios;
            audioEndTimeDictionary[audioSource] = newEndTime;

            // ������Ч
            audioStrategy.PlayAudio(audioSource, soundDetails);

            // �����Ƶ���ڲ���
            isAudioPlaying = true;

            // ����Э�����ȴ�һ��ʱ������ñ�־
            StartCoroutine(WaitAndResetAudioFlag(soundDetails.delayBetweenPlays));
        }
    }


    private IEnumerator WaitAndResetAudioFlag(float delay)
    {
        yield return new WaitForSeconds(delay);

        // �ȴ�һ��ʱ������ñ�־
        isAudioPlaying = false;
    }


    private AudioSource GetAudioSource(SoundType type)
    {
        // ������Ч���ͷ�����Ӧ����ƵԴ
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
        // ��ȡ��ǰ��������Ч��Ϣ
        SceneSoundItem item = AudioItemList.getSceneSoundItem(scene.name);
        if (item == null) return;

        // ��ȡ�������ֵ���ϸ��Ϣ������
        SoundDetails bg = soundList.getSoundDetails(item.bgSoundType);
        playBgMusicClip(bg);    // ���ű�������
    }

    private void stopAudio(Scene scene)
    {
        bgSource.Stop();    // ֹͣ��Ƶ���ţ�������ǰ����λ������Ϊ��Ƶ��ͷ
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playBgMusicClip(SoundDetails soundDetails)
    {
        bgSource.volume = soundDetails.volumeSize;  // ���ñ�����������
        bgSource.clip = soundDetails.soundClip;    // ���ñ���������Ƶ����
        if (bgSource.isActiveAndEnabled) bgSource.Play();  // ������ű�������
    }

    /// <summary>
    /// ���Ż�����
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playAmbientMusicClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;  // ���û�������Ƶ����
        if (ambientSource.isActiveAndEnabled) ambientSource.Play();  // ������Ż�����
    }
}
