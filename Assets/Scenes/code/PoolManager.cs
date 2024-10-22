using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private Pool[] Enemiespools;  //管理的多个对象池
    private Dictionary<GameObject, Pool> dictionary;

    protected override void Awake()
    {
        base.Awake();
        dictionary = new Dictionary<GameObject, Pool>();
        initialize(Enemiespools);

    }

    private void OnDisable()    //对象销毁时调用该方法
    {
#if UNITY_EDITOR
        checkPoolSize();
#endif

    }

    public void checkPoolSize()
    {
        foreach (var pool in Enemiespools)
        {
            if (pool.Size < pool.RuntimeSize)
            {
                Debug.LogWarning(string.Format("Pool:{0} runtime size is {1} bigger src size,Please change!",
                    pool.Prefab.name,
                    pool.RuntimeSize, pool.Size));
            }
        }
    }

    private void initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in dictionary!");   //防止添加了相同的key，因为字典中的key要保持唯一
            }
#endif
            dictionary.Add(pool.Prefab, pool);  //添加键值对
            //创建所有对象池中的所有对象,注意是屏幕空间的对象
            GameObject poolParent = new GameObject("Pool: " + pool.Prefab.name);   //创建一个空游戏对象作为对象池的父对象
            RectTransform poolRectTransform = poolParent.AddComponent<RectTransform>(); // 添加 RectTransform 组件
            poolRectTransform.SetParent(transform, false);  // 设置父对象，并设置 worldPositionStays 参数为 false
            pool.initialize(poolRectTransform);    //初始化
        }
    }

    /// <summary>
    /// 通过字典找到预制体对应的对象池，然后再得到可用的对象
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体</param>
    /// <param name="postion">他的位置属性</param>
    /// <param name="rotation">他的旋转属性</param>
    /// <param name="localScale">他的本地缩放属性</param>
    /// <returns>
    /// 对象对应的对象池中的可用对象
    /// </returns>
    public GameObject getObjInPool(GameObject prefab, Vector3 postion, Quaternion rotation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager: The dictionary does not contain the object");
            return null;
        }
#endif
        return dictionary[prefab].preparedObj(postion, rotation, localScale);   //通过预制体对象找到对应的池，然后得到已经准备好的对象
    }

    public GameObject getObjInPool(GameObject prefab, RectTransform postion)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager: The dictionary does not contain the object");
            return null;
        }
#endif
        return dictionary[prefab].preparedObj(postion);   //通过预制体对象找到对应的池，然后得到已经准备好的对象
    }

}
