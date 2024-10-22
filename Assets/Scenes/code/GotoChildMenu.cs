using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ��ת���Ӳ˵�
public class GotoChildMenu : MonoBehaviour
{
    public GameObject btnLoad;  // ���ذ�ť����Ϸ����

    // ���ü��ذ�ť
    public void enableLoadButton()
    {
        btnLoad = transform.GetChild(5).gameObject;  // ��ȡ��6���Ӷ���������0��ʼ����Ϊ���ذ�ť
        btnLoad.SetActive(true);  // ������ذ�ť

        // ��ȡȫ�ֱ��������ߵı����������������ȡ��ұ��������б�
        List<PlayerData> dataList = Global.instance.gameObject.GetComponent<LocalData>().getPlayerLocalData();

        // ѭ���������������б�
        for (int i = 0; i < dataList.Count; i++)
        {
            var obj = transform.GetChild(5).GetChild(1).GetChild(i + 1).gameObject;  // ��ȡ���ذ�ť�µ��Ӷ�����Ϊ�浵��
            obj.SetActive(true);  // ����浵��
            obj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = dataList[i].dateTime;  // ���ô浵���е������ı�
            obj.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = dataList[i].sexVal.ToString();  // ���ô浵���е��Ա��ı�
            obj.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = dataList[i].level.ToString();  // ���ô浵���еĵȼ��ı�
        }
    }

    // ���·���
    private void Update()
    {
        // ������ذ�ť��Ϊ���Ұ�����Escape��
        if (btnLoad != null && Input.GetKeyDown(KeyCode.Escape))
        {
            btnLoad.SetActive(false);  // ���ؼ��ذ�ť
            btnLoad = null;  // �����ذ�ť�ÿ�
        }
    }
}
