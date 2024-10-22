using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 场景枚举值
public enum SceneEnumVal
{
    MainMenuScene = 0,      // 主菜单场景
    Main1L,                 // 主场景一层
    BattleScene,            // 战斗场景
    SexTransitionScene      // 性别过渡场景
}

// 全局变量管理者脚本
[DefaultExecutionOrder(2)]
public class Global : Singleton<Global>
{
    public bool isSuppress = false;                  // 是否抑制
    public bool isClear1L = false;                   // 是否清除一层
    public string battlePrevSceneName;              // 战斗前场景名称
    public string curMainSceneName;                 // 当前主场景名称

    public string mainScene1L = "main1L";           // 主场景一层名称
    public string mainScene2L = "main2L";           // 主场景二层名称
    public string mainScene3L = "main3L";           // 主场景三层名称

    [Header("敌人的种类")]
    public GameObject[] enemies;                    // 敌人游戏对象数组

    public RectTransform[] enemiesTransform;        // 敌人RectTransform数组

    public enum enemyName
    {
        Guard = 0,        // 卫兵
        Goblin,           // 哥布林
        Homeless,         // 流浪汉
        Lizardman,        // 蜥蜴人
        Nerd,             // 书呆子
        Orc,              // 兽人
        Prisoner,         // 囚犯
        Rogue,            // 盗贼
        Slime,            // 史莱姆
        Thug,             // 流氓
        Wolf,             // 狼人
        Yeti,             // 雪人
        Boss1L            // 一层boss
    }

    [Header("技能消耗")]
    public float xuli;                               // 蓄力

    [Header("战斗中的敌人")]
    public string battlePrevNpcName;                // 战斗前NPC名称

    public float enemy_x;
    public float enemy_y;
    public float enemy_z;

    [Header("已阵亡的敌人列表")]
    public Dictionary<string, GameObject> diedDic;  // 已阵亡的敌人字典，先死的先刷新，后死的后刷新

    //[Header("敌人的初始位置字典")]
    public Dictionary<string, Vector3> posDic;      // 敌人的初始位置字典，这个位置是要从数据库里读出来的

    [Header("管理场景中的敌人")]
    public GameObject totalEnemy;                   // 管理场景中的所有敌人的父对象

    [Header("玩家信息")]
    public PlayerData playerData;                   // 玩家信息

    public bool isWin;                              // 是否胜利
    public bool isTin;                              // 是否逃跑
    public bool inMainScene;                        // 是否在主场景中


    [Header("SQLite数据库文件路径")]
    public string databasePath = "Data Source=Assets/StreamingAssets/Karryn";  // SQLite数据库文件路径

    //声音类型
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

    //状态类型
    public enum StateType
    {
        IdleState,
        PatrolState,
        ChaseState
    }

    public enum EnemiesIndex    // 非战斗场景，层级窗口中敌人的索引
    {
        goblin,
        goblin1
    }

    private void Start()
    {
        // 从数据库把敌人的生死状态读取到该字典中，注意以下这些值仅作测试，实际要通过数据库/本地读取

        // 初始化已阵亡的敌人字典
        diedDic = new Dictionary<string, GameObject>();
        // 初始化敌人的初始位置字典
        posDic = new Dictionary<string, Vector3>();

        // 设置敌人初始位置
        enemy_x = -1f;
        enemy_y = -18.453f;
        enemy_z = 0f;
        // 新变量：初始化逃跑状态
        isTin = false;
        // 初始化玩家数据
        playerData = new PlayerData();
        // 初始化主场景状态
        inMainScene = false;

        // 设置玩家数据的测试用例
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
    public int num;           // 存档位编号
    public string name;       // 玩家名称
    public float x;           // 玩家X坐标
    public float y;           // 玩家Y坐标
    public float z;           // 玩家Z坐标
    public int level;         // 玩家等级
    public int sexVal;        // 玩家性别
    public int money;         // 玩家金钱
    public string dateTime;   // 存档日期时间
    public string nowTime;    // 白天/夜晚
}
