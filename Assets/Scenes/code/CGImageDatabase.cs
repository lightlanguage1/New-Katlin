using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class CGImageDatabase : MonoBehaviour
{
    public static event Action<Dictionary<string, int>> CGImagesLoaded; // 更新事件类型以支持直接使用图像名称

    private const string databaseFileName = "Karryn";
    private string databasePath;
    private Dictionary<string, int> imageToIdMap; // 图像名称到ID的映射字典
    private int nextAvailableId = 1;

    private void Start()
    {
        databasePath = Path.Combine(Application.streamingAssetsPath, databaseFileName);
        Debug.Log($"数据库路径: {databasePath}");

        if (!File.Exists(databasePath))
        {
            Debug.LogError("数据库文件不存在");
            return;
        }

        LoadAllCGImages();
        CGImagesLoaded?.Invoke(imageToIdMap); // 触发事件
    }

    private void LoadAllCGImages()
    {
        imageToIdMap = new Dictionary<string, int>(); // 初始化映射字典
        Debug.Log("初始化图像名称到ID映射字典。");

        try
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                Debug.Log("成功打开数据库连接。");

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT body, rightArm, chest, leftArm, clothes, head, eyebrows, eyes, mouth, accessories, weapon, effects FROM map_images";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        Debug.Log("成功执行SQL命令。");

                        while (reader.Read())
                        {
                            AddImagePart(reader.IsDBNull(0) ? null : reader.GetString(0));
                            AddImagePart(reader.IsDBNull(1) ? null : reader.GetString(1));
                            AddImagePart(reader.IsDBNull(2) ? null : reader.GetString(2));
                            AddImagePart(reader.IsDBNull(3) ? null : reader.GetString(3));
                            AddImagePart(reader.IsDBNull(4) ? null : reader.GetString(4));
                            AddImagePart(reader.IsDBNull(5) ? null : reader.GetString(5));
                            AddImagePart(reader.IsDBNull(6) ? null : reader.GetString(6));
                            AddImagePart(reader.IsDBNull(7) ? null : reader.GetString(7));
                            AddImagePart(reader.IsDBNull(8) ? null : reader.GetString(8));
                            AddImagePart(reader.IsDBNull(9) ? null : reader.GetString(9));
                            AddImagePart(reader.IsDBNull(10) ? null : reader.GetString(10));
                            AddImagePart(reader.IsDBNull(11) ? null : reader.GetString(11));
                        }
                    }
                }

                Debug.Log($"名称到ID映射字典中的ID总数: {imageToIdMap.Count}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("加载CG图像数据时发生异常：" + ex.Message);
        }
    }

    private void AddImagePart(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
            return;

        string cleanImageName = GetFileNameWithExtension(imageName).ToLower();

        if (!imageToIdMap.ContainsKey(cleanImageName))
        {
            imageToIdMap[cleanImageName] = nextAvailableId++;
            Debug.Log($"为图像 {cleanImageName} 分配了 ID {imageToIdMap[cleanImageName]}");
        }
        else
        {
            Debug.LogWarning($"{cleanImageName} 已存在，跳过重复分配。");
        }
    }

    private string GetFileNameWithExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".png";
        }

        return fileName;
    }
    public Dictionary<string, int> GetImageToIdMap()
    {
        return imageToIdMap; // 返回映射字典
    }
}
