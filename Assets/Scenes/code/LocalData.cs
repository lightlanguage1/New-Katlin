using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// �������ݹ�����
public class LocalData : MonoBehaviour
{
    private BinaryFormatter binaryFormatter;    // �����Ƹ�ʽ����
    private string saveSlotDir;                 // �浵Ŀ¼
    public string[] saveSlotPos;                // �浵�ļ�·������

    private void Awake()
    {
        binaryFormatter = new BinaryFormatter();
        saveSlotDir = Application.persistentDataPath + "/" + "SaveSlot";  // �����浵Ŀ¼·��
        saveSlotPos = new string[3];
        for (int i = 0; i < saveSlotPos.Length; i++)
        {
            saveSlotPos[i] = "save" + i.ToString() + ".save";  // �����浵�ļ�·��
        }
    }

    // ��ȡ��ұ��������б�
    public List<PlayerData> getPlayerLocalData()
    {
        List<PlayerData> playerDataList = new List<PlayerData>();
        for (int j = 0; j < 3; j++)
        {
            if (Load(j))
            {
                Debug.Log("��ȡ�浵�� " + Global.instance.playerData.num.ToString() + " �ɹ���");
                playerDataList.Add(Global.instance.playerData);
            }
            else
            {
                Debug.Log("��ȡ�浵�� " + Global.instance.playerData.num.ToString() + " ʧ�ܣ�");
            }
        }
        return playerDataList;
    }

    // �洢���ݵ�ָ���浵��
    public void Save(int num)   // num��ʾ�浵�۱��
    {
        if (Directory.Exists(saveSlotDir))
        {
            Debug.Log("���ִ浵Ŀ¼!");
        }
        else
        {
            Debug.Log("δ�ҵ��浵Ŀ¼!");
            Directory.CreateDirectory(saveSlotDir);
            Debug.Log("Ŀ¼�Ѵ�����·��Ϊ" + saveSlotDir);
        }

        // ��¼��ǰ����ʱ��
        DateTime curDateTime = DateTime.Now;
        string formatTime = curDateTime.ToString("yyyy-MM-dd hh:mm:ss");

        // ����ȫ�ֱ����������е��������
        Global.instance.playerData.num = num;                               // �浵�۱��
        Global.instance.playerData.level = PlayerManager.instance.level;    // ��ҵȼ�
        Global.instance.playerData.sexVal = (int)PlayerManager.instance.curHappyVal;  // ����Ա�ֵ
        Global.instance.playerData.x = GameObject.Find("Karryn").transform.localPosition.x;  // �����X���ϵı�������
        Global.instance.playerData.y = GameObject.Find("Karryn").transform.localPosition.y;  // �����Y���ϵı�������
        Global.instance.playerData.z = GameObject.Find("Karryn").transform.localPosition.z;  // �����Z���ϵı�������

        //���� ���洴��һ����־Ŀ¼�����ڸ�Ŀ¼�ڴ��� yyyy-mm-dd_log.txt �������ļ������10�����ұ��������ֽ���xxxx 
        // �����ļ������������л�Ϊ������д���ļ�
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        FileStream fStream = File.Create(playerSaveSlot);
        binaryFormatter.Serialize(fStream, Global.instance.playerData);
        Debug.Log("�浵��ɣ��浵·��Ϊ" + playerSaveSlot);
        fStream.Close();

        // ���µ������ݵ����ݿ�
        DbAccess db = GetComponent<DbAccess>();
        db.OpenDB(Global.instance.databasePath);
        string[] clos = { "Name", "PositionX", "PositionY", "PositionZ", "IsLive" };

        // ��ȡ���������е��ˣ��������ŵĺ���������
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

    // ��ָ���浵�ۼ�������
    private bool Load(int num)
    {
        if (!Directory.Exists(saveSlotDir))
        {
            Debug.Log("�浵Ŀ¼δ�������޷���ȡ!");
            return false;
        }

        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        if (!File.Exists(playerSaveSlot))
        {
            Debug.Log("�浵 " + playerSaveSlot + " �����ڣ��޷���ȡ!");
            return false;
        }

        FileStream file = File.Open(playerSaveSlot, FileMode.Open);
        Global.instance.playerData = (PlayerData)binaryFormatter.Deserialize(file);
        Debug.Log("�浵 " + playerSaveSlot + " ��ȡ�ɹ�!");
        file.Close();
        return true;
    }

    // ����¼��������������ڲ��ԣ�ʵ��Ӧ������������е���Ϣȷ��������
    public void onClicked()
    {
        if (Load(0))
        {
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
        }
        Debug.Log("����ʧ��!");
    }
}
