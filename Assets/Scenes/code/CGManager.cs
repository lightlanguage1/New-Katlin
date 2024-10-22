using System.Collections.Generic;
using UnityEngine;
using System;

public class CGManager : Singleton<CGManager>
{
    private Dictionary<string, int> cgImageCache; // ����Ϊ�ַ�����ID��ӳ��
    private Dictionary<int, CGImage> cgImagesById; // ���ڴ洢ID��CGImage��ӳ��

    private new void Awake()
    {
        base.Awake(); // ���û���ĳ�ʼ���߼�
        CGImageDatabase.CGImagesLoaded += OnCGImagesLoaded; // ����CGͼ�����ݼ�������¼�
        Debug.Log("CG �������ѻ��ѡ�");
    }

    private void OnDestroy()
    {
        CGImageDatabase.CGImagesLoaded -= OnCGImagesLoaded; // ȡ�������¼�
        Debug.Log("CG �����������١�");
    }

    private void OnCGImagesLoaded(Dictionary<string, int> loadedCGImages)
    {
        cgImageCache = loadedCGImages; // �����ݿ��ȡͼ��ӳ��
        cgImagesById = new Dictionary<int, CGImage>(); // ��ʼ��ID��CGImage��ӳ��

        // ���������������cgImagesById�ֵ�
        // ����:
        foreach (var kvp in loadedCGImages)
        {
            int id = kvp.Value;
            // ������Ҫ�����ݿ�������ط���ȡCGImage������䵽cgImagesById��
            // cgImagesById[id] = new CGImage { body = "your_body_image_path" }; // ʾ��
        }

        Debug.Log("�ɹ��� CGImageDatabase ��ȡ CG ͼ�񻺴档�����е�ͼ��������" + cgImageCache.Count);
    }

    public CGImage GetCGImageById(int id)
    {
        if (cgImagesById.TryGetValue(id, out CGImage cgImage))
        {
            return cgImage; // ���ض�Ӧ��CGImage����
        }
        else
        {
            Debug.LogWarning($"δ�ҵ� ID Ϊ {id} �� CG ͼ��");
            return null; // ���߿��Է���Ĭ��ֵ
        }
    }
}
