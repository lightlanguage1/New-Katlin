using System.Collections.Generic;
using UnityEngine;
using static Global;

[CreateAssetMenu(fileName = "SoundList", menuName = "Sound/SoundList")]
public class SoundList_SO : ScriptableObject
{
    public List<SoundDetails>soundDetailsList;

    public SoundDetails getSoundDetails(SoundType name)
    {
        return soundDetailsList.Find(s=>s.soundName == name);
    }
}


[System.Serializable]
public class SoundDetails   //声音详细信息
{
    public SoundType soundName;
    public AudioClip soundClip;
    [UnityEngine.Range(0.1f, 1.5f)]
    public float soundPitchMin = 0.8f;
    [UnityEngine.Range(0.1f, 1.5f)]
    public float soundPitchMax = 1.2f;
    [UnityEngine.Range(0.1f, 1f)]
    public float volumeSize = 0.2f;
    public float playDuration = 0.0f;  // 音频播放长度
    public float delayBetweenPlays = 0.0f;  // 音频播放间隔
}
