using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // 角色属性
    [SerializeField] protected float hp;        // 当前生命值
    [SerializeField] protected float maxhp;     // 最大生命值
    [SerializeField] protected float mp;        // 当前魔法值
    [SerializeField] protected float maxmp;     // 最大魔法值
    [SerializeField] private ProgressBar progressBar;  // 进度条组件
    [SerializeField] protected float atk;       // 攻击力
    [SerializeField] protected float def;       // 防御力
    [SerializeField] protected float sex;       // 性攻击力

    // 使用委托实现脚本之间通信
    public delegate void taskCompletedDelegate();

    public static event taskCompletedDelegate onTaskCompleted;

    // 任务开始（任务指的是开始切换卡琳姿态图的任务）
    public delegate void taskBeginDelegate();

    public static event taskBeginDelegate onTaskBegin;

    // 敌人死亡后执行的逻辑
    public delegate void enemyDie();

    public static event enemyDie onEnemyDie;

    private bool isBlink;   // 闪烁是否结束
    private bool isDie;

    protected virtual void OnEnable()
    {
        // 初始化角色属性
        hp = maxhp;
        isBlink = false;
        isDie = false;
    }

    protected virtual void initHpAndMp(float hp, float mp, float maxhp, float maxmp)
    {
        // 初始化进度条组件
        progressBar.initializeHpAndMp(hp, mp, maxhp, maxmp);
    }

    public virtual void takeDamage(float damage)
    {
        // 扣除生命值
        hp -= damage;
        if (hp <= 0f)
        {
            hp = 0;
            isDie = true;
            StartCoroutine(die());
        }
        // 更新进度条
        progressBar.updateHp(hp, maxhp);
        // 触发闪烁动画
        StartCoroutine(blinkAnimation());
    }

    private IEnumerator blinkAnimation()
    {
        yield return new WaitForSeconds(0.15f);  // 手动调整，扣血的时候再闪烁
        float blinkInterval = 0.1f;
        int blinkCount = 5;
        Image image = transform.GetChild(0).GetComponent<Image>();

        // 触发任务开始事件
        onTaskBegin?.Invoke();

        for (int i = 0; i < blinkCount; i++)
        {
            // 降低透明度
            Color newColor1 = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            image.color = newColor1;

            // 等待一段时间
            yield return new WaitForSeconds(blinkInterval);

            // 恢复透明度
            Color newColor2 = new Color(image.color.r, image.color.g, image.color.b, 1f);
            image.color = newColor2;

            yield return new WaitForSeconds(blinkInterval);
        }
        if (isDie)
        {
            isBlink = true;
            yield break;    // 防止 die 协程还未将敌人从列表中移除时，战斗协程过早被唤醒，有一些难度，好好思考下
        }
        // 触发任务完成事件
        onTaskCompleted?.Invoke();
    }

    public virtual void subtractMagic(float subtractMp)
    {
        // 扣除魔法值
        mp -= subtractMp;
        // 更新魔法值进度条
        progressBar.updateMp(mp, maxmp);
        if (mp <= 0f)
        {
            // 无法使用技能了
        }
    }

    private IEnumerator die()
    {
        yield return new WaitUntil(() => isBlink);
        isBlink = false;
        // 触发敌人死亡事件和任务完成事件
        onEnemyDie?.Invoke();
        onTaskCompleted?.Invoke();
    }

    public virtual void RestoreHp(float restoreHp)
    {
        if (restoreHp == hp) return;

        // 恢复生命值，并确保在 0 和 maxhp 之间
        hp = Mathf.Clamp(restoreHp + hp, 0, maxhp);
    }

    public virtual void RestoreMp(float restoreMp)
    {
        if (restoreMp == hp) return;

        // 恢复魔法值，并确保在 0 和 maxmp 之间
        hp = Mathf.Clamp(restoreMp + hp, 0, maxmp);
    }

    protected IEnumerator hpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (hp < maxhp)
        {
            yield return waitTime;
            // 按百分比恢复生命值
            RestoreHp(maxhp * percent);
        }
    }

    protected IEnumerator mpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (mp < maxmp)
        {
            yield return waitTime;
            // 按百分比恢复魔法值
            RestoreHp(maxhp * percent);
        }
    }
}
