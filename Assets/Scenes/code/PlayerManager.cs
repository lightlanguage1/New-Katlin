using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//玩家的数据统计
public class PlayerManager : Singleton<PlayerManager>
{
    //我的失误 这些因该加到global里面,直接更新global中的玩家数据就好了，这样写多一步麻烦些
    // 基本属性
    public string characterName = "karryn";  // 角色名字

    public int level = 1;             // 等级
    public int exper = 0;        // 经验值
    private GameObject battleCanvas;

    [Header("血条and蓝条and意志条")]
    public int maxHp = 2000;         // 最大生命值

    public float curHp = 2000;        // 当前生命
    public float maxMp = 2000;        // 最大精力
    public float curMp = 2000;       // 当前精力
    public float curVoli = 200;       //意志值
    public float maxVoli = 200;
    public float curHappyVal = 0;    //快乐 max is 200
    public float maxHappyVal = 200;

    //下面这5个值根据快乐值定义
    public float kouVal = 0;      //口

    public float bangVal = 0;     //棒
    public float ruVal = 0;       //乳
    public float qiangVal = 0;    //腔
    public float kaoVal = 0;      //尻
    public float factor = 5;      //控制5个属性的系数

    private float t;
    public float fillSpeed = 0.5f;
    private float curHpFillProgress;
    private float targetHpFillProgress;
    private float curMpFillProgress;
    private float targetMpFillProgress;
    private float curVolFillProgress;
    private float targetVolFillProgress;
    private float curHappyFillProgress;
    private float targetHappyFillProgress;

    private Coroutine m_hpCoroutine;
    private Coroutine m_mpCoroutine;
    private Coroutine m_volCoroutine;

    // 攻击和防御
    public float atk = 5;       // 攻击力

    public float def = 100;      // 防御力

    // 血条、精力条和魔法条图像
    public Image[] propertyBarImg;

    public enum ProgressBarType
    {
        hp = 0,
        mp,
        volition,
        sexProgress,
        bang,
        kou,
        ru,
        qiang,
        kao,
        textImg
    }

    public bool isDied = false; // 标记角色是否被击败

    //委托
    public delegate void updateProgressBar(int[] nums, ProgressBarType type);

    public event updateProgressBar onProgressBarChange;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()    //当脚本启用时调用 (战斗场景启用)
    {
    }

    public void loadResource()
    {
        // 从本地读取玩家存档数据，初始化玩家的所有相关属性
        propertyBarImg = new Image[Enum.GetValues(typeof(ProgressBarType)).Length]; //因为该函数重复调用

        battleCanvas = GameObject.Find("Battle Canvas").gameObject;
        var obj = battleCanvas.transform.Find("Karryn Property Bar");

        for (int i = 0; i < obj.childCount; i++)
        {
/*#if UNITY_EDITOR
            Debug.Log(obj.GetChild(i).GetComponent<Image>());
#endif*/
            propertyBarImg[i] = obj.GetChild(i).GetComponent<Image>();
        }
        initialize(curHp, curMp, curVoli, maxHp, maxMp, maxVoli, curHappyVal, maxHappyVal, atk, def);
    }

    public void releaseResource()
    {
        if (propertyBarImg.Length > 0)  
        {
            Array.Clear(propertyBarImg, 0, propertyBarImg.Length);
            propertyBarImg = null;  //触发GC 回收他
        }
    }

    public bool checkFail()
    {
        if (isDied)
        {
            isDied = false;
            return true;
        }
        return false;
    }

