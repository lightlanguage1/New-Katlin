using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Image dialogImg;

    public Text dialogText;

    public float textSpeed;

    // 交互UI界面
    public GameObject interactUI;

    [Header("NPC文本文件")]
    public TextAsset textFile;

    [Header("是否在对话")]
    public bool isTalk;

    public int index = 0;

    private karryn playerScript;    //保存卡琳脚本的变量

    private List<string> list = new List<string>(); //将定义的list初始化

    //委托，事件实现主角更改速度（实现对象通信）
    public delegate void heroSpeedDele(bool isTalk);
    public static event heroSpeedDele heroSpeedEvent;

    private void Awake()
    {
        readTextFromFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && index == list.Count) //循环访问list中的string 不然越界
        {
            //index == list.Count 说明此时已经对话完毕了，index为最后元素的下个元素索引
            playerScript.enabled = true;
            playerScript.GetComponent<Animator>().enabled = true;
            playerScript.GetComponent<Rigidbody2D>().WakeUp();
            index = 0;
            dialogImg.gameObject.SetActive(false);  //禁用对话框以及其中的文本
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTalk)
        {
            if (index == 0) //index为0说明第一次按空格键，因此启用对话框
            {
                //启用对话框以及文本
                dialogImg.gameObject.SetActive(true);
                //禁用玩家的控制器脚本,防止玩家未完成对话就走动
                playerScript = GameObject.Find("Karryn").GetComponent<karryn>();
                //当我按下移动键后，刚体的速度赋值为2，只有当我停止输入时他的速度才会为0，才能停下来，
                //因此禁用脚本时，我并没有停止输入（松手）也就时速度还是2，所以角色会一直移动
                playerScript.enabled = false;   //调用该脚本的ondisable函数
                playerScript.GetComponent<Animator>().enabled = false;
                playerScript.GetComponent<Rigidbody2D>().Sleep(); 
                //隐藏交互提示
                interactUI.SetActive(false);
            }
            //启用协程，逐个显示句子中的字
            StartCoroutine(setText());
        }
    }

    IEnumerator setText()
    {
        isTalk = false; //防止玩家再次按互动键，产生bug
        dialogText.text = "";   //清空，防止出现上次的对话
        for(int i = 0; i < list[index].Length;i++)
        {
            dialogText.text += list[index][i];  //将字符数组中的第一个字符添加到文本中
            yield return new WaitForSeconds(textSpeed); //text显示一个字符后等待若干秒在显示下一个
        }
        index++;    //循环结束第一句话全部显示了
        isTalk = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTalk = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTalk = false;
            if (interactUI.activeSelf)   //防止玩家按space对话禁用他后，再次禁用他
            {
                interactUI.SetActive(false);
            }
        }
    }

    private void readTextFromFile()
    {
        list.Clear();   //首先清理下，确保上次对话保存的内容被清空
        string[] lineData = textFile.text.Split('n');
        foreach (var line in lineData)
        {
            list.Add(line);
        }
    }
}