using System.Collections.Generic;
using UnityEngine;
using System;

public class CGManager : Singleton<CGManager>
{
    private Dictionary<string, int> cgImageCache; // 更新为字符串到ID的映射
    private Dictionary<int, CGImage> cgImagesById; // 用于存储ID到CGImage的映射

    private new void Awake()
    {
        base.Awake(); // 调用基类的初始化逻辑
        CGImageDatabase.CGImagesLoaded += OnCGImagesLoaded; // 订阅CG图像数据加载完成事件
        Debug.Log("CG 管理器已唤醒。");
    }

    private void OnDestroy()
    {
        CGImageDatabase.CGImagesLoaded -= OnCGImagesLoaded; // 取消订阅事件
        Debug.Log("CG 管理器已销毁。");
    }

    private void OnCGImagesLoaded(Dictionary<string, int> loadedCGImages)
    {
        cgImageCache = loadedCGImages; // 从数据库获取图像映射
        cgImagesById = new Dictionary<int, CGImage>(); // 初始化ID到CGImage的映射

        // 假设你在这里填充cgImagesById字典
        // 例如:
        foreach (var kvp in loadedCGImages)
        {
            int id = kvp.Value;
            // 这里需要从数据库或其他地方获取CGImage对象并填充到cgImagesById中
            // cgImagesById[id] = new CGImage { body = "your_body_image_path" }; // 示例
        }

        Debug.Log("成功从 CGImageDatabase 获取 CG 图像缓存。缓存中的图像数量：" + cgImageCache.Count);
    }

    public CGImage GetCGImageById(int id)
    {
        if (cgImagesById.TryGetValue(id, out CGImage cgImage))
        {
            return cgImage; // 返回对应的CGImage对象
        }
        else
        {
            Debug.LogWarning($"未找到 ID 为 {id} 的 CG 图像！");
            return null; // 或者可以返回默认值
        }
    }
}
