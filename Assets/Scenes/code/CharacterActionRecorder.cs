using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterActionRecorder
{
    private CGManager cgManager; // CG ͼ�������
    private Dictionary<int, Dictionary<BodyPart, string>> defaultMapCGPoses; // Ĭ�ϵ�ͼCG���Ƽ�¼
    private Dictionary<int, Dictionary<BodyPart, string>> defaultBattleCGPoses; // Ĭ��ս��CG���Ƽ�¼

    public event Action OnImageToIdMapInitialized; // ���Ƶ�IDӳ���ʼ������¼�

    public struct CGImageInfo
    {
        public string imageName; // ͼƬ����
        public string bodyPart; // ���岿λ
    }

    // ���캯��
    public CharacterActionRecorder(CGManager manager)
    {
        cgManager = manager;
        InitializeDefaultPoses();
    }

    // ��ʼ��Ĭ�����Ƽ�¼
    public void InitializeDefaultPoses()
    {
        defaultMapCGPoses = new Dictionary<int, Dictionary<BodyPart, string>>
        {
            { 1, GetDefaultMapCGPose(1) }, // ��ȡĬ�ϵ�ͼ CG ���Ƽ�¼
        };

        defaultBattleCGPoses = new Dictionary<int, Dictionary<BodyPart, string>>
        {
            { 1, GetDefaultBattleCGPose(1) }, // ��ȡĬ��ս�� CG ���Ƽ�¼
        };
    }

    // ��ȡĬ�ϵ�ͼCG����
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
        Debug.LogWarning("δ�ҵ� ID Ϊ " + id + " ��Ĭ�ϵ�ͼCG���Ƽ�¼");
        return null;
    }

    // ��ȡĬ��ս��CG����
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
        Debug.LogWarning("δ�ҵ� ID Ϊ " + id + " ��Ĭ��ս��CG���Ƽ�¼");
        return null;
    }

    /*    // ��ʼ�����Ƶ�IDӳ���ֵ�
        public void InitializeImageToIdMap()
        {
            // ��ȡ CGImageDatabase ��ʵ��
            CGImageDatabase cgImageDatabase = GameObject.FindObjectOfType<CGImageDatabase>();
            if (cgImageDatabase == null)
            {
                Debug.LogError("δ�ҵ� CGImageDatabase ʵ����");
                return; // ���û���ҵ�ʵ������������ʼ��
            }

            // �� CGImageDatabase ��ȡӳ���ֵ�
            var imageToIdMapFromDatabase = cgImageDatabase.GetImageToIdMap();
            if (imageToIdMapFromDatabase == null)
            {
                Debug.LogWarning("CGImageDatabase �е����Ƶ�IDӳ��Ϊ�ա�");
                return; // ���ӳ��Ϊ�գ�������ʼ��
            }

            // ʹ�����ݿ��е�ӳ���ʼ��������ֵ�
            imageToIdMap = new Dictionary<string, int>(imageToIdMapFromDatabase);

            // ����ӳ���ʼ������¼�
            OnImageToIdMapInitialized?.Invoke();

            foreach (var kvp in imageToIdMap)
            {
                Debug.Log($"ӳ��: {kvp.Key} -> {kvp.Value}");
            }
        }*/

    public CGImageInfo GetCGImageInfo(string imageName)
    {
        CGImageInfo imageInfo = new CGImageInfo();

        string lowerCaseImageName = imageName.ToLower().Replace(".png", "");

        var cgImageDatabase = GameObject.FindObjectOfType<CGImageDatabase>();
        if (cgImageDatabase != null)
        {
            // ����ͼ�����ƻ�ȡ ID
            if (cgImageDatabase.GetImageToIdMap().TryGetValue(lowerCaseImageName, out int id))
            {
                var cgImage = cgManager.GetCGImageById(id); // ���� ID ��ȡ CGImage
                if (cgImage != null)
                {
                    imageInfo.imageName = cgImage.body; // ������Ҫ�����������
                    imageInfo.bodyPart = cgImage.body;
                    Debug.Log($"�ɹ���ȡ����Ϊ {imageName} ��CGͼ����Ϣ: {cgImage.body}");
                }
                else
                {
                    Debug.LogWarning($"δ�ҵ� ID Ϊ {id} ��CGͼ��");
                }
            }
            else
            {
                Debug.LogWarning($"δ�ҵ�����Ϊ {imageName} ��ӳ��ID��");
            }
        }
        else
        {
            Debug.LogError("δ�ҵ� CGImageDatabase ʵ����");
        }

        return imageInfo;
    }


}
