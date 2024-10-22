using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using static CharacterActionRecorder;


// �������岿λö��
public enum BodyPart
{
    body,           // ����
    rightArm,       // �ұ�
    chest,          // �ز�
    leftArm,        // ���
    clothes,        // ��װ
    head,           // ͷ��
    eyebrows,       // üë
    eyes,           // �۾�
    mouth,          // ���
    accessories,    // ��Ʒ
    weapon,         // ����
    effects         // ��Ч
}

// �������ö��
public enum Expression
{
    neutral,    // ����
    happy,      // ����
    sad,        // ����
    angry,      // ����
    surprised   // ����
}

// �����ɫ״̬ö��
public enum CharacterState
{
    Normal,         // ����״̬
    UndressNormal,  // ���º������״̬
    InHeat,         // ��������״̬
    UndressInHeat,  // ���º����������״̬
    MapCG,          // ��ͼCG״̬
    BattleCG,       // ս��CG״̬
    TavernCG        // �ƹ�CG״̬
}

public class CGharacterController : MonoBehaviour
{
    public GameObject[] bodyParts; // �洢��ɫ���������岿λ
    public GameObject krmap;       // krmap �ǰ������� Image ����Ŀ�����
    private Image[] bodyPartImages; // �洢 krmap �µ����� Image ���

    private BodyPartState[] currentStates = new BodyPartState[System.Enum.GetValues(typeof(BodyPart)).Length];
    private CharacterState currentState = CharacterState.Normal;
    private int undressCount = 0;

    public CharacterActionRecorder characterActionRecorder;
    public CGManager cgManager;



    private void Start()
    {
        try
        {
            // ȷ�� CGManager ʵ������
            if (cgManager == null)
            {
                cgManager = FindObjectOfType<CGManager>();
                if (cgManager == null)
                {
                    Debug.LogError("No CGManager instance found in the scene.");
                    return;
                }
            }

            // ��ȡ krmap �µ����� Image ���
            bodyPartImages = krmap.GetComponentsInChildren<Image>();
            if (bodyPartImages == null || bodyPartImages.Length == 0)
            {
                Debug.LogError("No Image components found under krmap.");
                return;
            }

            // ���� CharacterActionRecorder ʵ�������� CGManager
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
            // ��ȡĬ�ϵ�ͼCG���Ƽ�¼
            var defaultMapCGPose = characterActionRecorder.GetDefaultMapCGPose(1);
            if (defaultMapCGPose == null)
            {
                Debug.LogWarning("Ĭ�ϵ�ͼCG���Ƽ�¼Ϊ�ա�");
                return;
            }

            // ��ȡӳ���ֵ��CGͼ�񻺴�
            var imageToIdMap = characterActionRecorder.GetImageToIdMap(); // ��ȡ���Ƶ�ID��ӳ��
            var cgImageCache = cgManager.GetCGImageCache(); // ��ȡCGͼ�񻺴�

            Debug.Log($"ӳ���ֵ��е�ID����: {imageToIdMap.Count}");
            Debug.Log($"CGͼ�񻺴��ֵ��е�ID����: {cgImageCache.Count}");

            foreach (var image in bodyPartImages)
            {
                var bodyPartName = image.gameObject.name;
                Debug.Log($"����ͼ��: {bodyPartName}");

                // ���Խ������岿λ����
                if (Enum.TryParse(bodyPartName, out BodyPart bodyPart) &&
                    defaultMapCGPose.TryGetValue(bodyPart, out var defaultImageName))
                {
                    if (!string.IsNullOrEmpty(defaultImageName))
                    {
                        // ��ȡ��Ӧ��ͼ��ID
                        if (imageToIdMap.TryGetValue(defaultImageName.ToLower(), out int imageId))
                        {
                            // ��CGͼ�񻺴��л�ȡͼ����Ϣ
                            if (cgImageCache.TryGetValue(imageId, out CGImage cgImage))
                            {
                                // ����ͼ��
                                Texture2D tex = LoadPNG(cgImage.body); // ʹ��CGImage�е�·������ͼ��
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
                                Debug.LogWarning($"δ�ҵ�ͼ��ID {imageId} �� CG ͼ�񻺴棡");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"δ�ҵ�ͼ������Ϊ {defaultImageName} ��ӳ��ID��");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"δ�ҵ����岿λ��Ĭ��ͼ�����ƣ����岿λ��{bodyPartName}��");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadMapCG �г����쳣: {ex.Message}\n{ex.StackTrace}");
        }*/
    }



    private void SetLayerToTransparent(string bodyPartName)
    {
        foreach (Image image in bodyPartImages)
        {
            if (image.gameObject.name == bodyPartName)
            {
                Color color = image.color;
                color.a = 0f; // ���� alpha Ϊ 0������ȫ͸��
                image.color = color;
                Debug.Log("�ѽ� " + bodyPartName + " ����Ϊ͸����");
                return;
            }
        }

        Debug.LogWarning("δ�ҵ�����Ϊ " + bodyPartName + " �����岿λ��");
    }

    // ���� PNG ͼƬ
    private Texture2D LoadPNG(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "img", "karryn", "map", fileName);
        Debug.Log($"���Լ���ͼ���ļ���{filePath}");

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                Debug.Log($"�ɹ�����ͼ���ļ���{filePath}");
                return tex;
            }
            else
            {
                Debug.LogError($"�޷�����ͼ�����ݣ�{filePath}���ļ������𻵡�");
                return null;
            }
        }
        else
        {
            Debug.LogError($"δ�ҵ�ͼ���ļ���{filePath}�������ļ�·�������ơ�");
            return null;
        }
    }







    public void SetCharacterState(CharacterState state)
        {
            currentState = state;
            // ����״̬����ty����
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
            // ���������ñ��飬����ı���ͺ��۾���״̬
            // ����Ը��ݾ�����Ҫ��ʵ���ⲿ���߼�
        }
    
    private void UpdateCharacter()
    {
        // ���½�ɫ����ۣ����ݵ�ǰ״̬������Ӧ��ͼ��
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
        // ����״̬�������´���
        switch (currentState)
        {
            case CharacterState.MapCG:
                undressCount = 6; // ��ͼCG�����´���Ϊ6��
                break;
            case CharacterState.BattleCG:
                undressCount = 9; // ս��CG�����´���Ϊ9��
                break;
            case CharacterState.TavernCG:
                undressCount = 8; // �ƹ�CG�����´���Ϊ8��
                break;
            default:
                undressCount = 0; // Ĭ�����´���Ϊ0��
                break;
        }
    }
}

// ��ʾÿ�����岿λ��״̬
[System.Serializable]
public class BodyPartState
{
    public int currentState = 0; // ��ǰ״̬��Ĭ��Ϊ0
}

// ���ڱ���ÿ�����岿λ����Ϣ
[System.Serializable]
public class BodyPartInfo : MonoBehaviour
{
    public BodyPart bodyPart; // ���岿λö��
}
