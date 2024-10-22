using UnityEngine;

namespace AudioStrategy
{
    public interface IAudioStrategy
    {
        void PlayAudio(AudioSource src, SoundDetails music);
    }

    //走路的声音策略
    public class WalkAudioStrategy : IAudioStrategy
    {
        public void PlayAudio(AudioSource src, SoundDetails music)
        {
            src.clip = music.soundClip;
            src.volume = music.volumeSize;
            src.Play();
        }
    }

    //按钮点击的声音策略
    public class ButtonClickedAudioStrategy : IAudioStrategy
    {
        public void PlayAudio(AudioSource src, SoundDetails music)
        {
            src.clip = music.soundClip;
            src.volume = music.volumeSize;
            src.Play();
        }
    }

    public class DajiSkillAudioStrategy : IAudioStrategy
    {
        public void PlayAudio(AudioSource src, SoundDetails music)
        {
            src.clip = music.soundClip;
            src.volume = music.volumeSize;
            src.Play();
        }
    }

    public class XuliSkillAudioStrategy : IAudioStrategy
    {
        public void PlayAudio(AudioSource src, SoundDetails music)
        {
            src.clip = music.soundClip;
            src.volume = music.volumeSize;
            src.Play();
        }
    }

    public class MoveCusorAudioStrategy : IAudioStrategy
    {
        public void PlayAudio(AudioSource src, SoundDetails music)
        {
            src.clip = music.soundClip;
            src.volume = music.volumeSize;
            src.Play();
        }
    }
}