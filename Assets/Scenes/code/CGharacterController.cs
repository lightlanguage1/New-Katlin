using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using static CharacterActionRecorder;


// 定义身体部位枚举
public enum BodyPart
{
    body,           // 身体
    rightArm,       // 右臂
    chest,          // 胸部
    leftArm,        // 左臂
    clothes,        // 服装
    head,           // 头部
    eyebrows,       // 眉毛
    eyes,           // 眼睛
    mouth,          // 嘴巴
    accessories,    // 饰品
    weapon,         // 武器
    effects         // 特效
}

// 定义表情枚举
public enum Expression
{
    neutral,    // 中立
    happy,      // 开心
    sad,        // 伤心
    angry,      // 生气
    surprised   // 惊讶
}

// 定义角色状态枚举
public enum CharacterState
{
    Normal,         // 正常状态
    UndressNormal,  // 脱衣后的正常状态
    InHeat,         // 性欲高涨状态
    UndressInHeat,  // 脱衣后的性欲高涨状态
    MapCG,          // 地图CG状态
    BattleCG,       // 战斗CG状态
    TavernCG        // 酒馆CG状态
}

public class CGharacterController : MonoBehaviour
{
    public GameObject[] bodyParts; // 存储角色的所有身体部位
    public GameObject krmap;       // krmap 是包含所有 Image 组件的空物体
    private Image[] bodyPartImages; // 存储 krmap 下的所有 Image 组件

    private BodyPartState[] currentStates = new BodyPartState[System.Enum.GetValues(typeof(BodyPart)).Length];
    private CharacterState currentState = CharacterState.Normal;
    private int undressCount = 0;

    public CharacterActionRecorder characterActionRecorder;
    public CGManager cgManager;



    private void Start()
    {
        try
        {
            // 确保 CGManager 实例存在
            if (cgManager == null)
            {
                cgManager = FindObjectOfType<CGManager>();
                if (cgManager == null)
                {
                    Debug.LogError("No CGManager instance found in the scene.");
                    return;
                }
            }

            // 获取 krmap 下的所有 Image 组件
            bodyPartImages = krmap.GetComponentsInChildren<Image>();
            if (bodyPartImages == null || bodyPartImages.Length == 0)
            {
                Debug.LogError("No Image components found under krmap.");
                return;
            }

            // 创建 CharacterActionRecorder 实例并传递 CGManager
            characterActionRecorder = new CharacterActionRecorder(cgManager);

            LoadMapCG();

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception in Start: {ex.Message}");
        }
    }

    private void LoadMapCG()
    {
/*        try
        {
            // 获取默认地图CG姿势记录
            var defaultMapCGPose = characterActionRecorder.GetDefaultMapCGPose(1);
            if (defaultMapCGPose == null)
            {
                Debug.LogWarning("默认地图CG姿势记录为空。");
                return;
            }

            // 获取映射字典和CG图像缓存
            var imageToIdMap = characterActionRecorder.GetImageToIdMap(); // 获取名称到ID的映射
            var cgImageCache = cgManager.GetCGImageCache(); // 获取CG图像缓存

            Debug.Log($"映射字典中的ID总数: {imageToIdMap.Count}");
            Debug.Log($"CG图像缓存字典中的ID总数: {cgImageCache.Count}");

            foreach (var image in bodyPartImages)
            {
                var bodyPartName = image.gameObject.name;
                Debug.Log($"处理图层: {bodyPartName}");

                // 尝试解析身体部位名称
                if (Enum.TryParse(bodyPartName, out BodyPart bodyPart) &&
                    defaultMapCGPose.TryGetValue(bodyPart, out var defaultImageName))
                {
                    if (!string.IsNullOrEmpty(defaultImageName))
                    {
                        // 获取对应的图像ID
                        if (imageToIdMap.TryGetValue(defaultImageName.ToLower(), out int imageId))
                        {
                            // 从CG图像缓存中获取图像信息
                            if (cgImageCache.TryGetValue(imageId, out CGImage cgImage))
                            {
                                // 加载图像
                                Texture2D tex = LoadPNG(cgImage.body); // 使用CGImage中的路径加载图像
                                if (tex != null)
                                {
                                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                                    image.sprite = sprite;
                                }
                                else
                                {
                                    SetLayerToTransparent(bodyPartName);
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"未找到图像ID {imageId} 的 CG 图像缓存！");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"未找到图像名称为 {defaultImageName} 的映射ID。");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"未找到身体部位或默认图像名称，身体部位：{bodyPartName}。");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadMapCG 中出现异常: {ex.Message}\n{ex.StackTrace}");
        }*/
    }



    private void SetLayerToTransparent(string bodyPartName)
    {
        foreach (Image image in bodyPartImages)
        {
            if (image.gameObject.name == bodyPartName)
            {
                Color color = image.color;
                color.a = 0f; // 设置 alpha 为 0，即完全透明
                image.color = color;
                Debug.Log("已将 " + bodyPartName + " 设置为透明。");
                return;
            }
        }

        Debug.LogWarning("未找到名称为 " + bodyPartName + " 的身体部位。");
    }

    // 加载 PNG 图片
    private Texture2D LoadPNG(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "img", "karryn", "map", fileName);
        Debug.Log($"尝试加载图像文件：{filePath}");

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                Debug.Log($"成功加载图像文件：{filePath}");
                return tex;
            }
            else
            {
                Debug.LogError($"无法加载图像数据：{filePath}，文件可能损坏。");
                return null;
            }
        }
        else
        {
            Debug.LogError($"未找到图像文件：{filePath}，请检查文件路径或名称。");
            return null;
        }
    }







    public void SetCharacterState(CharacterState state)
        {
            currentState = state;
            // 根据状态更新ty次数
            UpdateUndressCount();
            UpdateCharacter();
        }

        public void SetBodyPartState(BodyPart bodyPart, CharacterState state)
        {
            currentStates[(int)bodyPart].currentState = (int)state;
            UpdateCharacter();
        }





       public void SetExpression(Expression expression)
        {
            // 在这里设置表情，例如改变嘴巴和眼睛的状态
            // 你可以根据具体需要来实现这部分逻辑
        }
    
    private void UpdateCharacter()
    {
        // 更新角色的外观，根据当前状态激活相应的图层
        foreach (BodyPart bodyPart in Enum.GetValues(typeof(BodyPart)))
        {
            int stateIndex = currentStates[(int)bodyPart].currentState;
            for (int i = 0; i < bodyParts.Length; i++)
            {
                if (bodyParts[i].GetComponent<BodyPartInfo>().bodyPart == bodyPart)
                {
                    bodyParts[i].SetActive(i == stateIndex);
                }
            }
        }
    }


    private void UpdateUndressCount()
    {
        // 根据状态更新脱衣次数
        switch (currentState)
        {
            case CharacterState.MapCG:
                undressCount = 6; // 地图CG，脱衣次数为6次
                break;
            case CharacterState.BattleCG:
                undressCount = 9; // 战斗CG，脱衣次数为9次
                break;
            case CharacterState.TavernCG:
                undressCount = 8; // 酒馆CG，脱衣次数为8次
                break;
            default:
                undressCount = 0; // 默认脱衣次数为0次
                break;
        }
    }
}

// 表示每个身体部位的状态
[System.Serializable]
public class BodyPartState
{
    public int currentState = 0; // 当前状态，默认为0
}

// 用于保存每个身体部位的信息
[System.Serializable]
public class BodyPartInfo : MonoBehaviour
{
    public BodyPart bodyPart; // 身体部位枚举
}