    //切记升级一定要重新初始化这些值
    public void initialize(float hpCurVal, float mpCurval, float voliCurVal, float hpMaxVal, float mpMaxVal, float voliMaxVal,
        float happyCurVal, float happyMaxVal, float atkVal_, float defVal_)
    {
        //调整进度条
        curHpFillProgress = hpCurVal / hpMaxVal;
        targetHpFillProgress = curHpFillProgress;
        curMpFillProgress = mpCurval / mpMaxVal;
        targetMpFillProgress = curMpFillProgress;
        curVolFillProgress = voliCurVal / voliMaxVal;
        targetVolFillProgress = curVolFillProgress;
        curHappyFillProgress = happyCurVal / happyMaxVal;
        targetHappyFillProgress = curHappyFillProgress;

        propertyBarImg[(int)ProgressBarType.hp].fillAmount = curHpFillProgress;
        propertyBarImg[(int)ProgressBarType.mp].fillAmount = curMpFillProgress;
        propertyBarImg[(int)ProgressBarType.volition].fillAmount = curVolFillProgress;
        propertyBarImg[(int)ProgressBarType.sexProgress].fillAmount = curHappyFillProgress;

        //更新hp mp voli happy atk def
        curHp = hpCurVal;
        curMp = mpCurval;
        curVoli = voliCurVal;
        curHappyVal = happyCurVal;
        atk = atkVal_;
        def = defVal_;
        //触发事件
        onProgressBarChange?.Invoke(parseString(curHp.ToString()), ProgressBarType.hp);
        onProgressBarChange?.Invoke(parseString(curMp.ToString()), ProgressBarType.mp);
        onProgressBarChange?.Invoke(parseString(curVoli.ToString()), ProgressBarType.volition);
    }

    private int[] parseString(string str)
    {
        string s = str.ToString();
        int[] nums = new int[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            if (int.TryParse(s[i].ToString(), out int number) && i < s.Length)  //将字符（整数）转成int
            {
                nums[i] = number;
            }
            else
            {
                Debug.Log("字符转换int失败！");
            }
        }
        return nums;
    }

    private IEnumerator updateNumbers(int start, int end, ProgressBarType type)
    {
        while (start > end)
        {
            //减少进度
            start -= 1;
            onProgressBarChange?.Invoke(parseString(start.ToString()), type);
            yield return null;
        }
    }

    private void updateHp(float startHp, float hpCurVal, float maxVal)
    {
        targetHpFillProgress = hpCurVal / maxVal;

        if (m_hpCoroutine != null)
        {
            StopCoroutine(m_hpCoroutine);
            m_hpCoroutine = null;
        }
        m_hpCoroutine = StartCoroutine(changeImgFillPorgress(propertyBarImg[(int)ProgressBarType.hp], ProgressBarType.hp));
        StartCoroutine(updateNumbers((int)startHp, (int)hpCurVal, ProgressBarType.hp));
    }

    private void updateMp(float startMp, float mpCurVal, float maxVal)
    {
        targetMpFillProgress = mpCurVal / maxVal;

        if (m_mpCoroutine != null)
        {
            StopCoroutine(m_mpCoroutine);
            m_mpCoroutine = null;
        }
        m_mpCoroutine = StartCoroutine(changeImgFillPorgress(propertyBarImg[(int)ProgressBarType.mp], ProgressBarType.mp));
        StartCoroutine(updateNumbers((int)startMp, (int)mpCurVal, ProgressBarType.mp));
    }

    private void updateVoli(float startVoli, float voliCurVal, float maxVal)
    {
        targetVolFillProgress = voliCurVal / maxVal;

        if (m_volCoroutine != null)
        {
            StopCoroutine(m_volCoroutine);
            m_volCoroutine = null;
        }
        m_volCoroutine = StartCoroutine(changeImgFillPorgress(propertyBarImg[(int)ProgressBarType.volition], ProgressBarType.volition));
        StartCoroutine(updateNumbers((int)startVoli, (int)voliCurVal, ProgressBarType.volition));
    }

    private void updateHappy(float happyCurVal, float maxVal)
    {
        targetVolFillProgress = happyCurVal / maxVal;

        if (m_volCoroutine != null)
        {
            StopCoroutine(m_volCoroutine);
            m_volCoroutine = null;
        }
        m_volCoroutine = StartCoroutine(changeImgFillPorgress(propertyBarImg[(int)ProgressBarType.sexProgress], ProgressBarType.sexProgress));
    }

    //如果有回血的逻辑，在这里单独再添加一个协程

