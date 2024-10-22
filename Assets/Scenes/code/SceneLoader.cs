#define DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//同步异步加载场景，以及在特定的场景下，为持久单例们加载一些基础数据
public class SceneLoader : Singleton<SceneLoader>
{
    public static string mainMenuSceneKeyName = "MainMenuScene";
    public static string mainSceneKeyName = "MainScene";
    public Image transitionImg;
    public float fadeTime = 2.5f;
    private Color color;

    [Header("场景过度材质")]
    public Material transitionMaterial;

    public float duration = 2;
    private float weight = 0;
    private Texture2D screenShot;
    private Camera mainCamera;

    [Header("战斗管理器")]
    public GameObject battelManager;

    //根据资产管理器找到的键名加载场景，参数：指定场景键名，是否附加场景,是否加载完成后激活该场景
    public static void loadSceneByAddressable(string sceneKeyName, bool isAddtiveScene = false, bool isActivate = true)
    {
        LoadSceneMode mode = isAddtiveScene ? LoadSceneMode.Additive : LoadSceneMode.Single;    //追加和替换
        Addressables.LoadSceneAsync(sceneKeyName, mode, isActivate);
    }

    //区别在于 AssetReference sceneReference 资产的引用
    public static void loadSceneByAddressable(AssetReference sceneReference, bool isAddtiveScene = false, bool isActivate = true)
    {
        LoadSceneMode mode = isAddtiveScene ? LoadSceneMode.Additive : LoadSceneMode.Single;
        Addressables.LoadSceneAsync(sceneReference, mode, isActivate);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += onSceneChanged;   //为事件注册回调函数，场景切换时调用
        SceneManager.sceneLoaded += checkLoadedScene;   //场景加载完成后触发该事件
        SceneManager.sceneUnloaded += onSceneUnloaded;  //当场景被卸载时触发该事件
    }

    private void onSceneUnloaded(Scene scene)
    {
        Global.instance.battlePrevSceneName = scene.name;   //获取战斗场景的前一个场景名字
    }

    private void onSceneChanged(Scene previousScene, Scene loadScene)
    {
        if (loadScene.name == "BattleScene")
        {
            //将对象池管理器作为画布的子对象，在画布中显示
            PoolManager.instance.transform.SetParent(GameObject.Find("Battle Canvas").transform, false);
            PlayerManager.instance.transform.SetParent(GameObject.Find("Battle Canvas").transform, false);
        }
    }

    // 检查已加载的场景并执行相应的逻辑
    private void checkLoadedScene(Scene scene, LoadSceneMode mode)//胜利or逃跑主要逻辑
    {
        // 如果加载的是主场景"main1L"
        if (scene.name == "main1L")
        {
            // 记录当前主场景的名称
            Global.instance.curMainSceneName = scene.name;
            // 标记当前处于主场景中
            Global.instance.inMainScene = true;
            // TODO: 可以在此处执行一些与主场景相关的逻辑，比如加载资源等
            //Global.instance.loadResource(scene.name);
            
        }

        // 如果加载的是主场景"main1L"并且上一次是战斗胜利或逃跑
        if (scene.name == "main1L" && (Global.instance.isWin || Global.instance.isTin))
        {
            // 如果是逃跑，直接清空已阵亡的敌人字典，并重置逃跑标志
            if (Global.instance.isTin)
            {
                Global.instance.diedDic.Clear();
                Global.instance.isTin = false; // 重置逃跑标志
            }
            else // 否则是战斗胜利
            {
                // 重置战斗胜利标志
                Global.instance.isWin = false;
                // 获取所有NPC对象
                NPC[] npcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
                // 遍历所有NPC对象
                for (int i = 0; i < npcs.Length; i++)
                {
                    // 如果该NPC是上一场战斗中的敌人
                    if (npcs[i].name == Global.instance.battlePrevNpcName)
                    {
                        // 将该敌人加入阵亡队列，用于刷新
                        Global.instance.diedDic.Add(npcs[i].name, npcs[i].gameObject);
                    }
                    // 如果敌人在阵亡队列中
                    if (Global.instance.diedDic.ContainsKey(npcs[i].name))
                    {
                        // 不禁用该敌人对象，只需同步修改阵亡队列中敌人的活跃状态
                        Global.instance.diedDic[npcs[i].name].gameObject.SetActive(false);
                    }
                }
            }
        }

        // 如果加载的是战斗场景
        if (scene.name == "BattleScene")
        {
            // 加载战斗相关资源
            GameObject.FindFirstObjectByType<BattleManager>().loadResource();
            GameObject.FindFirstObjectByType<PlayerManager>().loadResource();
            // 标记当前不在主场景中
            Global.instance.inMainScene = false;
        }
        else
        {
            // 如果不是战斗场景，释放玩家相关资源
            GameObject.FindFirstObjectByType<PlayerManager>().releaseResource();
        }
    }


