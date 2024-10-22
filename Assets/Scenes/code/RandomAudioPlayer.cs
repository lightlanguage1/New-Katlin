using System.Collections;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [Header("音频源")]
    public AudioSource audioSource;

    [Header("音频片段数组")]
    public AudioClip[] audioClips;

    [Header("播放间隔的最小值和最大值")]
    public float minInterval = 5f;
    public float maxInterval = 10f;

    [Header("切换音频后的额外间隔时间")]
    public float additionalIntervalAfterSwitch = 2f;

    [Header("冷却时间，防止过于频繁的音频切换")]
    public float cooldownTime = 5f;

    [Header("上一次播放音频的时间")]
    private float lastPlayTime;

    void Start()
    {
        // 检查音频源和音频片段数组是否已设置
        if (audioSource == null || audioClips.Length == 0)
        {
            Debug.LogError("请设置Audio Source和至少一个Audio Clip！");
            return;
        }

        // 初始化为负的冷却时间，确保第一次播放不受限制
        lastPlayTime = -cooldownTime;

        // 开始播放随机音频的协程
        StartCoroutine(PlayRandomAudio());
    }

    // 协程：播放随机音频
    IEnumerator PlayRandomAudio()
    {
        while (true)
        {
            // 检查是否可以切换到新音频
            if (Time.time - lastPlayTime > cooldownTime)
            {
                // 随机选择一个音频片段
                AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];

                // 设置音频源的片段
                audioSource.clip = randomClip;

                // 播放音频
                audioSource.Play();

                // 记录最后一次播放时间
                lastPlayTime = Time.time;

                // 等待一段随机时间后再次播放
                float interval = Random.Range(minInterval, maxInterval) + additionalIntervalAfterSwitch;
                yield return new WaitForSeconds(interval);
            }
            else
            {
                // 如果还在冷却时间内，则等待一小段时间后再检查是否可以播放
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