    private IEnumerator changeImgFillPorgress(UnityEngine.UI.Image image, ProgressBarType type)
    {
        yield return new WaitForSeconds(0.5f);
        t = 0f;
        if (type == ProgressBarType.hp)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curHpFillProgress = Mathf.Lerp(curHpFillProgress, targetHpFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curHpFillProgress <= targetHpFillProgress)
                {
                    curHpFillProgress = targetHpFillProgress;
                    image.fillAmount = curHpFillProgress;
                    break;
                }
                image.fillAmount = curHpFillProgress;
                yield return null;
            }
        }
        else if (type == ProgressBarType.mp)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curMpFillProgress = Mathf.Lerp(curMpFillProgress, targetMpFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curMpFillProgress <= targetMpFillProgress)
                {
                    curMpFillProgress = targetMpFillProgress;
                    image.fillAmount = curMpFillProgress;
                    break;
                }
                image.fillAmount = curMpFillProgress;
                yield return null;
            }
        }
        else if (type == ProgressBarType.volition)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curVolFillProgress = Mathf.Lerp(curVolFillProgress, targetVolFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curVolFillProgress <= targetVolFillProgress)
                {
                    curVolFillProgress = targetVolFillProgress;
                    image.fillAmount = curVolFillProgress;
                    break;
                }
                image.fillAmount = curVolFillProgress;
                yield return null;
            }
        }
        else if (type == ProgressBarType.sexProgress)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curHappyFillProgress = Mathf.Lerp(curHappyFillProgress, targetHappyFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curHappyFillProgress <= targetHappyFillProgress)
                {
                    curHappyFillProgress = targetHappyFillProgress;
                    image.fillAmount = curHappyFillProgress;
                    break;
                }
                image.fillAmount = curHappyFillProgress;
                yield return null;
            }
        }
    }

    public void takeDamage(float damage)
    {
        float startHp = curHp;
        curHp -= damage;
        updateHp(startHp, curHp, maxHp);
        if (curHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        //战败
        isDied = true;
        curHp = maxHp;
        curMp = maxMp;
        curVoli = maxVoli;
        curHappyVal = 0f;
        updateHappyCorrelationValue(0);
    }

    public void subtractMagic(float subtractMp)
    {
        float startMp = curMp;
        curMp -= subtractMp;
        updateMp(startMp, curMp, maxMp);
        if (curMp <= 0f)
        {
            Die();
        }
    }

    public void subtractVolition(float subtractVol)
    {
        float startVoli = curVoli;
        curVoli -= subtractVol;
        updateVoli(startVoli, curVoli, maxVoli);
        if (curVoli <= 0f)
        {
            //禁止使用消耗意志的技能
        }
    }

    public void RestoreHp(float restoreHp)
    {
        if (restoreHp == curHp) return;
        curHp += restoreHp;
        //减血都写好了 加血自己想
    }

    public void RestoreMp(float restoreMp)
    {
        if (restoreMp == curMp) return;
        curMp += restoreMp;
    }

    public void RestoreVoli(float restoreVoli)
    {
        if (restoreVoli == curVoli) return;
        curVoli += restoreVoli;
    }

    public void addHappyVal(float HappyVal)
    {
        curHappyVal += HappyVal;
        if (curHappyVal >= 200)
        {
            Sex();
            return;
        }
        //增加到一定值就会触发动画和改变立绘
        updateHappyCorrelationValue(curHappyVal);
    }

    private void Sex()
    {
        curHappyVal = 0;
    }

    private void updateHappyCorrelationValue(float happyVal_)  //更新快乐值的相关的5个属性
    {
        //除以系数获得
        float res = happyVal_ / maxHappyVal;
        kouVal = res;
        bangVal = res;
        ruVal = res;
        qiangVal = res;
        kaoVal = res;
        //更新进度条
        propertyBarImg[(int)ProgressBarType.sexProgress].fillAmount = res;
        propertyBarImg[(int)ProgressBarType.kou].fillAmount = kouVal;
        propertyBarImg[(int)ProgressBarType.bang].fillAmount = bangVal;
        propertyBarImg[(int)ProgressBarType.ru].fillAmount = ruVal;
        propertyBarImg[(int)ProgressBarType.qiang].fillAmount = qiangVal;
        propertyBarImg[(int)ProgressBarType.kao].fillAmount = kaoVal;
    }
}