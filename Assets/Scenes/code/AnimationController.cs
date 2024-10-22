using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private void OnEnable()
    {
        TransitionSceneDialog.playCompleted += playAnim;
    }

    //切换场景对象销毁时调用
    private void OnDisable()
    {
        TransitionSceneDialog.playCompleted -= playAnim;
    }

    private void playAnim(Animator[] animators)
    {
        if (Global.instance.curMainSceneName == "main1L")   //如果角色在1楼主场景
        {
            string[] animState = { "Guard up", "Guard right", "Guard down", "Guard left" }; //注意列表会自动调整其中aimator的顺序
            //播放所有角色的动画
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].Play(animState[i]);
            }
        }
    }
}