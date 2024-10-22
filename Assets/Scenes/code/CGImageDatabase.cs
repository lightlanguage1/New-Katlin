using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class CGImageDatabase : MonoBehaviour
{
    public static event Action<Dictionary<string, int>> CGImagesLoaded; // �����¼�������֧��ֱ��ʹ��ͼ������

    private const string databaseFileName = "Karryn";
    private string databasePath;
    private Dictionary<string, int> imageToIdMap; // ͼ�����Ƶ�ID��ӳ���ֵ�
    private int nextAvailableId = 1;

    private void Start()
    {
        databasePath = Path.Combine(Application.streamingAssetsPath, databaseFileName);
        Debug.Log($"���ݿ�·��: {databasePath}");

        if (!File.Exists(databasePath))
        {
            Debug.LogError("���ݿ��ļ�������");
            return;
        }

        LoadAllCGImages();
        CGImagesLoaded?.Invoke(imageToIdMap); // �����¼�
    }

    private void LoadAllCGImages()
    {
        imageToIdMap = new Dictionary<string, int>(); // ��ʼ��ӳ���ֵ�
        Debug.Log("��ʼ��ͼ�����Ƶ�IDӳ���ֵ䡣");

        try
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                Debug.Log("�ɹ������ݿ����ӡ�");

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT body, rightArm, chest, leftArm, clothes, head, eyebrows, eyes, mouth, accessories, weapon, effects FROM map_images";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        Debug.Log("�ɹ�ִ��SQL���");

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

                Debug.Log($"���Ƶ�IDӳ���ֵ��е�ID����: {imageToIdMap.Count}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("����CGͼ������ʱ�����쳣��" + ex.Message);
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
            Debug.Log($"Ϊͼ�� {cleanImageName} ������ ID {imageToIdMap[cleanImageName]}");
        }
        else
        {
            Debug.LogWarning($"{cleanImageName} �Ѵ��ڣ������ظ����䡣");
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
        return imageToIdMap; // ����ӳ���ֵ�
    }
}
