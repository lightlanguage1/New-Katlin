using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(1)]  //首先执行该脚本
public class TransitionSceneDialog : MonoBehaviour
{
    [Header("过渡场景文本文件")]
    public TextAsset textFile;

    private List<string> list = new List<string>();

    public float textSpeed;

    public TextMeshProUGUI dialogText;

    public int index;

    public delegate void animationPlayCompleted(Animator[] animators);

    public static event animationPlayCompleted playCompleted;

    [SerializeField] private List<GameObject> objs;    //静态对象存在于整个程序的生命周期中

    public Animator[] animators;

    private void Awake()
    {
        readTextFromFile();
        index = 0;
    }

    private void Start()
    {
        StartCoroutine(setText());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && index == list.Count)     //文本显示完毕
        {
            gameObject.SetActive(false);
            index = 0;
            foreach (GameObject obj in objs)
            {
                obj.SetActive(true);
            }
            playCompleted?.Invoke(animators);
        }
    }

    private IEnumerator setText()
    {
        dialogText.text = "";
        //遍历文本中一行的每个字符
        for (int i = 0; i < list[index].Length; i++)
        {
            dialogText.text += list[index][i];
            yield return new WaitForSeconds(textSpeed);
        }
        index++;    //下一行
    }

    private void readTextFromFile()
    {
        list.Clear();
        string[] lineData = textFile.text.Split('\n');
        foreach (string line in lineData)
        {
            list.Add(line);
        }
    }
}