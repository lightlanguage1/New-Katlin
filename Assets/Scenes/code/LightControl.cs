using DG.Tweening;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public LightSwitch lightSwitch; // 灯光切换脚本对象
    private Light2D curLight;       // 当前灯光组件
    private LightDetails curlightDetails; // 当前灯光细节信息

    private void Start()
    {
        // 获取当前对象上的 Light2D 组件
        curLight = GetComponent<Light2D>();
    }

    // 实现灯光切换的逻辑
    public void ChangeLight(LightShift lightShift)
    {
        // 获取指定灯光转换类型的灯光细节信息
        curlightDetails = lightSwitch.GetLightDetails(lightShift);


        // 通过动画补间插件实现灯光颜色和强度的平滑过渡效果
        DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> tweenerCore = DOTween.To(() => curLight.color, c => curLight.color = c, curlightDetails.lightColor, GameTimeManager.instance.duration);
        DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tweenerCore1 = DOTween.To(() => curLight.intensity, c => curLight.intensity = c, curlightDetails.lightAmount, GameTimeManager.instance.duration);
    }

}