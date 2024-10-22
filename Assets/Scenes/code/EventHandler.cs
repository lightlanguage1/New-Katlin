using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

//委托应该都写在这里解耦，我的问题，之后就靠你优化了
public class EventHandler 
{
    public static event Action<SoundType> onSoundChange;
    public static void callSoundChangeEvent(SoundType type)
    {
        onSoundChange?.Invoke(type);
    }

    public static event Action<bool> onTrigger;
    public static void callTriggerStopEvent(bool isStop)
    {
        onTrigger?.Invoke(isStop);
    }
}
