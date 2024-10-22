using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// 本地数据管理器
public class LocalData : MonoBehaviour
{
    private BinaryFormatter binaryFormatter;    // 二进制格式化器
    private string saveSlotDir;                 // 存档目录
    public string[] saveSlotPos;                // 存档文件路径数组

    private void Awake()
    {
        binaryFormatter = new BinaryFormatter();
        saveSlotDir = Application.persistentDataPath + "/" + "SaveSlot";  // 构建存档目录路径
        saveSlotPos = new string[3];
        for (int i = 0; i < saveSlotPos.Length; i++)
        {
            saveSlotPos[i] = "save" + i.ToString() + ".save";  // 构建存档文件路径
        }
    }

    // 获取玩家本地数据列表
    public List<PlayerData> getPlayerLocalData()
    {
        List<PlayerData> playerDataList = new List<PlayerData>();
        for (int j = 0; j < 3; j++)
        {
            if (Load(j))
            {
                Debug.Log("读取存档槽 " + Global.instance.playerData.num.ToString() + " 成功！");
                playerDataList.Add(Global.instance.playerData);
            }
            else
            {
                Debug.Log("读取存档槽 " + Global.instance.playerData.num.ToString() + " 失败！");
            }
        }
        return playerDataList;
    }

    // 存储数据到指定存档槽
    public void Save(int num)   // num表示存档槽编号
    {
        if (Directory.Exists(saveSlotDir))
        {
            Debug.Log("发现存档目录!");
        }
        else
        {
            Debug.Log("未找到存档目录!");
            Directory.CreateDirectory(saveSlotDir);
            Debug.Log("目录已创建，路径为" + saveSlotDir);
        }

        // 记录当前日期时间
        DateTime curDateTime = DateTime.Now;
        string formatTime = curDateTime.ToString("yyyy-MM-dd hh:mm:ss");

        // 更新全局变量管理者中的玩家数据
        Global.instance.playerData.num = num;                               // 存档槽编号
        Global.instance.playerData.level = PlayerManager.instance.level;    // 玩家等级
        Global.instance.playerData.sexVal = (int)PlayerManager.instance.curHappyVal;  // 玩家性别值
        Global.instance.playerData.x = GameObject.Find("Karryn").transform.localPosition.x;  // 玩家在X轴上的本地坐标
        Global.instance.playerData.y = GameObject.Find("Karryn").transform.localPosition.y;  // 玩家在Y轴上的本地坐标
        Global.instance.playerData.z = GameObject.Find("Karryn").transform.localPosition.z;  // 玩家在Z轴上的本地坐标

        //需求； 桌面创建一个日志目录并且在该目录内创建 yyyy-mm-dd_log.txt 这样的文件最多有10个并且保存的最多字节是xxxx 
        // 创建文件，将对象序列化为二进制写入文件
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        FileStream fStream = File.Create(playerSaveSlot);
        binaryFormatter.Serialize(fStream, Global.instance.playerData);
        Debug.Log("存档完成，存档路径为" + playerSaveSlot);
        fStream.Close();

        // 更新敌人数据到数据库
        DbAccess db = GetComponent<DbAccess>();
        db.OpenDB(Global.instance.databasePath);
        string[] clos = { "Name", "PositionX", "PositionY", "PositionZ", "IsLive" };

        // 获取场景内所有敌人，包括活着的和已阵亡的
        List<NPC> npcList = new List<NPC>();
        var parent = GameObject.Find("Enemies");
        string[] closVal = new string[parent.transform.childCount * clos.Length];
        npcList.AddRange(FindObjectsByType<NPC>(FindObjectsSortMode.None));

        foreach (var pair in Global.instance.diedDic)
        {
            NPC npc = pair.Value.GetComponent<NPC>();
            npcList.Add(npc);
        }

        for (int i = 0; i < closVal.Length / clos.Length; i++)
        {
            closVal[i * clos.Length] = "'" + npcList[i].gameObject.name + "'";
            closVal[i * clos.Length + 1] = npcList[i].transform.localPosition.x.ToString();
            closVal[i * clos.Length + 2] = npcList[i].transform.localPosition.y.ToString();
            closVal[i * clos.Length + 3] = npcList[i].transform.localPosition.z.ToString();
            closVal[i * clos.Length + 4] = npcList[i].gameObject.activeSelf ? "1" : "0";
            db.UpdateInto("EnemyInfo", clos, closVal, clos.Length, "ID", i.ToString());
        }
        db.CloseSqlConnection();
    }

    // 从指定存档槽加载数据
    private bool Load(int num)
    {
        if (!Directory.Exists(saveSlotDir))
        {
            Debug.Log("存档目录未创建，无法读取!");
            return false;
        }

        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        if (!File.Exists(playerSaveSlot))
        {
            Debug.Log("存档 " + playerSaveSlot + " 不存在，无法读取!");
            return false;
        }

        FileStream file = File.Open(playerSaveSlot, FileMode.Open);
        Global.instance.playerData = (PlayerData)binaryFormatter.Deserialize(file);
        Debug.Log("存档 " + playerSaveSlot + " 读取成功!");
        file.Close();
        return true;
    }

    // 点击事件处理方法（仅用于测试，实际应根据玩家数据中的信息确定场景）
    public void onClicked()
    {
        if (Load(0))
        {
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
        }
        Debug.Log("加载失败!");
    }
}
