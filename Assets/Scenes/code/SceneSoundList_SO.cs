using System.Collections.Generic;
using UnityEngine;
using static Global;

[CreateAssetMenu(fileName = "SceneSoundList_SO",menuName = "SceneSound/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundList;

    public SceneSoundItem getSceneSoundItem(string name)
    {
        return sceneSoundList.Find(s=>s.sceneName == name);
    }
}

[System.Serializable]
public class SceneSoundItem
{
    public string sceneName;
    public SoundType ambientSoundType;
    public SoundType bgSoundType;
}