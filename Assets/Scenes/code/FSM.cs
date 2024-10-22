using UnityEngine;

// 抽象状态机类
public abstract class FSM
{
    protected NPC m_npc; // 引用 NPC 的实例

    // 进入状态时调用的方法
    public abstract void onEnter(NPC npc);

    // 逻辑更新方法，处理状态的主要逻辑
    public abstract void logicUpdate();

    // 物理更新方法，处理与物理相关的逻辑（放在 FixedUpdate 中）
    public abstract void physicsUpdate();

    // 退出状态时调用的方法
    public abstract void onExit();
}

// 状态：GoblinIdleState（空闲状态）
public class GoblinIdleState : FSM
{
    private float timer;    // 计时器暂停 NPC 移动
    private float timeout;  // 随机等待时间

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        timeout = Random.Range(0, 3); // 随机生成等待时间
        m_npc.moveDir = m_npc.generateRandomXyDir(); // 生成随机移动方向
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Idle"); // 播放空闲动画
        m_npc.isMove = false;
    }

    public override void logicUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= timeout)
        {
            timer = 0;
            if (!m_npc.foundPlayer())
            {
                m_npc.switchState(Global.StateType.PatrolState); // 没有发现玩家，切换到巡逻状态
            }
            else
            {
                m_npc.switchState(Global.StateType.ChaseState); // 发现玩家，切换到追逐状态
            }
        }
    }

    public override void physicsUpdate()
    {
        // 在此状态下没有物理更新
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // 停止播放动画
    }
}

// 状态：GoblinPatrolState（巡逻状态）
public class GoblinPatrolState : FSM
{
    private float timer;    // 限制 NPC 移动的时间
    private float timeout;  // 巡逻时间
    private bool isTimeout; // 是否超时

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        isTimeout = false;
        timeout = Random.Range(2, 5); // 随机生成巡逻时间
        m_npc.moveDir = m_npc.generateRandomXyDir(); // 生成随机移动方向
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk"); // 播放巡逻动画
        m_npc.isMove = true;
    }

    public override void logicUpdate()
    {
        if (m_npc.foundPlayer())  // 发现了玩家
        {
            // 切换状态
            m_npc.isMove = false;
            m_npc.switchState(Global.StateType.ChaseState); // 切换到追逐状态
        }
        else
        {
            m_npc.updateMoveDir(); // 更新移动方向
            timer += Time.deltaTime;
            if (timer >= timeout)
            {
                timer = 0;
                m_npc.switchState(Global.StateType.IdleState); // 超时，切换到空闲状态
            }
        }
    }

    public override void physicsUpdate()
    {
        // 在此状态下没有物理更新
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // 停止播放动画
    }
}

// 状态：GoblinChaseState（追逐状态）
public class GoblinChaseState : FSM
{
    private float speed; // 追逐速度
    private float timer; // 计时器
    private bool isSwitch; // 是否可以切换状态
    private Vector3 startPos; // 切换状态时的起始位置

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0;
        speed = 1.5f; // 设置追逐速度
        m_npc.isMove = false;
        isSwitch = false;
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk"); // 播放追逐动画
        startPos = m_npc.transform.localPosition; // 记录切换状态时的起始位置
    }

    public override void logicUpdate()
    {
        if (!m_npc.foundPlayer() && isSwitch)   // 玩家脱离检测范围
        {
            // 切换状态
            m_npc.switchState(Global.StateType.IdleState); // 玩家脱离检测范围，切换到空闲状态
        }
        else
        {
            checkCollider(); // 检测碰撞
            timer += Time.deltaTime;
            m_npc.transform.localPosition = Vector3.MoveTowards(m_npc.transform.localPosition, m_npc.playerPos, speed * Time.deltaTime);
            if (timer >= 2f) isSwitch = true; // 计时器超过2秒后可以切换状态
        }
    }

    // 检测碰撞
    private void checkCollider()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(m_npc.transform.position, m_npc.moveDir, m_npc.length, LayerMask.GetMask("BG"));
        if (hit2D.collider)
        {
            m_npc.switchState(Global.StateType.IdleState); // 碰到障碍物，切换到空闲状态
        }
    }

    // 获取世界坐标
    public Vector3 getWorldPostion(Vector2Int gridPos)
    {
        Vector3 v3 = m_npc.grid.CellToWorld((Vector3Int)gridPos);
        return v3;
    }

    // 获取追逐方向
    public Vector3 getChaseDir(Vector3 curPos, Vector3 nextPos)
    {
        // 计算移动方向向量
        Vector3 chaseDir = nextPos - curPos;

        // 归一化移动方向向量
        chaseDir.Normalize();

        // 返回归一化后的移动方向向量
        return chaseDir;
    }

    public override void physicsUpdate()
    {
        // 在此状态下没有物理更新
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // 停止播放动画
    }
}
