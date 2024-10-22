using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BagBar : MonoBehaviour
{
    private RectTransform fouces;
    private RectTransform cursor;
    private List<GameObject> optionList;    //选项
    private Vector3 offset;
    private Vector3 cursorOffset;
    private Coroutine cor;
    private bool isSelect;  //是否在背包中选择了选项
    private LocalData localData;
    private int time;   //按下esc键的次数
    private karryn karryn;


    private void Start()
    {
        localData = Global.instance.GetComponent<LocalData>();
        isSelect = false;
        time = 0;
        offset = new Vector3(620f, 0, 0);
        cursorOffset = new Vector3(0, -370f, 0);
        fouces = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(4).gameObject.GetComponent<RectTransform>();
        cursor = transform.GetChild(0).GetChild(2).GetChild(1).GetChild(4).GetComponent<RectTransform>();
        karryn = GameObject.Find("Karryn").GetComponent<karryn>();
        optionList = new List<GameObject>();
        for (int i = 2; i < 5; i++)
        {
            var obj = transform.GetChild(0).GetChild(i).gameObject;
            optionList.Add(obj);
        }
        cor = StartCoroutine(openBag());
    }

    private IEnumerator openBag()
    {
        while (true)
        {
            if (time % 2 == 0 && Input.GetKeyDown(KeyCode.Escape))
            {
                karryn.stopPlayerAction();
                time++;
                transform.GetChild(0).gameObject.SetActive(true);
                yield return StartCoroutine(open());
                time--;
                karryn.enablePlayerAction();
            }
            yield return null;
        }
    }

    private IEnumerator openEquipmentMenu()
    {
        yield break;
    }

    private IEnumerator openSaveSlotMenu()
    {
        yield return null;
        displaySaveSlotFromList(Global.instance.gameObject.GetComponent<LocalData>().getPlayerLocalData());
        int index = 0;
        int dir = 1;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                localData.Save(index);
                yield break;
            }

            //向下
            if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && index < 2)
            {
                index++;
                dir = 1;
                cursor.anchoredPosition += new Vector2(cursorOffset.x, dir * cursorOffset.y);
            }
            else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && index > 0)
            {
                index--;
                dir = -1;
                cursor.anchoredPosition += new Vector2(cursorOffset.x, dir * cursorOffset.y);
            }
            yield return null;
        }
    }

    private void displaySaveSlotFromList(List<PlayerData> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            var obj = transform.GetChild(0).GetChild(2).GetChild(1).GetChild(i + 1).gameObject;
            obj.SetActive(true);
            //obj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = dataList[i].dateTime;
            obj.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = dataList[i].sexVal.ToString();
            obj.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = dataList[i].level.ToString();
        }
    }

    private IEnumerator open()
    {
        yield return null;
        int n = 0;
        int dir = 1;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                transform.GetChild(0).gameObject.SetActive(false);
                yield break;
            }
            //选择
            if (Input.GetKeyDown(KeyCode.Space) && !isSelect)
            {
                isSelect = true;
                if (n == optionList.Count - 1)
                {
                    //移动平台可能不行
                    UnityEngine.Application.Quit();
                }
                if (optionList[n].name == "Equipment")
                {
                    optionList[n].SetActive(true);
                    yield return StartCoroutine(openEquipmentMenu());
                    optionList[n].SetActive(false);
                }
                else if (optionList[n].name == "Load Game")
                {
                    optionList[n].SetActive(true);
                    yield return StartCoroutine(openSaveSlotMenu());
                    optionList[n].SetActive(false);
                }
                isSelect = false;
            }

            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && n > 0)
            {
                n--;
                dir = -1;
                fouces.anchoredPosition += new Vector2(dir * offset.x, offset.y);
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && n < 2)
            {
                n++;
                dir = 1;
                fouces.anchoredPosition += new Vector2(dir * offset.x, offset.y);
            }
            yield return null;
        }
    }
}