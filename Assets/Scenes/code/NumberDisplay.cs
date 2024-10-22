using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerManager;

public class NumberDisplay : MonoBehaviour
{
    [SerializeField] private List<Sprite> spriteList;
    [SerializeField] private List<GameObject> numsHp;
    [SerializeField] private List<GameObject> numsMp;
    [SerializeField] private List<GameObject> numsVoli;

    private void OnEnable()
    {
        PlayerManager.instance.onProgressBarChange += numbersChaneg;
    }

    private  void OnDisable()
    {
        PlayerManager.instance.onProgressBarChange -= numbersChaneg;
    }

    private void numbersChaneg(int[] nums, ProgressBarType type)
    {
        int n = nums.Length;  //确定是几位数
        //反转数组,为什么要反转自己思考
        int start = 0, end = n - 1;
        while (start < end)
        {
            int temp = nums[start];
            nums[start] = nums[end];
            nums[end] = temp;
            start++;
            end--;
        }
        switch (type)
        {
            case ProgressBarType.hp:
                {
                    //首先禁用所有的，因为可能由4位数==>3位数
                    foreach(GameObject obj in numsHp)
                    {
                        obj.SetActive(false);
                    }
                    for (int i = 0; i < n; i++) 
                    {
                        numsHp[i].gameObject.SetActive(true);
                        numsHp[i].GetComponent<Image>().sprite = spriteList[nums[i]];
                    }
                    break;
                }
            case ProgressBarType.mp:
                {
                    //首先禁用所有的，因为可能由4位数==>3位数
                    foreach (GameObject obj in numsMp)
                    {
                        obj.SetActive(false);
                    }
                    for (int i = 0; i < n; i++)
                    {
                        numsMp[i].gameObject.SetActive(true);
                        numsMp[i].GetComponent<Image>().sprite = spriteList[nums[i]];
                    }
                    break;
                }
            case ProgressBarType.volition:
                {
                    //首先禁用所有的，因为可能由4位数==>3位数
                    foreach (GameObject obj in numsVoli)
                    {
                        obj.SetActive(false);
                    }
                    for (int i = 0; i < n; i++)
                    {
                        numsVoli[i].gameObject.SetActive(true);
                        numsVoli[i].GetComponent<Image>().sprite = spriteList[nums[i]];
                    }
                    break;
                }
            default:
                {
                    Debug.Log("没有这样的进度条,请检测！");
                    break;
                }
        }
    }
}