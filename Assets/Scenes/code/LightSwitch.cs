using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattern", menuName = "Light/LightPattern")]
public class LightSwitch : ScriptableObject
{
    public List<LightDetails> lightDetails;

    /// <summary>
    /// 根据灯光转换类型获取对应的灯光细节信息
    /// </summary>
    /// <param name="lightShift">灯光切换类型</param>
    /// <returns>对应的灯光细节信息</returns>
    public LightDetails GetLightDetails(LightShift lightShift)
    {
        LightDetails foundDetails = lightDetails.FirstOrDefault(l => string.Equals(l.lightShift.ToString(), lightShift.ToString(), StringComparison.OrdinalIgnoreCase));

        return foundDetails;
    }
}


// 标记为可序列化的灯光细节类
[System.Serializable]
public class LightDetails
{
    // 灯光转换类型
    public LightShift lightShift;

    // 灯光颜色
    public Color lightColor;

    // 灯光强度
    public float lightAmount;

}


// 定义灯光转换类型的枚举
public enum LightShift
{
    Forenoon, //上午（早晨）
    Morning,// 早晨
    Noon,//中午
    Afternoon,// 下午
    Night,//晚上
    Midnight, //午夜（深夜）
}
