using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ����ö��ֵ
public enum SceneEnumVal
{
    MainMenuScene = 0,      // ���˵�����
    Main1L,                 // ������һ��
    BattleScene,            // ս������
    SexTransitionScene      // �Ա���ɳ���
}

// ȫ�ֱ��������߽ű�
[DefaultExecutionOrder(2)]
public class Global : Singleton<Global>
{
    public bool isSuppress = false;                  // �Ƿ�����
    public bool isClear1L = false;                   // �Ƿ����һ��
    public string battlePrevSceneName;              // ս��ǰ��������
    public string curMainSceneName;                 // ��ǰ����������

    public string mainScene1L = "main1L";           // ������һ������
    public string mainScene2L = "main2L";           // ��������������
    public string mainScene3L = "main3L";           // ��������������

    [Header("���˵�����")]
    public GameObject[] enemies;                    // ������Ϸ��������

    public RectTransform[] enemiesTransform;        // ����RectTransform����

    public enum enemyName
    {
        Guard = 0,        // ����
        Goblin,           // �粼��
        Homeless,         // ���˺�
        Lizardman,        // ������
        Nerd,             // �����
        Orc,              // ����
        Prisoner,         // ����
        Rogue,            // ����
        Slime,            // ʷ��ķ
        Thug,             // ��å
        Wolf,             // ����
        Yeti,             // ѩ��
        Boss1L            // һ��boss
    }

    [Header("��������")]
    public float xuli;                               // ����

    [Header("ս���еĵ���")]
    public string battlePrevNpcName;                // ս��ǰNPC����

    public float enemy_x;
    public float enemy_y;
    public float enemy_z;

    [Header("�������ĵ����б�")]
    public Dictionary<string, GameObject> diedDic;  // �������ĵ����ֵ䣬��������ˢ�£������ĺ�ˢ��

    //[Header("���˵ĳ�ʼλ���ֵ�")]
    public Dictionary<string, Vector3> posDic;      // ���˵ĳ�ʼλ���ֵ䣬���λ����Ҫ�����ݿ����������

    [Header("�������еĵ���")]
    public GameObject totalEnemy;                   // �������е����е��˵ĸ�����

    [Header("�����Ϣ")]
    public PlayerData playerData;                   // �����Ϣ

    public bool isWin;                              // �Ƿ�ʤ��
    public bool isTin;                              // �Ƿ�����
    public bool inMainScene;                        // �Ƿ�����������


    [Header("SQLite���ݿ��ļ�·��")]
    public string databasePath = "Data Source=Assets/StreamingAssets/Karryn";  // SQLite���ݿ��ļ�·��

    //��������
    public enum SoundType
    {
        None,
        StartMenu,
        Main1L,
        Battle,
        MoveCursor,
        Click,
        Walk,
        Daji,
        Xuli
    }

    //״̬����
    public enum StateType
    {
        IdleState,
        PatrolState,
        ChaseState
    }

    public enum EnemiesIndex    // ��ս���������㼶�����е��˵�����
    {
        goblin,
        goblin1
    }

    private void Start()
    {
        // �����ݿ�ѵ��˵�����״̬��ȡ�����ֵ��У�ע��������Щֵ�������ԣ�ʵ��Ҫͨ�����ݿ�/���ض�ȡ

        // ��ʼ���������ĵ����ֵ�
        diedDic = new Dictionary<string, GameObject>();
        // ��ʼ�����˵ĳ�ʼλ���ֵ�
        posDic = new Dictionary<string, Vector3>();

        // ���õ��˳�ʼλ��
        enemy_x = -1f;
        enemy_y = -18.453f;
        enemy_z = 0f;
        // �±�������ʼ������״̬
        isTin = false;
        // ��ʼ���������
        playerData = new PlayerData();
        // ��ʼ��������״̬
        inMainScene = false;

        // ����������ݵĲ�������
        playerData = new PlayerData
        {
            name = "Karryn",
            x = -8.18f,
            y = -18.44f,
            z = 0f,
            sexVal = 0,
            num = 0,
            level = 1
        };
    }


    private void Update()
    {
    }
}

[System.Serializable]
public class PlayerData
{
    public int num;           // �浵λ���
    public string name;       // �������
    public float x;           // ���X����
    public float y;           // ���Y����
    public float z;           // ���Z����
    public int level;         // ��ҵȼ�
    public int sexVal;        // ����Ա�
    public int money;         // ��ҽ�Ǯ
    public string dateTime;   // �浵����ʱ��
    public string nowTime;    // ����/ҹ��
}
