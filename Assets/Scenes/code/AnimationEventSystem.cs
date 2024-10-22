using UnityEngine;

public class AnimationEventSystem : MonoBehaviour
{
    //当动画播完完毕，动画器发送message 调用该脚本内的注册的方法
    public void onAnimationEnd()
    {
        gameObject.SetActive(false);       
    }

    public void onPlayerMove()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Walk);
    }

    public void onDaji()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Daji);
    }

    public void onXuli()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Xuli);
    }
}
