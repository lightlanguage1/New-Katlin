using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 跳转到子菜单
public class GotoChildMenu : MonoBehaviour
{
    public GameObject btnLoad;  // 加载按钮的游戏对象

    // 启用加载按钮
    public void enableLoadButton()
    {
        btnLoad = transform.GetChild(5).gameObject;  // 获取第6个子对象（索引从0开始）作为加载按钮
        btnLoad.SetActive(true);  // 激活加载按钮

        // 获取全局变量管理者的本地数据组件，并获取玩家本地数据列表
        List<PlayerData> dataList = Global.instance.gameObject.GetComponent<LocalData>().getPlayerLocalData();

        // 循环遍历本地数据列表
        for (int i = 0; i < dataList.Count; i++)
        {
            var obj = transform.GetChild(5).GetChild(1).GetChild(i + 1).gameObject;  // 获取加载按钮下的子对象作为存档槽
            obj.SetActive(true);  // 激活存档槽
            obj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = dataList[i].dateTime;  // 设置存档槽中的日期文本
            obj.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = dataList[i].sexVal.ToString();  // 设置存档槽中的性别文本
            obj.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = dataList[i].level.ToString();  // 设置存档槽中的等级文本
        }
    }

    // 更新方法
    private void Update()
    {
        // 如果加载按钮不为空且按下了Escape键
        if (btnLoad != null && Input.GetKeyDown(KeyCode.Escape))
        {
            btnLoad.SetActive(false);  // 隐藏加载按钮
            btnLoad = null;  // 将加载按钮置空
        }
    }
}
