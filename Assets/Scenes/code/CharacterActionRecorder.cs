using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterActionRecorder
{
    private CGManager cgManager; // CG 图像管理器
    private Dictionary<int, Dictionary<BodyPart, string>> defaultMapCGPoses; // 默认地图CG姿势记录
    private Dictionary<int, Dictionary<BodyPart, string>> defaultBattleCGPoses; // 默认战斗CG姿势记录

    public event Action OnImageToIdMapInitialized; // 名称到ID映射初始化完成事件

    public struct CGImageInfo
    {
        public string imageName; // 图片名称
        public string bodyPart; // 身体部位
    }

    // 构造函数
    public CharacterActionRecorder(CGManager manager)
    {
        cgManager = manager;
        InitializeDefaultPoses();
    }

    // 初始化默认姿势记录
    public void InitializeDefaultPoses()
    {
        defaultMapCGPoses = new Dictionary<int, Dictionary<BodyPart, string>>
        {
            { 1, GetDefaultMapCGPose(1) }, // 获取默认地图 CG 姿势记录
        };

        defaultBattleCGPoses = new Dictionary<int, Dictionary<BodyPart, string>>
        {
            { 1, GetDefaultBattleCGPose(1) }, // 获取默认战斗 CG 姿势记录
        };
    }

    // 获取默认地图CG姿势
    public Dictionary<BodyPart, string> GetDefaultMapCGPose(int id)
    {
        if (id == 1)
        {
            return new Dictionary<BodyPart, string>
            {
                { BodyPart.body, "body_1" },
                { BodyPart.rightArm, "rightArm_2" },
                { BodyPart.chest, "chest_reg_1" },
                { BodyPart.leftArm, "leftArm_1" },
                { BodyPart.clothes, null },
                { BodyPart.head, "head_down_1" },
                { BodyPart.eyebrows, "eyebrows_normal_nico3" },
                { BodyPart.eyes, "eyes_down_sita1" },
                { BodyPart.mouth, "mouth_down_mu1" },
                { BodyPart.accessories, "hat_down_1" },
                { BodyPart.weapon, null },
                { BodyPart.effects, null }
            };
        }
        Debug.LogWarning("未找到 ID 为 " + id + " 的默认地图CG姿势记录");
        return null;
    }

    // 获取默认战斗CG姿势
    public Dictionary<BodyPart, string> GetDefaultBattleCGPose(int id)
    {
        if (id == 1)
        {
            return new Dictionary<BodyPart, string>
            {
                { BodyPart.body, "body_1" },
                { BodyPart.rightArm, "rightArm_2" },
                { BodyPart.chest, "chest_reg_1" },
                { BodyPart.leftArm, "leftArm_1" },
                { BodyPart.clothes, null },
                { BodyPart.head, "head_down_1" },
                { BodyPart.eyebrows, "eyebrows_normal_nico3" },
                { BodyPart.eyes, "eyes_down_sita1" },
                { BodyPart.mouth, "mouth_down_mu1" },
                { BodyPart.accessories, "hat_down_1" },
                { BodyPart.weapon, null },
                { BodyPart.effects, null }
            };
        }
        Debug.LogWarning("未找到 ID 为 " + id + " 的默认战斗CG姿势记录");
        return null;
    }

    /*    // 初始化名称到ID映射字典
        public void InitializeImageToIdMap()
        {
            // 获取 CGImageDatabase 的实例
            CGImageDatabase cgImageDatabase = GameObject.FindObjectOfType<CGImageDatabase>();
            if (cgImageDatabase == null)
            {
                Debug.LogError("未找到 CGImageDatabase 实例。");
                return; // 如果没有找到实例，则跳过初始化
            }

            // 从 CGImageDatabase 获取映射字典
            var imageToIdMapFromDatabase = cgImageDatabase.GetImageToIdMap();
            if (imageToIdMapFromDatabase == null)
            {
                Debug.LogWarning("CGImageDatabase 中的名称到ID映射为空。");
                return; // 如果映射为空，跳过初始化
            }

            // 使用数据库中的映射初始化本类的字典
            imageToIdMap = new Dictionary<string, int>(imageToIdMapFromDatabase);

            // 触发映射初始化完成事件
            OnImageToIdMapInitialized?.Invoke();

            foreach (var kvp in imageToIdMap)
            {
                Debug.Log($"映射: {kvp.Key} -> {kvp.Value}");
            }
        }*/

    public CGImageInfo GetCGImageInfo(string imageName)
    {
        CGImageInfo imageInfo = new CGImageInfo();

        string lowerCaseImageName = imageName.ToLower().Replace(".png", "");

        var cgImageDatabase = GameObject.FindObjectOfType<CGImageDatabase>();
        if (cgImageDatabase != null)
        {
            // 根据图像名称获取 ID
            if (cgImageDatabase.GetImageToIdMap().TryGetValue(lowerCaseImageName, out int id))
            {
                var cgImage = cgManager.GetCGImageById(id); // 根据 ID 获取 CGImage
                if (cgImage != null)
                {
                    imageInfo.imageName = cgImage.body; // 根据需要填充其他部分
                    imageInfo.bodyPart = cgImage.body;
                    Debug.Log($"成功获取名称为 {imageName} 的CG图像信息: {cgImage.body}");
                }
                else
                {
                    Debug.LogWarning($"未找到 ID 为 {id} 的CG图像！");
                }
            }
            else
            {
                Debug.LogWarning($"未找到名称为 {imageName} 的映射ID！");
            }
        }
        else
        {
            Debug.LogError("未找到 CGImageDatabase 实例。");
        }

        return imageInfo;
    }


}
