using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("血条and蓝条")]
    public float curHpFillProgress;

    public float targetHpFillProgress;
    public float curMpFillProgress;
    public float targetMpFillProgress;
    public float fillSpeed; 
    private float t;
    private bool isHp;
    private Coroutine hpCoroutine;
    private Coroutine mpCoroutine;

    [SerializeField] private Image hpFillImageBack;
    [SerializeField] private Image hpFillImageFront;
    [SerializeField] private Image mpFillImageBack;
    [SerializeField] private Image mpFillImageFront;

    private WaitForSeconds delayTime;   //延时时间

    private void Awake()
    {
        delayTime = new WaitForSeconds(0.35f);
        isHp = false;
        //获得hp的图像 前后
        hpFillImageBack = transform.Find("HpBack").GetComponent<Image>(); //查找直接子对象
        hpFillImageFront = transform.Find("HpFront").GetComponent<Image>();
        //获得mp的图像 前后
        mpFillImageBack = transform.Find("MpBack").GetComponent<Image>();
        mpFillImageFront = transform.Find("MpFront").GetComponent<Image>();
    }

    public void initializeHpAndMp(float hpCurVal, float mpCurval, float hpMaxVal, float mpMaxVal)
    {
        curHpFillProgress = hpCurVal / hpMaxVal;
        targetHpFillProgress = curHpFillProgress;
        curMpFillProgress = mpCurval / mpMaxVal;
        targetMpFillProgress = curMpFillProgress;
        //让hp mp 前后2张进度图都不显示
        hpFillImageFront.fillAmount = curHpFillProgress;
        hpFillImageBack.fillAmount = curHpFillProgress;
        mpFillImageBack.fillAmount = curMpFillProgress;
        mpFillImageFront.fillAmount = curMpFillProgress;
    }

    public void updateHp(float hpCurVal, float maxVal)
    {
        targetHpFillProgress = hpCurVal / maxVal;

        //防止该函数被反复调用，导致进度条既增加又减少出BUG
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
            hpCoroutine = null;
        }

        if (curHpFillProgress < targetHpFillProgress)  //增加进度
        {
            //后面的图先增加，然后增加前面的图
            isHp = true;
            hpFillImageBack.fillAmount = targetHpFillProgress;
            hpCoroutine = StartCoroutine(changeImgFillPorgress(hpFillImageFront));
        }
        else if (curHpFillProgress > targetHpFillProgress) //减少进度
        {
            //与上述相反
            isHp = true;
            hpFillImageFront.fillAmount = targetHpFillProgress;
            StartCoroutine(changeImgFillPorgress(hpFillImageBack));
        }
    }

    public void updateMp(float mpCurval, float maxVal)
    {
        targetMpFillProgress = mpCurval / maxVal;
        if (targetMpFillProgress <= 0) targetMpFillProgress = 0;

        if (mpCoroutine != null)
        {
            StopCoroutine(mpCoroutine);
            mpCoroutine= null;
        }

        if (curMpFillProgress < targetMpFillProgress)
        {
            isHp = false;
            mpFillImageBack.fillAmount = targetMpFillProgress;
            StartCoroutine(changeImgFillPorgress(mpFillImageFront));
        }
        else if (curMpFillProgress > targetMpFillProgress)
        {
            isHp = false;
            mpFillImageFront.fillAmount = targetMpFillProgress;
            StartCoroutine(changeImgFillPorgress(mpFillImageBack));
        }
    }

    private IEnumerator changeImgFillPorgress(Image img)
    {
        yield return delayTime;
        t = 0f;

        if (isHp)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curHpFillProgress = Mathf.Lerp(curHpFillProgress, targetHpFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curHpFillProgress <= targetHpFillProgress)
                {
                    curHpFillProgress = targetHpFillProgress;
                    img.fillAmount = curHpFillProgress;
                    break;
                }
                img.fillAmount = curHpFillProgress;
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curMpFillProgress = Mathf.Lerp(curMpFillProgress, targetMpFillProgress, t); //线性插值函数，a + (b - a) * t 插值公式
                if (curMpFillProgress <= targetMpFillProgress)
                {
                    curMpFillProgress = targetMpFillProgress;
                    img.fillAmount = curMpFillProgress;
                    break;
                }
                img.fillAmount = curMpFillProgress;
                yield return null;
            }
        }
    }
}