    private IEnumerator loadScneneByTransition(int sceneVal)
    {
        switch (sceneVal)
        {
            case (int)SceneEnumVal.MainMenuScene:
                {
                    //过渡加载逻辑。。。
                    yield break;
                }
            case (int)SceneEnumVal.Main1L:
                {
                    yield return StartCoroutine(FadeInOutLoadScene("main1L")); //执行loadScneneByTransition的协程执行这句将被挂起，直到方法执行完毕
                    yield break;
                }
            case (int)SceneEnumVal.BattleScene:
                {
                    //防止玩家移动角色
                    GameObject obj = GameObject.Find("Karryn");
                    if (obj != null)
                    {
                        obj.GetComponent<Animator>().enabled = false;
                        obj.GetComponent<karryn>().enabled = false;
                        obj.GetComponent<Rigidbody2D>().Sleep();
                    }

                    //启动协程加载场景
                    yield return StartCoroutine(LoadBattleScene("BattleScene"));
                    yield break;
                }
            case (int)SceneEnumVal.SexTransitionScene:
                {
                    yield return StartCoroutine(FadeInOutLoadScene("SexTransitionScene"));
                    yield break;
                }
        }
    }

    private IEnumerator FadeInOutLoadScene(string sceneName)
    {
        transitionImg.gameObject.SetActive(true);
        transitionImg.raycastTarget = true;

        //淡入
        while (transitionImg.color.a < 1)
        {
            color.a = Mathf.Clamp01(transitionImg.color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImg.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

        //淡出
        while (transitionImg.color.a > 0)
        {
            color.a = Mathf.Clamp01(transitionImg.color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImg.color = color;
            yield return null;
        }

        transitionImg.gameObject.SetActive(false);
        transitionImg.raycastTarget = false;
    }

    private IEnumerator LoadBattleScene(string sceneName)
    {
        //获取屏幕截图
        yield return StartCoroutine(getGrab());
        transitionMaterial.SetTexture("_MainTex", screenShot);  //将这个屏幕截图作为材质的主纹理
        if (!transitionMaterial.HasTexture("_MainTex")) Debug.Log("not _MainTex!");

        //创建一个Sprite对象，用于显示过渡效果
        GameObject spriteObj = new GameObject("TransitionSprite");

        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector3(0.5f, 0.5f, 0f));
#if DEBUG

        Debug.Log("Sprite size: " + spriteRenderer.bounds.size);
        Debug.Log("sprite size:" + sprite.textureRect.size.ToString());
#endif
        spriteRenderer.sprite = sprite;
        spriteRenderer.material = transitionMaterial;
        spriteRenderer.sortingOrder = 9999; //确保渲染在最前面

        spriteObj.transform.position = mainCamera.transform.position;

        while (weight < 1)
        {
            weight += Time.deltaTime / duration;
            weight = Mathf.Clamp01(weight); //限制在0-1之间

            transitionMaterial.SetFloat("_TransitionProgress", weight);
            float val = transitionMaterial.GetFloat("_TransitionProgress");
            //Debug.Log(val);
            yield return null;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);  //加载战斗场景
        weight = 0; //确保下次遇敌时，_TransitionProgress仍然是从0开始
    }

    private IEnumerator getGrab()
    {
        yield return new WaitForEndOfFrame(); //等待摄像机渲染完当前帧
        mainCamera = FindFirstObjectByType<Camera>();

        int width = UnityEngine.Device.Screen.width;
        int height = UnityEngine.Device.Screen.height;

        screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false); //创建纹理对象
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0); //0,0 代表屏幕坐标原点，即屏幕的左下角
        screenShot.Apply(); //应用读到的像素数据

#if DEBUG
        Debug.Log("width:" + width.ToString() + "  " + "height:" + height.ToString());
        Debug.Log("screenShot width:" + screenShot.width.ToString() + "screenShot height:" + screenShot.height.ToString());
#endif
    }

    public void loadGameScene(int sceneVal)
    {
        StartCoroutine(loadScneneByTransition(sceneVal));
    }
}