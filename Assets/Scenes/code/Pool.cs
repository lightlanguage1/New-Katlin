using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [SerializeField] private GameObject prefab; //对象预制体
    [SerializeField] private Queue<GameObject> queue;
    [SerializeField] private int size = 1;  //队列大小
    public GameObject Prefab => prefab; //属性表达式，返回私有的预制体对象
    public int Size => size;    
    public int RuntimeSize => queue.Count;  //获得运行时对象池实际大小，优化他，防止他扩容
    private Transform parent;

    private GameObject spawnObj()   //生成对象
    {
        var obj = GameObject.Instantiate(prefab, parent);   //创建预制体并为他指定父对象
        obj.SetActive(false);
        obj.name = obj.name.Replace("(Clone)", "");
        return obj;
    }

    public void initialize(Transform parent)
    {
        queue = new Queue<GameObject>();    //创建队列
        this.parent = parent;

        for (int i = 0; i < size; i++)
        {
            queue.Enqueue(spawnObj());
        }
    }

    private GameObject getAvailableObj()
    {
        GameObject availableObj = null;
        if (size > 1 && !queue.Peek().activeSelf)   //队列长度至少大于1，并且不能将队首活跃（正在执行任务的）对象出队列
        {
            availableObj = queue.Dequeue();
        }
        else
        {
            availableObj = spawnObj();
        }
        //出队的对象执行完任务后入队因为是进入队尾（这样写方便，避免再写一个入队函数）
        queue.Enqueue(availableObj);

        return availableObj;
    }

    public GameObject preparedObj(RectTransform position)   //准备好的对象
    {
        var obj = getAvailableObj();
        obj.GetComponent<RectTransform>().anchoredPosition = position.anchoredPosition;
        obj.SetActive(true);
        return obj;
    }
    
    public GameObject preparedObj(Vector3 position, Quaternion rotation)    //Quaternion 代表旋转
    {
        var obj = getAvailableObj();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public GameObject preparedObj(Vector3 position, Quaternion rotation,Vector3 localScale) //本地缩放是会受到父对象的缩放影响（一起缩放指定倍数）
    {
        var obj = getAvailableObj();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = localScale;
        obj.SetActive(true);
        return obj;
    }
}